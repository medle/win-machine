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
  /// <summary>
  /// Interaction logic for ОбразОкнаПоказаИсключения.xaml
  /// </summary>
  public partial class ОбразОкнаПоказаИсключения : Window
  {
    public ОбразОкнаПоказаИсключения()
    {
      InitializeComponent();
    }

    private void НаКнопкуПродолжения(object sender, RoutedEventArgs e)
    {
      Close();
    }
  }
}
