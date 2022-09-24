
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinMachine.App
{
    public class MachineDevice : IDisposable
    {
        private SerialConnection serial;

        private const string OkReplyPrefix = "OK: ";

        public MachineDevice()
        {
        }

        public void Open(string serialPortName, int baudRate)
        {
            if (!IsOpen)
            {
                serial = new SerialConnection(serialPortName, baudRate);
                serial.DtrEnable = true; // necessary for PiPico
                serial.NewLine = "\r\n";
                serial.Open();
            }
        }

        public bool IsOpen => (serial != null && serial.IsOpen);

        public void Close()
        {
            if (IsOpen)
            {
                try { Stop(); } catch (Exception) { /*ignored*/}
                serial.Close();
                serial.Dispose();
                serial = null;
            }
        }

        public void Dispose()
        {
            if (serial != null)
            {
                serial.Dispose();
                serial = null;
            }
        }

        private string RunCommand(string command)
        {
            if (!IsOpen) throw new Exception($"Can't run command [{command}]: device is not opened");

            string reply;
            try
            {
                serial.WriteLine(command);
                reply = serial.ReadLine();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to run command [{command}]", e);
            }

            if (reply != null && reply.StartsWith(OkReplyPrefix))
                return reply.Substring(OkReplyPrefix.Length);

            throw new Exception($"Command [{command}] resulted with unexpected reply [{reply}]");
        }

        public string PortName => serial.PortName;

        public int BaudRate => serial.BaudRate;

        public string Hello => RunCommand("HELLO");

        public string StartPWM(int hz, int duty1024, int deadClocks)
          => RunCommand($"PWM {hz} {duty1024} {deadClocks}");

        public string RunADC(int analogPin) => RunCommand($"ADC {analogPin}");

        public string Stop() => RunCommand("STOP");

        public string RunADC(int analogPin, IList<int> resultSamples)
        {
            string reply = RunADC(analogPin);
            ParseADCReplyToList(reply, resultSamples);
            return reply;
        }

        private void ParseADCReplyToList(string adcReplyText, IList<int> resultSamples)
        {
            if (resultSamples == null) throw new ArgumentNullException();
            resultSamples.Clear();

            int sampleValue = 0;
            bool isSampleActive = false;

            foreach (char ch in adcReplyText)
            {
                if (ch >= '0' && ch <= '9')
                {
                    sampleValue = (sampleValue * 10) + (ch - '0');
                    isSampleActive = true;
                }
                else if (ch == ' ')
                {
                    if (isSampleActive)
                    {
                        resultSamples.Add(sampleValue);
                        sampleValue = 0;
                        isSampleActive = false;
                    }
                }
                else if (ch == '\r' || ch == '\n') break;
                else throw new Exception($"Unexpected character {(int)ch} in GetADC reply");
            }

            if (isSampleActive)
            {
                resultSamples.Add(sampleValue);
            }
        }

        public int ConvertFrequencyToMicroseconds(string frequencyHz)
        {
            double hz;

            if (!Double.TryParse(frequencyHz, out hz))
                throw new Exception($"Can't convert frequency value [{frequencyHz}] to number");
            if (hz < 50 || hz > 100000)
                throw new Exception($"Frequency {hz} Hz out of range [50,100k]");

            return (int)(1000000 / hz);
        }

        public int ConvertDutyCycleToBase1024(string dutyCycle100)
        {
            double duty100;

            if (!double.TryParse(dutyCycle100, out duty100))
                throw new Exception($"Can't convert duty cycle value [{dutyCycle100}] to number");
            if (duty100 < 1 || duty100 > 99)
                throw new Exception($"Duty cycle {duty100}% out of range [1,99]");

            return (int)(1024 * duty100 / 100);
        }
    }
}
