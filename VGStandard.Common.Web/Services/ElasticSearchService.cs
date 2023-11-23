﻿using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using AutoMapper;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Nest;
using VGStandard.Core.Metadata;
using VGStandard.Core.Settings;
using VGStandard.Core.Utility;

namespace VGStandard.Common.Web.Services;

public interface IElasticSearchService
{
    string Index<T>(T model, string index) where T : class;
    Task<string> IndexAsync<T>(T model, string index) where T : class;
    Task<StandardResponse<T>> Search<T>(object searchRequest, string index) where T : class;
    Task<List<string>> IndexManyAsync<T>(IEnumerable<T> model, string index) where T : class;
    List<string> IndexMany<T>(IEnumerable<T> model, string index) where T : class;
    Task<int> UpdateAsync<T>(T model, string index) where T : class;
    Task<HttpResponseMessage> DeleteIndexAsync(string index);
    Task<HttpResponseMessage> CreateIndexAsync(string indexName, string mappingJson);
}

public class ElasticSearchService : IElasticSearchService
{

    private readonly ElasticClient _client;
    private readonly IMapper _mapper;
    private readonly AppSettings _settings;
    private readonly string _elasticSearchUrl;
    private readonly string _apiKey;

    public ElasticSearchService(IOptions<AppSettings> options, IMapper mapper, string elasticSearchUrl = null, string apiKey = null)
    {
        _mapper = mapper;
        _settings = options.Value;
        _elasticSearchUrl = elasticSearchUrl ?? _settings.ConnectionStrings.ElasticSearch;
        _apiKey = apiKey ?? _settings.ElasticSearchApiKey;
        _client = new ElasticClient(GetConfigSettings());
    }

    private ConnectionSettings GetConfigSettings()
    {
        var urls = _elasticSearchUrl.Split(',').Select(url => new Uri(url));
        var pool = new StaticConnectionPool(urls);
        var configSettings = new ConnectionSettings(pool)
            .DefaultFieldNameInferrer(fieldName => fieldName)
            .RequestTimeout(TimeSpan.FromMinutes(1))
            .DisableDirectStreaming()
            .BasicAuthentication("elastic", _apiKey);

        return configSettings;
    }

    public async Task<HttpResponseMessage> CreateIndexAsync(string indexName, string mappingJson)
    {
        var content = new StringContent(mappingJson, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json; charset=utf-8").MediaType);
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(_settings.ConnectionStrings.ElasticSearch);
            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("elastic" + ":" + _settings.ElasticSearchApiKey));
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {svcCredentials}");
            return await client.PutAsync(indexName, content);
        }
    }

    public async Task<HttpResponseMessage> DeleteIndexAsync(string indexName)
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(_settings.ConnectionStrings.ElasticSearch);
            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("elastic" + ":" + _settings.ElasticSearchApiKey));
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {svcCredentials}");
            return await client.DeleteAsync(indexName);
        }
    }

    public async Task<int> UpdateAsync<T>(T model, string index) where T : class
    {
        var value = model.GetType().GetProperties().SingleOrDefault(p => p.Name == "Identifier").GetValue(model);
        var response = await _client.UpdateAsync<T>(value.ToString(), u => u.Doc(model).Index(index));
        return response.IsValid ? 1 : 0;
    }

    public string Index<T>(T model, string index) where T : class
    {
        var value = model.GetType().GetProperties().Single(p => p.Name == "Identifier").GetValue(model);
        var result = _client.Index(model, m => m.Id(new Id(value.ToString())).Index(index));
        return result.Id;
    }

    public async Task<string> IndexAsync<T>(T model, string index) where T : class
    {
        var value = model.GetType().GetProperties().Single(p => p.Name == "Identifier").GetValue(model);
        var result = await _client.IndexAsync(model, m => m.Id(new Id(value.ToString())).Index(index));
        return result.Id;
    }

    public async Task<List<string>> IndexManyAsync<T>(IEnumerable<T> model, string index) where T : class
    {
        var value = model.GetType().GetProperties().Single(p => p.Name == "Identifier").GetValue(model);
        var result = await _client.IndexManyAsync(model, index);
        return result.Items.Select(i => i.Id).ToList();
    }

    public List<string> IndexMany<T>(IEnumerable<T> model, string index) where T : class
    {
        var result = _client.IndexMany(model, index);
        return result.Items.Select(i => i.Id).ToList();
    }

    public async Task<StandardResponse<T>> Search<T>(object searchRequest, string index) where T : class
    {
        var result = await _client.SearchAsync<T>(q => q
                                                .Sort(s2 => Sort<T>(((IApiRequest)searchRequest).Sort?.Split(',')))
                                                .Query(y => Query(searchRequest))
                                                .TrackScores()
                                                .Source(s => new SourceFilter { Includes = ((IApiRequest)searchRequest)?.Fields?.Replace(" ", "") })
                                                .From(((IApiRequest)searchRequest)?.CurrentPage)
                                                .Index(index)
                                                .Take(((IApiRequest)searchRequest)?.PageSize));

        return new StandardResponse<T>()
        {
            Total = result.Total,
            Data = _mapper.Map<List<T>>(result.Documents)
        };
    }

    private QueryContainer Query<T>(T request)
    {
        QueryContainer container = new QueryContainer();

        var nonInheritedProperties = request.GetNonInheritedProperties();
        foreach (var property in nonInheritedProperties)
        {
            var value = property.GetValue(request);
            if (IsLookup(property, value))
            {
                container &= (new MatchQuery
                {
                    Field = property.Name,
                    Query = (value.ToString() == "True" || value.ToString() == "False") ? value.ToString().ToLower() : value.ToString()
                });
            }
            else if (IsSearchable(property, value))
            {
                container &= (new WildcardQuery
                {
                    Field = property.Name,
                    Value = $"*{value.ToString().ToLower()}*"
                });
            }
        }

        return container;
    }

    private SortDescriptor<IApiRequest> Sort<T>(IEnumerable<string> sortStrings)
    {
        var descriptor = new SortDescriptor<IApiRequest>();
        if (sortStrings?.Any() ?? false)
        {
            foreach (var sortString in sortStrings)
            {
                if (typeof(T).GetProperty(SortablePropertyName(sortString)) != null)
                {
                    descriptor.Field(SortablePropertyName(sortString), (sortString[0] == '-') ? SortOrder.Descending : SortOrder.Ascending);
                }
            }
        }
        return descriptor;
    }

    private string SortablePropertyName(string sortString)
    {
        return sortString.Replace("-", "").Replace("+", "");
    }

    private bool IsSearchable(PropertyInfo property, object value)
    {
        var searchable = false;
        var hasValidSearchableValue =
            value != null &&
            !string.IsNullOrWhiteSpace(value?.ToString()) &&
            value.ToString() != "0";


        switch (property.Name.ToLower())
        {
            case "hostname":
                searchable = true;
                break;
        }

        return searchable;
    }

    private bool IsLookup(PropertyInfo property, object value)
    {
        var isLookup = false;
        //tired of fighting this property interogation battle, resorting to infinite if....
        switch (property.Name)
        {
            case "Id":
                long.TryParse(value?.ToString(), out var i);
                isLookup = i > 0;
                break;
            case "ApplicationUserId":
                isLookup = value != null && !string.IsNullOrWhiteSpace(value?.ToString());
                break;
            case "IsActive":
                isLookup = true;
                break;
            default:
                isLookup = false;
                break;
        }
        return isLookup;
    }

    private bool IsNumberGreaterThanZero(object value)
    {
        bool isNumber = long.TryParse(value.ToString(), out var i);
        return isNumber && i > 0;
    }

}
