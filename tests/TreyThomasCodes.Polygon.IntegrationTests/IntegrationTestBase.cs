// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TreyThomasCodes.Polygon.RestClient.Extensions;
using TreyThomasCodes.Polygon.RestClient.Services;

namespace TreyThomasCodes.Polygon.IntegrationTests;

/// <summary>
/// Base class for integration tests that provides common setup and teardown logic.
/// Handles dependency injection, Polygon client configuration, and resource disposal.
/// </summary>
public abstract class IntegrationTestBase : IDisposable
{
    private bool _disposedValue;

    /// <summary>
    /// Gets the host container for dependency injection.
    /// </summary>
    protected IHost Host { get; }

    /// <summary>
    /// Gets the configured Polygon client for API calls.
    /// </summary>
    protected IPolygonClient PolygonClient { get; }

    /// <summary>
    /// Initializes a new instance of the IntegrationTestBase class.
    /// Sets up the host with dependency injection and Polygon client configuration.
    /// </summary>
    protected IntegrationTestBase()
    {
        var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder();
        builder.Configuration.AddUserSecrets<IntegrationTestBase>();
        var apiKey = builder.Configuration["Polygon:ApiKey"];

        // Skip all tests in derived classes if no API key is configured
        Assert.SkipUnless(!string.IsNullOrEmpty(apiKey),
            "Polygon API key not configured in user secrets. Use: dotnet user-secrets set \"Polygon:ApiKey\" \"your-api-key-here\"");

        builder.Services.AddPolygonClient(options =>
        {
            options.ApiKey = apiKey!;
        });

        Host = builder.Build();
        PolygonClient = Host.Services.GetRequiredService<IPolygonClient>();
    }

    /// <summary>
    /// Disposes of managed resources.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Host.Dispose();
            }

            _disposedValue = true;
        }
    }

    /// <summary>
    /// Disposes of resources used by this instance.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
