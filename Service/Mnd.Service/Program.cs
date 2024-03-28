using Mnd.Service.BgWorker;
using Mnd.Service.Logic;
using Mnd.Service.Logic.Interfaces;
using Mnd.Service.SR;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<QueuedHostedService>();
builder.Services.AddSingleton<IBackgroundTaskQueue>(_ =>
{
    if (!int.TryParse(builder.Configuration.GetSection("Settings")["QueueCapacity"], out var queueCapacity))
    {
        queueCapacity = 100;
    }

    return new BackgroundTaskQueue(queueCapacity);
});
builder.Services.AddSingleton<ISettingsService>(_ => new SettingsService(builder.Configuration));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "All", policy =>
    {
        var uiOrigin = builder.Configuration.GetSection("Settings")["UiOrigin"]!;
        
        policy.WithOrigins(uiOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod();
        policy.AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("All");
app.UseAuthorization();
app.MapControllers();
app.MapHub<WsHub>("/ws");
app.Run();
