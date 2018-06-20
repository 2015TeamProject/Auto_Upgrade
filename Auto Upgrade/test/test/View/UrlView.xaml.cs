using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using test.Controller;
using test.Model;

namespace test.View
{
    /// <summary>
    /// UrlView.xaml 的交互逻辑
    /// </summary>
    public partial class UrlView : Window
    {
        public static string defaultUrl = "file://" + MainWindow.remoteFilePath + "RemoteConfig.xml";     // 初始化为默认路径
        public static string url;
        public static string urlConfigPath = System.AppDomain.CurrentDomain.BaseDirectory + "UrlConifg.config"; // 保存URL的配置文件

        // 当前软件使用的配置文件的路径 
        private static string currentConfigFilePath = System.AppDomain.CurrentDomain.BaseDirectory;

        // 虚拟远端的配置文件夹
        public static string remoteFilePath = currentConfigFilePath + "RemoteConfig\\";

        // 当前软件的配置文件
        public static string currentConfigFile = currentConfigFilePath + "CurrentConfig.xml";

        // 从远端获取下来的配置文件名均叫RemoteConfig.xml
        public static string currentRemoteConfig = currentConfigFilePath + "RemoteConfig.xml";

        private MainWindow parent;

        public string getUrl()
        {
            return url;
        }

        public UrlView(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;

            textBox.Text = url;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            url = textBox.Text;
            Controller.ConfigManager.CreateUrlConfig(url);

            WebClient client = new WebClient();
            try
            {
                client.DownloadFile(UrlView.url, currentRemoteConfig);            // 下载远端配置文件到当前目录
            }
            catch
            {
                MessageBox.Show("远端配置文件Url资源不存在或有误", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                ConfigManager.CreateXmlFile(new List<TargetInformation>(), currentRemoteConfig, true);
            }

            this.Close();
        }
    }
}
