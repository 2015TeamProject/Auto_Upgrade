using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Auto_Upgrade.Controllers
{
    class FileManager
    {
        public static string SelectFile()
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

        public static string SelectPath()
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

        public static void CopyToFloder(string path, string targetPath)
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
    }
}
