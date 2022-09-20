
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;

namespace WinMachine.Mvvm
{
  public class WindowServices
  {
    public class Wpf32Window : System.Windows.Forms.IWin32Window
    {
      public System.Windows.Window WpfWindow { get; private set; }
      public IntPtr Handle { get; private set; }

      public Wpf32Window(System.Windows.Window wpfWindow)
      {
        WpfWindow = wpfWindow;
        Handle = GetWindowHWND(wpfWindow);
      }
    }

    public static System.Windows.Forms.IWin32Window GetWin32Window(Window window)
    {
      return new Wpf32Window(window);
    }

    /// <summary>
    /// Returns HWND for a WPF window.
    /// </summary>
    public static IntPtr GetWindowHWND(Window window)
    {
      var helper = new System.Windows.Interop.WindowInteropHelper(window);
      return helper.Handle;
    }

    /// <summary>
    /// Execute non-synchronous action on UI thread.
    /// </summary>
    public static void BeginInvokeOnUIThread(Action action)
    {
      if (GetDispatcher().CheckAccess()) {
        action();
      }
      else {
        GetDispatcher().BeginInvoke(action);
      }
    }

    /// <summary>
    /// Execute synchronous action on UI thread.
    /// </summary>
    public static object InvokeOnUIThread(Delegate method)
    {
      if (GetDispatcher().CheckAccess()) {
        return method.DynamicInvoke();
      }
      else {
        return GetDispatcher().Invoke(method);
      }
    }

    public static object CreateInstanceOnUIThread(Type objectType)
    {
      Func<object> func = delegate { return Activator.CreateInstance(objectType); };
      return InvokeOnUIThread(func);
    }

    public static System.Windows.Threading.Dispatcher GetDispatcher()
    {
      return Application.Current.Dispatcher;
    }

    public static int ScreenWidthInPixels => System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;

    public static int ScreenHeightInPixels => System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;

    public static double ConvertWpfPointsToPixels(double wpfPoints, LengthDirection direction)
    {
      // https://stackoverflow.com/questions/3286175/how-do-i-convert-a-wpf-size-to-physical-pixels
      if (direction == LengthDirection.Horizontal) {
        return wpfPoints * ScreenWidthInPixels / SystemParameters.WorkArea.Width;
      }
      else {
        return wpfPoints * ScreenHeightInPixels / SystemParameters.WorkArea.Height;
      }
    }

    public static double ConvertPixelsToWpfPoints(int pixels, LengthDirection direction)
    {
      // https://stackoverflow.com/questions/3286175/how-do-i-convert-a-wpf-size-to-physical-pixels
      if (direction == LengthDirection.Horizontal) {
        return pixels * SystemParameters.WorkArea.Width / ScreenWidthInPixels;
      }
      else {
        return pixels * SystemParameters.WorkArea.Height / ScreenHeightInPixels;
      }
    }

    public enum LengthDirection
    {
      Vertical, // |
      Horizontal // ——
    }
  }
}
