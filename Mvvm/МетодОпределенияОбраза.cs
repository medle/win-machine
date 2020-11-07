
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;

namespace WinMachine.Mvvm
{
  /// <summary>
  /// Реализаци алгоритма определения типа образа по типу модели и создание нового объекта образа.
  /// </summary>
  public class МетодОпределенияОбраза
  {
    /// <summary>
    /// Возвращает новый объект образа для данного объекта модели (контекста данных).
    /// Чтобы образ для контекста нашелся, необходимо чтобы для класса контекста имя
    /// которого МодельXXX существовал класс ОбразXXX который является подклассом Window/Control.
    /// </summary>
    public static Тип СделатьОбразДляМодели<Тип>(object dataContext)
    {
      if (dataContext == null)
        ThrowBadContext(dataContext, "контекст данных = null", null);

      // проверяем префикс модели
      Type type = dataContext.GetType();
      string modelPrefix = "Модель";
      if (!type.Name.StartsWith(modelPrefix))
        ThrowBadContext(dataContext,
          "Имя класса контекста данных должно начинаться строкой [" + modelPrefix + "]", null);

      // собираем имя класса образа в том же простарнстве имен как и контекст данных
      string viewName = "Образ" + type.Name.Substring(modelPrefix.Length);
      string viewTypeName = type.Namespace + "." + viewName;

      // ищем тип образа
      Type viewType = null;
      try {
        viewType = type.Assembly.GetType(viewTypeName);
      }
      catch (Exception e) {
        ThrowBadContext(dataContext, "Невозможно найти класс: " + viewTypeName, e);
      }

      if (viewType == null)
        ThrowBadContext(dataContext, "Не существует класс образа c именем: " + viewTypeName, null);

      // создаем объект образа
      object obj = null;
      try {
        obj = WindowServices.CreateInstanceOnUIThread(viewType);
      }
      catch (Exception e) {
        ThrowBadContext(dataContext, "Невозможно создать объект типа " + viewTypeName, e);
      }

      // объект должен быть нужного типа
      if (obj is Тип) return (Тип)obj;
      ThrowBadContext(dataContext, "Тип " + viewTypeName + " не является подклассом " + typeof(Тип).FullName, null);
      return default(Тип);
    }

    /// <summary>
    /// Генерация сообщения об ошибке для алгоритма поиска окна по модели образа.
    /// </summary>
    private static object ThrowBadContext(object dataContext, string message, Exception innerException)
    {
      string header = "Нельзя показать окно диалога по контексту данных";
      string about = "";
      if (dataContext != null) {
        about = " [" + dataContext.GetType().FullName + "]";
      }
      throw new Exception(header + about + ": " + message, innerException);
    }
  }
}
