using GamersWorld.Events;
using GamersWorld.SDK;
using Microsoft.Extensions.Logging;

namespace GamersWorld.EventBusiness;

public class InvalidExpression(ILogger<InvalidExpression> logger) : IEventDriver<InvalidExpressionEvent>
{
    private readonly ILogger<InvalidExpression> _logger = logger;

    public async Task Execute(InvalidExpressionEvent appEvent)
    {
        //QUESTION Geçersiz bir expression söz konusu ise bu event tetiklenir. Devamında nasıl bir alarm mekanizması?

        _logger.LogWarning("{Expression}, Reason: {Reason}", appEvent.Expression, appEvent.Reason);
    }
}
