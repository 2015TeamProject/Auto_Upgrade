using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
    /// ConfigInformationView.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigInformationView : Page
    {
        private List<ConfigInformation> configInformationList;
        private Dictionary<string, bool> exist;
        private MainWindow parent;

        private static string configFilePath = System.AppDomain.CurrentDomain.BaseDirectory + "Config\\";

        private string configFileName;

        public ConfigInformationView(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;

            configInformationList = new List<ConfigInformation>();
            exist = new Dictionary<string, bool>();
            listView.ItemsSource = configInformationList;            
        }

        private void ClearList()
        {
            configInformationList.Clear();
            exist.Clear();
            this.AddButton.IsEnabled = true;
        }

        //解析xml文件
        public void AnalysisXml(String path, bool isEnableToModify=true)
        {
            this.ClearList();
            this.configFileName = System.IO.Path.GetFileName(path);
            this.configLabel.Content = "配置文件: " + this.configFileName;

            XmlDocument conXml = new XmlDocument();
            conXml.Load(path);

            XmlNode xn = conXml.SelectSingleNode("Files"); //得到根节点Files
            XmlNodeList xnl = xn.ChildNodes;  //得到根节点的所有子节点

            if (!isEnableToModify)
            {
                this.AddButton.Visibility = System.Windows.Visibility.Hidden;
                this.SaveAsButton.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                this.AddButton.Visibility = System.Windows.Visibility.Visible;
                this.SaveAsButton.Visibility = System.Windows.Visibility.Visible;
            }
            

            foreach (XmlNode file in xnl)
            {
                ConfigInformation configInformation;
                if (isEnableToModify) configInformation = new ConfigInformation();
                else
                {
                    configInformation = new ConfigInformation("Hidden", "False");
                }

                XmlNodeList fileList = file.ChildNodes;

                configInformation.path = fileList.Item(0).InnerText;
                exist.Add(configInformation.path, true);

                configInformation.fileName = fileList.Item(1).InnerText;
                configInformation.updateMethod = fileList.Item(2).InnerText;
                configInformation.md5 = fileList.Item(3).InnerText;

                if (configInformation.fileName == "Auto Upgrade.exe")
                {
                    configInformation.setEnable("False");
                }
                configInformationList.Add(configInformation);
            }

            listView.Items.Refresh();
            this.AutoSetWidth();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            this.ClearList();
            parent.frame.Content = parent.configListView;
        }

        private void confirm_Click(object sender, RoutedEventArgs e)
        {
            this.CreateXmlFile();
            this.ClearList();
            parent.frame.Content = parent.configListView;
        }

        public bool CreateXmlFile()
        {
            XmlDocument xmlDoc = new XmlDocument();
            //创建类型声明节点
            XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
            xmlDoc.AppendChild(node);

            //创建根节点
            XmlNode root = xmlDoc.CreateElement("Files");
            xmlDoc.AppendChild(root);

            foreach (ConfigInformation data in configInformationList)
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
                string path = configFilePath + this.configFileName;
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

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            listView.SelectedItem = ((Button)sender).DataContext;
            string path = configInformationList[listView.SelectedIndex].path;
            FileInfo f = new FileInfo(path);
            exist.Remove(f.FullName);

            configInformationList.RemoveAt(listView.SelectedIndex);
            listView.Items.Refresh();
        }

        private string SelectPath()
        {
            string path = string.Empty;
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Files (*.*)|*.*"
            };
            var result = openFileDialog.ShowDialog();

            if (result == true)
            {
                path = openFileDialog.FileName;
                return path;
            }
            else
            {
                return "";
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string path = SelectPath();
            if (path == "" || exist.ContainsKey(path)) return;
            exist.Add(path, true);

            string fileName = System.IO.Path.GetFileName(path);
            ConfigInformation c = new ConfigInformation(fileName, path, "Visible", "True");
            if (fileName == "Auto Upgrade.exe")
            {
                c.setUpdateMethod("替换");
                c.setEnable("False");
            }
            configInformationList.Add(c);

            listView.Items.Refresh();
            this.AutoSetWidth();
        }

        private void AutoSetWidth()
        {
            GridView gv = listView.View as GridView;
            if (gv != null)
            {
                foreach (GridViewColumn gvc in gv.Columns)
                {
                    gvc.Width = gvc.ActualWidth;
                    gvc.Width = Double.NaN;
                }
            }
        }

        private void SaveAs(object sender, RoutedEventArgs e)
        {
            SaveAs saveAs = new SaveAs(parent, configInformationList);
            saveAs.ShowDialog();
        }
    }

    public class ConfigInformation
    {
        public string fileName;
        public string path;
        public string updateMethod;
        public string md5;

        public string visible;
        public string enable;

        public string Enable
        {
            get
            {
                return enable;
            }
            set
            {
                enable = value;
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

        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
            }
        }

        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
            }
        }

        public string UpdateMethod
        {
            get
            {
                return updateMethod;
            }
            set
            {
                updateMethod = value;
            }
        }

        public string Md5
        {
            get
            {
                return md5;
            }
            set
            {
                md5 = value;
            }
        }

        public ConfigInformation(string visible="Visible", string enable="True")
        {
            this.visible = visible;
            this.enable = enable;
        }

        public ConfigInformation(string fileName, string path, string visible="Visible", string enable="True")
        {
            this.fileName = fileName;
            this.path = path;
            this.md5 = createMd5(this.path);
            this.visible = visible;
            this.enable = enable;
            this.updateMethod = "新增";
        }

        public void setUpdateMethod(string method)
        {
            this.updateMethod = method;
        }

        public void setEnable(string enable)
        {
            this.enable = enable;
        }

        public static string createMd5(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException(string.Format("<{0}>, 不存在", path));
            int bufferSize = 1024 * 16;//自定义缓冲区大小16K  
            byte[] buffer = new byte[bufferSize];
            Stream inputStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            HashAlgorithm hashAlgorithm = new MD5CryptoServiceProvider();
            int readLength = 0;//每次读取长度  
            var output = new byte[bufferSize];
            while ((readLength = inputStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                //计算MD5  
                hashAlgorithm.TransformBlock(buffer, 0, readLength, output, 0);
            }
            //完成最后计算，必须调用(由于上一部循环已经完成所有运算，所以调用此方法时后面的两个参数都为0)  
            hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
            string md5 = BitConverter.ToString(hashAlgorithm.Hash);
            hashAlgorithm.Clear();
            inputStream.Close();
            md5 = md5.Replace("-", "");
            return md5;
        }
    }
}



