using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using System.Xml;

namespace Auto_Upgrade
{
    /// <summary>
    /// ConfigEditView.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigEditView : Page
    {
        private List<ConfigData> dataList;
        private Dictionary<string, bool> exist;
        private MainWindow parent;

        // 本地配置文件目录
        private static string configFilePath = System.AppDomain.CurrentDomain.BaseDirectory + "Config\\";

        private void ClearDataList()
        {
            dataList.Clear();
            exist.Clear();
            ConfigFileName.Clear();
        }
        
        public ConfigEditView(MainWindow parent)
        {
            InitializeComponent();
            this.parent = parent;

            dataList = new List<ConfigData>();
            listView.ItemsSource = dataList;

            exist = new Dictionary<string, bool>();
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

        private void add_Click(object sender, RoutedEventArgs e)
        {
            string path = SelectPath();
            if (path == "" || exist.ContainsKey(path)) return;
            exist.Add(path, true);

            string fileName = System.IO.Path.GetFileName(path);
            ConfigData c = new ConfigData(fileName, path);
            if (fileName == "Auto Upgrade.exe")
            {
                c.setUpdateMethod("替换");
                c.setEnable("False");
            }
            dataList.Add(c);
            listView.Items.Refresh();
            this.AutoSetWidth();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.ClearDataList();
            parent.frame.Content = parent.configListView;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (this.ConfigFileName.Text == "")
            {
                MessageBox.Show("文件名不能为空", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (this.ConfigFileName.Text == "本地配置文件")
            {
                MessageBox.Show("不能命名为'本地配置文件'", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (this.ConfigFileName.Text == "远端配置文件")
            {
                MessageBox.Show("不能命名为'远端配置文件'", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!CreateXmlFile()) return;

            this.ClearDataList();
            parent.frame.Content = parent.configListView;
        }

         public bool CreateXmlFile(){
             XmlDocument xmlDoc = new XmlDocument();
             //创建类型声明节点
             XmlNode node = xmlDoc.CreateXmlDeclaration("1.0","utf-8","");
             xmlDoc.AppendChild(node);

             //创建根节点
             XmlNode root = xmlDoc.CreateElement("Files");
             xmlDoc.AppendChild(root);

            foreach (ConfigData data in dataList)
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
                string path = configFilePath + this.ConfigFileName.Text + ".xml";
                bool isPathExist = ConfigListView.fileExist.ContainsKey(path);

                if (isPathExist)
                {
                    MessageBoxResult dr = MessageBox.Show("文件已存在, 是否要覆盖保存", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (dr == MessageBoxResult.Cancel)
                    {
                        return false;
                    }
                }
                xmlDoc.Save(path);
                if (!isPathExist)
                {
                    ConfigListView.fileExist.Add(path, true);
                    parent.configListView.AddItem(this.ConfigFileName.Text + ".xml", path);
                }
             }
             catch(Exception e)
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
            string path = dataList[listView.SelectedIndex].path;
            FileInfo f = new FileInfo(path);
            exist.Remove(f.FullName);

            dataList.RemoveAt(listView.SelectedIndex);
            listView.Items.Refresh();
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

    }

    class ConfigData
    {
        public string fileName;
        public string path;
        public string updateMethod;
        public string md5;
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
        public ConfigData(string fileName, string path, string enable="True")
        {
            this.fileName = fileName;
            this.path = path;
            this.md5 = createMd5();
            this.updateMethod = "新增";
            this.enable = "True";
        }
        
        public void setEnable(string enable)
        {
            this.enable = enable;
        }

        public void setUpdateMethod(string method)
        {
            this.updateMethod = method;
        }

        private string createMd5()
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
    public class methodList : List<string>
    {
        public methodList()
        {
            this.Add("新增");
            this.Add("删除");
            this.Add("替换");
        }
    }
}

