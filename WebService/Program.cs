using Common.Interfaces.Service;
using WebService.Service;
using WebService.Service.CommunicationServices;
using WebService.Service.StatisticsServices;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(config =>
{
    config.DisconnectedCircuitRetentionPeriod = TimeSpan.FromSeconds(10);
});

builder.Services.AddBlazorBootstrap();

builder.Services.AddSingleton<MqttTopicStatusService<byte>>();
builder.Services.AddSingleton<IMqttService, MqttService>();

builder.Services.AddSingleton<ProductionFailureHandlerService>();

builder.Services.AddSingleton<ManufacturedAmountService>();

builder.Services.AddScoped<RfidAuthMqttService>();

builder.Services.AddHostedService<EmergencyShutdownService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();