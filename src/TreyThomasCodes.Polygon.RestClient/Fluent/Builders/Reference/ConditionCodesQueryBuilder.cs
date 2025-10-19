// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using TreyThomasCodes.Polygon.Models.Common;
using TreyThomasCodes.Polygon.Models.Reference;
using TreyThomasCodes.Polygon.RestClient.Requests.Reference;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.RestClient.Fluent.Builders.Reference;

/// <summary>
/// Fluent query builder for constructing and executing condition codes requests.
/// This builder provides a progressive, chainable API for retrieving trade and quote condition codes.
/// </summary>
public class ConditionCodesQueryBuilder
{
    private readonly IReferenceDataService _service;
    private AssetClass? _assetClass;
    private DataType? _dataType;
    private string? _id;
    private SipMappingType? _sipMapping;
    private SortOrder? _order;
    private int? _limit;
    private string? _sort;

    /// <summary>
    /// Initializes a new instance of the ConditionCodesQueryBuilder.
    /// </summary>
    /// <param name="service">The reference data service to execute the request against.</param>
    public ConditionCodesQueryBuilder(IReferenceDataService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <summary>
    /// Filters condition codes by asset class.
    /// </summary>
    /// <param name="assetClass">The asset class to filter by.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ConditionCodesQueryBuilder ForAssetClass(AssetClass assetClass)
    {
        _assetClass = assetClass;
        return this;
    }

    /// <summary>
    /// Filters condition codes by data type.
    /// </summary>
    /// <param name="dataType">The data type to filter by.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ConditionCodesQueryBuilder ForDataType(DataType dataType)
    {
        _dataType = dataType;
        return this;
    }

    /// <summary>
    /// Filters condition codes by ID. Can be a single ID or comma-separated list of IDs.
    /// </summary>
    /// <param name="id">The condition ID or IDs to filter by.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ConditionCodesQueryBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    /// <summary>
    /// Filters condition codes by SIP mapping type.
    /// </summary>
    /// <param name="sipMapping">The SIP mapping type to filter by.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ConditionCodesQueryBuilder WithSipMapping(SipMappingType sipMapping)
    {
        _sipMapping = sipMapping;
        return this;
    }

    /// <summary>
    /// Specifies the field to sort results by.
    /// </summary>
    /// <param name="field">The field name to sort by. Default is "asset_class".</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ConditionCodesQueryBuilder SortBy(string field)
    {
        _sort = field;
        return this;
    }

    /// <summary>
    /// Sets the sort order to ascending.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public ConditionCodesQueryBuilder Ascending()
    {
        _order = SortOrder.Ascending;
        return this;
    }

    /// <summary>
    /// Sets the sort order to descending.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public ConditionCodesQueryBuilder Descending()
    {
        _order = SortOrder.Descending;
        return this;
    }

    /// <summary>
    /// Limits the number of results returned.
    /// </summary>
    /// <param name="limit">The maximum number of results to return (maximum 1000).</param>
    /// <returns>The builder instance for method chaining.</returns>
    public ConditionCodesQueryBuilder Limit(int limit)
    {
        _limit = limit;
        return this;
    }

    /// <summary>
    /// Executes the query asynchronously and returns the results.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the condition codes response.</returns>
    public Task<PolygonResponse<List<ConditionCode>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var request = new GetConditionCodesRequest
        {
            AssetClass = _assetClass,
            DataType = _dataType,
            Id = _id,
            SipMapping = _sipMapping,
            Order = _order,
            Limit = _limit,
            Sort = _sort
        };

        return _service.GetConditionCodesAsync(request, cancellationToken);
    }
}
