using Grpc.Core;
using OneDriver.PowerSupply.Basic.GrpcHost.Protos;


namespace OneDriver.PowerSupply.Basic.GrpcHost.Services
{
    public class PowerSupplyService : Protos.PowerSupply.PowerSupplyBase

    {
        private readonly Device _device;

        public PowerSupplyService(Device device)
        {
            _device = device;
        }

        public override Task<StatusReply> OpenConnection(OpenRequest request, ServerCallContext context)
        {
            var code = _device.Connect(request.Port);
            return Task.FromResult(new StatusReply
            {
                Code = (int)code,
                Message = code == 0 ? "Connection opened." : "Failed to open connection."
            });
        }

        public override Task<StatusReply> SetVolts(SetRequest request, ServerCallContext context)
        {
            var code = _device.SetVolts(request.Channel, request.Value);
            return Task.FromResult(new StatusReply
            {
                Code = code,
                Message = code == 0 ? "Voltage set." : "Failed to set voltage."
            });
        }

        public override Task<StatusReply> SetAmps(SetRequest request, ServerCallContext context)
        {
            var code = _device.SetAmps(request.Channel, request.Value);
            return Task.FromResult(new StatusReply
            {
                Code = code,
                Message = code == 0 ? "Current set." : "Failed to set current."
            });
        }

        public override Task<StatusReply> AllChannelsOn(Empty request, ServerCallContext context)
        {
            var code = _device.AllChannelsOn();
            return Task.FromResult(new StatusReply
            {
                Code = code,
                Message = "All channels turned ON."
            });
        }

        public override Task<StatusReply> AllChannelsOff(Empty request, ServerCallContext context)
        {
            var code = _device.AllChannelsOff();
            return Task.FromResult(new StatusReply
            {
                Code = code,
                Message = "All channels turned OFF."
            });
        }

        public override async Task StreamProcessData(StreamRequest request, IServerStreamWriter<ProcessDataReply> responseStream, ServerCallContext context)
        {
            var channelNumber = request.ChannelNumber;

            while (!context.CancellationToken.IsCancellationRequested)
            {
                var voltage = _device.Elements[channelNumber].ProcessData.Voltage;
                var current = _device.Elements[channelNumber].ProcessData.Current;
                var timestamp = _device.Elements[channelNumber].ProcessData.TimeStamp;

                await responseStream.WriteAsync(new ProcessDataReply
                {
                    Voltage = voltage,
                    Current = current,
                    Timestamp = timestamp.ToLongTimeString() // ISO 8601 format
                });

                await Task.Delay(1000); // adjust as needed (e.g., every 500 ms)
            }
        }

    }
}
