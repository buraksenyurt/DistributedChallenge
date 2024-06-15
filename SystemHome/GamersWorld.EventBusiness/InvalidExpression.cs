using GamersWorld.Events;
using GamersWorld.Common.Enums;
using GamersWorld.Common.Responses;
using GamersWorld.SDK;
using Microsoft.Extensions.Logging;

namespace GamersWorld.EventBusiness;

/*
    System 123'ten bir rapor talebi yapılırken o sisteme özgü betik dil ile bir ifade yollanıyor.
    İfadenin geçersiz olması, belki sistemi indirmeye yönelik ifadeler içermesi vb durumlarda 
    System ABC'den System 123'e bir bilgilendirme yapılmakta. 
    Buna karşılık işletilecek fonksiyon aşağıdaki gibi.
*/
public class InvalidExpression(ILogger<InvalidExpression> logger) : IEventDriver<InvalidExpressionEvent>
{
    private readonly ILogger<InvalidExpression> _logger = logger;

    public async Task<BusinessResponse> Execute(InvalidExpressionEvent appEvent)
    {
        //TODO@buraksenyurt Must implement alert and warning mechanism
        _logger.LogWarning("{Expression}, Reason: {Reason}", appEvent.Expression, appEvent.Reason);

        return new BusinessResponse
        {
            Message = "Report expression doesn't validated.",
            StatusCode = StatusCode.InvalidExpression
        };
    }
}
