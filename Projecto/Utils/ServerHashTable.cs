using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADIDSTM
{
    public class ServerHashTable
    {
        private int numberOfServers = 0;
        private Dictionary<int, string> dataServerUrls;

        public ServerHashTable()
        {
            dataServerUrls = new Dictionary<int, string>();
        }

        public void addServer(string url) {
            dataServerUrls.Add(numberOfServers, url);
            numberOfServers++;
        }

        public string getServerByPadiIntID(int id)
        {
            if (numberOfServers <= 0)
            {
                //TODO create our exception
                throw new Exception("No Data Servers, this should not happen");
            }
            int serverSet = id % numberOfServers;
            return dataServerUrls[serverSet];
        }
    }
}
