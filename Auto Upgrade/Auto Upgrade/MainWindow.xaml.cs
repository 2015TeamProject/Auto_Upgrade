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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace Auto_Upgrade
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public ConfigListView configListView;
        public ConfigEditView configEditView;
        public ConfigInformationView configInformationView;

        public MainWindow()
        {
            InitializeComponent();
            configListView = new ConfigListView(this);
            configEditView = new ConfigEditView(this);
            configInformationView = new ConfigInformationView(this);
            frame.Content = configListView;
        }   

        public void ShutDown()
        {
            App.Current.Shutdown();
        }

        private void frame_Navigated(object sender, NavigationEventArgs e)
        {

        }

    }
}
