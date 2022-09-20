
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;

namespace WinMachine.Mvvm
{
    public class ViewLocator
    {
        public static T FindViewForModel<T>(object dataContext)
        {
            if (dataContext == null)
                ThrowBadContext(dataContext, "Null data context", null);

            Type type = dataContext.GetType();
            string modelPostfix = "Model";
            if (!type.Name.EndsWith(modelPostfix))
                ThrowBadContext(dataContext, "Class name must end with [" + modelPostfix + "]", null);

            string name = type.Name;
            string viewName = name.Substring(0, name.Length - modelPostfix.Length) + "View";
            string viewTypeName = type.Namespace + "." + viewName;

            Type viewType = null;
            try
            {
                viewType = type.Assembly.GetType(viewTypeName);
            }
            catch (Exception e)
            {
                ThrowBadContext(dataContext, "Can't find class: " + viewTypeName, e);
            }

            object obj = null;
            try
            {
                obj = WindowServices.CreateInstanceOnUIThread(viewType);
            }
            catch (Exception e)
            {
                ThrowBadContext(dataContext, "Can't create object of type " + viewTypeName, e);
            }

            if (obj is T) return (T)obj;
            ThrowBadContext(dataContext, 
                "Type " + viewTypeName + " is not a subclass of " + typeof(T).FullName, null);
            return default(T);
        }

        private static object ThrowBadContext(object dataContext, string message, Exception innerException)
        {
            string header = "Can't show window for data context";
            string about = "";
            if (dataContext != null)
            {
                about = " [" + dataContext.GetType().FullName + "]";
            }
            throw new Exception(header + about + ": " + message, innerException);
        }
    }
}
