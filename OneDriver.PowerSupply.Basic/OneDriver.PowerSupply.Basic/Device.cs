using OneDriver.Framework.Base;
using OneDriver.Framework.Libs.Validator;
using OneDriver.Framework.Module.Parameter;
using OneDriver.PowerSupply.Abstract;
using OneDriver.PowerSupply.Basic.Channels;
using OneDriver.PowerSupply.Basic.Products;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Serilog;

namespace OneDriver.PowerSupply.Basic
{
    public class Device : CommonDevice<DeviceParams, ChannelParams, ChannelProcessData>
    {
        public Device(string name, IValidator validator, IPowerSupplyHAL powerSupplyHAL) :
            base(new DeviceParams(name), validator, new ObservableCollection<BaseChannelWithProcessData<ChannelParams, ChannelProcessData>>())
        {
            _powerSupplyHAL = powerSupplyHAL;
            Init();
        }

        private void Init()
        {
            Parameters.PropertyChanging += Parameters_PropertyChanging;
            Parameters.PropertyChanged += Parameters_PropertyChanged;
            Parameters.PropertyReadRequested += Parameters_PropertyReadRequested;
            _powerSupplyHAL.AttachToProcessDataEvent(ProcessDataChanged);


            for (var i = 0; i < _powerSupplyHAL.NumberOfChannels; i++)
            {
                var item = new BaseChannelWithProcessData<ChannelParams, ChannelProcessData>(new ChannelParams("Ch" + i.ToString()), new ChannelProcessData());
                
                item.Parameters.PropertyChanged += Parameters_PropertyChanged;
                item.Parameters.PropertyChanging += Parameters_PropertyChanging;
                item.Parameters.PropertyReadRequested += Parameters_PropertyReadRequested;
                Elements.Add(item);
            }   

        }

        private void Parameters_PropertyReadRequested(object sender, PropertyReadRequestedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Parameters.MaxVolts):
                    e.Value = _powerSupplyHAL.MaxVoltageInVolts;
                    break;
                case nameof(Parameters.MaxAmps):
                    e.Value = _powerSupplyHAL.MaxCurrentInAmpere;
                    break;
            }
        }

        private void ProcessDataChanged(object sender, InternalDataHAL e)
        {
            Elements[e.ChannelNumber].ProcessData.Current = e.CurrentCurrent;
            Elements[e.ChannelNumber].ProcessData.Voltage = e.CurrentVoltage;
            Elements[e.ChannelNumber].ProcessData.TimeStamp = e.TimeStamp;
        }

        private void Parameters_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            int index = -1;
            switch (e.PropertyName)
            {
                case nameof(ChannelParams.DesiredAmps):
                    var channel = this.Elements.FirstOrDefault(x => x.Parameters == (ChannelParams)sender);
                    index = this.Elements.IndexOf(channel);
                    _powerSupplyHAL.SetDesiredAmps(index, (((ChannelParams)sender).DesiredAmps));
                    break;
                case nameof(ChannelParams.DesiredVolts):
                    channel = this.Elements.FirstOrDefault(x => x.Parameters == (ChannelParams)sender);
                    index = this.Elements.IndexOf(channel);
                    _powerSupplyHAL.SetDesiredVolts(index, (((ChannelParams)sender).DesiredVolts));
                    break;
                case nameof(ChannelParams.ControlMode):
                    channel = this.Elements.FirstOrDefault(x => x.Parameters == (ChannelParams)sender);
                    index = this.Elements.IndexOf(channel);
                    _powerSupplyHAL.SetMode(index, (((ChannelParams)sender).ControlMode));
                    break;
            }
        }

        private void Parameters_PropertyChanging(object sender, PropertyValidationEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(BaseChannelWithProcessData<ChannelParams, ChannelProcessData>.Parameters.DesiredAmps):
                    if ((double)e.NewValue > Parameters.MaxAmps)
                    {
                        Log.Error("Desired Amps is greater than Max Amps");
                        throw new ArgumentOutOfRangeException(e.PropertyName);
                    }

                    break;
                case nameof(BaseChannelWithProcessData<ChannelParams, ChannelProcessData>.Parameters.DesiredVolts):
                    if ((double)e.NewValue > Parameters.MaxVolts)
                    {
                        Log.Error("Desired Volts is greater than Max Volts");
                        throw new ArgumentOutOfRangeException(e.PropertyName);
                    }
                    break;
            }
        }

        readonly IPowerSupplyHAL _powerSupplyHAL;
        protected override int CloseConnection() => (int)_powerSupplyHAL.Close();

        protected override int OpenConnection(string initString) => (int)_powerSupplyHAL.Open(initString, validator);

        public override int AllChannelsOff() => (int)_powerSupplyHAL.AllOff();

        public override int SetVolts(int channelNumber, double volts) => (int)_powerSupplyHAL.SetDesiredVolts(channelNumber, volts);

        public override int SetAmps(int channelNumber, double amps) => (int)_powerSupplyHAL.SetDesiredAmps(channelNumber, amps);

        public override int AllChannelsOn() => (int)_powerSupplyHAL.AllOn();
    }
}
