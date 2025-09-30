// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Options;
using TreyThomasCodes.Polygon.RestClient.Configuration;

namespace TreyThomasCodes.Polygon.RestClient.Authentication;

/// <summary>
/// HTTP message handler that automatically adds Polygon.io API key authentication to outgoing requests.
/// This handler intercepts HTTP requests and adds the API key as a Bearer token in the Authorization header.
/// </summary>
public class PolygonAuthenticationHandler : DelegatingHandler
{
    private readonly PolygonOptions _options;

    /// <summary>
    /// Initializes a new instance of the PolygonAuthenticationHandler with the specified options.
    /// </summary>
    /// <param name="options">The configuration options containing the Polygon.io API key.</param>
    /// <exception cref="ArgumentNullException">Thrown when the options parameter is null.</exception>
    public PolygonAuthenticationHandler(IOptions<PolygonOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Sends an HTTP request with Polygon.io API key authentication added as a Bearer token in the Authorization header.
    /// This method intercepts the request, adds the API key, and forwards it to the next handler in the pipeline.
    /// </summary>
    /// <param name="request">The HTTP request message to authenticate and send.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the HTTP response message.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the Polygon API key is not configured or is empty.</exception>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException("Polygon API key is not configured.");
        }

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.ApiKey);

        return await base.SendAsync(request, cancellationToken);
    }
}