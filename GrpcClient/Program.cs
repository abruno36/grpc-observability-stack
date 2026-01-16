using Grpc.Net.Client;
using GrpcServer;
using Grpc.Core;

Console.WriteLine("Cliente gRPC iniciado");

using var channel = GrpcChannel.ForAddress("https://localhost:7176");
var client = new Greeter.GreeterClient(channel);

try
{
    var response = await client.SayHelloAsync(
        new HelloRequest { Name = "error" }); // força erro

    Console.WriteLine(response.Message);
}
catch (RpcException ex)
{
    Console.WriteLine("Erro ao chamar serviço gRPC");
    Console.WriteLine($"Status: {ex.StatusCode}");
    Console.WriteLine($"Detalhe: {ex.Status.Detail}");
}
