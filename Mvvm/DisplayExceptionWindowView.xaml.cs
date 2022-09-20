using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WinMachine.Mvvm
{
  public partial class DisplayExceptionWindowView : Window
  {
    public DisplayExceptionWindowView()
    {
      InitializeComponent();
    }

    private void OnContinueClick(object sender, RoutedEventArgs e)
    {
      Close();
    }
  }
}
