using OneDriver.Framework.Libs.Announcer;
using OneDriver.Framework.Module;

namespace OneDriver.PowerSupply.Basic.Products
{
    public interface IPowerSupplyHAL : IHalLayer<InternalDataHAL>, IStringReader, IStringWriter
    {
        public string Identification { get; }
        public Abstract.Contracts.Definition.ControlMode[] Mode { get; }
        public Framework.Module.Definition.DeviceError SetMode(double channelNumber, Abstract.Contracts.Definition.ControlMode mode);
        public double MaxCurrentInAmpere { get; }
        public double MaxVoltageInVolts { get; }
        public string GetErrorMessage(int code);
        public Framework.Module.Definition.DeviceError SetDesiredVolts(double channelNumber, double volts);
        public Framework.Module.Definition.DeviceError GetActualVolts(double channelNumber, out double volts);
        public Framework.Module.Definition.DeviceError SetDesiredAmps(double channelNumber, double amps);
        public Framework.Module.Definition.DeviceError GetActualAmps(double channelNumber, out double amps);
        public Framework.Module.Definition.DeviceError AllOff();
        public Framework.Module.Definition.DeviceError AllOn();
    }
}
