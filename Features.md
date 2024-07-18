# Feature/POC Çalýþmalarý

Köklü deðiþikliðe sebep olabilecek özellikler veaya araþtýrmalar ile ilgili açýlan alt branch'lere ait detaylý bilgiler.

## System HAL Servisinin Ayrýþtýrýlmasý

System HAL içerisinde yer alan Audit servisinin DistributedChallenges solution'ý dýþýna çýkartýlmasý ve Dockerize edilerek iþletilmesi için baþlatýlan çalýþmadýr. 

Audit servis temsilen arayüzden gelen bir rapor talebindeki ifadeyi denetlemek için kullanýlan REST tabanlý bir .Net servisidir. Bu servis içerisinde kullanýlan bazý paket baðýmlýlýklarý bulunmaktadýr.

- Resistance; Resilience deneyleri için kullanýlan ve local ortamda depolanan nuget paketidir.
- JudgeMiddleware; Bazý performans ve girdi çýktý loglamalarý için kullanýlan ve local ortamda depolanan nuget paketidir.
- Consul; Servisin Consul üzerinden keþfedilebilmesi için kullanýlan nuget paketidir.

### Plan

Ýlk olarak SystemHAL içerisindeki Eval.AuditLib içeriði Eval.AuditApi içerisine alýnýp projenin lightweight bir versiyonu hazýrlanýr. Sonrasýnda proje DistributedChallenge çözümünden ayrýþtýrýlýr. Yeni çözümdeki proje dockerize edilir. Burada karþýlaþýlabilecek ve çözülmesi gereken bazý problemler söz konusudur.

- **Local Nuget Repo Sorunu:** Dockerize edilen projeye ait container oluþturulurken container dýþýndaki ama ana makinedeki BaGet server'ýna eriþip local nuget baðýmlýlýklarýný çözümleyebilmelidir.
- **Consul Problemi:** Genel çözümde Service Discovery için kullanýlan Consul, ayrý bir docker-compose içeriði ile ayaðla kaldýrýlan farklý bir container olarak çalýþmaktadýr. Dockerize edilen Eval.AuditApi servisi ayaða kalkarken kendisini Consul hizmetine bildirmektedir. Dockerize edilen servisin diðer docker container'ýndaki Consul hizmetine baðýmsýz olarak eriþebiliyor olmasý gerekmektedir.

Yukarýda bahsedilen maddeler plan dahilinde çözümlenmesi gereken meselelerdir.

## Ýþlemler