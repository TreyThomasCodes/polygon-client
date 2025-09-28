// Copyright 2025 Trey Thomas
// SPDX-License-Identifier: MPL-2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using System.Text.Json;
using TTC.Polygon.RestClient.Api;
using TTC.Polygon.RestClient.Authentication;
using TTC.Polygon.RestClient.Configuration;
using TTC.Polygon.RestClient.Services;

namespace TTC.Polygon.RestClient.Extensions;

/// <summary>
/// Extension methods for configuring Polygon.io client services in the dependency injection container.
/// Provides multiple overloads for different configuration scenarios including direct API key, configuration sections, and action-based configuration.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Polygon.io client services to the dependency injection container using configuration from the "Polygon" section.
    /// This method automatically looks for the configuration in the standard "Polygon" section of the provided configuration.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration containing Polygon.io settings in the "Polygon" section.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddPolygonClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddPolygonClient(configuration.GetSection(PolygonOptions.SectionName));
    }

    /// <summary>
    /// Adds Polygon.io client services to the dependency injection container using a specific configuration section.
    /// This method allows you to specify a custom configuration section for Polygon.io settings.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configurationSection">The configuration section containing Polygon.io settings.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddPolygonClient(
        this IServiceCollection services,
        IConfigurationSection configurationSection)
    {
        services.Configure<PolygonOptions>(configurationSection);
        return services.AddPolygonClientCore();
    }

    /// <summary>
    /// Adds Polygon.io client services to the dependency injection container using an action to configure options.
    /// This method allows you to configure Polygon.io options programmatically using a configuration action.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configureOptions">An action to configure the Polygon.io options.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddPolygonClient(
        this IServiceCollection services,
        Action<PolygonOptions> configureOptions)
    {
        services.Configure(configureOptions);
        return services.AddPolygonClientCore();
    }

    /// <summary>
    /// Adds Polygon.io client services to the dependency injection container using direct API key configuration.
    /// This is the simplest method for adding Polygon.io services when you have the API key available directly.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="apiKey">The Polygon.io API key for authentication.</param>
    /// <param name="baseUrl">Optional custom base URL for the Polygon.io API. Defaults to https://api.polygon.io if not specified.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddPolygonClient(
        this IServiceCollection services,
        string apiKey,
        string? baseUrl = null)
    {
        services.Configure<PolygonOptions>(options =>
        {
            options.ApiKey = apiKey;
            if (!string.IsNullOrEmpty(baseUrl))
            {
                options.BaseUrl = baseUrl;
            }
        });
        return services.AddPolygonClientCore();
    }

    private static IServiceCollection AddPolygonClientCore(this IServiceCollection services)
    {
        services.AddTransient<PolygonAuthenticationHandler>();

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new TTC.Polygon.Models.Json.ULongScientificNotationConverter() }
        };

        var settings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(options),
        };

        services.AddRefitClient<IPolygonStocksApi>(settings)
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<PolygonOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = options.Timeout;
            })
            .AddHttpMessageHandler<PolygonAuthenticationHandler>();

        services.AddRefitClient<IPolygonReferenceApi>(settings)
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<PolygonOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = options.Timeout;
            })
            .AddHttpMessageHandler<PolygonAuthenticationHandler>();

        services.AddTransient<IStocksService, StocksService>();
        services.AddTransient<IReferenceDataService, ReferenceDataService>();
        services.AddTransient<IPolygonClient, PolygonClient>();

        return services;
    }
}