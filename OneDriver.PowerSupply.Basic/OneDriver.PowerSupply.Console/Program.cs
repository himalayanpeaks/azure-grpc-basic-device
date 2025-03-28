using OneDriver.Framework.Libs.Validator;
using System.Net;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var ps = new OneDriver.PowerSupply.Basic.Device("ps", new ComportValidator(), new OneDriver.PowerSupply.Basic.Products.Kd3005p());
        ps.Connect("COM5");
        ps.SetVolts(0, 7);
    }
}