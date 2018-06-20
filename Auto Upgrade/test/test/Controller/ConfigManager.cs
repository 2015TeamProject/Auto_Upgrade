using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using test.Model;
using test.View;

namespace test.Controller
{
    class ConfigManager
    {

        public static bool CreateXmlFile(List<TargetInformation> configInformationList, string path,
            bool isDirectCoverage = false)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //创建类型声明节点
            XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
            xmlDoc.AppendChild(node);

            //创建根节点
            XmlNode root = xmlDoc.CreateElement("Files");
            xmlDoc.AppendChild(root);

            foreach (TargetInformation data in configInformationList)
            {
                XmlNode node1 = xmlDoc.CreateNode(XmlNodeType.Element, "File", null);
                CreateNode(xmlDoc, node1, "Path", data.Path);
                CreateNode(xmlDoc, node1, "Name", data.FileName);
                CreateNode(xmlDoc, node1, "Method", data.UpdateMethod);
                CreateNode(xmlDoc, node1, "MD5", data.Md5);
                root.AppendChild(node1);
            }

            try
            {
                if (!isDirectCoverage && File.Exists(path))
                {
                    MessageBoxResult dr = MessageBox.Show("文件已存在, 是否要覆盖保存", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (dr == MessageBoxResult.Cancel)
                    {
                        return false;
                    }
                }
                xmlDoc.Save(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return true;
        }

        private static void CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
        }

        public static void AnalysisXml(string path, List<TargetInformation> configList, bool isLocalConfig = true)
        {
            XmlDocument conXml = new XmlDocument();
            conXml.Load(path);

            XmlNode xm = conXml.SelectSingleNode("Files");  //得到根节点Files
            XmlNodeList xml = xm.ChildNodes;                //得到根节点的所有子节点

            string deleteButtonVisible;         // 如果是远端配置文件或者当前使用的配置文件， delete按钮是不可见的
            string updateMethodEnable;
            if (isLocalConfig)
            {
                deleteButtonVisible = "Visible";
                updateMethodEnable = "True";
            }
            else
            {
                deleteButtonVisible = "Hidden";
                updateMethodEnable = "False";
            }

            configList.Clear();
            foreach (XmlNode file in xml)
            {
                XmlNodeList fileList = file.ChildNodes;
                TargetInformation configInformation;
                configInformation = new TargetInformation();
                configInformation.Path = fileList.Item(0).InnerText;
                configInformation.FileName = fileList.Item(1).InnerText;
                configInformation.UpdateMethod = fileList.Item(2).InnerText;
                configInformation.Md5 = fileList.Item(3).InnerText;
                configInformation.DeleteButtonVisible = deleteButtonVisible;
                configInformation.UpdateMethodEnable = updateMethodEnable;
                configList.Add(configInformation);
            }

        }

        public static void CreateUrlConfig(string path = "")
        {
            byte[] data;
            FileStream fs = new FileStream(UrlView.urlConfigPath, FileMode.Create);
            if (path != "")
            {
                //获得字节数组
                data = System.Text.Encoding.Default.GetBytes(path);
            }
            else
            {
                //获得字节数组
                data = System.Text.Encoding.Default.GetBytes(UrlView.defaultUrl);
            }
            //开始写入
            fs.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
        }

        public static string ReadUrlConfig()
        {
            if (!File.Exists(UrlView.urlConfigPath))
            {
                CreateUrlConfig();
            }
            StreamReader sr = new StreamReader(UrlView.urlConfigPath, Encoding.Default);
            string line = sr.ReadLine();
            sr.Close();
            return line;
        }

        public static void XMLConfigPathChange(string xmlPath)
        {
            List<TargetInformation> config = new List<TargetInformation>();

            AnalysisXml(xmlPath, config);

            foreach (TargetInformation c in config)
            {
                c.Path = xmlPath + "/" + c.FileName;
            }

            CreateXmlFile(config, xmlPath, true);
        }
    }
}

