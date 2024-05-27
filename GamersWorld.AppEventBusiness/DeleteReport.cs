using GamersWorld.AppEvents;
using GamersWorld.SDK;

namespace GamersWorld.AppEventBusiness;

public class DeleteReport
    : IEventExecuter<ReportProcessCompletedEvent>
{
    public async Task<int> Execute(ReportProcessCompletedEvent appEvent)
    {
        // Dokümanı Local Storage'dan silme operasyonunu gerçekleştir
        throw new NotImplementedException();
    }
}
