using GamersWorld.AppEvents;
using GamersWorld.SDK;

namespace GamersWorld.AppEventBusiness;

/*
    Kullanıcı form aracılığı ile bir rapor talebinde bulunduğunda Event Trigger Service
    yeni bir ReportRequestedEvent nesnesi hazırlar ve bunu kuyruğa gönderir.

    Kuyruğu dinleyen taraf bu event gerçekleştiğinde 
    aşağıdaki nesnenin Execute metodunu icra edip buradaki işlemleri yapmalıdır.
*/
public class PostReportRequest
    : IEventExecuter<ReportRequestedEvent>
{
    public async Task<int> Execute(ReportRequestedEvent appEvent)
    {
        //TODO@buraksenyurt Must implement App Service Post request
        
        // Reporting App Service'e bir POST talebi yapılır
        throw new NotImplementedException();
    }
}
