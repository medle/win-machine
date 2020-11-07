
using System;
using System.ComponentModel;

namespace WinMachine.Mvvm
{
  public class ViewModelBase : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void RaisePropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void InvokeOnUIThread(Action action)
    {
      System.Windows.Application.Current.Dispatcher.Invoke(action);
    }
  }
}
