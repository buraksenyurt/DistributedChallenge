using GamersWorld.AppEvents;
using GamersWorld.SDK;
using GamersWorld.SDK.Messages;

namespace GamersWorld.AppEventBusiness;

public class DeleteReport
    : IEventDriver<ReportProcessCompletedEvent>
{
    public async Task<BusinessResponse> Execute(ReportProcessCompletedEvent appEvent)
    {
        //TODO@buraksenyurt Must implement Report Process Completed actions
        
        // Dokümanı Local Storage'dan silme operasyonunu gerçekleştir
        throw new NotImplementedException();
    }
}
