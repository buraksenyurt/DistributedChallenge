using Eval.AuditLib.Model;

namespace Eval.AuditLib.Contracts;

public interface IExpressionValidator
{
    ExpressionCheckResponse IsValid(ExpressionCheckRequest request);
}