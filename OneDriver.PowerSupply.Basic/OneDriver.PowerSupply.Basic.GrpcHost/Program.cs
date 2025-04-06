using Microsoft.Azure.Devices.Client;
using OneDriver.PowerSupply.Basic;
using OneDriver.PowerSupply.Basic.Products;
using OneDriver.PowerSupply.Basic.GrpcHost.Services;
using OneDriver.Framework.Libs.Validator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

Console.WriteLine("Enter your Azure IoT device connection string:");
string deviceConnectionString = Console.ReadLine();


builder.Services.AddSingleton<DeviceClient>(provider =>
{
    var client = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);
    client.OpenAsync().GetAwaiter().GetResult();
    return client;
});

builder.Services.AddSingleton<Device>(provider =>
{
    var validator = new ComportValidator();
    var hal = new Kd3005p();
    return new Device("Korad", validator, hal);
});

var app = builder.Build();

app.MapGrpcService<PowerSupplyService>();
app.MapGrpcReflectionService();

app.MapGet("/", () => "Use a gRPC client to communicate.");

app.Run();
