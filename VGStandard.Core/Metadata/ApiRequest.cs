using System.ComponentModel.DataAnnotations;
using VGStandard.Application;
using VGStandard.Core.Metadata.Paging;

namespace VGStandard.Core.Metadata;

public interface IApiRequest : IFieldSelectable, ISortable, IPageable
{

}
/// <summary>
/// Represents a standard api request.
/// Supports sorting, paging, and field selection parameters.
/// </summary>
public class ApiRequest : IApiRequest, ISortable, IPageable
{
	/// <summary>
	/// The total number of records to return.
	/// If not specified then a default number of records will be returned.
	/// </summary>
	[Range(1, Constants.MaxPageSize)]
	public int? PageSize { get; init; } = 100;

	/// <summary>
	/// The page to start returning records
	/// If not specified an offset of 1 will be used.
	/// </summary>
	public int? CurrentPage { get; init; } = 1;

	/// <summary>
	/// The fields to sort by.
	/// If multiple sort parameters, the list should be comma separated.
	/// Use - to indicate a descending sort.
	/// Use + to indicate an ascending sort.
	/// Consecutive sorts are respected, ie; +a,-b,+c, etc..
	/// If not specified then a default sort will be used.
	/// </summary>
	[StringLength(200)]
	public string? Sort { get; set; } = "-id";

	/// <summary>
	/// The fields to return.
	/// If multiple, fields must be comma separated.
	/// If not specified, then all fields will be returned.
    /// Field selection allows for only entire subobjects; no further.
	/// </summary>
	[StringLength(500)]
	public string? Fields { get; init; }
}

/// <summary>
/// Extension methods to perform sorting, paging, and field selection operations
/// off the <see cref="ApiRequest"/> class.
/// </summary>
public static class RequestableExtensions
{
	/// <summary>
	/// Returns a page of data based on the <see cref="IPageable.CurrentPage"/>
	/// and <see cref="IPageable.PageSize"/> properties.
	/// </summary>
	/// <typeparam name="T">The type of the elements of source.</typeparam>
	public static IQueryable<T> Paginate<T>(this IQueryable<T> source, ApiRequest paging)
	{
		int offset = paging.Offset();
		int limit = paging.PageSize ?? Constants.MaxPageSize;
		return source
			.Skip(offset)
			.Take(limit);
	}
}
