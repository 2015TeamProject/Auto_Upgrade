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

namespace Auto_Upgrade
{
    /// <summary>
    /// ConfigCreationView.xaml 的交互逻辑
    /// </summary>
    /// 

    public partial class ConfigCreationView : Page, AutoSetWidth
    {
        // 父窗口
        private MainWindow parent;
        private List<ConfigInformation> configInformationList;
        private Dictionary<string, bool> exist;

        public ConfigCreationView(MainWindow parent)
        {
            InitializeComponent();

            this.parent = parent;

            configInformationList = new List<ConfigInformation>();
            listView.ItemsSource = configInformationList;

            exist = new Dictionary<string, bool>();

        }

        public void ClearToReady()
        {
            exist.Clear();
            configInformationList.Clear();
            configFileName.Clear();         // 文本框文字清除
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.ClearToReady();
            parent.frame.Content = parent.configListView;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string path = FileManager.SelectFile();
            if (path == "" || exist.ContainsKey(path)) return;
            exist.Add(path, true);

            string fileName = System.IO.Path.GetFileName(path);
            ConfigInformation c = new ConfigInformation(fileName, path, "Visible", "True");
            configInformationList.Add(c);
            listView.Items.Refresh();
            this.autoSetWidth();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.configFileName.Text == "")
            {
                MessageBox.Show("文件名不能为空", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (this.configFileName.Text == "本地配置文件")
            {
                MessageBox.Show("不能命名为'本地配置文件'", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (this.configFileName.Text == "远端配置文件")
            {
                MessageBox.Show("不能命名为'远端配置文件'", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string fileName = ConfigListView.configFilePath + this.configFileName.Text + ".xml";
            if (!ConfigManager.CreateXmlFile(configInformationList, fileName))
                return;

            parent.configListView.addLocalConfig(this.configFileName.Text + ".xml", fileName);
            this.ClearToReady();
            parent.frame.Content = parent.configListView;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            listView.SelectedItem = ((Button)sender).DataContext;
            string path = configInformationList[listView.SelectedIndex].Path;
            exist.Remove(path);

            configInformationList.RemoveAt(listView.SelectedIndex);
            listView.Items.Refresh();
        }

        public void autoSetWidth()
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
}
