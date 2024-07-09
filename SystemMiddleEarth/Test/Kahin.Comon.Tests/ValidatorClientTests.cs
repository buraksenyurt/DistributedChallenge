using System.Net;
using System.Net.Http.Json;
using Kahin.Common.Requests;
using Kahin.Common.Responses;
using Kahin.Common.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace Kahin.Common.Tests;

public class ValidatorClientTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _mockHttpClient;
    private readonly Mock<ILogger<ValidatorClient>> _mockLogger;

    public ValidatorClientTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockHttpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://mockaddress/api")
        };
        _mockLogger = new Mock<ILogger<ValidatorClient>>();
    }

    [Fact]
    public async Task ValidateExpression_Returns_True_When_ResponseIsValid()
    {
        // Arrange
        var request = new CreateReportRequest
        {
            Expression = "Geçtiğimiz yıl en iyi yorum ala ilk 10 oyunun ciro değerleri."
        };
        var responseContent = new ExpressionCheckResponse { IsValid = true };
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(responseContent)
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponseMessage);

        var validatorClient = new ValidatorClient(_mockHttpClient, _mockLogger.Object);

        // Act
        var result = await validatorClient.ValidateExpression(request);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ValidateExpression_Returns_False_When_ResponseIsInvalid()
    {
        // Arrange
        var request = new CreateReportRequest
        {
            Expression = "Oyunlara en çok harcama yapan ilk 1000 müşterinin e-posta bilgileri."
        };
        var responseContent = new ExpressionCheckResponse { IsValid = false };
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(responseContent)
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponseMessage);

        var validatorClient = new ValidatorClient(_mockHttpClient, _mockLogger.Object);

        // Act
        var result = await validatorClient.ValidateExpression(request);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateExpression_Returns_False_When_ResponseIsNotSuccess()
    {
        // Arrange
        var request = new CreateReportRequest
        {
            Expression = "Kategori bazlı oyun kiralama rakamlarının yıl bazlı değerlendirmeleri."
        };
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponseMessage);

        var validatorClient = new ValidatorClient(_mockHttpClient, _mockLogger.Object);

        // Act
        var result = await validatorClient.ValidateExpression(request);

        // Assert
        Assert.False(result);
    }
}
