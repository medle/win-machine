using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;

namespace WinMachine.Mvvm
{
  /// <summary>
  /// Реализация службы диалогов в WPF.
  /// </summary>
  public class СлужбаДиалогов: ИСлужбаДиалогов
  {
    /// <summary>
    /// Конструктор.
    /// </summary>
    public СлужбаДиалогов()
    { 
    }

    private MessageBoxResult ShowMessageBox(Window окноВладелец, string сообщение, string заголовок, MessageBoxButton кнопка, MessageBoxImage иконка)
    // СТ 09.11.2016
    {
      Func<MessageBoxResult> задействуйОснПоток = delegate {
        MessageBoxResult результатДиалога;

        if (окноВладелец != null)
        {
          результатДиалога = MessageBox.Show(окноВладелец, сообщение, заголовок, кнопка, иконка);
        }
        else
        {
          результатДиалога = MessageBox.Show(сообщение, заголовок, кнопка, иконка);
        }

        return результатДиалога;
      };

      return (MessageBoxResult)WindowServices.InvokeOnUIThread(задействуйОснПоток);
    }

    private MessageBoxResult ShowMessageBox(string сообщение, string заголовок, MessageBoxButton кнопка, MessageBoxImage иконка)
    {
      return ShowMessageBox(null, сообщение, заголовок, кнопка, иконка);
    }

    /// <summary>
    /// Показать в диалоге текст сообщения.
    /// </summary>
    public void Показать(string сообщение)
    {
      Window owner = GetCurrentTopOwnerWindow();
      string title = "Сообщение...";
      if (owner != null) {
        //MessageBox.Show(owner, сообщение, title, MessageBoxButton.OK, MessageBoxImage.Information);
        ShowMessageBox(owner, сообщение, title, MessageBoxButton.OK, MessageBoxImage.Information);
      } else {
        //MessageBox.Show(сообщение, title, MessageBoxButton.OK, MessageBoxImage.Information);
        ShowMessageBox(сообщение, title, MessageBoxButton.OK, MessageBoxImage.Information);
      }
    }

    /// <summary>
    /// Показать в диалоге текст вопроса.
    /// </summary>
    public bool ПоказатьВопрос(string сообщение)
    {
      Window owner = GetCurrentTopOwnerWindow();
      MessageBoxResult result;
      string title = "Вопрос...";
      if (owner != null) {
        //result = MessageBox.Show(owner, сообщение, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        result = ShowMessageBox(owner, сообщение, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
      } else {
        //result = MessageBox.Show(сообщение, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        result = ShowMessageBox(сообщение, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
      }
      return (result == MessageBoxResult.Yes);
    }

    /// <summary>
    /// Показать в диалоге описание ошибки.
    /// </summary>
    public void Показать(Exception exception, string заголовок)
    {
      // остальные сообщения показываем с подробностями 
      try {
        ПоказатьОкноДиалога(new ОбразОкнаПоказаИсключения(), 
          new МодельОкнаПоказаИсключения(exception, заголовок));
      } catch (Exception e) {
        string сообщение = e.GetType().Name + ": " + e.Message;
        MessageBox.Show(сообщение, 
          "Ошибка в системе сообщений об ошибках");
      }
    }

    private List<Window> стекОконДиалога = new List<Window>();

    /// <summary>
    /// Возвращает текущее окно верхнего уровня, от которого следует строить 
    /// появляющиеся окна диалога.
    /// </summary>
    private Window GetCurrentTopOwnerWindow()
    {
      Window владелец = null;
      try
      {
        Func<Window> задействуйОснПоток = delegate { return Application.Current.MainWindow; };
        владелец = (Window)WindowServices.InvokeOnUIThread(задействуйОснПоток);
      }
      catch(InvalidOperationException)
      {
        return null;
      }
      if (стекОконДиалога.Count > 0)
      {
        владелец = стекОконДиалога[стекОконДиалога.Count - 1];
      }
      return владелец;
    }

    /// <summary>
    /// Показать окно в режиме диалога.
    /// </summary>
    public bool? ПоказатьОкноДиалога(Window window, object dataContext)
    {
      Window owner = GetCurrentTopOwnerWindow();

      // помещаем в стек окно которое сейчас будет отображено
      if(owner != null) this.стекОконДиалога.Add(window);

      // выполнение диалога
      try {

        Func<bool?> func = delegate { return ShowDialogWithContext(window, dataContext, owner); };
        return (bool?)WindowServices.InvokeOnUIThread(func);

      } finally {
        // всегда убираем из стека отработанное окно
        if (owner != null) this.стекОконДиалога.Remove(window);
      }
    }

    /// <summary>
    /// Выполнение показа окна WPF с данным контекстом данных и данным окном владельцем.
    /// Может исполняться только в потоке диспетчера.
    /// </summary>
    private bool? ShowDialogWithContext(Window dialogWindow, object dataContext, Window ownerWindow)
    {
      // затемнение родительского окна
      double originalOpacity = 1;
      if (ownerWindow != null) {
        originalOpacity = ownerWindow.Opacity;
        ownerWindow.Opacity = originalOpacity * 0.8;
      }

      try {

        // устанавливаем на окно контекст данных 
        object originalContext = dialogWindow.DataContext;
        dialogWindow.DataContext = dataContext;
        if (ownerWindow != null && dialogWindow != ownerWindow) {
          dialogWindow.Owner = ownerWindow;
        }

        // и отправляем окно на показ в WPF
        bool? result = dialogWindow.ShowDialog();
        dialogWindow.DataContext = originalContext;
        return result;
      }
      finally {
        // всегда восстановливаем прозрачность родительского окна
        if (ownerWindow != null) ownerWindow.Opacity = originalOpacity;
      }
    }

    /// <summary>
    /// Найти для объекта контекста данных образ и показать его в виде окна диалога.
    /// </summary>
    public bool? ПоказатьОкноДиалога(object dataContext)
    {
      Window образ = МетодОпределенияОбраза.СделатьОбразДляМодели<Window>(dataContext);
      return ПоказатьОкноДиалога(образ, dataContext);
    }

    /// <summary>
    /// Возвращает текущее рабочее окно из стека диалогов. Вернет null если текущий поток
    /// не является потоком в которм было создано главное окно приложения.
    /// </summary>
    public Window ДатьТекущееОкно()
    {
      return GetCurrentTopOwnerWindow();
    }
  }
}
