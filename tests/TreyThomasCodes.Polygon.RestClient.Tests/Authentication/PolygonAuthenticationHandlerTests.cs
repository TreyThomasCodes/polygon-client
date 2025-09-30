using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using System.Net.Http.Headers;
using TreyThomasCodes.Polygon.RestClient.Authentication;
using TreyThomasCodes.Polygon.RestClient.Configuration;

namespace TreyThomasCodes.Polygon.RestClient.Tests.Authentication;

/// <summary>
/// Unit tests for the PolygonAuthenticationHandler class.
/// Tests the authentication handler's ability to add Bearer token authentication to HTTP requests.
/// </summary>
public class PolygonAuthenticationHandlerTests
{
    private const string TestApiKey = "test-api-key-12345";
    private const string TestUrl = "https://test.server/endpoint";

    /// <summary>
    /// Creates a mock IOptions&lt;PolygonOptions&gt; with the specified API key.
    /// </summary>
    /// <param name="apiKey">The API key to configure in the options.</param>
    /// <returns>A mock IOptions&lt;PolygonOptions&gt; instance.</returns>
    private static IOptions<PolygonOptions> CreateMockOptions(string? apiKey)
    {
        var options = new PolygonOptions { ApiKey = apiKey! };
        var mockOptions = new Mock<IOptions<PolygonOptions>>();
        mockOptions.Setup(x => x.Value).Returns(options);
        return mockOptions.Object;
    }

