using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;

namespace WinMachine.Mvvm
{
    public class DialogService : IDialogService
    {
        public DialogService()
        {
        }

        private MessageBoxResult ShowMessageBox(
            Window owner, string message, string header, MessageBoxButton button, MessageBoxImage icon)
        {
            Func<MessageBoxResult> func = delegate
            {
                MessageBoxResult result;

                if (owner != null)
                {
                    result = MessageBox.Show(owner, message, header, button, icon);
                }
                else
                {
                    result = MessageBox.Show(message, header, button, icon);
                }

                return result;
            };

            return (MessageBoxResult)WindowServices.InvokeOnUIThread(func);
        }

        private MessageBoxResult ShowMessageBox(string message, string header, MessageBoxButton button, MessageBoxImage icon)
        {
            return ShowMessageBox(null, message, header, button, icon);
        }

        public void Show(string messsage)
        {
            Window owner = GetCurrentTopOwnerWindow();
            string title = "Message...";
            if (owner != null)
            {
                ShowMessageBox(owner, messsage, title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                ShowMessageBox(messsage, title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public bool ShowQuestion(string сообщение)
        {
            Window owner = GetCurrentTopOwnerWindow();
            MessageBoxResult result;
            string title = "Question...";
            if (owner != null)
            {
                result = ShowMessageBox(owner, сообщение, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            }
            else
            {
                result = ShowMessageBox(сообщение, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            }

            return result == MessageBoxResult.Yes;
        }

        public void Show(Exception exception, string header)
        {
            try
            {
                ShowDialog(new DisplayExceptionWindowView(),
                  new DisplayExceptionWindowModel(exception, header));
            }
            catch (Exception e)
            {
                string message = e.GetType().Name + ": " + e.Message;
                MessageBox.Show(message, "Error in error reporting");
            }
        }

        private List<Window> windowStack = new List<Window>();

        private Window GetCurrentTopOwnerWindow()
        {
            Window owner = null;
            try
            {
                Func<Window> func = delegate { return Application.Current.MainWindow; };
                owner = (Window)WindowServices.InvokeOnUIThread(func);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            if (windowStack.Count > 0)
            {
                owner = windowStack[windowStack.Count - 1];
            }
            return owner;
        }

        public bool? ShowDialog(Window window, object dataContext)
        {
            Window owner = GetCurrentTopOwnerWindow();

            if (owner != null) this.windowStack.Add(window);

            try
            {
                Func<bool?> func = delegate { return ShowDialogWithContext(window, dataContext, owner); };
                return (bool?)WindowServices.InvokeOnUIThread(func);

            }
            finally
            {
                if (owner != null) this.windowStack.Remove(window);
            }
        }

        private bool? ShowDialogWithContext(Window dialogWindow, object dataContext, Window ownerWindow)
        {
            double originalOpacity = 1;
            if (ownerWindow != null)
            {
                originalOpacity = ownerWindow.Opacity;
                ownerWindow.Opacity = originalOpacity * 0.8;
            }

            try
            {
                object originalContext = dialogWindow.DataContext;
                dialogWindow.DataContext = dataContext;
                if (ownerWindow != null && dialogWindow != ownerWindow)
                {
                    dialogWindow.Owner = ownerWindow;
                }

                bool? result = dialogWindow.ShowDialog();
                dialogWindow.DataContext = originalContext;
                return result;
            }
            finally
            {
                if (ownerWindow != null) ownerWindow.Opacity = originalOpacity;
            }
        }

        public bool? ShowDialog(object dataContext)
        {
            Window view = ViewLocator.FindViewForModel<Window>(dataContext);
            return ShowDialog(view, dataContext);
        }

        public Window GetCurrentWindow()
        {
            return GetCurrentTopOwnerWindow();
        }
    }
}
