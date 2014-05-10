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
        private long timestamp = DateTime.UtcNow.Ticks;
        private List<int> serversUnderMaintenance = new List<int>();


        public ServerHashTable()
        {
            dataServerUrls = new Dictionary<int, string>();
        }

        public long GetTimeStamp(){
          return timestamp;
        }
        public int addServer(string url) {
            dataServerUrls.Add(numberOfServers, url);
            return numberOfServers++;
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

        public bool IsServerAvailable(int id)
        {
          return serversUnderMaintenance.Contains(id);
        }

        public bool RemoverServerFromMaintenanceList(int id)
        {
          return serversUnderMaintenance.Remove(id);
        }
    }
}
