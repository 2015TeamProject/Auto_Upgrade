using Auto_Upgrade.Controllers;
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

namespace Auto_Upgrade
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public ConfigListView configListView;
        public ConfigDetailsView configDetailsView;
        public ConfigCreationView configCreationView;

        public static string appName = System.IO.Path.GetFileName(System.Windows.Forms.Application.ExecutablePath).Replace(".EXE", ".exe");

        public MainWindow()
        {
            InitializeComponent();
            Url.url = ConfigManager.ReadUrlConfig();        // 读取UrlConfig.config文件中的url
            configListView = new ConfigListView(this);
            configDetailsView = new ConfigDetailsView(this);
            configCreationView = new ConfigCreationView(this);

            frame.Content = configListView;
        }

        public void ShutDown()
        {
            ShutDownController.shutDown();
        }
    }
}
