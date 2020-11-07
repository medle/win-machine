
using System;

namespace WinMachine.Mvvm
{
  /// <summary>
  /// Класс для измерений продолжительности времени в миллисекундах.
  /// </summary>
  public class MilliTimer
  {
    // при создании запоминает текущее время
    public DateTime StartTime = DateTime.Now;

    /// <summary>
    /// Возвращает количество миллисекунд с начала измерения
    /// </summary>
    public int Elapsed { get { 
      return (int)DateTime.Now.Subtract(StartTime).TotalMilliseconds; } }

    /// <summary>
    /// Возвращает строку с форматированным количеством прошедших
    /// с момента старта миллисекунд.
    /// </summary>
    public override string ToString()
    {
      return this.Elapsed + "ms";
    }

    public void Trace(string header)
    {
      System.Diagnostics.Trace.WriteLine(header + ": " + ToString());
    }
  }
}