    /// <summary>
    /// A simple test handler that records the request and returns a success response.
    /// </summary>
    private class TestInnerHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        public HttpResponseMessage ResponseToReturn { get; set; } = new(HttpStatusCode.OK);

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(ResponseToReturn);
        }
    }

    /// <summary>
    /// Tests that the constructor throws ArgumentNullException when options is null.
    /// </summary>
    [Fact]
    public void Constructor_WhenOptionsIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PolygonAuthenticationHandler(null!));
    }

    /// <summary>
    /// Tests that SendAsync adds the correct Authorization header with Bearer token.
    /// </summary>
    [Fact]
    public async Task SendAsync_WithValidApiKey_AddsAuthorizationHeader()
    {
        // Arrange
        var options = CreateMockOptions(TestApiKey);
        var innerHandler = new TestInnerHandler();
        var handler = new PolygonAuthenticationHandler(options)
        {
            InnerHandler = innerHandler
        };

        var request = new HttpRequestMessage(HttpMethod.Get, TestUrl);
        var client = new HttpClient(handler);

        // Act
        await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        var capturedRequest = innerHandler.LastRequest;
        Assert.NotNull(capturedRequest);
        Assert.NotNull(capturedRequest.Headers.Authorization);
        Assert.Equal("Bearer", capturedRequest.Headers.Authorization.Scheme);
        Assert.Equal(TestApiKey, capturedRequest.Headers.Authorization.Parameter);
    }

    /// <summary>
    /// Tests that SendAsync throws InvalidOperationException when API key is null.
    /// </summary>
    [Fact]
    public async Task SendAsync_WithNullApiKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var options = CreateMockOptions(null);
        var innerHandler = new TestInnerHandler();
        var handler = new PolygonAuthenticationHandler(options)
        {
            InnerHandler = innerHandler
        };

        var request = new HttpRequestMessage(HttpMethod.Get, TestUrl);
        var client = new HttpClient(handler);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => client.SendAsync(request, TestContext.Current.CancellationToken));
        Assert.Equal("Polygon API key is not configured.", exception.Message);
    }

    /// <summary>
    /// Tests that SendAsync throws InvalidOperationException when API key is empty string.
    /// </summary>
    [Fact]
    public async Task SendAsync_WithEmptyApiKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var options = CreateMockOptions(string.Empty);
        var innerHandler = new TestInnerHandler();
        var handler = new PolygonAuthenticationHandler(options)
        {
            InnerHandler = innerHandler
        };

        var request = new HttpRequestMessage(HttpMethod.Get, TestUrl);
        var client = new HttpClient(handler);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => client.SendAsync(request, TestContext.Current.CancellationToken));
        Assert.Equal("Polygon API key is not configured.", exception.Message);
    }

    /// <summary>
    /// Tests that SendAsync throws InvalidOperationException when API key is whitespace only.
    /// </summary>
    [Fact]
    public async Task SendAsync_WithWhitespaceApiKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var options = CreateMockOptions("   ");
        var innerHandler = new TestInnerHandler();
        var handler = new PolygonAuthenticationHandler(options)
        {
            InnerHandler = innerHandler
        };

        var request = new HttpRequestMessage(HttpMethod.Get, TestUrl);
        var client = new HttpClient(handler);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => client.SendAsync(request, TestContext.Current.CancellationToken));
        Assert.Equal("Polygon API key is not configured.", exception.Message);
    }

    /// <summary>
    /// Tests that SendAsync preserves existing headers while adding Authorization header.
    /// </summary>
    [Fact]
    public async Task SendAsync_WithExistingHeaders_PreservesExistingHeaders()
    {
        // Arrange
        var options = CreateMockOptions(TestApiKey);
        var innerHandler = new TestInnerHandler();
        var handler = new PolygonAuthenticationHandler(options)
        {
            InnerHandler = innerHandler
        };

        var request = new HttpRequestMessage(HttpMethod.Get, TestUrl);
        request.Headers.Add("User-Agent", "Test-RestClient/1.0");
        request.Headers.Add("Accept", "application/json");
        var client = new HttpClient(handler);

        // Act
        await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        var capturedRequest = innerHandler.LastRequest;
        Assert.NotNull(capturedRequest);
        Assert.NotNull(capturedRequest.Headers.Authorization);
        Assert.Equal("Bearer", capturedRequest.Headers.Authorization.Scheme);
        Assert.Equal(TestApiKey, capturedRequest.Headers.Authorization.Parameter);
        Assert.Contains("Test-RestClient/1.0", capturedRequest.Headers.GetValues("User-Agent"));
        Assert.Contains("application/json", capturedRequest.Headers.GetValues("Accept"));
    }

    /// <summary>
    /// Tests that SendAsync overwrites existing Authorization header with new Bearer token.
    /// </summary>
    [Fact]
    public async Task SendAsync_WithExistingAuthorizationHeader_OverwritesWithBearerToken()
    {
        // Arrange
        var options = CreateMockOptions(TestApiKey);
        var innerHandler = new TestInnerHandler();
        var handler = new PolygonAuthenticationHandler(options)
        {
            InnerHandler = innerHandler
        };

        var request = new HttpRequestMessage(HttpMethod.Get, TestUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", "old-token");
        var client = new HttpClient(handler);

        // Act
        await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        var capturedRequest = innerHandler.LastRequest;
        Assert.NotNull(capturedRequest);
        Assert.NotNull(capturedRequest.Headers.Authorization);
        Assert.Equal("Bearer", capturedRequest.Headers.Authorization.Scheme);
        Assert.Equal(TestApiKey, capturedRequest.Headers.Authorization.Parameter);
        Assert.NotEqual("Basic", capturedRequest.Headers.Authorization.Scheme);
        Assert.NotEqual("old-token", capturedRequest.Headers.Authorization.Parameter);
    }

    /// <summary>
    /// Tests that SendAsync handles different HTTP methods correctly.
    /// </summary>
    [Theory]
    [InlineData("GET")]
    [InlineData("POST")]
    [InlineData("PUT")]
    [InlineData("DELETE")]
    [InlineData("PATCH")]
    public async Task SendAsync_WithDifferentHttpMethods_AddsAuthorizationHeader(string httpMethod)
    {
        // Arrange
        var options = CreateMockOptions(TestApiKey);
        var innerHandler = new TestInnerHandler();
        var handler = new PolygonAuthenticationHandler(options)
        {
            InnerHandler = innerHandler
        };

        var method = new HttpMethod(httpMethod);
        var request = new HttpRequestMessage(method, TestUrl);
        var client = new HttpClient(handler);

        // Act
        await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        var capturedRequest = innerHandler.LastRequest;
        Assert.NotNull(capturedRequest);
        Assert.NotNull(capturedRequest.Headers.Authorization);
        Assert.Equal("Bearer", capturedRequest.Headers.Authorization.Scheme);
        Assert.Equal(TestApiKey, capturedRequest.Headers.Authorization.Parameter);
    }

    /// <summary>
    /// Tests that SendAsync properly forwards the request to the inner handler and returns the response.
    /// </summary>
    [Fact]
    public async Task SendAsync_CallsInnerHandler_AndReturnsResponse()
    {
        // Arrange
        var expectedContent = "Test response content";
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(expectedContent)
        };

        var options = CreateMockOptions(TestApiKey);
        var innerHandler = new TestInnerHandler
        {
            ResponseToReturn = expectedResponse
        };
        var handler = new PolygonAuthenticationHandler(options)
        {
            InnerHandler = innerHandler
        };

        var request = new HttpRequestMessage(HttpMethod.Post, TestUrl);
        var client = new HttpClient(handler);

        // Act
        var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(expectedContent, await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        // Verify the request was processed
        var capturedRequest = innerHandler.LastRequest;
        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest.Method);
        Assert.Equal(TestUrl, capturedRequest.RequestUri?.ToString());
        Assert.NotNull(capturedRequest.Headers.Authorization);
        Assert.Equal("Bearer", capturedRequest.Headers.Authorization.Scheme);
        Assert.Equal(TestApiKey, capturedRequest.Headers.Authorization.Parameter);
    }

    /// <summary>
    /// Tests that SendAsync preserves the original request URI.
    /// </summary>
    [Fact]
    public async Task SendAsync_PreservesRequestUri()
    {
        // Arrange
        var options = CreateMockOptions(TestApiKey);
        var innerHandler = new TestInnerHandler();
        var handler = new PolygonAuthenticationHandler(options)
        {
            InnerHandler = innerHandler
        };

        var originalUri = new Uri(TestUrl);
        var request = new HttpRequestMessage(HttpMethod.Get, originalUri);
        var client = new HttpClient(handler);

        // Act
        await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        var capturedRequest = innerHandler.LastRequest;
        Assert.NotNull(capturedRequest);
        Assert.Equal(originalUri, capturedRequest.RequestUri);
    }

    /// <summary>
    /// Tests that SendAsync works with API keys containing special characters.
    /// </summary>
    [Fact]
    public async Task SendAsync_WithSpecialCharactersInApiKey_AddsAuthorizationHeader()
    {
        // Arrange
        var specialApiKey = "test-key_123!@#$%^&*()+=";
        var options = CreateMockOptions(specialApiKey);
        var innerHandler = new TestInnerHandler();
        var handler = new PolygonAuthenticationHandler(options)
        {
            InnerHandler = innerHandler
        };

        var request = new HttpRequestMessage(HttpMethod.Get, TestUrl);
        var client = new HttpClient(handler);

        // Act
        await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        var capturedRequest = innerHandler.LastRequest;
        Assert.NotNull(capturedRequest);
        Assert.NotNull(capturedRequest.Headers.Authorization);
        Assert.Equal("Bearer", capturedRequest.Headers.Authorization.Scheme);
        Assert.Equal(specialApiKey, capturedRequest.Headers.Authorization.Parameter);
    }
}