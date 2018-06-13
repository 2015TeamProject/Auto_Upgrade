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
    /// RemoteVersionPath.xaml 的交互逻辑
    /// </summary>
    public partial class RemoteVersionPath : Window
    {
        private string createVersionPath = "";

        private string remoteUrl = "";

        private bool done = false;

        public string getRemoteUrl()
        {
            return remoteUrl;   
        }

        public string getPath()
        {
            return createVersionPath;
        }

        public bool getDone()
        {
            return done;
        }

        public RemoteVersionPath()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (this.url.Text == "")
            {
                MessageBox.Show("请输入URL", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string targetPath = FileManager.SelectPath();
            if (targetPath == "") return;
            createVersionPath = targetPath;
            remoteUrl = this.url.Text;
            done = true;
            this.Close();
        }
    }
}
