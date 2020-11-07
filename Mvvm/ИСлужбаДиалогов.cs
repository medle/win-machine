
using System;
using System.Windows;
using System.Collections.Generic;

namespace WinMachine.Mvvm
{
  /// <summary>
  /// Служба взаимодействия с пользователем
  /// </summary>
  public interface ИСлужбаДиалогов
  {
    /// <summary>
    /// Показать в диалоге текст сообщения.
    /// </summary>
    void Показать(string сообщение);

    /// <summary>
    /// Показать в диалоге текст вопроса.
    /// </summary>
    bool ПоказатьВопрос(string сообщение);

    /// <summary>
    /// Показать в диалоге описание ошибки.
    /// </summary>
    void Показать(Exception ошибка, string заголовок);

    /// <summary>
    /// Показать окно в режиме диалога.
    /// </summary>
    bool? ПоказатьОкноДиалога(Window window, object dataContext);

    /// <summary>
    /// Найти для объекта контекста данных образ и показать его в виде окна диалога.
    /// </summary>
    bool? ПоказатьОкноДиалога(object dataContext);

    /// <summary>
    /// Возвращает текущее рабочее окно из стека диалогов.
    /// </summary>
    Window ДатьТекущееОкно();
  }
}
