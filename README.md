# Distributed Challange

Bu repoda aslında asenkron mesaj kuyruklarını hedef alan bir dağıtık sistem problemi oluşturmaya ve bu problemin çözümünü uygulamaya çalışıyorum. Öncelikle vakanın temel senaryosu ile işe başlamak lazım. Hangi enstrümanları ve platformları kullanacağımıza sonrasında karar verebiliriz.

## Vaka Senaryosu

Kullanıcılarına oyun kiralayan bir internet şirketi olduğunu düşünelim. Şirketin son kullanıcılara _(End Users)_ sunduğu web ve mobile bazlı uygulamalar dışında şirket genel merkezinde kullanılan Back Office tadında farklı bir uygulamaları daha var. Bu uygulamada yer alan ekranlardan birisi de raporlama talepleri için kullanılıyor. Şirketin sahip olduğu veri miktarı ve rapor taleplerinin belli bir onay sürecinden geçip içeriklerini farklı alanlardan toplaması nedeniyle bir raporun çekilmesi bazen birkaç dakikayı bulabiliyor. Her ne kadar şirket içerisinde bu işleri üstelenen bir raporlama ekibi bulunsa da personelin kullandığı web sayfalarında bu şekilde belirsiz süreyle beklenmeleri istenmiyor. Çözüm olarak rapor talebinin girildiği bir formun tasarlanması ve talebin raporlama ekibine ait uygulamalara ulaştırılıp hazır hale geldikten sonra personelin bilgilendirilmesi şeklinde bir yol izlenmesine karar veriliyor. Tüm bu sürecin tamamen sistem tarafından gerçekleştirilmesi ve otomatize edilmesi isteniyor.

## Çözümleme ile İlgili Bilgiler

Buradaki senaryonun çözümü noktasında oldukça basit ilerlemeye çalışacağım. Bu amaçla tek bir solution içerisinde tüm paydaşların .Net tabanlı uygulamaları olacak. Kilit noktalardan birisi raporların hazırlanmasının uzun sürmesi ve bizim backoffice personelinin rapor talep ettiği sayfada iken bekletmek istemeyişimiz. Rapor hazırlandığında ise onu bir şekilde bilgilendirmemiz. Uygulama açıksa belki bir popup ile ve hatta ekstradan e-posta bildirimi ile. Tahmin edileceği üzere aynı Solution içerisinde bir çözümle yapacak olsak da çözüme dahil olan Process'ler network üzerinde farklı lokasyonlardaki Node'larda çalışıyor olabilirler.

## Aday Çözüm

Bu problemi aşağıdaki gibi çözmeye çalıştığımızı düşünelim.

