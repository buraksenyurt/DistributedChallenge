# Distributed Challenge

Bu repoda asenkron mesaj kuyruklarını hedef alan bir dağıtık sistem problemi oluşturmaya ve bu problemin çözümünü uygulamaya çalışıyorum.

- [Distributed Challenge](#distributed-challenge)
  - [Geliştirme Ortamı](#geliştirme-ortamı)
  - [Senaryo](#senaryo)
  - [Aday Çözüm](#aday-çözüm)
  - [Envanter](#envanter)
  - [Baş Ağrıtacak Dayanıklılık Senaryoları](#baş-ağrıtacak-dayanıklılık-senaryoları)
  - [Yapılacaklar Listesi _(ToDo List)_](#yapılacaklar-listesi-todo-list)
  - [Sistemin Çalıştırılması](#sistemin-çalıştırılması)
  - [Docker Unsurları](#docker-unsurları)
  - [Dış Sistem Entegrasyonları](#dış-sistem-entegrasyonları)
    - [Redis Stream Entgrasyonu ile Event Takibi](#redis-stream-entgrasyonu-ile-event-takibi)
    - [SonarQube ile Kod Kalite Ölçümü](#sonarqube-ile-kod-kalite-ölçümü)
    - [Secure Vault Entegrasyonu](#secure-vault-entegrasyonu)
    - [Local NuGet Entegrasyonu](#local-nuget-entegrasyonu)
    - [Github Actions ve Local Nuget Repo Problemi](#github-actions-ve-local-nuget-repo-problemi)
    - [Servisler için HealthCheck Uygulaması](#servisler-için-healthcheck-uygulaması)
    - [Resiliency Deneyleri](#resiliency-deneyleri)
      - [AuditApi için Resiliency Kullanımı](#auditapi-için-resiliency-kullanımı)
      - [AuditApi Resiliency Test Koşumları](#auditapi-resiliency-test-koşumları)
    - [Service Discovery ve Hashicorp Consul Entegrasonu](#service-discovery-ve-hashicorp-consul-entegrasonu)
    - [Ftp Entegrasyonu ile Arşivleme Stratejisi](#ftp-entegrasyonu-ile-arşivleme-stratejisi)
    - [Planlı İşler (Scheduled Jobs)](#planlı-i̇şler-scheduled-jobs)
    - [Elasticsearch ve Kibana Entegrasyonu ile Log Takibi](#elasticsearch-ve-kibana-entegrasyonu-ile-log-takibi)
    - [Ölçüm Metrikleri için Prometheus ve Grafana Entegrasyonu](#ölçüm-metrikleri-için-prometheus-ve-grafana-entegrasyonu)
  - [POCO Diagramları](#poco-diagramları)
  - [Tartışılabilecek Problemler](#tartışılabilecek-problemler)
  - [Youtube Anlatımları](#youtube-anlatımları)

## Geliştirme Ortamı

Süreç boyunca iki farklı ortamda çalışma fırsatı buldum. İlki Moon isimli ev bilgisayarım. Ubuntu tabanlı sistemin özellikleri şöyle.

| Özellik   | Açıklama                      |
|-----------|-------------------------------|
| OS        | Ubuntu 22.04 LTS              |
| CPU       | Intel® Core™ i7 2.80GHz × 8   |
| RAM       | 32 Gb                         |
| IDE       | Visual Studio Code            |
| Framework | .Net 8.0                      |

Diğer sistem ise iş bilgisayarı ve Windows tabanlı.

| Özellik   | Açıklama                      |
|-----------|-------------------------------|
| OS        | Windows 10 Enterprise         |
| CPU       | Intel(R) Core(TM) i7 2.30GHz  |
| RAM       | 32 Gb                         |
| IDE       | Visual Studio Pro 2022        |
| Framework | .Net 8.0                      |

Ancak bunların haricinde herhangi bir lokasyondan da çalışma şansımız var. Bu noktada Github Codespaces oldukça işe yarıyor.

## Senaryo

Kullanıcılarına oyun kiralayan bir internet şirketi olduğunu düşünelim. Şirketin son kullanıcılara _(End Users)_ sunduğu web ve mobile bazlı uygulamalar dışında şirket genel merkezinde kullanılan Back Office tadında farklı bir programları daha var. Bu programda yer alan ekranlardan birisi de raporlama talepleri için kullanılıyor. Şirketin sahip olduğu veri miktarı ve rapor taleplerinin belli bir onay sürecinden geçip içeriklerini farklı alanlardan toplaması nedeniyle bir raporun çekilmesi bazen birkaç dakikayı bulabiliyor. Her ne kadar şirket içerisinde bu işleri üstelenen bir raporlama ekibi bulunsa da personelin kullandığı web sayfalarında bu şekilde belirsiz süreyle beklenmeleri istenmiyor. Çözüm olarak rapor talebinin girildiği bir formun tasarlanması ve talebin raporlama ekibine ait uygulamalara ulaştırılıp hazır hale geldikten sonra personelin bilgilendirilmesi şeklinde bir yol izlenmesine karar veriliyor. Tüm bu sürecin tamamen sistem tarafından gerçekleştirilmesi ve otomatize edilmesi isteniyor. İşte Örnek Birkaç Rapor İfadesi;

| Rapor Başlığı | İfade (Expression)  |
|---------------|---------------------|
| En Pozitif Yorumları Toplayanlar  | Geçtiğimiz yıl kiraladığımız oyunlardan en çok pozitif yorum alan ilk 25 adedinin ülke bazlı satış rakamları.|
| Güncel Firma Bazlı Kiralamalar  | Dün yapılan kiralamalardan firma ve oyun türü bazında gruplanmış kar değerleri. |
| En İyi Oyuncuların Harcamaları  | Geçen ay en yüksek puan alan oyuncuların yaptığı oyun için satın alma harcamalarının toplam tutarları.  |
| Çak Norizin Bireysel Başarımları  | Dövüş oyunlarından puan olarak benimkini geçen en iyi 10 oyunun kullanım istatistikleri.  |
| En Sağlıklı Oyuncularımız | Oyun oynarken platformumuz üzerinden yemek siparişi verenler arasında en sağlıklı şekilde tıkınanlar  |
| Patlamış Mısır Sevdalıları  | Hafta sonu oyuncularından en çok patlamış mısır siparişi veren ilk 1000 kullanıcının tercih ettiği markalar |
| AFK Modunda Takılan Sakinler  | Oyunun başından kalkıp AFK modundan kalan kullanıcılardan son bir ayda en yüksek süre duraksayanlar |
| Strateji Oyunlarından Vazgeçmeyenler  |Kırk yaş üstü olup 90lı yılların en popüler strateji oyunlarını kiralamaya devam edenlerin ülke bazlı harcama değerleri.  |

## Aday Çözüm

Bu problemi aşağıdaki gibi çözmeye çalıştığımızı düşünelim.

![Challenge Solution Candidate](./images/high_level_design.png)

Senaryodaki adımları da aşağıdaki gibi tarifleyelim.

1. **CEO**'muz geçtiğimiz yıl en çok pozitif yorum alan oyunlardan ilk 50sini ülke bazında satış rakamları ile birlikte talep eder. Bu talebi **web** uygulamasındaki **forma** girer.
2. Bu rapor talebi web form üstünden kaydedildiğinde şimdilik **Event Trigger Service** olarak adlandırılan ve başka bir process'de yer alan bir servis tetiklenir. Servise formdaki veriler benzersiz bir ID ile _(işlemleri tüm süreç boyunca benzersiz bir **GUID** ile takip edebilmek için)_ damgalanarak **POST** metoduyla gönderilir.
3. **Event Trigger Service**'in tek işi gelen içeriği **ReportRequestedEvent** isimli bir olay mesajı olarak hazırlayıp kuyruğa bırakmaktır.
4. Şimdilik **Event Consumer/Publisher Gateway(Gamersworld.EventHost)** diye adlandırdığımız başka bir process olayları dinlemek ve bazı aksiyonlar almakla görevlidir. **ReportRequestedEvent** isimli olayları dinleyen thread'leri vardır.
5. **Event Consumer/Publisher Gateway(Gamersworld.EventHost)** servisi bir **ReportRequestedEvent** yakaladığında **Reporting App Service(Kahin.Gateway)** isimli bir başka servise **POST** çağrısı gönderir. **Reporting App Service(Kahin.Gateway)**'in gelen rapor taleplerini toplayan bir başka process olduğunu ifade edebiliriz.
6. İç çalışma dinamiğini pek bilmediğimiz **Reporting App Service(Kahin.Gateway)** belli bir zaman diliminde raporun hazırlanmasından sorumludur. Hazırlanan raporu kendi **Local Storage** alanında saklar ve hazır olduğunda bunun için şimdilik **External Reader Service(GamersWorld.Service.Gateway)** olarak adlandırılan ve kendi Process'i içinde çalışan bir diğer servise **POST** bildiriminde bulunur.
7. **External Reader Service(GamersWorld.Service.Gateway)**, raporun hazır olduğuna dair **ReportReadyEvent** isimli yeni bir olay mesajı hazırlar ve bunu kuyruğa bırakır.
8. **Event Consumer/Publisher(Gamersworld.EventHost)** tarafındaki Process **ReportReadyEvent** isimli olayları dinler.
9. **Event Consumer/Publisher(Gamersworld.EventHost)** tarafında **ReportReadyEvent** yakalandığında yine farklı bir process'te çalışan **Reporting File Service** hizmeti **GET** ile çağrılır ve üretilen rapora ait PDF çıktısı çekilir.
10. Servisten çekilen PDF içeriği bu sefer **Back Office** uygulamasının bulunduğu ağ tarafındaki **local storage**'e aktarılır. Aynı anda bu sefer **ReportIsHereEvent** isimli bir başka olay mesajı kuyruğa bırakılır.
11. Kendi process'i içinde çalışan **Report Trace Service** isimli servis uygulaması **ReportIsHereEvent** olayını dinler. _(Bunu da belki Event Consumer/Publisher(Gamersworld.EventHost) isimli Host uygulamasında ele alabiliriz)_
12. Report Trace Service hizmeti, **ReportIsHereEvent** isimli bir olay yakaladığında **Local Storage**'a gider ve ilgili PDF'i çeker.
13. Rapor artık hazırdır. Rapor e-posta ile CEO'ya gönderilir ve **Local Storage** ortamlarında gerekli temizlikler yapılır.

Senaryoda dikkat edileceği üzere bazı ihlal noktaları da vardır. Örneğin **Form** uygulamasından girilen rapor talebindeki ifade geçersiz olabilir. Sistemden bilgi sızdırmaya yönelik güvensiz ifadeler içerebilir vs. Bu gibi bir durumda **SystemMiddleEarth**'nin geçersiz bir mesaj olduğuna dair **SystemHome**'ü bilgilendirmesi de gerekir. **SystemMiddleEarth**'nin kendi içinde ele alıp detay loglarla değerlendirdiği bu durum **SystemHome**'e bir Event olarak girer ve buna karşılık da bir süreç başlayabilir. Resimde görülen soru işaret kısmı dikkat edileceği üzere hata durumu yakalandıktan sonra ne yapılacağına dairdir. Rapor talebi sahibinin bilgilendirilmesi için e-posta gönderimi, sistem loglarına bilgi düşülmesi, form uygulamasında popup ile bilgilendirme yapılması vs gibi bir işler bütünü başlatılabilir.

## Envanter

Güncel olarak çözüm içerisinde yer alan ve bir runtime'a sahip olan uygulamalara ait envanter aşağıdaki gibidir.

| **Sistem**     | **Servis**                       | **Tür**     | **Görev**                                                 | **Dev Adres**  |
|----------------|----------------------------------|-------------|-----------------------------------------------------------|----------------|
| HAL            | Eval.AuditApi                    | REST        | Rapor talebindeki ifadeyi denetlemek                      | localhost:5147 |
| HOME           | GamersWorld.Service.Gateway      | REST        | Middle Earth için rapor statü güncelleme hizmeti          | localhost:5102 |
| HOME           | GamersWorld.Service.Messenger    | REST        | Web önyüz için backend servisi                            | localhost:5234 |
| HOME           | GamersWorld.WebApp               | Self Hosted | Web uygulaması                                            | localhost:5093 |
| HOME           | GamersWorld.EventHost            | Self Hosted | Home sistemindeki event yönetim hizmeti                   | N/A            |
| HOME           | GamersWorld.JobHost              | Self Hosted | Planlı işler işletmecisi                                  | N/A            |
| MIDDLE EARTH   | Kahin.Service.ReportingGateway   | REST        | Rapor hazırlama, yollama ve durum güncellemesi            | localhost:5218 |
| MIDDLE EARTH   | Kahin.EventHost                  | Self Hosted | Middle Earth tarafında çalışan event yönetim hizmeti      | N/A            |
| DOCKER COMPOSE | RabbitMQ                         |             | Async Event Queue                                         | localhost:5672 |
| DOCKER COMPOSE | Redis                            |             | Distributed Key Value Store                               | localhost:6379 |
| DOCKER COMPOSE | LocalStack                       |             | Local AWS Secrets Manager                                 | localhost:4566 |
| DOCKER COMPOSE | BaGet                            |             | Local NuGet Server                                        | localhost:5000 |
| DOCKER COMPOSE | Postgresql                       |             | Rapor veritabanı                                          | N/A            |
| DOCKER COMPOSE | Consul                           |             | Service Discovery için                                    | localhost:8500 |
| DOCKER COMPOSE | Ftp Server                       |             | Ftp senaryolarını işletmek için                           | localhost      |
| DOCKER COMPOSE | Elasticsearch                    |             | Uygulama loglarını depolamak için                         | localhost:9200 |
| DOCKER COMPOSE | Kibana                           |             | Logları izlemek için                                      | localhost:5601 |
| DOCKER COMPOSE | Prometheus                       |             | Ölçüm metriklerini toplamak için                          | localhost:9090 |
| DOCKER COMPOSE | Sonarqube                        |             | Statik kod taraması ile kod kalite metriklerini almak için| localhost:9000 |
| DOCKER COMPOSE | Grafana                          |             | Prometheus'e akan metrikleri görsel olarak izlemek için   | localhost:3000 |
| SYSTEM ASGARD  | Heimdall                         | Self Hosted | Servis izleme uygulaması                                  | localhost:5247 |

**NOT: Yeni servisler ilave edildikçe burası güncellenmelidir.**

![inventory](./images/current_inventory.png)

## Baş Ağrıtacak Dayanıklılık Senaryoları

Dağıtık sistemlerin davranışları göz önüne alındığında bazı yanılgılar söz konusudur. Bunlar ilk kez [Peter Deutsch](https://en.wikipedia.org/wiki/Fallacies_of_distributed_computing) tarafından ele alınmış ve aşağıdaki maddeler ile özetlenmiştir. Yani bir dağıtık sistemde aşağıdakilerin sorunsuz olarak işleyeceği düşünülemez.

- **Ağ güvenilirdir _(Relability)_**: Ağ üzerindeki paketlerin kaybolabileceği, hasar görebileceği ya da bozulabileceği gerçeğinin atlanmasıdır.
- **Gecikme sıfırdır _(Latency)_ :** Birden fazla servisin iletişimde olduğu durumlarda ağ genelinde cevap verebilirlik sürelerinin yine de sıfır gecikme sürelerine yakın gerçekleşeceği yanılgısına düşmektir.
- **Bant genişliği sonsuzdur _(Infinite bandwith)_ :** Ağ genişliğinin servisler arası çok yüksek boyutlarda verileri taşırken bile sorun yaşamayacağını düşünmektir.
- **Ağ güvenlidir _(Secure Network)_ :** Ağın yetkisiz erişimlere, güvenlik tehditlerine, siber saldırılara _(Man in the Middle gibi)_ açık olmadığını düşünmektir.
- **Topoloji değişmez :** Ağ topolojisinde yer alan IP adreslerinin değişmeyeceğini, servislerin taşınmayacağını, yeni node'ların eklenip çıkartılmayacağını zannetmektir.
- **Tek bir yönetici _(Administrator)_ vardır :** Ağı yönetmek için birden fazla kişi ve hatta takım olduğunuda, aralarındaki iletişimde yanlış anlaşılmalar olabileceğini göz ardı etmektir.
- **Taşıma maliyeti sıfırdır _(Transport Cost)_ :** Ağ üzerine taşınan verinin bandwidth harcamadığını, gecikmelere maruz kalmadığını ve genel performansa etki edecek bir maliyetinin olmadığını düşünmektir.
- **Ağ homojendir :** Ağın tek bir protokol ile çalıştığını, bu protokollerin farklı performanslar sunmadığını ve farklı segmentler için farklı konfigürasyonlara sahip olamayacağını düşünmektir.

Dolayısıyla tasarladığımız dağıtık sistemin tüm bunları sorunsuz şekilde karşılamadığını baştan kabul etmekte yarar var.

Envanterimize göre sistemin genel dayanıklılığını test edebileceğimiz ve Resilience taktiklerini ele alabileceğimiz senaryoları aşağıdaki listeye ekleyebiliriz. [Netflix'in Chaos Monkey'si](https://netflix.github.io/chaosmonkey/) tadında sistemde bu tip sorunları çıkartabilecek bazı uygulamalar üzerinde de çalışılabilir.

- [ ] WebApp rapor talep eder. Messenger servis cevap veremez durumdadır.
- [ ] WebApp rapor talep eder. Messenge servis talebi alır. Event nesnesi oluşur. Event Host uygulaması kapalıdır.
- [ ] WebApp rapor talep eder. Messenge servis talebi alır. Event nesnesi oluşur ama RabbitMQ aşağı inmiş haldedir.
- [ ] Rapor talebi System MiddleEarth'e gönderilmek istenir. Kahin.Service.ReportingGateway cevap veremez durumdadır.
- [ ] Kahin.Service.ReportingGateway talebi alır, AuditApi cevap veremez durumdadır.
- [ ] Kahin.Service.ReportingGateway talebi alır. AuditApi ifadeyi onaylar. Redis aşağı inmiş haldedir.
- [ ] WebApp rapor talep eder. Messenger servis talebi alır. Event nesnesi oluşur. Rapor talebi Kahin.Service.ReportingGateway'e ulaşır. AuditApi ifadeyi onaylar. Rapor hazır hale gelir. Geri bildirim için GamersWorld.Service.Gateway servisi çağrılır. GamersWorld.Service.Gateway ayakta ama talep fazlalığı sebebiyle cevap verebilir konumda değildir.
- [ ] AWS Secrets Manager herhangi bir anda aşağıya iner.
- [ ] System MiddleEarth servislerinin cevap verme sürelerinde _(Response Times)_ gecikmeler söz konusudur ve belirgin bir darboğaz oluşmaya başlar.
- [ ] System MiddleEarth'teki Redis servisi herhangi bir zamanda çöker.

## Yapılacaklar Listesi _(ToDo List)_

- [x] Solution yapısı ve proje isimlendirmeleri gözden geçirilebilir.
- [x] Solution için **Sonarqube** entegrasyonu yapılıp kod kalite metrikleri ölçümlenebilir.
- [x] Servis fonksiyon çağrılarının response time değerlerinin ölçümü için middleware tarafına ek bir nuget paketi yazılabilir.
- [ ] Bazı kütüphaneler için birim testler _(Unit Tests)_ yazılarak **Code Coverage** değerleri yükseltilebilir.
- [ ] Kahin _(SystemMiddleEarth)_ sistemindeki projeler için ayrı bir **Solution** açılabilir.
- [x] Loglama altyapısı **Elasticsearch**'e alınabilir. _(Birkaç uygulama için gerçekleştirildi)_
- [ ] Messenger servisi **gRPC** türüne evrilebilir.
- [ ] Bazı **Exception** durumları için **Custom Exception** sınıfları yazılabilir. Ancak servis tarafında Exception döndürmek yerine hata ile ilgili bilgi barındıran bir Response mesaj döndürülmesi daha iyi olabilir.
- [ ] Daha önceden çekilmiş raporlar için tekrardan üretim sürecini başlatmak yerine **Redis** tabanlı bir caching sistemi kullanılabilir.
- [ ] Tüm servisler **HTTPS** protokolünde çalışacak hale getirilebilir.
- [ ] Uçtan uca testi otomatik olarak yapacak bir **RPA _(Robotik Process Automation)_** eklentisi konulabilir. Belki otomatik **UI** testleri için **Playwright** aracından yararlanılabilir.
- [x] Bazı servislerin ayakta olup olmadıklarını kontrol etmek için bu servislere **HealthCheck** mekanizması eklenebilir.
- [x] URL adresleri, **RabbitMQ** ortam bilgileri **(Development, Test, Pre-Production, Production)** gibi alanlar için daha güvenli bir ortamdan **(Hashicorp Vault, AWS Secrets Manager, Azure Key Vault, Google Cloud Secret Manager, CyberArk Conjur vb)** tedarik edilecek şekilde genel bir düzenlemeye gidilebilir.
- [ ] Log mesajları veya **Business Response** nesnelerindeki metinsel ifadeler için çoklu dil desteği _(Multilanguage Support)_ getirilebilir.
- [ ] **SystemHome**'daki **Event Host** uygulaması bir nevi Pipeline akışına da sahip. **Event** yönetiminde ilgili business nesneler çağırılmadan önce ve sonrası için akan veri içeriklerini loglayacak ortak bir mekanizma yazılabilir.
- [ ] Birçok fonksiyonda standart girdi çıktı loglaması, **exception handling** gibi **Cross-Cutting Concern** olarak da adlandırılan işlemler söz konusu. Bu kullanımda **AOP**_(Aspect Oriented Programming)_ tabanlı bir yaklaşıma gidilebilir belki.
- [x] Sistemdeki servisleri izlemek için Microsoft **HealthCheck** paketinden yararlanılan bir arabirim geliştirilebilir.
- [ ] **SystemAsgard**'daki **Heimdall** uygulamasındaki monitoring bilgileri **Prometheus, Application Insights, Seq, Datadog** gibi dış araçlara gönderilebilir.
- [x] **SystemHome** tarafındaki web uygulaması için kimlik ve yetki kontrol mekanizması ilave edilip **SignalR** mesajlarının belirli kullanıcılar için çıkması sağlanabilir.

## Sistemin Çalıştırılması

Nihai senaryonun işletilmesinde birçok servisin aynı anda ayağa kalkması gereken durumlar söz konusudur. RabbitMq, Redis, Localstack, Postgresql, BaGet bunlar arasındadır. Tüm bu hizmetler için bir docker-compose dosyası mevcuttur. İlk olarak bu container'ların ayakta olması gerekmektedir.

```bash
# Docker-Compose'u sistemde build edip çalıştırmak için
sudo docker-compose up --build

# Sonraki çalıştırmalarda aşağıdaki gibide ilerlenebilir
sudo docker-compose up -d

# Uygulama loglarını görmek için
sudo docker-compose logs -f

# Container'ları durdurmak için
sudo docker-compose down
```

Solution içerisinde yer alan uygulamaları tek seferde çalıştırmak için run_all isimli bir shell script dosyası hazırlanmıştır. 
_(Bu arada eğer Windows tabanlı bir sistemde ve Visual Studio ortamında çalışıyorsanız Solution'ı başlatmanız yeterlidir. Tüm servis ve uygulamalar otomatik açılır. Tabii bunun için Solution özelliklerinden Multiple Startup Projects'in seçili olması gerektiğini unutmayalım)_

```bash
#!/bin/bash

# Start ReportingGateway Service
gnome-terminal --title="MIDDLE EARTH - Reporting Gateway" -- bash -c "cd ../SystemMiddleEarth/Kahin.Service.ReportingGateway && dotnet run; exec bash"

# Start Messenger Service
gnome-terminal --title="HOME - Messenger Service" -- bash -c "cd ../SystemHome/GamersWorld.Service.Messenger && dotnet run; exec bash"

# Start JWT Token Service
gnome-terminal --title="HOME - JWT Token Service" -- bash -c "cd ../SystemHome/GamersWorld.Service.Identity && dotnet run; exec bash"

# Start Home EventHost
gnome-terminal --title="HOME - Event Consumer Host" -- bash -c "cd ../SystemHome/GamersWorld.EventHost && dotnet run; exec bash"

# Start Home JobHost
gnome-terminal --title="HOME - Scheduled Job Host" -- bash -c "cd ../SystemHome/GamersWorld.JobHost && dotnet run; exec bash"

# Start Home Gateway Service
gnome-terminal --title="HOME - Gateway Service" -- bash -c "cd ../SystemHome/GamersWorld.Service.Gateway && dotnet run; exec bash"

# Start Eval.Api Service
# Docker Service haline getirildiği için gerek kalmadı
# gnome-terminal --title="HAL - Expression Auditor" -- bash -c "cd ../SystemHAL/Eval.AuditApi && dotnet run; exec bash"

# Start Web App
gnome-terminal --title="HOME - Web App" -- bash -c "cd ../SystemHome/GamersWorld.WebApp && dotnet run; exec bash"

# Start Middle Earth System EventHost
gnome-terminal --title="MIDDLE EARTH - Event Consumer Host" -- bash -c "cd ../SystemMiddleEarth/Kahin.EventHost && dotnet run; exec bash"

# Start Asgard System Heimdall
gnome-terminal --title="ASGARD - Heimdall Monitoring UI" -- bash -c "cd ../SystemAsgard/Heimdall && dotnet run; exec bash"
```

## Docker Unsurları

Bu büyük çaplı çalışmada birçok servis docker-compose aracılığıyla ayağa kaldırılıyor. Postgres, PgAdmin, Elastichsearch, Kibana gibi bilinen hizmetler dışında EvalApi isimli servis uygulamamızda aynı network içerisinde çalıştırılmakta. Kullandığımız güncel servislere ait bilgileri aşağıdaki tabloda bulabilirsiniz.

| Hizmet Adı                 | Servis Adı   | Adresi                   |
|----------------------------|--------------|--------------------------|
| RabbitMQ, mesaj kuyruğu yönetimini sağlar. | rabbitmq    | amqp, localhost:15672  |
| Redis, key:value store türlü veri tabanıdır. Burada stream özelliği ile mesajlaşma özelliği kullanılıyor. | redis        | localhost:6379          |
| LocalStack, AWS servislerinin lokal ortamda taklit edilmesini sağlar. | localstack   | localhost:4566   |
| BaGet, NuGet paket yönetimi için kullanılan bir sunucu uygulamasıdır. | baget        | localhost:5000   |
| PostgreSQL, ilişkisel bir veritabanı yönetim sistemidir. | postgres     | localhost:5432          |
| PgAdmin, PostgreSQL veritabanlarını yönetmek için kullanılan bir web arayüzüdür. | pgadmin      | localhost:5050   |
| Consul, servis keşfi ve yapılandırma yönetimini kolaylaştırır. Burada Service Discovery için kullanılıyor. | consul       | localhost:8500   |
| FTP Sunucusu, dosya transferi için kullanılır. | ftp-server   | ftp, localhost:21      |
| Elasticsearch, tam metin arama(full text search) ve analiz motorudur. Burada daha çok logları akıttığımız yer. | elasticsearch | localhost:9200   |
| Kibana, Elasticsearch ile veri görselleştirmesi yapar. | kibana       | localhost:5601   |
| Prometheus, sistem izleme ve uyarı işlevi görür. | prometheus   | localhost:9090   |
| Grafana, metriklerin ve logların görselleştirilmesi için kullanılır. | grafana      | localhost:3000   |
| Sonarqube, statik kod tarama aracı. | sonarqube | localhost:9000  |

## Dış Sistem Entegrasyonları

### Redis Stream Entgrasyonu ile Event Takibi

**System MiddleEarth** içerisinde yer alan **Kahin.Service.ReportingGateway**, gelen bir rapor talebini aldıktan sonra raporun hazırlanması için bir süreç başlatır. Bu süreç muhtemelen uzun sürebilecek bir iştir. Bu nedenle **System MiddleEarth** içinde bir mesajlaşma kuyruğu söz konusudur. Bu sefer **RabbitMQ** yerine **Redis** kullanılmıştır. Redis tarafı in-memory de çalışabilen dağıtık bir key:value store olarak düşünülür ancak aynı zamanda **Pub/Sub** modelini de destekler. Dolayısıyla gelen rapor talebini burada kuyruğa bırakıp bir dinleyicinin almasını ve işlemleri ilerletmesini sağlayabiliriz. **Redis** tarafı yine **docker-compose** içerisinde konuşlandırılmıştır. Sistemde bir docker imajı olarak ayağa kalkar. Web uygulamasından geçerli bir rapor **talebi Kahin.Service.ReportingGateway**'e ulaştığında redis'e düşen mesaj kabaca aşağıdaki komutlar ile yakalanabilir.

```bash
# Önce redis client terminale girilir
sudo docker exec -it distributedchallenge-redis-1 redis-cli

# Key'ler sorgulanır
keys *

# Key içeriklerine bakılır
XRANGE reportStream - +
```

Bir deneme sonrası sistemde oluşan görüntü aşağıdaki gibidir.

![Runtime_06](./images/runtime_06.png)

### SonarQube ile Kod Kalite Ölçümü

Projenin teknik borçlanma değerlerini ölçümlemek ve kod kalitesini iyileştirmek için **Sonarqube** aracını kullanmaya karar verdim. **Local** ortamda **Sonarqube** kurulumu için Docker imajından yararlanılabilir. _(Artık Sonarqube hizmetini docker-compose dosyasından işletmekteyiz)_

Kurulum sonrası **localhost:9000** adresine gidilir ve standart kullanıcı adı ve şifre ile giriş yapılır _(admin,admin)_ Sistem ilk girişte şifre değişikliği istenebilir. Sonrasında bir proje oluşturulur. **DistributedChallengeProject** isimli bir proje oluşturup ve .Net Core taraması yapması için gerekli adımları seçerek devam edebiliriz. **SQ**, proje için bir anahtar _(key)_ üretecektir. Bu key değerinden yararlanılarak tarama aşağıdaki terminal komutları ile çalıştırılabilir. Zaten anahtar değer üretimi sonrası **SQ** hangi komutların çalıştırılması gerektiğini dokümanda gösterir.

```bash
# Sistem yüklü olması gereken araçlardan birisi
dotnet tool install --global dotnet-sonarscanner

# Tarama başlatma komutu
dotnet sonarscanner begin /k:"DistributedChallengeProject" /d:sonar.host.url="http://localhost:9000"  /d:sonar.token="sqp_4935e304c6d865c146e5f6064902583241c54b87"

# Solution için build operasyonu
dotnet build

# Tarama durdurma komutu
dotnet sonarscanner end /d:sonar.token="sqp_4935e304c6d865c146e5f6064902583241c54b87"
```

İlk tarama sonuçlarına göre projenin şu anki skor kartı aşağıdaki ekran görüntüsündeki gibidir.

![Sonar Scanner Day 1](./images/sonar_scanner_day_1.png)

Tarama yaklaşık 1200 satır kod tespiti yapmış. Bunun %3.1'inin tekrarlı kodlardan oluştuğu ifade ediliyor _(Duplicate Codes)_ Kodun güvenilirliği ile ilgili olarak 21 sorunlu nokta mevcut. Ayrıca bakımı maliyeti çıkartacak derecede sıkıntılı 41 maddemiz bulunuyor. Bunlardan birisinin etkisi yüksek riskli olarak işaretlenmiş. Ne yazık ki şu an itibariyle Code Coverage değerimiz %0.0. Dolayısıyla birim testler ile projenin kod kalitesini güçlendirmemiz lazım. Dolayısıyla proje kodlarımız henüz birkaç günlük yol katetmiş olmasına rağmen teknik borç bırakma eğilimi gösteriyor.

### Secure Vault Entegrasyonu

Solution içerisinde yer alan birçok parametre genelde appsettings konfigurasyonlarından besleniyor. Burada URL, username, password gibi birçok hassas bilgi yer alabilir. Bu bilgileri daha güvenli bir ortamda tutmak tercih edilen bir yöntemdir. Hatta secret bilgileri dev,test,pre-prod ve prod gibi farklı dağıtım ortamları için farklılaştırılabilir. Cloud provider'larda bu amaçla kullanılabilecek birçok Vault ürünü söz konusu. Bunlardan birisi ve ilk denediğim **Hashicorp**'un **Vault** ürünü idi. Ancak bir sebepten VaultSharp nuget aracından eklenen key değerlerini çekmeyi başaramadım. Bunun üzerine alternatif bir yaklaşım aradım ve [LocalStack'te](https://github.com/localstack/localstack) karar kıldım. Bu ortam development testleri için yeterli. Localstack kısaca bir **Cloud Service Emulator** olarak tanımlanıyor. Örneğin **AWS Cloud Provider**'a bağlanmadan local ortamda **AWS**'nin birçok özelliğini kullanabiliyoruz. Ben AWS'nin **Secrets Manager** hizmetini local ortamda kullanmaktayım _(Bir SaaS - Software as a Service çözümü)_. İlk denemeyi **System Middle Earth**'teki **Kahin.Service.ReportingGateway** üzerinde yaptım. Bu uygulama **Redis** ve **SystemHAL**'deki **EvalApi** adres bilgilerini normal şartlarda appsettings dosyasında okuyor. Bunların vault üstünden karşılanması için gerekli değişiklikler yapıldı.

```bash
# LocalStack docker-compose ile ayağa kalktıktan sonra aws komut satırı aracı ile yönetilebilir
# Ubuntu tarafında bu kurulum için şu adımlar izlenebilir
sudo apt update
# yoksa python3 yüklenebilir
sudo apt install python3 python3-pip -y 
pip3 install awscli --upgrade --user
echo 'export PATH=$HOME/.local/bin:$PATH' >> ~/.bashrc
source ~/.bashrc
aws --version

# Örnek secret key:value çiflerinin eklenmesi için
aws configure set aws_access_key_id test
aws configure set aws_secret_access_key test
aws configure set region eu-west-1

aws --endpoint-url=http://localhost:4566 secretsmanager create-secret --name RedisConnectionString --secret-string "localhost:6379"
aws --endpoint-url=http://localhost:4566 secretsmanager create-secret --name EvalServiceApiAddress --secret-string "localhost:5147"
aws --endpoint-url=http://localhost:4566 secretsmanager create-secret --name HomeGatewayApiAddress --secret-string "localhost:5102"

# Secret bilgilerini görmek için (tümü)
aws --endpoint-url=http://localhost:4566 secretsmanager list-secrets

# Belirli id değerine sahip olanların değerlerini görmek için
aws --endpoint-url=http://localhost:4566 secretsmanager get-secret-value --secret-id RedisConnectionString
aws --endpoint-url=http://localhost:4566 secretsmanager get-secret-value --secret-id EvalServiceApiAddress

# Bellir bir secret içeriğini silmek için
aws --endpoint-url=http://localhost:4566 secretsmanager delete-secret --secret-id RedisConnectionString
```

![Vault Runtime](/images/vault_01.png)

Vault bilgilerini okumak ve her ihtimale karşı docker container sıfırlanırsa yeniden oluşturmak için iki hazır shell script dosyası yer alıyor. **add_secrets.sh** ile secret'lerin yüklenmesi, **get_secrets.sh** ile de eklenmiş secret'lerin görülmesi sağlanabilir. Bu shell script'lerinin çalıştırılabilmesi için aşağıdaki komut ile gerekli yetkilerin verilmesi gerekebilir.

```bash
chmod +x manage_secrets.sh
```

**NOT:** Secrets bilgileri artık environment bazlı hale getirildi. **add_secrets** ve **get_secrets** dosyalarının güncel hallerini takip edelim. Şu anda **tag** bazlı bir ayrım söz konusu. Buna göre çalışma zamanlarındaki configurasyon içeriklerinde yer alan **Environment** anahtarının değerine göre _(Development, Test, Preprod veya Production)_ ortam değişkenleri Secrets Manager'den **tag** bazlı olarak çekiliyor.

### Local NuGet Entegrasyonu

**Solution** içerisinde yer alan birçok proje **Secret Vault** kullanmak durumunda. Hatta farklı amaçlarla da ortak kütüphaneler kullanmaları gerekebilir. Bazı kütüphaneleri **Solution** bağımsız olarak düşünüp yerel bir **Nuget** yönetimi ile ortak kullanıma açabiliriz. Şirket içerisindeki projeler için ortak kullanılabilecek bir çok paket için iyi bir çözüm yöntemidir. Bu amaçla kendi Ubuntu sistemimde yerel bir **Nuget** server kullanmak istedim. [Baguette](https://loic-sharma.github.io/BaGet/) kullanılabilecek iyi çözümlerden birisi. **Web API** tarzı platformu sebebiyle nuget paketlerini basit terminal komutları ile eklemek mümkün. Tabii öncesinde bir Nuget paketi hazırlamak lazım. Bunun için yeni bir sistemimiz var. Ortak kullanılabilecek ve **DistributedChallenge** solution'ına dahil edilmemesi gereken _(ki o zaman Add Project Reference olarak kullanarak bir sıkı bağlılık oluştururuz)_ paketler için **SystemSergeant** isimli bir klasörümüz var. Nuget paketi haline gelecek kütüphaneleri burada toplayabiliriz. **SecretsAgent** isimli paketimiz şimdilik **AWS Secrets Manager** taklidi yapan bir component desteği sağlıyor. Library olarak oluşturduktan sonra aşağıdaki komut ile nuget paketi hazırlanabilir.

```bash
# Bir dotnet kütüphanesi için nuget paketi oluşturmak
dotnet pack -c Release

# Olurda kendi paketlerimiz güncellendikten sonra
# bir sebepten yeni sürümler çekilemez,
# Local Cache temizliği işe yarayabilir
dotnet nuget locals all --clear

# Local Nuget cache listesi için
dotnet nuget locals all --list

# Solution içerisinde yüklü paketlere ait detaylı bilgiler için
dotnet list package
```

Bu komutla oluşan .nupkg uzantılı dosya Nuget paketimizdir. Bu ve diğer olası paketleri SystemSergeant altındaki Packages klasöründe depolayabiliriz.

**Baguette**'yi kolaylık olması açısından docker-compose dosyasında konuşlandırdık ve bir container olarak işletiyoruz. Bu uygulama ayağa kalktığında örneğin aşağıdaki komut ile Packages klasöründeki **nupkg** uzantılı paketleri **BaGet** veritabanına kayıt edebiliriz.

```bash
dotnet nuget push -s http://localhost:5000/v3/index.json *.nupkg

# Eğer local nuget reposundan paket silmek istersek aşağıdaki gibi bir komut kullanabiliriz.
dotnet nuget delete -s http://localhost:5000/v3/index.json Resistance 1.0.5
```

![Nuget Server 01](/images/local_nuget_01.png)

![Nuget Server 02](/images/local_nuget_02.png)

Pek tabii bu yeterli değil. Makinede local nuget store olarak **5000** adresinden hizmet veren **Feed** bilgisinin de kayıt edilmesi lazım. Bunun için glogal **NuGet.config** dosyasının değiştirilmesi gerekiyor. Kendi sistemimde **~/.nuget/NuGet** adresinde yer alan NuGet.config içeriğini aşağıdaki gibi güncelleyebiliriz.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="LocalPackages" value="http://localhost:5000/v3/index.json"/>
  </packageSources>
</configuration>
```

Bu işlemin ardından esas itibariyle makinedeki tüm .net projelerinde **Local Nuget** deposundaki paketlerimizi kullanabiliriz.

```bash
# Örneğin Kahin.MQ projesinde artık aşağıdaki komut ile SecretsAgent paketimizi kullanabiliriz.
dotnet add package SecretsAgent
```

![Nuget Server 03](/images/local_nuget_03.png)

### Github Actions ve Local Nuget Repo Problemi

Şu anda **github actions** ile ilgili bir sorun var. Doğal olarak local nuget reposuna github actions'ın erişimi yok. Bunu aşmak için **ngrok** ürününden yararlandım. **Ngrok**'a kayıt olduktan sonra [şu adresteki talimatlar](https://dashboard.ngrok.com/get-started/setup/linux) ile ilerlenebilir. Local repoyu **Ngrok**'a kayıt etmek için örneğin aşağıdaki gibi bir kullanım yeterli olacaktır.

```bash
# local ortama Ngrok client aracını kurmak için
snap install ngrok

# Bize verilen authentication-key ile doğrulanmak için
ngrok config add-authtoken [AuthenticationKey]

# Ngro tarafında static domain oluşturulduktan sonra ise aşağıdaki gibi gelinebilir.
ngrok http --domain=monkfish-singular-blindly.ngrok-free.app 5000
```

Bu komut ile local makineye gelen talepler de izlenebilir.

![Ngrok 01](/images/ngrok_01.png)

```yml
name: .NET

on:
  push:
    branches: [ "main","dev" ]
  pull_request:
    branches: [ "main","dev" ]

jobs:
  build:

    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v4
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: 8.0.x
      
   - name: Restore dependencies
      run: dotnet restore --source https://monkfish-singular-blindly.ngrok-free.app/v3/index.json --source https://api.nuget.org/v3/index.json

    - name: Build
      run: dotnet build --no-restore
      
    - name: Test
      run: dotnet test --no-build --verbosity normal
```

### Servisler için HealthCheck Uygulaması

Envanterimizdeki servislerin sayısı giderek artıyor. Bu servislerin sağlık durumlarını kontrol etmek için Health Check mekanizmalarından yararlanabiliriz. Kendi yazdığımız servisler için birer health kontrol noktası eklemiştik ancak sistemimizde RabbitMQ, Redis, Localstack, BaGet vb başka servisler de var. Bu tip hizmetlerin sağlık durumlarını kolayca izleyebiliriz. Bunun için [Microsoft.AspNetCore.HealthChecks Nuget paketi](https://www.nuget.org/packages/AspNetCore.HealthChecks.System) ve nimetlerinden yararlanılabilir. Kendisi birçok servis için hazır builder ve UI içeriyor. SystemAsgard içerisinde yer alan Heimdall isimli uygulama bu paketi kullanan bir monitoring uygulaması. Örneğin sistemdeki Redis ve RabbitMQ servisleri ayakta ise şöyle bir arayüz elde edebiliriz.

![Service monitoring 01](./images/heimdall_01.png)

Diğer sistem servislerini ekledikten sonraki duruma da bir bakalım. Aşağıdaki ekran görüntüsünde olduğu gibi servislerin durumlarını izlememiz mümkün.

![Service monitoring 02](./images/heimdall_02.png)

### Resiliency Deneyleri

**ÖNEMLİ NOT!: Resistance isimli paket ayrıca [Global Nuget Reposuna](https://www.nuget.org/packages/Resistance/) taşınmıştır.**

Dağıtık sistemlerini dayanıklılığını(Resiliency) artırmak için servislerin, ağın, fiziki kapasitelerin belli koşullar altında kalarak hatalara sebebiyet vermesi ve bu durumda sistemin nasıl davranış sergileyeceğinin gözlemlenmesi önemli. Bu tip testler ile sistemin dayanıklılığını artıracak ve baş ağrıtan hataların önüne geçecek şekilde tedbirler alınabilir. Bu amaçla sistemlerdeki servislerde bazı deneylerin başlatılması için SystemSergeant altına yeni bir NuGet paketi açıldı. Resistance isimli bu pakette Api çalışm zamanı hattına monte edilecek farklı türde Middleware nesneleri var. Bu paket sayesinde aşağıdaki durumları simüle edebiliriz.

- **Network Failure:** 500 Internal Server Error ile Network Failure durumunun oluşturulması
- **Service Outage:** Belli aralıklarla tekrar edecek şekilde 503 Service Unavailable durumunun oluşturulması
- **Latency:** Service cevap sürelerinin kasıtlı olarak geciktirilmesi
- **Resource Contention:** HTTP 429 Too Many Request durumunun oluşturulması
- **Data Inconsistency:** Veri tutarsızlığı durumunun oluşturulması

Pek tabii bu manuel operasyonar haricinde [Shopify Toxiproxy](https://github.com/Shopify/toxiproxy) hazır paketler de kullanılabilir. Ben hafifsiklet bir çözüm ile ilerledim.

#### AuditApi için Resiliency Kullanımı

SystemHAL içerisinde yer alan ve talep edilen rapordaki ifadeyi sözüm ona denetleyen bir REST Api hizmetimiz var. Bu hizmette yukarıda bahsi geçen vakaları simüle edebilebiliriz. Bunun için Resistance isimli Nuget paketini kullanıyoruz. İlgili simülasyonları etkinleştirmek içinse appsettings.json dosyası içeriğindeki ResistenceFlags sekmesini kullanabiliriz.

```json
"ResistanceFlags": {
  "NetworkFailureIsActive": true,
  "LatencyIsActive": false,
  "ResourceRaceIsActive": false,
  "OutageIsActive": false,
  "DataInconsistencyIsActive": false
}
```

Örneğin yukarıdaki versiyona göre Network Failure simülasyonu aktif. Buna göre servisimiz arada bir HTTP 500 Internal Server Error dönecek. İşte bu tam da istediğimiz türden bir simülasyonun başlangıcı olabilir. Bu durumun diğer sistemler tarafındaki ele alınış biçimi, .Net'in yeni sürümleri ile gelen Resiliency özelliklerinin denenmesi ve hatta Event mesajlaşma yapsının buna göre nasıl tasarlayabileceğimizi keşfetmek açısından iyi bir zemin oluşmakta. Peki çalışma zamanındaki bu özellikleri nasıl değiştirebiliriz. AuditApi servisimizi bildiğiniz üzere bir Docker imajı haline getirmiş ve docker-compose üzerinden Container olarak etkinleştirmiştik. Dolayısıyla container içerisindeki app klasöründe yer alan appsettings.json dosyasını değiştirmek yeterli olacaktır. Ben genellikle appsettings.json dosyasını Container dışında yani host işletim sisteminde değiştirip aşağıdaki komut ile Container içerisien kopyalıyorum. Dosya değiştiği için servis anlık kesintiye uğrayıp tekrardan ayağa kalkacağı için gönderilen yeni ResistanceFlags ayarları ile işletilecektir.

```bash
# Burada 5cca benim sistemimdeki Container'ın ID değeri idi. Sizde farklı bir değer olabilir.
sudo docker cp ./appsettings.json 5cca:/app/appsettings.json
```

#### AuditApi Resiliency Test Koşumları

Senaryoların işletilmesi ve sonuçların irdelenmesi için aşağıdaki gibi basit bir çizelge kullanılabilir.

| **Vaka**             | **Senaryo**                                                                      | **Üretim** | **Metrik**                                    | **Sistem Tepkisi** |
|---------------------|----------------------------------------------------------------------------------|------------|-------------------------------------------------|--------------------|
| **Latency**         | Servis Response üretiminde 1000 ile 3000 milisaniye süreyle geciktirme yapılması | n/a        | MinDelayMs = 1000, MaxDelayMs = 3000           |                    |
| **Outage**          | Dakika başına 10 saniye boyunca hizmet kesintisi yapılması                       | HTTP 503   | Duration = 10 Sec, Frequency = 1 Min           |                    |
|                     | 5 Dakika başına 30 saniye boyunca hizmet kesintisi yapılması                     | HTTP 503   | Duration = 30 Sec, Frequency = 5 Min           |                    |
| **Inconsistency**   | Response içeriğine %50 olasılıkla haricen veri segmenti eklenmesi                | n/a        | DataInconsistencyProbability = 50%            |                    |
|                     | Response içeriğine %20 olasılıkla haricen veri segmenti eklenmesi                | n/a        | DataInconsistencyProbability = 20%            |                    |
| **Resource Race**   | Eş zamanlı istekler için hata oluşması                                           | HTTP 429   | DueTime = 5, Period = 5 sec, RequestLimit = 5   |                    |
| **Network Failure** | %25 olasılıkla servis hatası oluşması                                            | HTTP 500   | NetworkFailureProbability = 25%               |                    |

### Service Discovery ve Hashicorp Consul Entegrasonu

Sistemlerimizde birçok servis yer alıyor. Aslında bu servisler ayağa kalkarken kendilerini bir Service Discovery mekanizmasına kayıt edebilirler. Buna göre servis istemcileri sadece servis adlarından yararlanarak ilgili hizmetleri Service Discovery mekanizması üzerinden keşfedebilirler. Normalde bunun için AWS Secrets Manager deposunu kullanıyorduk. Ancak buraya sadece hasas şifre bilgileri için konuşlandırabiliriz. Bu nedenle envanterimizdeki servisleri yavaş yavaş Hashicorp'un Consul isimli örneği üzerinden yönetilecek şekilde Secrets Manager'dan çıkartacağız. Consul ürünü ile ayrıca servislerin sağlık durumlarını incelemek de mümküm.

Tabii consul ürününün çok fazla meziyeti var. Ben şimdilik sadece Service Discovery noktasında ele almak istiyorum. [Daha fazla bilgi için buraya bakabilirsiniz](https://developer.hashicorp.com/consul).

Ürünü deneyimlemek için her zaman olduğu gibi bir docker-compose üzerinden bir kurgu kullanıyoruz.

```yml
  consul:
    image: hashicorp/consul:latest
    ports:
      - '8500:8500'
```

İlk denemede System Home tarafından web uygulaması ile arka planda kullandığın backend servisi ele alındı. Buna göre GamersWorld.Services.Messenger ayağa kalkarken kendisini consul'a kayıt ediyor. Web App tarafı da onu çağırırken Service Discovery'de tanımlı ismi üzerinden hareket ediyor.

İşte örnek bir ekran görüntüsü. Buna göre web tarafı backend servisine gitmek için **http:// web-backend-service** adresini kullanıyor. Service Discovery'de gerekli yönlendirmeyi yaparak talebi localhost' a indiriyor.

![Service Discovery](./images/service_discovery_01.png)

### Ftp Entegrasyonu ile Arşivleme Stratejisi

Birden fazla sistemin bir arada olduğu senaryolarda ftp bazlı süreçler de olabiliyor. Bunu deneyimlemek için yine docker-compose üzerinden kullanabileceğimiz bir ftp imajına başvurduk. [Alpine tabanlı bu basit ftp imajı](https://hub.docker.com/r/delfer/alpine-ftp-server) ile sistemde sanki bir ftp sunucusu ile işlem yapıyormuşuz stratejisini hayata geçirebildik. Senaryomuza göre önyüzde hali hazırda duran bazı raporlar için doğrudan silme ve arşive gönderme seçeneklerimiz var. Arşive gönderme aslında raporun veritabanından silinmesi ama belli bir süre zarfında ftp sunucusunda yaşatılması anlamına gelmekte. Elbette bu bizim uydurduğumuz bir senaryo. Ftp-server imajı ile çalışırken bazı problemler de yaşadım. Bunlardan birisi docker-compose dosyasında belirtilen kullanıcının bir türlü oluşturulmamasıydı. Bu nedenle docker container'a bir terminal açıp ilgili kullanıcıyı manuel olarak ekledim.

Diğer bir sorunda FTP upload işlemleri için kullandığım FluentFTP paketinin dosya yazma işini bir türlü gerçekleştiremeyişiydi. Bunun sebebi ise ftpuser klasöründe documents'ı oluşturmaması ve buraya ftp kullanıcısı için yazma hakkı vermememizdi. Bu nedenle aşağıdaki komutları not olarak eklemek istedim.

```bash
# Docker container'a terminal açılır
docker exec -it distributedchallenge_ftp-server_1 sh

# Ftp kullanıcısı eklenir ve şifresi belirlenir
adduser -D -h /home/ftpuser userone
echo "userone:123" | chpasswd

# Senaryoda geçerli olan documents klasörü oluşturulur
mkdir -p /home/ftpuser/documents

# Ftp kullanıcısı için yetkiler verilir (yazma,okuma,silme)
chmod -R 755 /home/ftpuser/documents
chown -R userone:userone /home/ftpuser/documents
chmod -R 775 /home/ftpuser/documents
```

Sonuç olarak önyüzden arşivleme işlemi başlatıldığında bir başka event-business işletilir ve her şey yolunda giderse söz konusu rapor ftp sunucusuna yüklenirken veritabanından da silinir.

![Runtime 14](/images/runtime_14.png)

### Planlı İşler (Scheduled Jobs)

Pek çok büyük sistemde belli periyotlarda tekrarlanan işler söz konusudur. Örneğin bizim uygulamamızda tüm raporların için kullanıclar tarafından belirlenen yaşam süreleri var. 10 dakika, Yarım saat, 1 Saat, 1 gün gibi. Bu süreler dolduğunda ilgili kaynaklardaki _(veri tabanı, ftp sunucusu vb)_ içeriklerin temizlenmesi planlı işlerden birisi olabilir. Bu tip bir iş örneğin 10 dakikada bir çalışacak şekilde planlanabilir. .Net tarafında basit bir timer mekanizması ile ilerlenebileceği gibi [Quartz](https://www.nuget.org/packages/Quartz) veya **Hangfire** gibi paketlerden de yararlanılabilir. Örneğimizde **GamersWorld.JobHost** isimli terminal uygulamasını [Hangfire](https://www.nuget.org/packages/Hangfire/) ile çalışacak şekilde genişlettik. Planlı işler bazı durumlarda sistemde beklenmedik sorunlara da neden olabiliyor. Özellikle dağıtık sistemlerde entegre oldukları noktalar açısından düşünülünce bu önemli bir detay olabiliyor. Kurumsal ölçekteki sistemlerde n sayıda planlanmış ve oldukça karmaşık süreçler koşturan planlı işler _(Scheduled Jobs)_ sistemleri inanılaz derecede yorabiliyorlar. Bizim örneğimizde şimdilik tek bir iş var. Süresi dolan rapoları veri tabanı ve ftp'den silmek. Peki işlemler sırasında veritabanı bağlantısı yoksa ya da ftp'ye ulaşılamıyorsa sistemin genelinin vereceği tepki ne olmalı? Bu vakayı da dayanıklılık senaryolarımıza dahil edebiliriz.

Aşağıdaki ekran görüntülerinde arşivlenen ve süresi dolan raporların hem veri tabanından hem de ftp sunucusundan silinmesi ile ilgili çalışma zamanı görüntüleri yer alıyor.

![Before Archive Runtime](/images/runtime_15.png)

İlk görüntüde dikkat edileceği üzere arşive atılmış ve örnek bir kayıt var. Bu kayıt planlı iş ele alana kadar veri tabanında archived değeri true olarak duracak. Bu nedenle ftp sunucusunda bir süre daha yaşamakta. Planlı iş devreye girdikten sonra ise söz konusu kayıt veri tabanından ve ftp'den kalıcı olarak siliniyor. Aşağıdaki ekran görüntüsü de bunun kanıtı.

![After Archive Runtime](/images/runtime_16.png)

Tabii burada açıkta kalan bir nokta daha var. Arşivlenmediği halde süresi dolan raporlar için de bir planlı iş eklemek gerekebilir. Bu tip raporlar ftp'de olmayan ama süresi dolduğu halde db'de kalmaya devam eden türden raporlardır.

### Elasticsearch ve Kibana Entegrasyonu ile Log Takibi

Sistemlerin ürettiği **log**ları uygulamaların terminal pencereleri yerine Kibana gibi araçlar üzerinden monitör etmek dağıtık sistemler için önemli bir ihtiyaç. Farklı uygulamalardan akan log sayısı arttıkça bunları pencerelerden takip etmek zorlaştığı gibi loglar üzerinde sorgulama yapmak da neredeyse imkansız hale geliyor. Yüksek log üretimini ve bu küme üzerinde hızlı arama operasyonunu **Elasticsearch** gibi bir çözümle giderebiliriz. Nitelik **Elasitcsearch**'e akan logları görsel olarak takip ederken de bir araca ihtiyacımız var. Bu noktada **Kibana** sık kullanılan çözümlerin başında gelmekte.

Bu çözümde **Elasticsearch** ve **Kibana** implemantasyonu için yine **docker-compose** kompozisyonundan yararlanıyoruz. Geliştirme ortamı baz alınarak hareket ettiğimizi ifade edebilirim. İlk implementasyonumuzu **AuditApi** servisi üstünde gerçekleştirdik. **Kibana** ile **Elasticsearch** arasındaki entegrasyonda halletmem gereken bazı sorunlar oluştu. Logları **Discover** arabiriminde göremedikten sonra biraz araştırma yaparak gerekli veri akışını _(Data Stream)_ elle eklemeye karar verdim. Bunun için **Kibana Dev Tools** arabiriminden yararlanabiliriz. Bu arabirim bir servis API desteği sunarak bazı yönetsel işleri yapabilmemizi sağlamakta.

Takip eden ilk komut ile bir index şablonu oluşturmaktayız. **index_patterns** kısmında geçen ifade aynı zamanda **AuditApi** kodu içerisinde belirttiğimiz log desenini işaret ediyor.

```text
PUT _index_template/auditapi-logs-template
{
  "index_patterns": ["auditapi-logs-development*"],
  "data_stream": {},
  "template": {
    "mappings": {
      "properties": {
        "@timestamp": {
          "type": "date"
        },
        "message": {
          "type": "text"
        },
        "System": {
          "type": "keyword"
        },
        "Environment": {
          "type": "keyword"
        },
        "Level": {
          "type": "keyword"
        },
        "SourceContext": {
          "type": "keyword"
        }
      }
    }
  }
}
```

Bu işlemin ardından da aşağıdaki komut ile yukarıdaki index şablonu için bir **Data Stream** oluşturuyoruz.

```text
PUT /_data_stream/auditapi-logs-development
```

Artık bir Data Stream mevcut olduğundan AuditApi kodundan gönderilen logların akacağı kanal tanımlanmış bulunuyor. Bu işlemler ardından Kibana'dan ilgili logları izleyebiliriz. Aşağıdaki ekran görüntüsünde örnek bir çıktı görmektesiniz. Zamanlar diğer sistemlerdeki uygulama loglarını da bu ortamlara alacağız.

![ELK Dev Tools](/images/elk_01.png)

MiddleEarth sisteminde yer alan ve Redis Stream üzerinden event yorumlayan Kahin.EventHost için de Kibana'nın aynı arabiriminden aşağıdaki sorguları kullandık.

```text
PUT _index_template/kahin-event-host-logs-template
{
  "index_patterns": ["kahin-event-host-logs-development*"],
  "data_stream": {},
  "template": {
    "mappings": {
      "properties": {
        "@timestamp": {
          "type": "date"
        },
        "message": {
          "type": "text"
        },
        "System": {
          "type": "keyword"
        },
        "Environment": {
          "type": "keyword"
        },
        "Level": {
          "type": "keyword"
        },
        "SourceContext": {
          "type": "keyword"
        }
      }
    }
  }
}

PUT /_data_stream/kahin-event-host-logs-development
```

Bu arada unutmamamız gereken bir adım da söz konusu veri akışları için birer **Data View** oluşturulması gerekliliği. Bunu da örneğin aşağıdaki ekran görüntüsünde olduğu gibi yapabiliriz.

![ELK Add DataView](/images/elk_02.png)

### Ölçüm Metrikleri için Prometheus ve Grafana Entegrasyonu

System HOME içerisinde geliştirdiğimiz JobHost isimli bir terminal uygulamamız var. Bu uygulama belli periyotlarda planlı işler yürütmekte. Bunun için Hangfire paketinden yararlanıyoruz. Çok doğal olarak belirli periyotlarda çalışan bu işlerin çalışma zamanı metrikleri hakkında bilgi sahibi olmak önemli. Örneğin kaç iş başarılı oldu, kaçı hata aldı ya da ortalama çalışma süreleri gibi değerler sistemdeki sorunları gözlemlemek, erken tedbirler almak veya türlü alarm sistemlerini tetiklemek için kritik öneme sahip.

Bu noktada sık kullanılan yöntemlerden birisi host uygulamada oluşan metrikleri Prometheus gibi bir sisteme yollamak ve Grafana ile monitör etmek. Bunun için JobHost uygulaması içerisinde metrik değerleri veren bir server yer alıyor. Örneğimizde 1903 nolu porttan ulaşılmakta. Hatta aşağıdaki gibi bir çıktı elde edilmesi bekleniyor.

![Metric Server](/images/metric_server.png)

Docker-Compose dosyasında detaylarını görebileceğiniz Prometheus hizmeti, prometheus.yml dosyasında belirtilen süre aralıklarına göre bu adrese gelip ölçüm değerlerini alıyor. Ölçüm değerlerini kod içerisinden göndermekteyiz. Counter ve Histogram türlü birkaç örnek yer alıyor. Prometheus tarafında toplanan verilerin görsel olarak ele alınması içinse Grafana'ya başvuruyoruz. Eğer her şey yolunda giderse Grafana tarafında bir Dashboard oluştururken örnek olarak isimlendirdiğimiz aşağıdaki metriklere ulaşabilmemiz lazım.

- archiver_job_success_total
- archiver_job_failure_total
- eraser_job_success_total
- eraser_job_failure_total
- archiver_job_duration_seconds
- eraser_job_duration_seconds

Host uygulama, Prometheus ve Grafana arasındaki iletişimi aşağıdaki grafikle de özetleyebiliriz. Bahsi geçen Archiver ve Eraser isimli işler FTP sunucusu ve Postgresql veri tabanını kullanan bazı süreçleri işletiyorlar. Dolayısıyla süreçlerin ortlama işlem süreleri veya olası sorunlar sebebiyle başarısız sonuçlanmalarına dair sıklık sayıları bir dağıtık sistem çözümü düşünüldüğünde bilinmesi gereken metrik değerler.

![Prometheus Diagram](/images/prometheus_00.png)

Grafana üzerine aldığımız metriklere ait örnek bir Dashboard'u aşağıda görebilirsiniz. Burada Archiver işinin çalışma sürelerini görüntülemekteyiz.

![Grafana Rapor](/images/prometheus_01.png)

Elbette Grafana ve Prometheus sistemine akan metrikleri yorumlamak ve doğru Dashboard panellerini hazırlamak bana kalırsa biraz daha farklı uzmanlıklar gerektiriyor gibi. Hatta tam anlamıyla bir DevOps konusu olarak da düşünülebilir. Söz gelimi Prometheus'da akan verileri sorgulamak için PromQL isimli kendi sorgulama dilini kullanabiliyoruz. Yukarıdaki grafik için aşağıdaki sorgu kullanıldı örneğin.

```PromQL
rate(archiver_job_duration_seconds_sum[1m])
```

## POCO Diagramları

Çözümün 26.07.2024 tarihli resmine baktığımızda HAL, MiddleEarth, Home, Asgard ve Sergeant isimli farklı sistemlerden oluştuğunu görüyoruz. Bu sistemlerden Home, HAL ve MiddleEarth birbirlileriye daha ilişkili süreçler içeriyor. Şu ana kadar uygulanan düzensiz kodlama taktikleri veya gerilla kodlama pratikleri içeride teknik borç yükünü artırmaya başladı. Bunlardan birisi de bolca kullandığımız POCO _(Plain OLD CLR Objects)_ türlerimiz. Örneğin HOME sisteminde kullanılan ve Domain projesinde konuşlandırılmış tiplerin durumu aşağıdaki gibi.

![System HOME POCO Diagrams](/images/sys_home_pocos.png)

Buna göre bazı düzeltmeler yapılması yerinde olacak gibi duruyor.Örneğin HAL sadece expression kontrolü yaparken, Middle Earth rapor hazırlayıp dokümantasonu geri vermekle yükümlü. Ancak Middle Earth ile Home arasında taşınan nesnelerde gereksi veriler de gidiyor gibi. Bir konsolidasyon gerekli. İsimlendirmeler biraz daha açıklayıcı olabilir. Kim DTO, kim Web API için bir payload çok da anlaşılmıyor.

## Tartışılabilecek Problemler

- System HOME çözümündeki proje izolasyonu bir mimari stile oturtulabilir mi? Clean Architecture, Vertical Slice Architecture vs
- Service operasyonları değiştiğinde ThunderClient veya Postman gibi test araçlarındaki koleksiyonların güncellenmesi unutulduğunda servis dokümantasyonları tutarsızlaşabiliyor. Nasıl mücadele edilebilir.

## Youtube Anlatımları

Kodlar üzerinde ilerledikçe çözüm gittikçe büyümeye ve karmaşıklaşmaya başladı. Bazı önemli mevzulara ait anlatımları Youtube kanalında bulabilirsiniz.

- [.Net 8 ile Distributed Systems Challenge - 01 - Tanıtım](https://youtu.be/gMo8D2vUKR4)
- [.Net 8 ile Distributed Systems Challenge - 02 - Event Mekanizması](https://youtu.be/iOPHGECy5bU)
- [.Net 8 ile Distributed Systems Challenge - 03 - Secrets Management](https://youtu.be/kozb3c37f9k)
- [.Net 8 ile Distributed Systems Challenge - 04 - Güncel Durum Değerlendirmesi](https://youtu.be/LvZOCmnqujE)
- [.Net 8 ile Distributed Systems Challenge - 05 - Health Checks](https://youtu.be/dtiTMet4qFM)
- [.Net 8 ile Distributed Systems Challenge - 06 - SignalR ile Push Notification](https://youtu.be/P2IUqybwIKA)
- [.Net 8 ile Distributed Systems Challenge - 07 - SignalR ile JWT Kullanımı](https://youtu.be/29yi32GuPqU)
- [.Net 8 ile Distributed Systems Challenge - 08 - Resiliency Simulation Setup](https://youtu.be/Ihx5MYY4WDE)
- [.Net 8 ile Distributed Systems Challenge - 09 - Resiliency - Retry Mekanizması ile Güçlendirme](https://youtu.be/PDJ4GDNyWlQ)
- ...
