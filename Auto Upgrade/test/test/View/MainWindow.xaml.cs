using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using test.Controller;
using test.Model;

namespace test.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 当前软件使用的配置文件的路径 
        private static string currentConfigFilePath = System.AppDomain.CurrentDomain.BaseDirectory;

        // 虚拟远端的配置文件夹
        public static string remoteFilePath = currentConfigFilePath + "RemoteConfig\\";

        // 当前软件的配置文件
        public static string currentConfigFile = currentConfigFilePath + "CurrentConfig.xml";

        // 从远端获取下来的配置文件名均叫RemoteConfig.xml
        public static string currentRemoteConfig = currentConfigFilePath + "RemoteConfig.xml";

        public MainWindow()
        {
            InitializeComponent();

            if (!Directory.Exists(remoteFilePath))
                Directory.CreateDirectory(remoteFilePath);

            UrlView.url = ConfigManager.ReadUrlConfig();        // 读取UrlConfig.config文件中的url

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

            DirectoryInfo TheFolder = new DirectoryInfo(currentConfigFilePath);
            FileInfo[] fileConfigList;
            fileConfigList = TheFolder.GetFiles();          // 获取根目录下所有文件

            List<TargetInformation> configInformationList = new List<TargetInformation>();
            foreach (FileInfo f in fileConfigList)
            {
                if (f.Name.Substring(f.Name.Length - 4) == ".xml") continue;    // 跳过.xml的配置文件
                configInformationList.Add(new TargetInformation(f.Name, f.FullName, "Hidden", "False"));
            }

            ConfigManager.CreateXmlFile(configInformationList, currentConfigFile, true);   // 创建当前软件运行的配置文件
        }

        public void ShutDown()
        {
            ShutDownController.shutDown();
        }

        private void SetUrl(object sender, RoutedEventArgs e)
        {
            UrlView url = new UrlView(this);
            url.ShowDialog();
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            string appName = System.Windows.Forms.Application.ExecutablePath;
            UpdateClass.Update updateObject = new UpdateClass.Update(appName, currentConfigFile, currentRemoteConfig, UrlView.url);
            if (updateObject.update())
                this.ShutDown();
        }

    }
}
