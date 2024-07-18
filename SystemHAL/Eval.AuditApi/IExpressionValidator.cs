using Eval.AuditApi.Model;

namespace Eval.AuditApi.Contracts;

public interface IExpressionValidator
{
    ExpressionCheckResponse IsValid(ExpressionCheckRequest request);
}