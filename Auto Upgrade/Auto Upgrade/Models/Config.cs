using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Upgrade.Models
{
    // 配置文件项的 信息
    class Config
    {
        private string configName;               // 配置文件名
        private string path;                     // 配置文件的路径
        private string createButtonText;         // 按钮显示的文字是 生成版本还是更新
        private string deleteButtonVisible;      // 删除按钮的可见性
        private string createButtonVisible;      // 生成版本/更新按钮的可见性

        public string ConfigName
        {
            get
            {
                return configName;
            }
            set
            {
                configName = value;
            }
        }

        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
            }
        }

       public string CreateButtonText
       {
            get
            {
                return createButtonText;
            }
            set
            {
                createButtonText = value;
            }
        }

        public string DeleteButtonVisible
        {
            get
            {
                return deleteButtonVisible;
            }
            set
            {
                deleteButtonVisible = value;
            }
        }

        public string CreateButtonVisible
        {
            get
            {
                return createButtonVisible;
            }
            set
            {
                createButtonVisible = value;
            }
        }

        public Config(string configName, string path, string deleteButtonVisible = "Visible", string createButtonVisible = "Visible", string createButtonText = "生成版本")
        {
            this.configName = configName;
            this.path = path;
            this.deleteButtonVisible = deleteButtonVisible;
            this.createButtonVisible = createButtonVisible;
            this.createButtonText = createButtonText;
        }
    }
}
