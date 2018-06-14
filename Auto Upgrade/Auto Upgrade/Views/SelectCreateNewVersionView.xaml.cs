using Auto_Upgrade.Controllers;
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

namespace Auto_Upgrade.Views
{
    /// <summary>
    /// SelectCreateNewVersionView.xaml 的交互逻辑
    /// </summary>
    public partial class SelectCreateNewVersionView : Window
    {

        private string createVersionPath  = "";
        private string configPath = "";

        public string getPath()
        {
            return createVersionPath;
        }

        public string getConfigPath()
        {
            return configPath;
        }

        public SelectCreateNewVersionView()
        {
            InitializeComponent();
        }

        private void CreateLocal_Click(object sender, RoutedEventArgs e)
        {
            string targetPath = FileManager.SelectPath();
            if (targetPath == "") return;
            createVersionPath = targetPath;
            configPath = targetPath;
            this.Close();
        }

        private void CreateRemote_Click(object sender, RoutedEventArgs e)
        {
            RemoteVersionPath remoteVersionPath = new RemoteVersionPath();
            remoteVersionPath.ShowDialog();
            createVersionPath = remoteVersionPath.getPath();
            configPath = remoteVersionPath.getRemoteUrl();
            if (remoteVersionPath.getDone()) this.Close();
        }
    }
}
