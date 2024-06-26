using GamersWorld.WebApp;
using GamersWorld.WebApp.Utility;
using JudgeMiddleware;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SecretsAgent;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddScoped<MessengerServiceClient>();
builder.Services.AddSingleton<ISecretStoreService, SecretStoreService>();
builder.Services.AddSingleton<IUserIdProvider, EmployeeUserIdProvider>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());
builder.Services.AddSignalR();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.AddJudgeMiddleware(new MetricOptions
{
    DurationThreshold = TimeSpan.FromSeconds(2),
    DeactivateInputOutputBehavior = true,
});

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseHealthChecks("/health");

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<StatusHub>("notifyHub");

app.Run();
