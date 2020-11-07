using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinMachine.Mvvm
{
  public class Dialogs
  {
    public static void ShowMessage(string message)
    {
      var служба = new СлужбаДиалогов();
      служба.Показать(message);
    }

    public static void ShowError(Exception e, string caption)
    {
      var служба = new СлужбаДиалогов();
      служба.Показать(e, caption);
    }
  }
}
