
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Text;

namespace WinMachine.Mvvm
{
    public class DisplayExceptionWindowModel : ViewModelBase
    {
        public string Header { get; set; }
        private string headerByDefault = "Error occured";

        public DisplayExceptionWindowModel(Exception exception, string header)
        {
            this.Header = (header != null) ? header : headerByDefault;

            var e = exception;
            int number = 1;
            do
            {
                this.errorLevels.Add(new ErrorLevel(number++, e));
                e = e.InnerException;
            } while (e != null);

            this.SelectedErrorLevel = this.errorLevels[0];
        }

        private ObservableCollection<ErrorLevel> errorLevels = new ObservableCollection<ErrorLevel>();
        public ObservableCollection<ErrorLevel> ErrorLevels
        {
            get { return errorLevels; }
        }

        public string ErrorTypeName
        {
            get
            {
                return (this.selectedErrorLevel != null) ?
                  this.selectedErrorLevel.exception.GetType().FullName : "";
            }
        }

        public string ErrorStackText
        {
            get
            {
                return (this.selectedErrorLevel != null) ?
                  GetErrorStackText(this.selectedErrorLevel.exception) : "";
            }
        }

        public string ErrorText
        {
            get
            {
                return (this.selectedErrorLevel != null) ?
                  this.selectedErrorLevel.exception.Message : "";
            }
        }

        private ErrorLevel selectedErrorLevel;
        public ErrorLevel SelectedErrorLevel
        {
            get
            {
                return this.selectedErrorLevel;
            }
            set
            {
                this.selectedErrorLevel = value;
                RaisePropertyChanged(nameof(ErrorTypeName));
                RaisePropertyChanged(nameof(ErrorStackText));
                RaisePropertyChanged(nameof(ErrorText));
            }
        }

        public class ErrorLevel
        {
            public int number;
            public Exception exception;

            public ErrorLevel(int number, Exception exception)
            {
                this.number = number;
                this.exception = exception;
            }

            public override string ToString()
            {
                string message = this.exception.Message;
                if (message != null)
                {
                    message = message.Replace("\r", "[CR]");
                    message = message.Replace("\n", "[LF]");
                }
                return this.number + ") " + message;
            }
        }

        public static string GetErrorStackText(Exception e)
        {
            if (e == null) return "Null exception object";

            StringBuilder buf = new StringBuilder();
            foreach (string line in e.StackTrace.Split('\n'))
            {
                if (!line.Contains(" System.")) buf.Append(line);
            }

            return buf.ToString();
        }
    }
}
