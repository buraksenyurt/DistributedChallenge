# Resistance

Bu pakette, Web Api servislerine eklenebilecek ve çeşitli Resilience durumlarının oluşmasını sağlayacak Middleware davranışlarına yer verilmektedir. Örneğin bir servisin belli bir koşulda çağıran tarafa 500 Internal Server Error dönerek bir Network Failure durumunu simüle etmek için NetworkFailureBehavior'ı middleware'e ekleyebiliriz.

## NetworkFailureBehavior

Api çağrılarında belli bir yüzdeye göre HTTP 500 InternalServerError dönülmesine neden olur.

## Usage

```csharp
var app = builder.Build();

// Returns HTTP 500 with %25 probability for Network Failure simulation
app.AddResistance(new Options
{
    NetworkFailureIsActive = true,
    NetworkFailureProbability = FailureProbability.Percent25
});

```
