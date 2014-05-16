using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PADIDSTM
{
    public class Utils
    {
        public const string BASE_URL = "tcp://localhost:";
        public const int TIME_TO_WAIT_FOR_TRANSACTION = 3000;
        public const int ADMIN_PORT = 1337;
        public const int HEARTBEAT_INTERVAL = 2500;
        public const int HEARTBEAT_INTERVAL_CHECK = 5000;
        public const int WAITING_FOR_TX_TIMEOUT = 5;

        public static string getDataServerUrl(int port) 
        {
            return BASE_URL + port + "/Server";
        }

        public static string LocalIPAddress() {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
        public static string getMasterServerUrl()
        {
            return BASE_URL + 1000 + "/MasterServer";
        }

    }
}
