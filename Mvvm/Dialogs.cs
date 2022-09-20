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
      var service = new DialogService();
      service.Show(message);
    }

    public static void ShowError(Exception e, string caption)
    {
      var service = new DialogService();
      service.Show(e, caption);
    }
  }
}
