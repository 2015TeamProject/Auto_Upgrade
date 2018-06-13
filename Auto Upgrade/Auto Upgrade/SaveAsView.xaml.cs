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

namespace Auto_Upgrade
{
    /// <summary>
    /// SaveAsView.xaml 的交互逻辑
    /// </summary>
    public partial class SaveAsView : Window
    {
        private MainWindow parent;
        private List<ConfigInformation> configInformationList;

        public SaveAsView(MainWindow parent, List<ConfigInformation> configInformationList)
        {
            InitializeComponent();
            this.parent = parent;
            this.configInformationList = configInformationList;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
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
            parent.frame.Content = parent.configListView;
            this.Close();
        }
    }
}
