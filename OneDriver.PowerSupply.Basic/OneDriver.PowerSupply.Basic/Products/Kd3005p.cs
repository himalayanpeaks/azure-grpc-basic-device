using OneDriver.Framework.Libs.Announcer;
using System.IO.Ports;
using OneDriver.Framework.Libs.Validator;
using OneDriver.Framework.Module;
using OneDriver.Toolbox;
using Serilog;
using System.Globalization;

namespace OneDriver.PowerSupply.Basic.Products
{
    public class Kd3005p : DataTunnel<InternalDataHAL>, IPowerSupplyHAL
    {
        public Kd3005p()
        {
            ComPort = new SerialPort
            {
                ReadTimeout = 1000,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                BaudRate = 9600
            };
            MaxCurrentInAmpere = 5;
            MaxVoltageInVolts = 30;
            NumberOfChannels = 1;
            Mode = new Abstract.Contracts.Definition.ControlMode[NumberOfChannels];
            Identification = "KORAD KD3005P";
        }

        protected override void FetchDataForTunnel(ref InternalDataHAL data)
        {
            for (var i = 0; i < NumberOfChannels; i++)
            {
                GetActualVolts(i, out var volts);
                GetActualAmps(i, out var amps);
                if(volts != prediousVolts || amps != prediousAmps)
                    data = new InternalDataHAL(i, volts, amps);
                prediousVolts = volts;
                prediousAmps = amps;    
            }
        }

        private double prediousVolts;
        private double prediousAmps;
        public Framework.Module.Definition.DeviceError Read(out string readData)
        {
            readData = "";
            try
            {
                ComPort.DiscardOutBuffer();
                readData = ComPort.ReadLine().ToString(new CultureInfo("en-EN"));
                Thread.Sleep(300);
            }
            catch (TimeoutException e)
            {
                Log.Error(e.ToString());
                return Framework.Module.Definition.DeviceError.Timeout;
            }
            catch (InvalidOperationException e)
            {
                Log.Error(e.ToString());
                return Framework.Module.Definition.DeviceError.ConnectionError;
            }
            catch (ArgumentException e)
            {
                Log.Error(e.ToString());
                return Framework.Module.Definition.DeviceError.DataIsNull;
            }
            return Framework.Module.Definition.DeviceError.NoError;
        }

        public Framework.Module.Definition.DeviceError Write(string data)
        {
            try
            {
                //ComPort.DiscardInBuffer();
                ComPort.WriteLine(data);
                Thread.Sleep(300);

            }
            catch (TimeoutException e)
            {
                Log.Error(e.ToString());
                return Framework.Module.Definition.DeviceError.Timeout;
            }
            catch (InvalidOperationException e)
            {
                Log.Error(e.ToString());
                return Framework.Module.Definition.DeviceError.ConnectionError;
            }
            catch(ArgumentException e)
            {
                Log.Error(e.ToString());
                return Framework.Module.Definition.DeviceError.DataIsNull;
            }
            return Framework.Module.Definition.DeviceError.NoError;
        }

        private SerialPort ComPort { get; set; }

        public ConnectionError Open(string initString, IValidator validator)
        {
            ComPort.PortName = validator.ValidationRegex.Match(initString).Groups[1].Value;
            try
            {
                if (!ComPort.IsOpen)
                {
                    ComPort.Open();
                    Write("*IDN?");
                    if((Read(out var identification) is var err) && err != Framework.Module.Definition.DeviceError.NoError)
                    {
                        Log.Error(ComPort.PortName + " " + ConnectionError.CommunicationError.GetDescription());
                        return ConnectionError.CommunicationError;
                    }
                    StartProcessDataAnnouncer();
                }
                else
                    Log.Error(ComPort.PortName + " " + ConnectionError.AlreadyOpened.GetDescription());

            }
            catch (ArgumentException)
            {
                Log.Error(ComPort.PortName + " " + ConnectionError.InvalidName.GetDescription());
                return ConnectionError.InvalidName;
            }
            catch (UnauthorizedAccessException)
            {
                Log.Error(ComPort.PortName + " " + ConnectionError.UnauthorizedAccess.GetDescription());
                return ConnectionError.UnauthorizedAccess;
            }
            catch (IOException)
            {
                Log.Error(ComPort.PortName + " " + ConnectionError.IOError.GetDescription());
                return ConnectionError.IOError;
            }
            catch (InvalidOperationException)
            {
                Log.Error(ComPort.PortName + " " + ConnectionError.InvalidOperation.GetDescription());
                return ConnectionError.InvalidOperation;
            }
            catch (Exception)
            {
                Log.Error(ComPort.PortName + " " + ConnectionError.UnknownError.GetDescription());
                return ConnectionError.UnknownError;
            }

            return ConnectionError.NoError;
        }

