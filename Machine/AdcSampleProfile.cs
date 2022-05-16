using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinMachine.Machine
{
  public class AdcSampleProfile
  {
    public int MinValue { get; private set; }

    public int MaxValue { get; private set; }

    public bool IsDC { get; private set; }

    public bool IsAC => !IsDC;

    public AdcSampleProfile(bool isDC, int minValue, int maxValue)
    {
      IsDC = isDC;
      MinValue = minValue;
      MaxValue = maxValue;
    }

    public static readonly AdcSampleProfile DC8bit = new AdcSampleProfile(true, 0, 255);

    // ACS712(5A): 0A=>2.52V=>512adc, -5A=>1.6V=>327adc, 5A=>3.4V=>697adc
    public static readonly AdcSampleProfile ACS712AC10bit = new AdcSampleProfile(false, 327, 697);

    // Works for both 8bit 5V output and 3V3 8bit output. 
    // In the case of 5V: 0A=>2.52V=129adc, -5A=1.52V=>129-51=78adc, +5A=>3.52V=>129+51=180adc
    //public static readonly AdcSampleProfile ACS712AC8bit = new AdcSampleProfile(false, 78, 180);

    // Calibrated with divider: 0A=>133adc, +1.74A=>150adc, 1.74*2.87=5, (150-133=17)*2.87=49
    // +5A=>133+49=182adc, -5A=>133-49=84
    //public static readonly AdcSampleProfile ACS712AC8bit = new AdcSampleProfile(false, 84, 182);

    // 23.09.2020: 0A=128adc, 1.74A=145adc, 1.74*2.87=5, (145-128=17)*2.87=49
    // +5A=>128+49=177, -5A=128-49=79
    //public static readonly AdcSampleProfile ACS712AC8bit = new AdcSampleProfile(false, 81, 179);
    // manually adjusted 15/05/2022 to get zero amp at the center (notebook)
    public static readonly AdcSampleProfile ACS712AC8bit = new AdcSampleProfile(false, 83, 181);
  }
}
