using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinMachine.App
{
    public class AdcSampleProfile
    {
        public int MinInput { get; private set; }

        public int MaxInput { get; private set; }

        public string OutputUnit { get; private set; }

        public int MinOutput { get; private set; }

        public int MaxOutput { get; private set; }

        public bool IsDC { get; private set; }

        public bool IsAC => !IsDC;

        public int InputSpan => (MaxInput - MinInput);

        public int OutputSpan => (MaxOutput - MinOutput);

        public AdcSampleProfile(bool isDC, int minInput, int maxInput, 
            string outputUnit, int minOutput, int maxOutput)
        {
            IsDC = isDC;
            MinInput = minInput;
            MaxInput = maxInput;
            OutputUnit = outputUnit;
            MinOutput = minOutput;
            MaxOutput = maxOutput;
        }

        public double TranslateToRate(int inputValue) 
            => ((double)(inputValue - MinInput) / InputSpan);

        public double TranslateToOutput(int inputValue) 
            => MinOutput + (TranslateToRate(inputValue) * OutputSpan);

        public static readonly AdcSampleProfile DC8bit = 
            new AdcSampleProfile(true, 0, 255, "V", 0, 5);

        // 23.09.2020: 0A=128adc, 1.74A=145adc, 1.74*2.87=5, (145-128=17)*2.87=49
        // +5A=>128+49=177, -5A=128-49=79
        public static readonly AdcSampleProfile ACS712_5A8bit = 
            new AdcSampleProfile(false, 79, 177, "A", -5, 5);

        // (0.5V,4.5V) => (-30A,+30A) from=255*10/100=25 to=255-25=230
        public static readonly AdcSampleProfile ACS712_30A8bit = 
            new AdcSampleProfile(false, 25, 230, "A", -30, 30);
    }
}
