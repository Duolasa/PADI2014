using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PADIDSTM
{
    [Serializable]
    public class ServerHashTable
    {
        private int numberOfServers = 0;
        private Dictionary<int, string> dataServerUrls;
        private Dictionary<string, int> remoteUrlToAdminPort;
        private Dictionary<int, bool> serverAliveList;
        private Dictionary<int, bool> serverAlreadyDied = new Dictionary<int, bool>();
        private long timestamp = DateTime.UtcNow.Ticks;
        private List<int> serversUnderMaintenance = new List<int>();

        public ServerHashTable()
        {
            dataServerUrls = new Dictionary<int, string>();
            remoteUrlToAdminPort = new Dictionary<string, int>();
            serverAliveList = new Dictionary<int, bool>();
        }

        public long GetTimeStamp(){
          return timestamp;
        }

        public int addServer(int port) {
            string remoteUrl = Utils.getDataServerUrl(port);
            dataServerUrls.Add(numberOfServers, remoteUrl);
            remoteUrlToAdminPort.Add(remoteUrl, port + Utils.ADMIN_PORT);
            serverAliveList.Add(numberOfServers, true);
            return numberOfServers++;
        }

        public int getAdminPortByUrl(string url) {
            return remoteUrlToAdminPort[url];
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

        public int getNumberOfServers() 
        {
            return numberOfServers;
        }

        public Dictionary<int, string>.ValueCollection getAllUrls()
        {
           Dictionary<int, string>.ValueCollection UrlColl = new Dictionary<int,string>(dataServerUrls).Values;
           return UrlColl;
        }

        public bool IsServerAvailable(int id)
        {
          return serversUnderMaintenance.Contains(id);
        }

        public bool RemoverServerFromMaintenanceList(int id)
        {
          return serversUnderMaintenance.Remove(id);
        }

        public void serverIsAlive(int uid)
        {
          serverAliveList[uid] = true;
        }

        public  Dictionary<int, string> getDeadServers()
        {
          Dictionary<int, string> deadServers = new Dictionary<int, string>();
          Dictionary<int, bool> serverListCopy = new Dictionary<int, bool>(serverAliveList);
          foreach (KeyValuePair<int, bool> pair in serverListCopy)
          {
            if(!pair.Value) {
              deadServers.Add(pair.Key, dataServerUrls[pair.Key]);
            }
            serverAliveList[pair.Key] = false;
       
          }

          return deadServers;
        }

        public Dictionary<int, string> getDictionary(){
            return dataServerUrls;
        }

      public Dictionary<int, bool> getAlreadyDiedBefore(){
        return serverAlreadyDied;
      }

    }
}
