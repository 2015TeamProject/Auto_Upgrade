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
    /// SetUrl.xaml 的交互逻辑
    /// </summary>
    public partial class SetUrl : Window
    {
        static public string url = System.IO.Path.GetFullPath(@".\RemoteConfig");

        ConfigListView parent;

        public SetUrl(ConfigListView parent)
        {
            InitializeComponent();
            this.textBox.Text = url;
            this.parent = parent;
            this.parent.url = url;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            url = this.textBox.Text;
            this.Close();
            parent.url = url;
        }

        
    }
}
