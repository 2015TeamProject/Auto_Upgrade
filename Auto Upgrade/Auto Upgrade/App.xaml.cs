using Auto_Upgrade.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Auto_Upgrade
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwin = Application.Current.MainWindow as MainWindow;
            if (mainwin.tabConrol.SelectedIndex != 0)
                mainwin.tabConrol.Items.Remove(mainwin.tabConrol.SelectedItem);
        }
    }

}
