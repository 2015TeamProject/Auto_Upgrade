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
using System.Xml;

namespace Auto_Upgrade
{
    /// <summary>
    /// SaveAs.xaml 的交互逻辑
    /// </summary>
    public partial class SaveAs : Window
    {
        private static string configFilePath = System.AppDomain.CurrentDomain.BaseDirectory + "Config\\";
        private MainWindow parent;
        private List<ConfigInformation> configInformationList;
        public SaveAs(MainWindow parent, List<ConfigInformation> configInformationList)
        {
            InitializeComponent();
            this.parent = parent;
            this.configInformationList = configInformationList;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (this.textBox.Text == "")
            {
                MessageBox.Show("文件名不能为空", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (this.textBox.Text == "本地配置文件")
            {
                MessageBox.Show("不能命名为'本地配置文件'", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (this.textBox.Text == "远端配置文件")
            {
                MessageBox.Show("不能命名为'远端配置文件'", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!CreateXmlFile()) return;

            parent.frame.Content = parent.configListView;
            this.Close();
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
                string path = configFilePath + this.textBox.Text + ".xml";
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
                    parent.configListView.AddItem(this.textBox.Text + ".xml", path);
                }
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

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
