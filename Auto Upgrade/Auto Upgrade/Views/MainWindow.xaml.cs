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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Auto_Upgrade.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public ConfigListView configListView;
        public ConfigDetailsView configDetailsView;
        public ConfigCreationView configCreationView;

        public static string appName = System.IO.Path.GetFileName(System.Windows.Forms.Application.ExecutablePath).Replace(".EXE", ".exe");

        public MainWindow()
        {
            InitializeComponent();
            UrlView.url = ConfigManager.ReadUrlConfig();        // 读取UrlConfig.config文件中的url
            configListView = new ConfigListView(this);
            configDetailsView = new ConfigDetailsView(this);
            configCreationView = new ConfigCreationView(this);

            TabItem item = new TabItem();
            item.Header = "配置文件列表";
            Frame tabFrame = new Frame();
            tabFrame.Content = configListView;
            item.Content = tabFrame;

            tabConrol.Items.Add(item);
        }

        public void ShutDown()
        {
            ShutDownController.shutDown();
        }


        public void addItem(Page page, string header)
        {
            TabItem item = new TabItem();
            item.Header = header;
            Frame frame = new Frame();
            frame.Content = page;
            item.Content = frame;
            tabConrol.Items.Add(item);
            tabConrol.SelectedIndex = tabConrol.Items.Count - 1;
        }

        public void returnConfigListView()
        {
            tabConrol.SelectedIndex = 0;
        }

        private void SetUrl_Click(object sender, RoutedEventArgs e)
        {
            UrlView url = new UrlView(this);
            url.ShowDialog();
        }

        private void CreateConfig_Click(object sender, RoutedEventArgs e)
        {
            this.addItem(new ConfigCreationView(this), "untitled");
        }
    }
}
