using OneDriver.PowerSupply.Abstract;
using OneDriver.PowerSupply.Basic.Channels;

namespace OneDriver.PowerSupply.Basic
{
    public class DeviceViewModel : CommonDeviceViewModel<DeviceParams, ChannelParams, ChannelProcessData>
    {
        
        public DeviceViewModel(Device device) : base(device)
        {
        }        
    }
}
