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

        private bool isNeedToClose;
        private string updateFileName;

        // 配置文件列表
        private List<Config> configList;

        // 父窗口
        private MainWindow parent;

        public ConfigListView(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;

            isNeedToClose = false;

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
                // 更新软件
                List<TargetInformation> remoteConfig = new List<TargetInformation>();
                List<TargetInformation> currentConfig = new List<TargetInformation>();

                ConfigManager.AnalysisXml(currentRemoteConfig, remoteConfig, false);
                ConfigManager.AnalysisXml(currentConfigFile, currentConfig, false);

                WebClient client = new WebClient();
                string url = System.IO.Path.GetDirectoryName(UrlView.url);
                MessageBox.Show(url);
                foreach (TargetInformation file in remoteConfig)
                {
                    if (file.FileName == MainWindow.appName)
                    {
                        if (file.Md5 != Md5Creator.createMd5(currentConfigFilePath + MainWindow.appName)
                               && (file.UpdateMethod == "替换" || file.UpdateMethod == "新增"))
                        {
                            isNeedToClose = true;
                            updateFileName = file.FileName;
                        }
                        continue;
                    }


                    if (file.UpdateMethod == "新增" || file.UpdateMethod == "替换")
                    {
                        if (File.Exists(currentConfigFilePath + file.FileName))
                        {
                            foreach (TargetInformation config in currentConfig)
                            {
                                if (config.FileName == file.FileName)
                                {
                                    config.Md5 = file.Md5;
                                    config.UpdateMethod = file.UpdateMethod;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            currentConfig.Add(file);
                        }
                        client.DownloadFile(url + "/" + file.FileName, currentConfigFilePath + file.FileName);
                    }
                    else if (file.UpdateMethod == "删除")
                    {
                        foreach (TargetInformation cf in currentConfig)
                        {
                            if (file.Md5 == cf.Md5 && file.FileName == cf.FileName)
                            {
                                FileInfo ff = new FileInfo(cf.Path);
                                currentConfig.Remove(cf);
                                ff.Delete();
                                break;
                            }
                        }
                    }
                    else if (file.UpdateMethod == "覆盖后重启")
                    {
                        if (file.Md5 != Md5Creator.createMd5(currentConfigFilePath + MainWindow.appName))
                        {
                            isNeedToClose = true;
                            updateFileName = file.FileName;
                        }
                    }
                }
                
                ThreadHelper arg = new ThreadHelper { c = currentConfig };
                Thread t = new Thread(new ParameterizedThreadStart(updateConfig));
                t.Start(arg);
                if (isNeedToClose) parent.ShutDown();
            }
            else
            {
                string p = FileManager.SelectPath();
                FileManager.CopyToFloder(path, p);
                ConfigManager.XMLConfigPathChange(p + "/" + configName);
            }
        }

        class ThreadHelper
        {
            public List<TargetInformation> c;
        }

        private void updateConfig(Object arg)
        {
            List<TargetInformation> currentConfig = (arg as ThreadHelper).c;

            ConfigManager.CreateXmlFile(currentConfig, currentConfigFile, true);
            MessageBox.Show("更新成功", "提示", MessageBoxButton.OK);

            if (isNeedToClose)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = currentConfigFilePath + "update.exe"; //启动的应用程序名称  

                MessageBox.Show("程序即将重启", "提示", MessageBoxButton.OK);
                startInfo.Arguments = "\"" + System.IO.Path.GetDirectoryName(UrlView.url) + "\\" + updateFileName + "\"" + " " + "\"" + currentConfigFilePath + MainWindow.appName + "\"";

                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;      //不使用系统外壳程序启动，重定向时此处必须设为false
                startInfo.RedirectStandardOutput = true; //重定向输出，而不是默认的显示在dos控制台上
                Process.Start(startInfo);
            }
        }

        private void ShowConfig_Click(object sender, RoutedEventArgs e)
        {
            listView.SelectedItem = ((System.Windows.Controls.Button)sender).DataContext;
            string path = configList[listView.SelectedIndex].Path;
            bool canModify = (
                    configList[listView.SelectedIndex].ConfigName == "本地配置文件"
                    || configList[listView.SelectedIndex].ConfigName == "远端配置文件") ? false : true;

            parent.configDetailsView.ClearToReady();
            parent.configDetailsView.showDetails(path, canModify);
            parent.frame.Content = parent.configDetailsView;
        }

        private void CreateConfig_Click(object sender, RoutedEventArgs e)
        {
            parent.configCreationView.ClearToReady();
            parent.frame.Content = parent.configCreationView;
        }

        private void SetUrl_Click(object sender, RoutedEventArgs e)
        {
        UrlView url = new UrlView(parent);
            url.ShowDialog();
        }
    }

}
