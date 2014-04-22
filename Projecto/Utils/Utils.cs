using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADIDSTM
{
    public class Utils
    {
        public const string BASE_URL = "tcp://localhost:";

        public static string getDataServerUrl(int port) 
        {
            return BASE_URL + port + "/Server";
        }

        public static string getMasterServerUrl()
        {
            return BASE_URL + 1000 + "/MasterServer";
        }
    }
}
