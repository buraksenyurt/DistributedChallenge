using System.Net;
using System.Text;
using System.Text.Json;
using GamersWorld.Events;
using GamersWorld.Common.Enums;
using GamersWorld.Common.Responses;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace GamersWorld.EventBusiness.Tests;

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
    public async Task Should_Log_Success_Message_When_Response_Is_Success()
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
        await _postReportRequest.Execute(eventPayload);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Report request sent")),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()
        ), Times.Once);
    }

    [Fact]
    public async Task Should_Log_Error_Message_When_Api_Fails()
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
        await _postReportRequest.Execute(eventPayload);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Report request unsuccessful")),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()
        ), Times.Once);
    }

    [Fact]
    public async Task Should_Log_Error_Message_When_Response_Status_Is_Fail()
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
        await _postReportRequest.Execute(eventPayload);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Report request sent")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()
            ), Times.Never);

        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Report request unsuccessful")),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()
        ), Times.Once);
    }
}