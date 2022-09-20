
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Ports;

namespace WinMachine.App
{
    public class SerialConnection : SerialPort
    {
        private static bool DebugPortFinder = false;

        public static readonly int[] PopularBaudRates = { 9600, 115200 };
        public static readonly int[] OtherBaudRates = { /*14400, 19200, 28800, 38400, 57600*/ };

        public const string DefaultPortName = "COM1";

        public SerialConnection(string portName, int baudRate = 115200) : base(portName, baudRate)
        {
            ReadTimeout = 3000;
            WriteTimeout = 3000;
        }

        public string Read(int length)
        {
            var buf = new char[length];
            for (int i = 0; i < length; i++) buf[i] = (char)ReadChar();
            return new string(buf);
        }

        public new void Close()
        {
            if (IsOpen)
            {
                BaseStream.Flush();
                DiscardInBuffer();
                BaseStream.Close();
                base.Close();
            }
        }

        public static SerialConnection Find(string query, string expectedReply)
        {
            Func<SerialConnection, bool> isDeviceAvailable = delegate (SerialConnection conn)
            {
                if (DebugPortFinder) Console.WriteLine($"{conn.PortName} {conn.BaudRate} query=[{query}]");
                conn.Write(query);
                var reply = conn.Read(expectedReply.Length);
                if (DebugPortFinder) Console.WriteLine("reply=[" + reply + "]");
                return (reply == expectedReply);
            };

            var found = Find(PopularBaudRates, isDeviceAvailable);
            if (found == null) found = Find(OtherBaudRates, isDeviceAvailable);
            return found;
        }

        public static SerialConnection Find(int[] baudRates, Func<SerialConnection, bool> isDeviceAvailable)
        {
            foreach (var portName in SerialPort.GetPortNames().Reverse())
            {
                foreach (int baudRate in baudRates)
                {
                    var conn = TryOpenAndCheckDevice(portName, baudRate, isDeviceAvailable);
                    if (conn != null) return conn;
                }
            }

            return null;
        }

        private static SerialConnection TryOpenAndCheckDevice(
          string portName, int baudRate, Func<SerialConnection, bool> isDeviceAvailable)
        {
            SerialConnection conn = null;

            try
            {
                conn = new SerialConnection(portName, baudRate);
                conn.Open();
                if (isDeviceAvailable(conn)) return conn;
            }
            catch (UnauthorizedAccessException)
            {
                // port is not available
            }
            catch (TimeoutException)
            {
                // baud rate or protocol error
            }
            catch (IOException)
            {
                // unexpected failure
            }

            if (conn != null)
            {
                conn.Close();
                conn.Dispose();
            }

            return null;
        }
    }
}
