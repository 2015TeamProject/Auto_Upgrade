using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace update
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(2000);

            FileInfo ff = new FileInfo(args[1]);
            ff.Delete();
            if (args[0] != "")
            {
                WebClient client = new WebClient();
                client.DownloadFile(args[0], System.AppDomain.CurrentDomain.BaseDirectory + System.IO.Path.GetFileName(args[0]));
                System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + System.IO.Path.GetFileName(args[0]));
            }
        }
    }
}
