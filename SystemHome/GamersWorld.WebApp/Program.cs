using GamersWorld.WebApp.Utility;
using JudgeMiddleware;
using SecretsAgent;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddScoped<MessengerServiceClient>();
builder.Services.AddSingleton<ISecretStoreService, SecretStoreService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.AddJudgeMiddleware(new MetricOptions
{
    DurationThreshold = TimeSpan.FromSeconds(2)
});

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
