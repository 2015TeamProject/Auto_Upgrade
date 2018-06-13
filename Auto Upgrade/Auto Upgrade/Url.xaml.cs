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

namespace Auto_Upgrade
{
    /// <summary>
    /// Url.xaml 的交互逻辑
    /// </summary>
    public partial class Url : Window
    {
        public static string defaultUrl = "file://" + ConfigListView.remoteFilePath + "RemoteConfig.xml";     // 初始化为默认路径
        public static string url;    
        public static string urlConfigPath = System.AppDomain.CurrentDomain.BaseDirectory + "UrlConifg.config"; // 保存URL的配置文件

        private MainWindow parent;

        public string getUrl()
        {
            return url;
        }

        public Url(MainWindow parent)
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
            ConfigManager.CreateUrlConfig(url);
            parent.configListView.setCurrentRemoteConfig();    // 为了更新当前配置文件中UrlConfig.config的md5值，以及刷新远端配置文件
            this.Close();
        }
    }
}
