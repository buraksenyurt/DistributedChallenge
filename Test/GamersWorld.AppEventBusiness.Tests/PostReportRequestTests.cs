using System.Net;
using System.Text;
using System.Text.Json;
using GamersWorld.AppEvents;
using GamersWorld.Common.Enums;
using GamersWorld.Common.Messages.Responses;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace GamersWorld.AppEventBusiness.Tests;

public class PostReportRequestTests
{
    private readonly Mock<ILogger<PostReportRequest>> _loggerMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly PostReportRequest _postReportRequest;

    public PostReportRequestTests()
    {
        _loggerMock = new Mock<ILogger<PostReportRequest>>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost:5218")
        };

        _httpClientFactoryMock
            .Setup(factory => factory.CreateClient(It.IsAny<string>()))
            .Returns(_httpClient);

        _postReportRequest = new PostReportRequest(_loggerMock.Object, _httpClientFactoryMock.Object);
    }

    [Fact]
    public async Task Should_Return_Success_Response_Test()
    {
        // Arrange
        var eventPayload = new ReportRequestedEvent
        {
            TraceId = Guid.NewGuid(),
            Title = "Yıllık bazda en iyi yorum alan oyun satışları",
            Expression = "SELECT * FROM Reports WHERE CategoryId=1 ORDER BY Id Desc"
        };

        var createReportResponse = new CreateReportResponse
        {
            Status = StatusCode.Success,
            DocumentId = "1001-12-edd4e07d-2391-47c1-bf6f-96a96c447585"
        };

        var responseContent = new StringContent(
            JsonSerializer.Serialize(createReportResponse),
            Encoding.UTF8,
            "application/json"
        );

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = responseContent
            });

        // Act
        var result = await _postReportRequest.Execute(eventPayload);

        // Assert
        Assert.Equal(StatusCode.Success, result.StatusCode);
        Assert.Equal($"Report request sent. DocumentId: {createReportResponse.DocumentId}", result.Message);
    }

    [Fact]
    public async Task Should_Return_Fail_Response_When_Api_Fail_Test()
    {
        // Arrange
        var eventPayload = new ReportRequestedEvent
        {
            TraceId = Guid.NewGuid(),
            Title = "Yıllık bazda en iyi yorum alan oyun satışları",
            Expression = "SELECT * FROM Reports WHERE CategoryId=1 ORDER BY Id Desc"
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
            });

        // Act
        var result = await _postReportRequest.Execute(eventPayload);

        // Assert
        Assert.Equal(StatusCode.Fail, result.StatusCode);
        Assert.Equal("Report request unsuccessful.", result.Message);
    }

    [Fact]
    public async Task Should_Return_Fail_Response_When_Api_Returns_Fail_Status_Test()
    {
        // Arrange
        var eventPayload = new ReportRequestedEvent
        {
            TraceId = Guid.NewGuid(),
            Title = "Yıllık bazda en iyi yorum alan oyun satışları",
            Expression = "SELECT * FROM Reports WHERE CategoryId=1 ORDER BY Id Desc"
        };

        var createReportResponse = new CreateReportResponse
        {
            Status = StatusCode.Fail,
        };

        var responseContent = new StringContent(
            JsonSerializer.Serialize(createReportResponse),
            Encoding.UTF8,
            "application/json"
        );

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = responseContent
            });

        // Act
        var result = await _postReportRequest.Execute(eventPayload);

        // Assert
        Assert.Equal(StatusCode.Fail, result.StatusCode);
    }
}