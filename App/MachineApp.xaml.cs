using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WinMachine.App
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class MachineApp : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      this.DispatcherUnhandledException += OnDispatcherUnhandledException;

      var mainWindow = new MainWindow();
      Application.Current.MainWindow = mainWindow;
      Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

      using (var model = new MainWindowModel(mainWindow.MainGraphDrawingVisual)) {
        mainWindow.Loaded += delegate { model.OnWindowLoaded(); };
        mainWindow.Closed += delegate { model.OnWindowClosed(); }; 
        mainWindow.DataContext = model;
        mainWindow.Show();
      }
    }

    private void OnDispatcherUnhandledException(object sender,
      DispatcherUnhandledExceptionEventArgs e)
    {
      WinMachine.Mvvm.Dialogs.ShowError(e.Exception, "Unhandled exception");  
      e.Handled = true;
    }

    protected override void OnExit(ExitEventArgs e)
    {
    }
  }
}
