using GamersWorld.AppEvents;
using GamersWorld.Common.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace GamersWorld.AppEventBusiness.Tests;

public class PostReportRequestTests
{
    private readonly Mock<ILogger<PostReportRequest>> _loggerMock;
    private readonly PostReportRequest _postReportRequest;
    public PostReportRequestTests()
    {
        _loggerMock = new Mock<ILogger<PostReportRequest>>();
        _postReportRequest = new PostReportRequest(_loggerMock.Object);
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

        // Act
        var result = await _postReportRequest.Execute(eventPayload);

        // Assert
        Assert.Equal(StatusCode.Success, result.StatusCode);
        Assert.Equal("Rapor talebi gönderildi", result.Message);
    }
}