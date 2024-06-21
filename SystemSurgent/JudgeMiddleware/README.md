# JudgeMiddleware

Web Api ve Web uygulamalarında Middleware'e eklenebilecek basit eklentiler içerir.

## PerformanceBehavior

Bir api çağrısının performansını ölçümler. Varsayılan olarak açıktır. Kapatmak için MetricOptions değeri true olarak işaretlenebilir.

## InputOutputBehavior

Bir web api çağrısındaki girdi ve çıktı değerlerini loglar. Varsayılan olarak açıktır. Kapatmak için MetricOptions değeri true olarak işaretlenebilir.

## Usage

```csharp
var app = builder.Build();

app.AddJudgeMiddleware(new MetricOptions
{
    DurationThreshold = TimeSpan.FromSeconds(2)
});
```
