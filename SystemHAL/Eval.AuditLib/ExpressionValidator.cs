using Eval.AuditLib.Contracts;
using Eval.AuditLib.Model;
using Microsoft.Extensions.Logging;

namespace Eval.Lib;

public class ExpressionValidator
    : IExpressionValidator
{
    private readonly ILogger<ExpressionValidator> _logger;
    public ExpressionValidator(ILogger<ExpressionValidator> logger)
    {
        _logger = logger;
    }

    public ExpressionCheckResponse IsValid(ExpressionCheckRequest request)
    {
        if (string.IsNullOrEmpty(request.Expression))
        {
            _logger.LogError("Expression boş veya null geldi");
            return new ExpressionCheckResponse
            {
                IsValid = false
            };
        }

        //TODO@buraksenyurt Burada gelen ifadenin geçerli bir sorgu olup olmadığını kontrol edecek bir fonksiyonellik olmalı

        // Deneysel olarak gelen ifadenin geçerli olup olmadığına karar vermek için zar atıyoruz
        Random random = new();
        var value = random.Next(1, 9);
        var isValid = value % 7 != 0;
        _logger.LogWarning("{Rakam}...Gelen ifade geçerli mi? {IsValid}", value, isValid);
        return new ExpressionCheckResponse
        {
            IsValid = isValid
        };
    }
}
