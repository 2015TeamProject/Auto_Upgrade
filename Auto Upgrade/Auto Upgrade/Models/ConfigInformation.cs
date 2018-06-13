using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Upgrade
{
    public class ConfigInformation
    {
        private string fileName;
        private string path;
        private string updateMethod;
        private string md5;
        private string deleteButtonVisible;
        private string updateMethodEnable;

        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
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

        public string UpdateMethod
        {
            get
            {
                return updateMethod;
            }
            set
            {
                updateMethod = value;
            }
        }

        public string Md5
        {
            get
            {
                return md5;
            }
            set
            {
                md5 = value;
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

        public string UpdateMethodEnable
        {
            get
            {
                return updateMethodEnable;
            }
            set
            {
                updateMethodEnable = value;
            }
        }

        public ConfigInformation(string deleteButtonVisible = "Visible", string updateMethodEnable = "True")
        {
            this.deleteButtonVisible = deleteButtonVisible;
            this.updateMethodEnable = updateMethodEnable;
        }

        public ConfigInformation(string fileName, string path, string deleteButtonVisible = "Visible", string updateMethodEnable = "True", string updateMethod="新增")
        {
            this.fileName = fileName;
            this.path = path;
            this.md5 = Md5Creator.createMd5(this.path);
            this.deleteButtonVisible = deleteButtonVisible;
            this.updateMethodEnable = updateMethodEnable;
            this.updateMethod = updateMethod;
        }
    }
}
