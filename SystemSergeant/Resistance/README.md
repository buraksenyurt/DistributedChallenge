# Resistance

Bu pakette, Web Api servislerine eklenebilecek ve çeşitli Resilience durumlarının oluşmasını sağlayacak Middleware davranışlarına yer verilmektedir. Örneğin bir servisin belli bir koşulda çağıran tarafa 500 Internal Server Error dönerek bir Network Failure durumunu simüle etmek için NetworkFailureBehavior'ı middleware'e ekleyebiliriz. Ya da servis cevap sürelerinde belli periyotlarda gecikmelere sebebiyet vermek için LatencyBehavior davranışını çalışma zamanı hattına monte edebiliriz.

## NetworkFailureBehavior

Api çağrılarında belli bir yüzdeye göre HTTP 500 InternalServerError dönülmesine neden olur.

## LatencyBehavior

Api çağrılarında cevap sürelerinin belli mili saniye değerlerinde geciktirilmesini sağlar.

## ResourceRaceBehavior

Eş zamanlı istek sayısının ayarlanarak HTTP 429 TooManyRequest probleminin oluşturulmasını sağlar.

## Usage

Normal şartlarda tüm simülasyon reçeteleri pasiftir. Etkinleştirmek için açık bir şekilde IsActive özelliklerine true değerlerinin atanması gerekir. Bazı reçeteler kendi özel parametrelerine ihtiyaç duyabilir.

```csharp
var app = builder.Build();

// Returns HTTP 500 with %25 probability for Network Failure simulation
app.AddResistance(new Options
{
    NetworkFailureIsActive = true,
    NetworkFailureProbability = FailureProbability.Percent25,
});

```
