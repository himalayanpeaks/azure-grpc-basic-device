using OneDriver.PowerSupply.Basic;
using OneDriver.PowerSupply.Basic.Products;
using OneDriver.PowerSupply.Basic.GrpcHost.Services;
using OneDriver.Framework.Libs.Validator;

var builder = WebApplication.CreateBuilder(args);

// Register gRPC services
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.AddSingleton<Device>(provider =>
{
    var validator = new ComportValidator();
    var hal = new Kd3005p();
    return new Device("Korad", validator, hal);
});

var app = builder.Build();

app.MapGrpcService<PowerSupplyService>();
app.MapGrpcReflectionService(); // allows grpcurl to work

app.MapGet("/", () => "Use a gRPC client like grpcurl to talk to this server.");

app.Run();
