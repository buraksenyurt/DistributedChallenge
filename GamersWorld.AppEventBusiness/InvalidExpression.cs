using GamersWorld.AppEvents;
using GamersWorld.SDK;
using GamersWorld.SDK.Messages;

namespace GamersWorld.AppEventBusiness;

/*
    System 123'ten bir rapor talebi yapılırken o sisteme özgü betik dil ile bir ifade yollanıyor.
    İfadenin geçersiz olması, belki sistemi indirmeye yönelik ifadeler içermesi vb durumlarda 
    System ABC'den System 123'e bir bilgilendirme yapılmakta. 
    Buna karşılık işletilecek fonksiyon aşağıdaki gibi.
*/
public class InvalidExpression
    : IEventExecuter<InvalidExpressionEvent>
{
    public async Task<BusinessResponse> Execute(InvalidExpressionEvent appEvent)
    {
        //TODO@buraksenyurt Must implement alert and warning mechanism

        throw new NotImplementedException();
    }
}
