using Microsoft.EntityFrameworkCore;
using WaveSync.Api.Data;
using WaveSync.Api.Hubs;
using WaveSync.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<RoomStateService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WaveSyncDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Database")
    ));

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");
app.MapHub<WaveSyncHub>("/hubs/wavesync");

app.MapControllers();

app.Run();