using Grpc.Core;
using Prometheus;

namespace GrpcServer.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;

    // 🔢 Métrica Prometheus (core da observabilidade)
    private static readonly Counter GrpcRequestsTotal =
        Metrics.CreateCounter(
            "grpc_requests_total",
            "Total gRPC requests",
            new CounterConfiguration
            {
                LabelNames = new[] { "method", "status" }
            });

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(
        HelloRequest request,
        ServerCallContext context)
    {
        try
        {
            // 🔥 Erro simulado (500 / Internal)
            if (request.Name == "error")
            {
                throw new RpcException(
                    new Status(StatusCode.Internal, "Erro simulado"));
            }

            // ✅ Métrica de sucesso
            GrpcRequestsTotal
                .WithLabels("SayHello", "OK")
                .Inc();

            _logger.LogInformation(
                "gRPC request OK | Method={Method} | Name={Name}",
                "SayHello",
                request.Name);

            return Task.FromResult(new HelloReply
            {
                Message = $"Hello {request.Name}"
            });
        }
        catch (RpcException ex)
        {
            // ❌ Métrica de erro
            GrpcRequestsTotal
                .WithLabels("SayHello", ex.StatusCode.ToString())
                .Inc();

            // 🧠 Log estruturado (investigação de erro)
            _logger.LogError(
                ex,
                "gRPC error | Method={Method} | Status={Status} | Name={Name}",
                "SayHello",
                ex.StatusCode,
                request.Name);

            throw;
        }
    }
}
