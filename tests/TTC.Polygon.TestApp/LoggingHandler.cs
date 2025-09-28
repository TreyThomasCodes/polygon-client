using Microsoft.Extensions.Logging;
using System.Text;

namespace TTC.Polygon.TestApp;

public class LoggingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingHandler> _logger;

    public LoggingHandler(ILogger<LoggingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Log request
        await LogRequestAsync(request);

        // Send the request
        var response = await base.SendAsync(request, cancellationToken);

        // Log response
        await LogResponseAsync(response);

        return response;
    }

    private async Task LogRequestAsync(HttpRequestMessage request)
    {
        var requestBody = string.Empty;
        if (request.Content != null)
        {
            requestBody = await request.Content.ReadAsStringAsync();
        }

        _logger.LogInformation("HTTP Request: {Method} {Uri}", request.Method, request.RequestUri);
        _logger.LogInformation("Request Headers: {Headers}", FormatHeaders(request.Headers));

        if (!string.IsNullOrEmpty(requestBody))
        {
            _logger.LogInformation("Request Body: {Body}", requestBody);
        }
    }

    private async Task LogResponseAsync(HttpResponseMessage response)
    {
        var responseBody = string.Empty;
        if (response.Content != null)
        {
            responseBody = await response.Content.ReadAsStringAsync();
        }

        _logger.LogInformation("HTTP Response: {StatusCode} {ReasonPhrase}", (int)response.StatusCode, response.ReasonPhrase);
        _logger.LogInformation("Response Headers: {Headers}", FormatHeaders(response.Headers));

        if (!string.IsNullOrEmpty(responseBody))
        {
            _logger.LogInformation("Response Body: {Body}", responseBody);
        }
    }

    private static string FormatHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
    {
        var sb = new StringBuilder();
        foreach (var header in headers)
        {
            sb.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
        }
        return sb.ToString();
    }
}