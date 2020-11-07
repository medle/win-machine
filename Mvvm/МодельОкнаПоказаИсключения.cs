using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Text;

namespace WinMachine.Mvvm
{
  /// <summary>
  /// Логика отображения элементов окна диалога показа данных о возникшем исключении.
  /// </summary>
  public class МодельОкнаПоказаИсключения: WinMachine.Mvvm.ViewModelBase
  {
    public string Заголовок { get; set; }

    private string заголовокПоУмолчанию = "Выполнение операции привело к отказу в программе.";

    /// <summary>
    /// Конструктор.
    /// </summary>
    public МодельОкнаПоказаИсключения(Exception exception, string заголовок)
    {
      this.Заголовок = (заголовок != null) ? заголовок : заголовокПоУмолчанию;

      var e = exception;
      int number = 1;
      do {
        this.этажиОшибки.Add(new ЭтажОшибки(number++, e));
        e = e.InnerException;
      } while (e != null);

      this.ВыбранныйЭтажОшибки = this.этажиОшибки[0];
    }

    /// <summary>
    /// Этажи ошибки соответствуют уровням вложенности исключений.
    /// </summary>
    private ObservableCollection<ЭтажОшибки> этажиОшибки = new ObservableCollection<ЭтажОшибки>();
    public ObservableCollection<ЭтажОшибки> ЭтажиОшибки
    {
      get { return этажиОшибки; }
    }

    /// <summary>
    /// Имя типа ошибки соответствует имени класса экземпляра исключения.
    /// </summary>
    public string ИмяТипаОшибки 
    {
      get 
      {
        return (this.выбранныйЭтажОшибки != null) ? 
          this.выбранныйЭтажОшибки.exception.GetType().FullName : ""; 
      }
    }

    /// <summary>
    /// Текст стека вызовов отражает точку возникновения исключения и точки 
    /// размотки стека до ее получения.
    /// </summary>
    public string ТекстСтекаВызовов
    {
      get
      {
        return (this.выбранныйЭтажОшибки != null) ? 
          ДатьТекстСтекаВызовов(this.выбранныйЭтажОшибки.exception) : "";
      }
    }

    public string ТекстОшибки
    {
      get {
        return (this.выбранныйЭтажОшибки != null) ?
          this.выбранныйЭтажОшибки.exception.Message : "";
      }
    }

    /// <summary>
    /// Пользователь может выбрать любой уровень вложенности ошибки.
    /// </summary>
    private ЭтажОшибки выбранныйЭтажОшибки;
    public ЭтажОшибки ВыбранныйЭтажОшибки
    {
      get
      {
        return this.выбранныйЭтажОшибки;
      }
      set 
      { 
        this.выбранныйЭтажОшибки = value;
        RaisePropertyChanged(nameof(ИмяТипаОшибки));
        RaisePropertyChanged(nameof(ТекстСтекаВызовов));
        RaisePropertyChanged(nameof(ТекстОшибки));
      }
    }

    /// <summary>
    /// Реализация контейнера уровня вложенности.
    /// </summary>
    public class ЭтажОшибки 
    {
      public int number;
      public Exception exception;

      public ЭтажОшибки(int number, Exception exception) 
      {
        this.number = number;
        this.exception = exception;
      }

      public override string ToString()
      {
        string message = this.exception.Message;
        // удалим переводы строк в сообщении
        if (message != null) {
          message = message.Replace("\r", "[CR]");
          message = message.Replace("\n", "[LF]");
        }
        return this.number + ") " + message;
      }
    }

    public static string ДатьТекстСтекаВызовов(Exception e)
    {
      if (e == null) return "Нет объекта Exception для описания стека";

      // оставляем только строки стека которые не относятся к 
      // классам пространства имен System, это не очень надежный
      // символьный тест, но он удалит много ненужных строк
      StringBuilder buf = new StringBuilder();
      foreach (string line in e.StackTrace.Split('\n')) {
        if (!line.Contains(" System.")) buf.Append(line);
      }

      return buf.ToString();
    }
  }
}
