using GrpcServer.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 🔥 CONFIGURAÇÃO DO SERILOG (ANTES DE TUDO)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/grpc-server-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7)
    .CreateLogger();

builder.Host.UseSerilog();

// ---------------- KESTREL ----------------
builder.WebHost.ConfigureKestrel(options =>
{
    // gRPC API (HTTP/2 + HTTPS)
    options.ListenLocalhost(7176, o =>
    {
        o.Protocols = HttpProtocols.Http2;
        o.UseHttps();
    });

    // Metrics ONLY (HTTP/1.1)
    options.ListenLocalhost(5184, o =>
    {
        o.Protocols = HttpProtocols.Http1;
    });
});

builder.Services.AddGrpc();

var app = builder.Build();

// 🚀 LOG DE INICIALIZAÇÃO (FORÇA CRIAR O ARQUIVO)
Log.Information("🚀 gRPC Server iniciado com sucesso");

// ---------------- PIPELINE ----------------
app.UseRouting();
app.UseHttpMetrics();
app.UseMetricServer();

app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "gRPC Server rodando");

app.Run();
