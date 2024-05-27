using GamersWorld.AppEvents;
using GamersWorld.SDK;

namespace GamersWorld.AppEventBusiness;

/*
    Rapor karşı sistemden diğer sistemin Local Storage'ına indiğinde
    ReportIsHereEvent hazırlanıp kuyruğa bırakılmış oluyor. 

    Bu olay yakalanırsa aşağıdaki nesne fonksiyonundaki işlemler yapılacak.
*/
public class UsePreparedReport
    : IEventExecuter<ReportIsHereEvent>
{
    public async Task<int> Execute(ReportIsHereEvent appEvent)
    {
        // Dokümanı Local Storage'dan oku
        // E-posta ile gönder
        // raporun hazırlandığına dair bir bilgilendirme olayı(ReportProcessCompleted) hazırlayıp fırlat
        throw new NotImplementedException();
    }
}
