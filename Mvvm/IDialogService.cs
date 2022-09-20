
using System;
using System.Windows;
using System.Collections.Generic;

namespace WinMachine.Mvvm
{
  public interface IDialogService
  {
    void Show(string message);
    bool ShowQuestion(string message);
    void Show(Exception e, string header);
    bool? ShowDialog(Window window, object dataContext);
    bool? ShowDialog(object dataContext);
    Window GetCurrentWindow();
  }
}
