using Grpc.Net.Client;
using Grpc.Core;
using GrpcServer;

Console.WriteLine("Starting gRPC load generator (50% error rate)...");

var channel = GrpcChannel.ForAddress("https://localhost:7176", new GrpcChannelOptions
{
    HttpHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    }
});

var client = new Greeter.GreeterClient(channel);

var sendError = false;

while (true)
{
    try
    {
        var name = sendError ? "error" : "ok";

        await client.SayHelloAsync(new HelloRequest
        {
            Name = name
        });

        Console.WriteLine(sendError
            ? "Sent ERROR request"
            : "Sent OK request");
    }
    catch (RpcException ex)
    {
        Console.WriteLine($"gRPC error: {ex.StatusCode}");
    }

    // alterna → 50% erro
    sendError = !sendError;

    await Task.Delay(200); // ~5 req/s
}
