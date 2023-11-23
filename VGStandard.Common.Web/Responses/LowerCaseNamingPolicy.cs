using Newtonsoft.Json.Serialization;
using System.Text.Json;

namespace VGStandard.Common.Web.Responses;

public class LowerCaseNamingPolicy : JsonNamingPolicy, IContractResolver
{
    public override string ConvertName(string name) => name.ToLower();

    public JsonContract ResolveContract(Type type)
    {
        throw new NotImplementedException();
    }
}
