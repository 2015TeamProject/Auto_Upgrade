using Auto_Upgrade.Controllers;
using Auto_Upgrade.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

namespace Auto_Upgrade.Views
{
    /// <summary>
    /// ConfigListView.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigListView : Page
    {
        // 当前软件使用的配置文件的路径 
        private static string currentConfigFilePath = System.AppDomain.CurrentDomain.BaseDirectory;

        // 当前软件的配置文件
        private static string currentConfigFile = currentConfigFilePath + "CurrentConfig.xml";

        // 从远端获取下来的配置文件名均叫RemoteConfig.xml
        private static string currentRemoteConfig = currentConfigFilePath + "RemoteConfig.xml";

        // 存放本地的配置文件的路径
        public static string configFilePath = currentConfigFilePath + "Config\\";

        // 虚拟远端的配置文件夹
        public static string remoteFilePath = currentConfigFilePath + "RemoteConfig\\";

        // 配置文件列表
        private List<Config> configList;

        // 父窗口
        private MainWindow parent;

        public ConfigListView(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;

            configList = new List<Config>();
            listView.ItemsSource = configList;      // 绑定数据源

            // 创建存在本地的配置文件的文件夹
            if (!Directory.Exists(configFilePath))
                Directory.CreateDirectory(configFilePath);

            if (!Directory.Exists(remoteFilePath))
                Directory.CreateDirectory(remoteFilePath);

            setCurrentRemoteConfig();               // 设置当前和远端配置文件项
            setLocalConfig();                       // 设置本地所有配置文件项， 顺序不能颠倒
        }

        public void setCurrentRemoteConfig()
        {
            setCurrentConfig();                     // 设置当前配置文件项, 顺序不能颠倒
            setRemoteConfig();                      // 获取远端配置文件项，顺序不能颠倒
        }

        private void setCurrentConfig()
        {
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

            if (configList.Count != 0) configList.RemoveAt(configList.Count - 1);
            configList.Add(new Config("本地配置文件", currentConfigFile, "Hidden", "Hidden"));
            listView.Items.Refresh();
        }

        private void setRemoteConfig()
        {
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

            if (configList.Count >= 2) configList.RemoveAt(configList.Count - 2);
            configList.Insert(configList.Count - 1, new Config("远端配置文件", currentRemoteConfig, "Hidden", "Visible", "更新"));
            listView.Items.Refresh();
        }

        private void setLocalConfig()
        {
            DirectoryInfo TheFolder = new DirectoryInfo(configFilePath);
            FileInfo[] fileConfigList;
            fileConfigList = TheFolder.GetFiles();
            foreach (FileInfo f in fileConfigList)
            {
                configList.Insert(configList.Count - 2, new Config(f.Name, f.FullName));
            }
        }
    
        public void addLocalConfig(string configFileName, string path)
        {
            foreach(Config config in configList)
            {
                if (config.Path == path)
                {
                    return;
                }
            }
            configList.Insert(configList.Count - 2, new Config(configFileName, path));
            listView.Items.Refresh();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            listView.SelectedItem = ((System.Windows.Controls.Button)sender).DataContext;
            string path = configList[listView.SelectedIndex].Path;
            FileInfo f = new FileInfo(path);

            MessageBoxResult dr = System.Windows.MessageBox.Show("是否删除配置文件" + f.Name, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (dr == MessageBoxResult.Cancel)
            {
                return;
            }
            else
            {
                configList.RemoveAt(listView.SelectedIndex);
                f.Delete();
                listView.Items.Refresh();
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            listView.SelectedItem = ((System.Windows.Controls.Button)sender).DataContext;
            string path = configList[listView.SelectedIndex].Path;
            string configName = configList[listView.SelectedIndex].ConfigName;
            if (listView.Items.Count - 2 == listView.SelectedIndex)
            {

                UpdateClass.Update updateObject = new UpdateClass.Update(currentConfigFilePath + MainWindow.appName, currentConfigFile, currentRemoteConfig, UrlView.url);
                if (updateObject.update()) parent.ShutDown();
            }
            else
            {
                string p = FileManager.SelectPath();
                FileManager.CopyToFloder(path, p);
                ConfigManager.XMLConfigPathChange(p + "/" + configName);
            }
        }

        private void ShowConfig_Click(object sender, RoutedEventArgs e)
        {
            listView.SelectedItem = ((System.Windows.Controls.Button)sender).DataContext;
            string path = configList[listView.SelectedIndex].Path;
            bool canModify = (
                    configList[listView.SelectedIndex].ConfigName == "本地配置文件"
                    || configList[listView.SelectedIndex].ConfigName == "远端配置文件") ? false : true;


            ConfigDetailsView configDetailsView = new ConfigDetailsView(parent);
            configDetailsView.showDetails(path, canModify);
            parent.addItem(configDetailsView, configList[listView.SelectedIndex].ConfigName);
        }

    }

}
