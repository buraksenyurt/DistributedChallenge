# Feature(POC) Çalışmaları

Köklü değişikliğe sebep olabilecek özellikler veaya araştırmalar ile ilgili açılan alt branch'lere ait detaylı bilgiler.

- [Feature(POC) Çalışmaları](#feature-poc-çalışmaları)
	- [System HAL Servisinin Ayrıştırılması](#system-hal-servisinin-ayrıştırılması)
		- [Plan](#plan)
		- [Uygulama](#uygulama)

## System HAL Servisinin Ayrıştırılması

**Branch -> pocDockerizeAuditApi**

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

**PRB01** kodlu sorun için root klasörde **nuget.config** dosyası oluşturuldu ve BaGet adresi olarak **host.docker.internal:5000/v3/index.json** kullanıldı. Ancak bu çözüm, docker imajı için build alınırken işe yarıyor. Projeyi bu **nuget.config** dosyası ile build ettiğimizde bu sefer de **localhost:5000** adresli nuget adresine bakmadığı için **Restore** işlemleri sırasında hata alınıyor.

```bash
# Docker imajını oluşturmak için root klasördeyken aşağıdaki komutu çalıştırmak yeterli
docker build -t systemhome/evalapi -f Eval.AuditApi/Dockerfile .
```

**PRB02** kodlu problemin çözümü için programın Consule hizmetine ait konfigurasyon ayarları aşağıdaki gibi değiştirildi.

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
