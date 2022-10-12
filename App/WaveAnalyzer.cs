using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WinMachine.App
{
    public class WaveAnalyzer
    {
        private int stepHz = 25;
        private int waitBeforeAdcMs = 50;

        public int Analyze(MachineDevice machineDevice, int channel)
        {
            var samples = new List<int>();

            var centerHz = machineDevice.FrequencyHz;
            machineDevice.RunADC(channel, samples);
            var centerMax = samples.Max();

            var belowHz = centerHz - stepHz;
            machineDevice.SendPWM(belowHz);
            Thread.Sleep(waitBeforeAdcMs);
            machineDevice.RunADC(channel, samples);
            var belowMax = samples.Max();

            var aboveHz = centerHz + stepHz;
            machineDevice.SendPWM(aboveHz);
            Thread.Sleep(waitBeforeAdcMs);
            machineDevice.RunADC(channel, samples);
            var aboveMax = samples.Max();

            int nextHz = centerHz;
            if (belowMax >= centerMax && belowMax >= aboveMax) nextHz = belowHz;
            if (aboveMax >= centerMax && aboveMax >= belowMax) nextHz = aboveHz;

            if (nextHz != aboveHz) machineDevice.SendPWM(nextHz);
            return nextHz;
        }
    }
}
