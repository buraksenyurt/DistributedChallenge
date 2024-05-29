using GamersWorld.AppEvents;
using GamersWorld.SDK;
using GamersWorld.SDK.Messages;

namespace GamersWorld.AppEventBusiness;

/*
    Rapor karşı sistemden diğer sistemin Local Storage'ına indiğinde
    ReportIsHereEvent hazırlanıp kuyruğa bırakılmış oluyor. 

    Bu olay yakalanırsa aşağıdaki nesne fonksiyonundaki işlemler yapılacak.
*/
public class UsePreparedReport
    : IEventDriver<ReportIsHereEvent>
{
    public async Task<BusinessResponse> Execute(ReportIsHereEvent appEvent)
    {
        //TODO@buraksenyurt Must implement Use Report steps
        // Dokümanı Local Storage'dan oku
        // E-posta ile gönder
        // raporun hazırlandığına dair bir bilgilendirme olayı(ReportProcessCompleted) hazırlayıp fırlat
        throw new NotImplementedException();
    }
}
