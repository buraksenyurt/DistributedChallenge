# Distributed Challange

Bu repoda aslında asenkron mesaj kuyruklarını hedef alan bir dağıtık sistem problemi oluşturmaya ve bu problemin çözümünü uygulamaya çalışıyorum. Öncelikle vakanın temel senaryosu ile işe başlamak lazım. Hangi enstrümanları ve platformları kullanacağımıza sonrasında karar verebiliriz.

## Vaka Senaryosu

Kullanıcılarına oyun kiralayan bir internet şirketi olduğunu düşünelim. Şirketin son kullanıcılara _(End Users)_ sunduğu web ve mobile bazlı uygulamalar dışında şirket genel merkezinde kullanılan Back Office tadında farklı bir uygulamaları daha var. Bu uygulamada yer alan ekranlardan birisi de raporlama talepleri için kullanılıyor. Şirketin sahip olduğu veri miktarı ve rapor taleplerinin belli bir onay sürecinden geçip içeriklerini farklı alanlardan toplaması nedeniyle bir raporun çekilmesi bazen birkaç dakikayı bulabiliyor. Her ne kadar şirket içerisinde bu işleri üstelenen bir raporlama ekibi bulunsa da personelin kullandığı web sayfalarında bu şekilde belirsiz süreyle beklenmeleri istenmiyor. Çözüm olarak rapor talebinin girildiği bir formun tasarlanması ve talebin raporlama ekibine ait uygulamalara ulaştırılıp hazır hale geldikten sonra personelin bilgilendirilmesi şeklinde bir yol izlenmesine karar veriliyor. Tüm bu sürecin tamamen sistem tarafından gerçekleştirilmesi ve otomatize edilmesi isteniyor.

## Çözümleme ile İlgili Bilgiler

Buradaki senaryonun çözümü noktasında oldukça basit ilerlemeye çalışacağım. Bu amaçla tek bir solution içerisinde tüm paydaşların .Net tabanlı uygulamaları olacak. Kilit noktalardan birisi raporların hazırlanmasının uzun sürmesi ve bizim backoffice personelinin rapor talep ettiği sayfada iken bekletmek istemeyişimiz. Rapor hazırlandığında ise onu bir şekilde bilgilendirmemiz. Uygulama açıksa belki bir popup ile ve hatta ekstradan e-posta bildirimi ile. Tahmin edileceği üzere aynı Solution içerisinde bir çözümle yapacak olsak da çözüme dahil olan Process'ler network üzerinde farklı lokasyonlardaki Node'larda çalışıyor olabilirler.

## Zorluk Seviyesini Artırma

Yukarıda bahsedilen senaryoda sisteme dahil olan tüm uygulamaların aynı firmanın dahili ağı _(Internal Network)_ içerisinde yer aldığı varsayılmıştır. Senaryoyu zorlaştırmak için raporlamayı yapan uygulamanın/servisinin internet üzerinden erişilebilen bir 3rd Party servis sağlayacısına ait olduğunu düşünebilirsiniz.
