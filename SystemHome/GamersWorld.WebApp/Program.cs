using GamersWorld.WebApp;
using GamersWorld.WebApp.Services;
using JudgeMiddleware;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Steeltoe.Common.Http.Discovery;
using Steeltoe.Discovery.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDependencies();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());
builder.Services.AddSignalR();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDiscoveryClient();
builder.Services.AddHttpClient<MessengerServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://web-backend-service");
})
.AddServiceDiscovery()
.AddRoundRobinLoadBalancer();

var app = builder.Build();

app.AddJudgeMiddleware(new Options
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
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<StatusHub>("notifyHub");

app.Run();
