using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;
using OneDriver.PowerSupply.Basic.GrpcHost.Protos;
using OneDriver.PowerSupply.Basic.GrpcHost.Services;

var builder = WebApplication.CreateBuilder(args);

// Register gRPC services
builder.Services.AddGrpc();


builder.Services.AddGrpcReflection();

var app = builder.Build();


app.MapGrpcService<PowerSupplyService>();
app.MapGrpcReflectionService(); // <- this works for grpcurl

app.MapGet("/", () => "Use a gRPC client like grpcurl to talk to this server.");

app.Run();
