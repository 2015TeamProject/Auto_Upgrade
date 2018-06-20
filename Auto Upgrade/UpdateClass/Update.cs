using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace UpdateClass
{
    public class Update
    {
        private bool isNeedToClose = false;
        private string updateFileName = "";
        private string appName;
        private string currentConfigFile;
        private string currentRemoteConfig;
        private string url;

        public Update(string appName, string currentConfigFile, string currentRemoteConfig, string url)
        {
            this.appName = appName;
            this.currentConfigFile = currentConfigFile;
            this.currentRemoteConfig = currentRemoteConfig;
            this.url = url;
        }


        public bool update()
        {
            // 更新软件
            List<TargetInformation> remoteConfig = new List<TargetInformation>();
            List<TargetInformation> currentConfig = new List<TargetInformation>();

            ConfigManager.AnalysisXml(currentRemoteConfig, remoteConfig, false);
            ConfigManager.AnalysisXml(currentConfigFile, currentConfig, false);

            WebClient client = new WebClient();
            foreach (TargetInformation file in remoteConfig)
            {
                if (file.FileName == appName)
                {
                    if (file.Md5 != Md5Creator.createMd5(appName)
                           && (file.UpdateMethod == "替换" || file.UpdateMethod == "新增"))
                    {
                        isNeedToClose = true;
                        updateFileName = file.FileName;
                    }
                    continue;
                }


                if (file.UpdateMethod == "新增" || file.UpdateMethod == "替换")
                {
                    if (File.Exists(System.IO.Path.GetDirectoryName(currentConfigFile) + file.FileName))
                    {
                        foreach (TargetInformation config in currentConfig)
                        {
                            if (config.FileName == file.FileName)
                            {
                                config.Md5 = file.Md5;
                                config.UpdateMethod = file.UpdateMethod;
                                break;
                            }
                        }
                    }
                    else
                    {
                        currentConfig.Add(file);
                    }
                    client.DownloadFile(url + "/" + file.FileName, System.IO.Path.GetDirectoryName(currentConfigFile) + file.FileName);
                }
                else if (file.UpdateMethod == "删除")
                {
                    foreach (TargetInformation cf in currentConfig)
                    {
                        if (file.Md5 == cf.Md5 && file.FileName == cf.FileName)
                        {
                            FileInfo ff = new FileInfo(cf.Path);
                            currentConfig.Remove(cf);
                            ff.Delete();
                            break;
                        }
                    }
                }
                else if (file.UpdateMethod == "覆盖后重启")
                {
                    if (file.Md5 != Md5Creator.createMd5(appName))
                    {
                        isNeedToClose = true;
                        updateFileName = file.FileName;
                    }
                }
            }
            ThreadHelper arg = new ThreadHelper { c = currentConfig };
            Thread t = new Thread(new ParameterizedThreadStart(updateConfig));
            t.Start(arg);
            return isNeedToClose;
        }

        class ThreadHelper
        {
            public List<TargetInformation> c;
        }

        private void updateConfig(Object arg)
        {
            List<TargetInformation> currentConfig = (arg as ThreadHelper).c;

            ConfigManager.CreateXmlFile(currentConfig, currentConfigFile, true);
            MessageBox.Show("更新成功", "提示", MessageBoxButton.OK);

            if (isNeedToClose)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = System.IO.Path.GetDirectoryName(currentConfigFile) + "/"+ "update.exe"; //启动的应用程序名称  

                MessageBox.Show("程序即将重启", "提示", MessageBoxButton.OK);
                startInfo.Arguments = "\"" + System.IO.Path.GetDirectoryName(url) + "\\" + updateFileName + "\"" + " " + "\"" + appName + "\"";

                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;      //不使用系统外壳程序启动，重定向时此处必须设为false
                startInfo.RedirectStandardOutput = true; //重定向输出，而不是默认的显示在dos控制台上
                Process.Start(startInfo);
            }
        }
    }
}
