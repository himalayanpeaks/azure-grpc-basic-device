using OneDriver.PowerSupply.Abstract.Channels;

namespace OneDriver.PowerSupply.Basic.Channels
{
    /// <summary>
    /// Unused class
    /// </summary>
    public class Channel : CommonChannel<ChannelParams, ChannelProcessData>
    {
        public Channel(ChannelParams parameters, ChannelProcessData processData) : base(parameters, processData)
        {
        }
    }
}