        public ConnectionError Close()
        {
            try
            {
                ComPort.Close();
            }
            catch (Exception)
            {
                Log.Error(ComPort.PortName + " " + ConnectionError.ErrorInDisconnecting.GetDescription());
                return ConnectionError.ErrorInDisconnecting;
            }
            return ConnectionError.NoError;
        }

        public double MaxCurrentInAmpere { get; }
        public double MaxVoltageInVolts { get; }
        public string Identification { get; private set; }
        public Abstract.Contracts.Definition.ControlMode[] Mode { get; }
        public Framework.Module.Definition.DeviceError SetMode(double channelNumber, Abstract.Contracts.Definition.ControlMode mode)
        {
            return Framework.Module.Definition.DeviceError.NoError;
        }
        public string GetErrorMessage(int code)
        {
            if (Enum.IsDefined(typeof(Framework.Module.Definition.DeviceError), code))
                return ((Framework.Module.Definition.DeviceError)code).ToString();

            return $"Unknown error code: {code}";
        }


        public Framework.Module.Definition.DeviceError SetDesiredVolts(double channelNumber, double volts)
        {
            var str = "VSET" + (channelNumber + 1) + ":" + volts.ToString(new CultureInfo("en-EN"));
            var err = Write(str);
            if (err != Framework.Module.Definition.DeviceError.NoError)
                return err;
            else
                Write("OUT" + (channelNumber + 1));

            return Framework.Module.Definition.DeviceError.NoError;
        }

        public Framework.Module.Definition.DeviceError GetActualVolts(double channelNumber, out double volts)
        {
            volts = 0;
            string command = $"VOUT{channelNumber + 1}?";

            if (Write(command) is var err && err != Framework.Module.Definition.DeviceError.NoError)
                return err;
            
            if (Read(out var val) is var readErr && readErr != Framework.Module.Definition.DeviceError.NoError)
                return readErr;

            if (double.TryParse(val, NumberStyles.Float, new CultureInfo("en-EN"), out volts))
                return Framework.Module.Definition.DeviceError.NoError;
            Log.Error(val + " : Invalid response from device.");
            return Framework.Module.Definition.DeviceError.InvalidResponse; 
        }
        public Framework.Module.Definition.DeviceError GetActualAmps(double channelNumber, out double amps)
        {
            amps = 0;
            string command = $"IOUT{channelNumber + 1}?";

            if (Write(command) is var err && err != Framework.Module.Definition.DeviceError.NoError)
                return err;
            if (Read(out var val) is var readErr && readErr != Framework.Module.Definition.DeviceError.NoError)
                return readErr;

            if (double.TryParse(val, NumberStyles.Float, new CultureInfo("en-EN"), out amps))
                return Framework.Module.Definition.DeviceError.NoError;
            Log.Error(val + " : Invalid response from device.");
            return Framework.Module.Definition.DeviceError.InvalidResponse;
        }

        public Framework.Module.Definition.DeviceError SetDesiredAmps(double channelNumber, double amps)
        {
            var str = "ISET" + (channelNumber + 1) + ":" + amps.ToString(new CultureInfo("en-EN"));
            var err = Write(str);
            if(err != Framework.Module.Definition.DeviceError.NoError)
                return err;
            else
                Write("OUT" + (channelNumber + 1));
            return Framework.Module.Definition.DeviceError.NoError;
        }

        public Framework.Module.Definition.DeviceError AllOff()
        {
            return Write("OUT0");
        }

        public Framework.Module.Definition.DeviceError AllOn()
        {
            return Write("OUT1");
        }

        public void StartProcessDataAnnouncer() => StartAnnouncingData();

        public void StopProcessDataAnnouncer() => StopAnnouncingData();

        public void AttachToProcessDataEvent(DataEventHandler processDataEventHandler)
            => DataEvent += processDataEventHandler;

        public int NumberOfChannels { get; }
    }
}
