# Feature(POC) Çalışmaları

Köklü değişikliğe sebep olabilecek özellikler veaya araştırmalar ile ilgili açılan alt branch'lere ait detaylı bilgiler.

- [Feature(POC) Çalışmaları](#featurepoc-çalışmaları)
  - [System HAL Servisinin Ayrıştırılması](#system-hal-servisinin-ayrıştırılması)
    - [Plan](#plan)
    - [Uygulama](#uygulama)
  - [SignalR Tarafında JWT Bazlı Doğrulama](#signalr-tarafında-jwt-bazlı-doğrulama)

## System HAL Servisinin Ayrıştırılması

Bu feature için kullanılan branch adı; **pocDockerizeAuditApi** şeklindedir.

System HAL içerisinde yer alan Audit servisinin DistributedChallenges solution'ı dışına çıkartılması ve Dockerize edilerek işletilmesi için başlatılan çalışmadır. Audit servis temsilen arayüzden gelen bir rapor talebindeki ifadeyi denetlemek için kullanılan REST tabanlı bir .Net servisidir. Bu servisin bazı paket bağımlılıkları bulunuyor.

- **Resistance**; Resilience deneyleri için kullanılan ve local ortamda depolanan nuget paketidir.
- **JudgeMiddleware**; Bazı performans ve girdi çıktı loglamaları için kullanılan ve local ortamda depolanan nuget paketidir.
- **Consul**; Servisin Consul üzerinden keşfedilebilmesi için kullanılan nuget paketidir.

### Plan

İlk olarak **SystemHAL** içerisindeki **Eval.AuditLib** içeriği **Eval.AuditApi** içerisine alınıp projenin biraz daha hafifletilmesi söz konusu. Sonrasında proje **DistributedChallenge** çözümünden ayrıştırılabilir. Yeni çözümdeki proje dockerize edilir. Burada karşılaşılabilecek ve çözülmesi gereken bazı problemler vardır.

- **PRB01 - Local Nuget Repo Sorunu:** Proje local nuget bağımlılıklarına sahip olduğundan bunları **Baget** sunucusunun **localhost:5000** adresi üstünden aramakta. Docker build işlemi sırasında çalıştırılan restore operasyonu container dışındaki bu adrese erişemeyecek. Bunun yerine Baget sunucusunun docker-compose ile ayağa kaldırdığı konuma erişebiliyor olması gerekir.
- **PRB02 - Consul Problemi:** Genel çözümde **Service Discovery** için kullanılan **Consul**, ayrı bir docker-compose içeriği ile ayağla kaldırılan farklı bir container olarak çalışmaktadır. **Dockerize** edilen **Eval.AuditApi** servisi ayağa kalkarken kendisini Consul hizmetine bildirmeye çalışacaktır. Dockerize edilen servisin diğer docker container'ındaki Consul hizmetine bağımsız olarak erişebiliyor olması gerekmektedir.

Yukarıda bahsedilen maddeler plan dahilinde çözümlenmesi gereken meselelerdir.

### Uygulama

**PRB01(Windows Sistemler için Geçerli)** kodlu sorun için root klasörde **nuget.config** dosyası oluşturuldu ve BaGet adresi olarak **host.docker.internal:5000/v3/index.json** kullanıldı. Ancak bu çözüm, docker imajı için build alınırken işe yarıyor. Projeyi bu **nuget.config** dosyası ile build ettiğimizde bu sefer de **localhost:5000** adresli nuget adresine bakmadığı için **Restore** işlemleri sırasında hata alınıyor.

*Not : Bu sorun windows sistemlerde yaşanıyor nitekim Linux tabanlı sistemlerde host.docker.internal şeklinde bir kavram bulunmuyor. Doğrudan localhost kullanılabilir.*

Aşağıdaki komutları çalıştırırken Restore işleminde hata alınmamsı için Baget servisinin aktif ve ilgili paketleri içerir olduğundan emin olalım.

```bash
# Docker imajını oluşturmak için root klasördeyken aşağıdaki komutu çalıştırmak yeterli (Windows için)
docker build -t systemhal/evalapi -f Eval.AuditApi/Dockerfile .

# Linux sistemde çalışırken host.docker.internal adresi geçerli olmayacaktır
# Bu nedenle aşağıdaki komut ile ilerlemek gerekecektir.
sudo docker build --network host -t systemhal/evalapi -f Eval.AuditApi/Dockerfile .
```

**PRB02(Windows Sistemler için Geçerli)** kodlu problemin çözümü için programın Consule hizmetine ait konfigurasyon ayarları aşağıdaki gibi değiştirildi.

```json
"Consul": {
  "Host": "host.docker.internal",
  "Discovery": {
    "ServiceName": "hal-audit-service",
    "Hostname": "localhost",
    "Port": 5147
  }
}
```

Normal sürümde **Consule:Host** için **localhost** kullanmak yeterli. Ancak servis uygulamasında **docker container** içerisine aldığımızda localhost erişebileceği bir adres olmaktan çıkıyor. Bunun yerine **local nuget** paketinde yaptığımız gibi **host.docker.internal** üzerinden erişim arıyoruz. Diğer yandan **Discovery** sekmesindeki bilgiler, **AuditApi** servisine dışarıdan gelen **Distributed Challenges** çözümündeki erişimler için. Burada **localhost** kullanmak gerekiyor nitekim **Consul**, **hal-audit-service** olarak gelen talepleri docker-compose ile container'ı içeri alınan **localhost:5147**'ye yönlendiriyor olacak. Bu gereksinimler docker-compose dosyasında da 5147:8080 yönlendirmesinin yapılmasını gerekirmekte. docker-compose dosyasının bu çözüm için uygulanmış hali aşağıdaki gibidir.

```yml
services:
  rabbitmq:
    image: rabbitmq:management
    environment:
      RABBITMQ_DEFAULT_USER: scothtiger
      RABBITMQ_DEFAULT_PASS: 123456
    ports:
      - "5672:5672"
      - "15672:15672"

  redis:
    image: redis:latest
    ports:
      - "6379:6379"

  localstack:
    image: localstack/localstack:latest
    ports:
      - "4566:4566"
      - "4571:4571"
    environment:
      - SERVICES=secretsmanager
      - DEBUG=1
      - DATA_DIR=/var/lib/localstack/data
    volumes:
      - localstack_data:/var/lib/localstack

  baget:
    image: loicsharma/baget:latest
    ports:
      - "5000:80"
    environment:
      - Baget__Database__Type=Sqlite
      - Baget__Database__ConnectionString=Data Source=/var/baget/baget.db
      - Baget__Storage__Type=FileSystem
      - Baget__Storage__Path=/var/baget/packages
    volumes:
      - baget_data:/var/baget

  postgres:
    image: postgres:latest
    environment:
      POSTGRES_USER: johndoe
      POSTGRES_PASSWORD: somew0rds
      POSTGRES_DB: ReportDb
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgres/data

  pgadmin:
    image: dpage/pgadmin4:latest
    environment:
      PGADMIN_DEFAULT_EMAIL: scoth@tiger.com
      PGADMIN_DEFAULT_PASSWORD: 123456
    ports:
      - "5050:80"
    depends_on:
      - postgres

  consul:
    image: hashicorp/consul:latest
    ports:
      - '8500:8500'
  
  ftp-server:
    image: delfer/alpine-ftp-server
    ports:
      - "21:21"
      - "21000-21010:21000-21010"
    environment:
      FTP_USER: "userone"
      FTP_PASS: "123"
      PASV_MIN_PORT: 21000
      PASV_MAX_PORT: 21010
    volumes:
      - ftp_data:/home/ftpuser/ftp_data

  audit-web-api:
    image: systemhome/evalapi
    ports:
      - "5147:8080"

volumes:
  localstack_data:
  baget_data:
  postgres_data:
  ftp_data:
```

## SignalR Tarafında JWT Bazlı Doğrulama

Söz konusu feature için **pocJwtWithSignalR** isimli branch kullanılıyor.

Bu çalışmada talep edilen raporların hazır olması, rapor ifadesinin doğrulama kontrolüne takılması, önceden hazırlanmış bir raporun arşive gönderilmesi gibi işlemlerde web ara yüzünde popup bildirimleri yapmaktayız. Burada sadece giriş yapan çalışanlara bildirim göndermek için **employeeId** gibi bir değerden yararlanıyor ve **localhost:5037/notifyHub** adresine gelirken bunu parametre olarak gönderiyoruz. Bu doğru görünse de ortada güvenlik riski olduğu da aşikar. Herhangibir kanaldan employeeId değerini bildiğimiz takdirde web socket'e mesaj gönderimi mümkün olabilir. Hatta bu durumu göstermek için SystemAsgard altında PirateClient isimli basit bir Nodejs uygulaması var. Bu uygulama Web uygulamasında açılan WebSocket'e mesaj gönderiyor. Bu aşağıdaki gibi istenmeyen bir mesaj bildiriminin yapılmasına yol açabilir.

![SignalR without JWT](/images/jwt_signalr_01.png)

Çözüm olarak bir otoriteden yararlanıp kullanıcıyı doğrulamak ve geçerli bir **JWT** token ürettirerek **SignalR** iletişimini bu token ile belli süre boyunca *(token ömrü boyunca)* yetkiye bağlamak bir tercih olabilir. Bunu çok basit bir şekilde ele alıyoruz. **IdentityServer**, **Klerk** vb parçalardan önce kendi kolay kullanıcı doğrulama ve JWT üretme servisimizi ele alıyoruz.

![JWT with SignalR](/images/jwt_signalr_00.png)

Kullanıcı bilgilerini, **encrpyt** edilmiş şifreleri ile birlikte veri tabanında bir tabloda tutmaktayız. En büyük sorunlardan birisi web uygulaması ile push notification işlemini gerçekleştiren event business nesnelerinin ayrı process'lerde işlemesi. Web uygulamasından login olunduğunda elde edilen token, Event Business nesneleri tarafından da **SignalR** tarafına yapılan push notification işleminde gerekli. Örneğin yeni bir rapor talebinde bulunulduğunda, login olan kullanıcı için üretilen ve SignalR tarafındaki yetki *(Authority)* için kullanılan token bilgisi, **System Middle Earth** veya **System Audit** tarafından gelen aksiyonlar sonrası oluşan **Rabbit MQ** olaylarının ele alındığı business nesneler için de gerekli. Arada geçen süre belirsiz. Dolayısıyla token bilgisini belli bir süre kayıt altında tutmak gerekebilir. En azından daha iyi bir çözüm bulana kadar bu şekilde ilerlenebilir. Yani **token** bilgisini **employeeId(RegistrationId)** ile ilişkilendirip yaşam süreleri boyunca saklayıp event dönüşlerinde push notification için de kullanabiliriz. Bu elbette başka bir sorunu daha gündeme getirecektir. Token süreleri dolduğunda depolanan token'ların da düşürülmesi için bir aksiyon alınmalıdır.

İşin içerisine JWT tabanlı yetkilendirmeyi bağladığımızda Jack Sparrow üzülecektir :D

![SignalR with JWT](/images/jwt_signalr_02.png)
