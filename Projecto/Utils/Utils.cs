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
        public const int TIME_TO_WAIT_FOR_TRANSACTION = 3000;

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
