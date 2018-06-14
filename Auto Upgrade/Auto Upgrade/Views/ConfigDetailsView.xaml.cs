using Auto_Upgrade.Controllers;
using Auto_Upgrade.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Auto_Upgrade.Views
{
    /// <summary>
    /// ConfigDetailsView.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigDetailsView : Page, AutoSetWidth
    {
        // 父窗口
        private MainWindow parent;

        // 配置文件详细信息
        private List<TargetInformation> configInformationList;
        private Dictionary<string, bool> exist;
        private string configFileName;
        private string path;

        public ConfigDetailsView(MainWindow parent)
        {
            InitializeComponent();

            this.parent = parent;

            configInformationList = new List<TargetInformation>();
            listView.ItemsSource = configInformationList;

            exist = new Dictionary<string, bool>();
        }

        public void ClearToReady()
        {
            configInformationList.Clear();
            exist.Clear();
            configFileName = "";
            path = "";
        }

        public void showDetails(string path, bool canModify)
        {
            this.path = path;
            this.configFileName = System.IO.Path.GetFileName(path);
            this.configLabel.Content = "配置文件: " + this.configFileName;

            ConfigManager.AnalysisXml(path, configInformationList, canModify);

            if (!canModify)
            {
                this.addButton.Visibility = System.Windows.Visibility.Hidden;
                this.saveAsButton.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                this.addButton.Visibility = System.Windows.Visibility.Visible;
                this.saveAsButton.Visibility = System.Windows.Visibility.Visible;
            }

            listView.Items.Refresh();
            this.autoSetWidth();

            foreach (TargetInformation configInformation in configInformationList)
            {
                exist.Add(configInformation.Path, true);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            listView.SelectedItem = ((Button)sender).DataContext;
            string path = configInformationList[listView.SelectedIndex].Path;
            exist.Remove(path);

            configInformationList.RemoveAt(listView.SelectedIndex);
            listView.Items.Refresh();
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            SaveAsView saveAsView = new SaveAsView(parent, configInformationList);
            saveAsView.ShowDialog();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string path = FileManager.SelectFile();
            if (path == "" || exist.ContainsKey(path)) return;
            exist.Add(path, true);

            string fileName = System.IO.Path.GetFileName(path);
            TargetInformation c = new TargetInformation(fileName, path, "Visible", "True");
           
            configInformationList.Add(c);

            listView.Items.Refresh();
            this.autoSetWidth();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.ClearToReady();
            parent.frame.Content = parent.configListView;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigManager.CreateXmlFile(configInformationList, this.path, true);
            this.ClearToReady();
            parent.frame.Content = parent.configListView;
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
