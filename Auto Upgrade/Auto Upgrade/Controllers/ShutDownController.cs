using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Upgrade.Controllers
{
    class ShutDownController
    {
        public static void shutDown()
        {
            App.Current.Shutdown();
        }

    }
}
