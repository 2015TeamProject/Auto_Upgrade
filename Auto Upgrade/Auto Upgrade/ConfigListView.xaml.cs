using System;
using System.Collections.Generic;
using System.IO;
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

using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml;
using System.Windows.Forms;
using System.Threading;

namespace Auto_Upgrade
{
    /// <summary>
    /// ConfigListView.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigListView : Page
    {
        // 本地配置文件目录
        private static String configFilePath = System.AppDomain.CurrentDomain.BaseDirectory + "Config\\";
        private static String currentConfigFilePath = System.AppDomain.CurrentDomain.BaseDirectory + "CurrentConfig\\";
        private bool isGoToUpdate;
        private string updateSoftwarePath;


        public string url = System.IO.Path.GetFullPath(@".\RemoteConfig");
        // 配置文件列表
        private List<Config> configList;
        private MainWindow parent;

        public static Dictionary<string, bool> fileExist = new Dictionary<string, bool>();       // 判断配置文件是否已存在，去重

        public void AddItem(string file, string fileFullName)
        {
            configList.Insert(configList.Count - 2, new Config(file, fileFullName));
            listView.Items.Refresh();
        }

        public ConfigListView(MainWindow parent)
        {
            InitializeComponent();

            this.parent = parent;

            configList = new List<Config>();
            listView.ItemsSource = configList;          // 绑定数据源
            isGoToUpdate = false;

            // 获取本地配置文件信息
            setLocalConfig();

            //当前配置文件信息
            setCurrentConfig();

            // 获取远程配置文件信息
            setRemoteConfig();
        }

        private void setRemoteConfig()
        {
            DirectoryInfo TheFolder = new DirectoryInfo(url);
            FileInfo[] fileConfigList;
            fileConfigList = TheFolder.GetFiles("*.xml");
            foreach (FileInfo f in fileConfigList)
            {
                f.CopyTo(currentConfigFilePath + f.Name, true);
                try
                {
                    configList.Insert(configList.Count - 1, new Config("远端配置文件", currentConfigFilePath + f.Name, "Hidden", "Visible", "更新"));
                }
                catch
                {

                }
            }
            listView.Items.Refresh();
        }

        private void setCurrentConfig()
        {
            DirectoryInfo TheFolder = new DirectoryInfo(currentConfigFilePath);
            FileInfo[] fileConfigList;
            fileConfigList = TheFolder.GetFiles("currentConfig.xml");
            foreach (FileInfo f in fileConfigList)
            {
                configList.Add(new Config("本地配置文件", f.FullName, "Hidden", "Hidden"));
            }
            listView.Items.Refresh();
        }

        private void setLocalConfig()
        {
            DirectoryInfo TheFolder = new DirectoryInfo(configFilePath);
            FileInfo[] fileConfigList;
            fileConfigList = TheFolder.GetFiles();
            foreach (FileInfo f in fileConfigList)
            {
                configList.Add(new Config(f.Name, f.FullName));
                fileExist.Add(f.FullName, true);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            listView.SelectedItem = ((System.Windows.Controls.Button)sender).DataContext;
            string path = configList[listView.SelectedIndex].path;
            FileInfo f = new FileInfo(path);

            MessageBoxResult dr = System.Windows.MessageBox.Show("是否删除配置文件" + f.Name, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (dr == MessageBoxResult.Cancel)
            {
                return;
            }
            else
            {
                configList.RemoveAt(listView.SelectedIndex);
                fileExist.Remove(f.FullName);
                f.Delete();
                listView.Items.Refresh();
            }
        }

        private string SelectPath()
        {
            FolderBrowserDialog m_Dialog = new FolderBrowserDialog();
            DialogResult result = m_Dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return "";
            }
            string m_Dir = m_Dialog.SelectedPath.Trim();
            return m_Dir;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            listView.SelectedItem = ((System.Windows.Controls.Button)sender).DataContext;
            string path = configList[listView.SelectedIndex].path;
            if (listView.Items.Count - 2 == listView.SelectedIndex)
            {
                // 更新软件
                List<ConfigInformation> remoteConfig = AnalysisXml(path);
                List<ConfigInformation> currentConfig = AnalysisXml(configList[listView.Items.Count - 1].path);

                List<AddFileInformation> addFileList = new List<AddFileInformation>();

                foreach (ConfigInformation file in remoteConfig)
                {
                    if (file.fileName == "Auto Upgrade.exe")
                    {
                        if (file.md5 != ConfigInformation.createMd5(System.AppDomain.CurrentDomain.BaseDirectory + "Auto Upgrade.exe"))
                        {
                            isGoToUpdate = true;
                            updateSoftwarePath = file.path;
                        }
                        continue;
                    }
                    if (file.updateMethod == "新增" || file.updateMethod == "替换")
                    {
                        FileInfo f = new FileInfo(file.path);
                        if (File.Exists(currentConfigFilePath + file.fileName))
                        {
                            foreach (ConfigInformation config in currentConfig)
                            {
                                if (config.fileName == file.fileName)
                                {
                                    config.md5 = file.md5;
                                    config.updateMethod = file.updateMethod;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            f.CopyTo(currentConfigFilePath + file.fileName, true);
                            addFileList.Add(new AddFileInformation(file.fileName, currentConfigFilePath + file.fileName));
                        }

                    }
                    else if (file.updateMethod == "删除")
                    {
                        FileInfo f = new FileInfo(file.path);
                        foreach (ConfigInformation cf in currentConfig)
                        {
                            if (file.md5 == cf.md5)
                            {
                                FileInfo ff = new FileInfo(cf.path);
                                ff.Delete();
                                currentConfig.Remove(cf);
                                break;
                            }
                        }
                    }
                }

                ThreadHelper arg = new Auto_Upgrade.ConfigListView.ThreadHelper { c = currentConfig, a = addFileList };
                Thread t = new Thread(new ParameterizedThreadStart(updateConfig));
                t.Start(arg);
                if (isGoToUpdate) parent.ShutDown();
            }
            else
            {
                string targetPath = SelectPath();
                if (targetPath == "") return;
                CopyToFloder(path, targetPath);
            }
        }

        class ThreadHelper
        {
            public List<ConfigInformation> c;
            public List<AddFileInformation> a;
        }
        
        private void updateConfig(Object arg)
        {
            List<AddFileInformation> addFileList = (arg as ThreadHelper).a;
            List<ConfigInformation> currentConfig = (arg as ThreadHelper).c;

            foreach (AddFileInformation add in addFileList)
            {
                while (!File.Exists(add.filePath)) ;
                currentConfig.Add(new ConfigInformation(add.fileName, add.filePath, "Visible", "True"));
            }

            CreateXmlFile(currentConfig);
            System.Windows.MessageBox.Show("更新成功", "提示", MessageBoxButton.OK);
            if (isGoToUpdate)
            {
                isGoToUpdate = false;
                System.Windows.MessageBox.Show("程序即将重启", "提示", MessageBoxButton.OK);

                updateSoftwarePath = updateSoftwarePath.Insert(0, "\"");
                updateSoftwarePath = updateSoftwarePath.Insert(updateSoftwarePath.Length, "\"");
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + "update.exe"; //启动的应用程序名称  
                startInfo.Arguments = updateSoftwarePath;
                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;//不使用系统外壳程序启动，重定向时此处必须设为false
                startInfo.RedirectStandardOutput = true; //重定向输出，而不是默认的显示在dos控制台上
                Process.Start(startInfo);
            }
        }



        private List<ConfigInformation> AnalysisXml(String path)
        {
            XmlDocument conXml = new XmlDocument();
            conXml.Load(path);

            XmlNode xm = conXml.SelectSingleNode("Files"); //得到根节点Files
            XmlNodeList xml = xm.ChildNodes;  //得到根节点的所有子节点

            List<ConfigInformation> config = new List<ConfigInformation>();

            foreach (XmlNode file in xml)
            {
                ConfigInformation configInformation;
                configInformation = new ConfigInformation();
                XmlNodeList fileList = file.ChildNodes;
                configInformation.path = fileList.Item(0).InnerText;
                configInformation.fileName = fileList.Item(1).InnerText;
                configInformation.updateMethod = fileList.Item(2).InnerText;
                configInformation.md5 = fileList.Item(3).InnerText;
                config.Add(configInformation);
            }
            return config;
        }

        private bool CreateXmlFile(List<ConfigInformation> dataList)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //创建类型声明节点
            XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
            xmlDoc.AppendChild(node);

            //创建根节点
            XmlNode root = xmlDoc.CreateElement("Files");
            xmlDoc.AppendChild(root);

            foreach (ConfigInformation data in dataList)
            {
                XmlNode node1 = xmlDoc.CreateNode(XmlNodeType.Element, "File", null);
                CreateNode(xmlDoc, node1, "Path", data.path);
                CreateNode(xmlDoc, node1, "Name", data.fileName);
                CreateNode(xmlDoc, node1, "Method", data.updateMethod);
                CreateNode(xmlDoc, node1, "MD5", data.md5);
                root.AppendChild(node1);
            }

            try
            {
                string path = currentConfigFilePath + "currentConfig.xml";
                xmlDoc.Save(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return true;
        }

        public void CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
        }

        private static void CopyToFloder(string path, string targetPath)
        {
            List<string> pathList = new List<string>();
            pathList.Add(path);

            XmlDocument conXml = new XmlDocument();
            conXml.Load(path);
            XmlNode xn = conXml.SelectSingleNode("Files"); //得到根节点Files
            XmlNodeList xnl = xn.ChildNodes;  //得到根节点的所有子节点
            foreach (XmlNode file in xnl)
            {
                XmlNodeList fileList = file.ChildNodes;
                pathList.Add(fileList.Item(0).InnerText);
            }

            foreach (string pp in pathList)
            {
                FileInfo f = new FileInfo(pp);
                try
                {
                    f.CopyTo(targetPath + "/" + f.Name, true);
                }
                catch
                {

                }
            }
        }

        private void ShowConfig_Click(object sender, RoutedEventArgs e)
        {
            listView.SelectedItem = ((System.Windows.Controls.Button)sender).DataContext;
            string path = configList[listView.SelectedIndex].path;
            bool canModify = (
                    configList[listView.SelectedIndex].configName == "本地配置文件"
                    || configList[listView.SelectedIndex].configName == "远端配置文件") ? false : true;
            parent.configInformationView.AnalysisXml(path, canModify);
            parent.frame.Content = parent.configInformationView;
        }

        private void SetUrl(object sender, RoutedEventArgs e)
        {
            SetUrl setUrl = new SetUrl(this);
            setUrl.ShowDialog();
        }

        private void NewConfig(object sender, RoutedEventArgs e)
        {
            parent.frame.Content = parent.configEditView;
        }


        class Config
        {
            public string configName;
            public string path;

            public string create;       // 按钮是生成版本还是更新
            public string visible;
            public string visible1;


            public string ConfigName
            {
                get
                {
                    return configName;
                }
                set
                {
                    configName = value;
                }
            }

            public string Visible
            {
                get
                {
                    return visible;
                }
                set
                {
                    visible = value;
                }
            }

            public string Visible1
            {
                get
                {
                    return visible1;
                }
                set
                {
                    visible1 = value;
                }
            }

            public string Create
            {
                get
                {
                    return create;
                }
                set
                {
                    create = value;
                }
            }

            public Config()
            {

            }

            public Config(string configName, string path, string visible= "Visible", string visible1 = "Visible", string create = "生成版本")
            {
                this.configName = configName;
                this.path = path;
                this.visible = visible;
                this.visible1 = visible1;
                this.create = create;
            }
        }
        
        class AddFileInformation
        {
            public string fileName;
            public string filePath;

            public AddFileInformation(string fileName, string filePath)
            {
                this.fileName = fileName;
                this.filePath = filePath;
            }
        }
    }

  
}