![Challange Solution Candidate](https://github.com/buraksenyurt/DistributedChallange/assets/2705782/9c9439a4-ce29-49cb-84c7-d0c48ba2c11f)

Senaryodaki adımları da aşağıdaki gibi tarifleyelim.

1. **CEO**'muz geçtiğimiz yıl en çok pozitif yorum alan oyunlardan ilk 50sini ülke bazında satış rakamları ile birlikte talp eder. Bu talebi **web** uygulamasındaki **forma** girer.
2. Bu rapor talebi web form üstünden kaydedildiğinde şimdilik **Event Trigger Service** olarak adlandırılan ve başka bir process'de yer alan bir servis tetiklenir. Servise formdaki veriler benzersiz bir ID ile _(işlemleri tüm süreç boyunca benzersiz bir **GUID** ile takip edebilmek için)_ damgalanarak **POST** metoduyla gönderilir.
3. **Event Trigger Service**'in tek işi gelen içeriği **ReportRequestedEvent** isimli bir olay mesajı olarak hazırlayıp kuyruğa bırakmaktır.
4. Şimdilik **Event Consumer/Publisher Gateway** diye adlandırdığımız başka bir process olayları dinlemek ve bazı aksiyonlar almakla görevlidir. **ReportRequestedEvent** isimli olayları dinleyen thread'leri vardır.
5. **Event Consumer/Publisher Gateway** servisi bir **ReportRequestedEvent** yakaladığında **Reporting App Service** isimli bir başka servise **POST** çağrısı gönderir. **Reporting App Service**'in gelen rapor taleplerini toplayan bir başka process olduğunu ifade edebiliriz.
6. İç çalışma dinamiğini pek bilmediğimiz **Reporting App Service** belli bir zaman diliminde raporun hazırlanmasından sorumludur. Hazırlanan raporu kendi **Local Storage** alanında saklar ve hazır olduğunda bunun için şimdilik **External Reader Service** olarak adlandırılan ve kendi Process'i içinde çalışan bir diğer servise **POST** bildiriminde bulunur.
7. **External Reader Service**, raporun hazır olduğuna dair **ReportReadyEvent** isimli yeni bir olay mesajı hazırlar ve bunu kuyruğa bırakır.
8. **Event Consumer/Publisher** tarafındaki Process **ReportReadyEvent** isimli olayları dinler.
9. **Event Consumer/Publisher** tarafında **ReportReadyEvent** yakalandığında yine farklı bir process'te çalışan **Reporting File Service** hizmeti **GET** ile çağrılır ve üretilen rapora ait PDF çıktısı çekilir.
10. Servisten çekilen PDF içeriği bu sefer **Back Office** uygulamasının bulunduğu ağ tarafındaki **local storage**'e aktarılır. Aynı anda bu sefer ReportIsHereEvent isimli bir başka olay mesajı kuyruğa bırakılır.
11. Kendi process'i içinde çalışan **Report Trace Service** isimli servis uygulaması **ReportIsHereEvent** olayını dinler.
12. Report Trace Service hizmeti, **ReportIsHereEvent** isimli bir olay yakaladığında **Local Storage**'a gider ve ilgili PDF'i çeker.
13. Rapor artık hazırdır. Rapor e-posta ile CEO'ya gönderilir ve Local Storage ortamlarında gerekli temizlikler yapılır.

## Solution için Aday Uygulama Türleri

- Rapor formu girilen arabirim basit bir Asp.Net MVC uygulaması olabilir.
- Rapor taleplerine ait olayları oluşturup gönderen Event Trigger Asp.Net Web Api olabilir.
- Asenkron mesaj kuyruğu için RabbitMQ kullanılabilir. İşi kolaylaştırmak adına RabbitMQ bir Docker Container ile işletilebilir.
- Event Consumer/Publisher Service tarafı esas itibariyle rapor talebi, rapor alında ve rapor hazır gibi aksiyonları sürekli dinleyen ve farklı aksiyonları tetikleyen bir ara katman gibi duruyor. Sürekli çalışır konumda olan bir process olması ve farklı gerçekleşen aksiyonlar için başka bağımlılıkları tetikleyebilecek bir yapıda tasarlanması iyi olabilir. Bu anlamda sürekli çalışan dinleyici ve aksiyonları gerçekleştiren bir kütüphane topluluğuna ihtiyaç duyabiliriz. Yani host uygulamanın hangi olay gerçekleştiğinde hangi aksiyonları alacağını belki bir DI Container üstünden çözümleyerek çalışıyor olması gerekebilir. Bu durumda üzerinde durduğumuz bu Challange içerisindeki başka bir Challange olarak karşımıza çıkıyor.
- External Reader Service aslında Reporting App Service'in tüketimesi için açılmış bir Endpoint sağlayıcı rolünde. Onu da ayrı bir Web API hizmeti olarak tasarlayabiliriz.
- Reporting App Service ve Reporting File Service birer Web API hizmeti olarak tasarlanabilirler. Local Storage olarak senaryoyu kolayca uygulayabilmek adına fiziki dosya sistemini kullanacaklarını düşünebiliriz. Yani hazırlanması bir süre alacak PDF içeriğini fiziki diske kaydetip buradan okuyarak External Reader Service'e iletebilirler.
- Report Trace Service uygulaması da esasında sürekli ayakta olması ve ReportIsHere olayını dinlemesi gereken bir konumadır. Bu anlamda sürekli çalışan bir Console uygulaması olarak da tasarlanabilir.

  **Not:** Dikkat edileceği üzere Event Consumer/Publisher olarak araya koyduğumuz katman bazı olaylar ile ilgili olarak bir takım aksiyonlar alıyor. Örneğin ReportRequestEvent gerçekleştiğinde POST ile bir dış servise talep gönderiyor ve ReportIsHereEvent gerçekleştiğinde de GET Report ile PDF çekip Back Office tarafındaki Local Storage'a kaydediyor. Dolayısıyla bir event karşılığında bir takım aksiyonlar icra ediyor. Bunları A event'i için şu Interface implementasyonunu çağır şeklinde başka bir katmana alarak runtime'da çözümlenebilir bağımlılıkar haline getirebilir ve böylece runtime çalışıyorken yeni event-aksiyon bağımlılıklarını sisteme dahil edebiliriz belki de. Dolayısıyla aradaki Event Consumer/Publisher'ın tasarımı oldukça önemli. 

## Zorluk Seviyesini Artırma

Yukarıda bahsedilen senaryoda sisteme dahil olan tüm uygulamaların aynı firmanın dahili ağı _(Internal Network)_ içerisinde yer aldığı varsayılmıştır. Senaryoyu zorlaştırmak için raporlamayı yapan uygulamanın/servisinin internet üzerinden erişilebilen bir 3rd Party servis sağlayacısına ait olduğunu düşünebilirsiniz.
