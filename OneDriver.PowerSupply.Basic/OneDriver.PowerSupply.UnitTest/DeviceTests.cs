using Xunit;
using Moq;
using OneDriver.PowerSupply.Basic;
using OneDriver.PowerSupply.Basic.Products;
using OneDriver.Framework.Libs.Validator;
using System.Collections.ObjectModel;
using OneDriver.PowerSupply.Basic.Channels;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.ComponentModel.DataAnnotations;

public class DeviceTests
{
    private readonly Mock<IValidator> _validatorMock;
    private readonly Mock<IPowerSupplyHAL> _halMock;
    private readonly Device _device;

    public DeviceTests()
    {
        _validatorMock = new Mock<IValidator>();
        _halMock = new Mock<IPowerSupplyHAL>();

        _halMock.Setup(h => h.NumberOfChannels).Returns(2);
        _halMock.Setup(h => h.MaxVoltageInVolts).Returns(30);
        _halMock.Setup(h => h.MaxCurrentInAmpere).Returns(5);
        _halMock.Setup(h => h.Mode).Returns(new OneDriver.PowerSupply.Abstract.Contracts.Definition.ControlMode[2]);

        _device = new Device("TestDevice", _validatorMock.Object, _halMock.Object);
    }

    [Fact]
    public void Constructor_InitializesDeviceWithCorrectChannelCount()
    {
        Assert.Equal(2, _device.Elements.Count);
    }

    [Fact]
    public void AllChannelsOn_CallsHAL_AllOn()
    {
        _halMock.Setup(h => h.AllOn()).Returns(OneDriver.Framework.Module.Definition.DeviceError.NoError);

        var result = _device.AllChannelsOn();

        _halMock.Verify(h => h.AllOn(), Times.Once);
        Assert.Equal((int)OneDriver.Framework.Module.Definition.DeviceError.NoError, result);
    }

    [Fact]
    public void AllChannelsOff_CallsHAL_AllOff()
    {
        _halMock.Setup(h => h.AllOff()).Returns(OneDriver.Framework.Module.Definition.DeviceError.NoError);

        var result = _device.AllChannelsOff();

        _halMock.Verify(h => h.AllOff(), Times.Once);
        Assert.Equal((int)OneDriver.Framework.Module.Definition.DeviceError.NoError, result);
    }

    [Fact]
    public void SetVolts_CallsHAL_WithCorrectParameters()
    {
        int channel = 0;
        double volts = 12.5;
        _halMock.Setup(h => h.SetDesiredVolts(channel, volts)).Returns(OneDriver.Framework.Module.Definition.DeviceError.NoError);

        var result = _device.SetVolts(channel, volts);

        _halMock.Verify(h => h.SetDesiredVolts(channel, volts), Times.Once);
        Assert.Equal((int)OneDriver.Framework.Module.Definition.DeviceError.NoError, result);
    }

    [Fact]
    public void SetAmps_CallsHAL_WithCorrectParameters()
    {
        int channel = 1;
        double amps = 3.2;
        _halMock.Setup(h => h.SetDesiredAmps(channel, amps)).Returns(OneDriver.Framework.Module.Definition.DeviceError.NoError);

        var result = _device.SetAmps(channel, amps);

        _halMock.Verify(h => h.SetDesiredAmps(channel, amps), Times.Once);
        Assert.Equal((int)OneDriver.Framework.Module.Definition.DeviceError.NoError, result);
    }
}
