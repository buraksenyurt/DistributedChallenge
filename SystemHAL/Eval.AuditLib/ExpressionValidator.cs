using Eval.AuditLib.Contracts;
using Eval.AuditLib.Model;
using Microsoft.Extensions.Logging;

namespace Eval.Lib;
public class ExpressionValidator(ILogger<ExpressionValidator> logger) : IExpressionValidator
{
    private readonly ILogger<ExpressionValidator> _logger = logger;

    public ExpressionCheckResponse IsValid(ExpressionCheckRequest request)
    {
        if (string.IsNullOrEmpty(request.Expression))
        {
            _logger.LogError("Null or empty expression!");

            return new ExpressionCheckResponse
            {
                IsValid = false
            };
        }

        //TODO@buraksenyurt Burada gelen ifadenin geçerli bir sorgu olup olmadığını kontrol edecek bir fonksiyonellik olmalı

        Random random = new();
        var value = random.Next(1, 9);
        var isValid = value % 7 != 0;
        _logger.LogWarning("{Number}...Is expression valid? {IsValid}", value, isValid);

        return new ExpressionCheckResponse
        {
            IsValid = isValid
        };
    }
}
