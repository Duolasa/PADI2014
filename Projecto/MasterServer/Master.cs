using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Diagnostics;

namespace PADIDSTM
{
    public class Master : MarshalByRefObject, IMaster
    {
        private const string dataServerExeLocation = "DataServer";
        private ServerHashTable dataServers = new ServerHashTable();
        private System.Object lockDataServers = new System.Object();
        private static List<Process> dataProcesses = new List<Process>();
        private static Process thisProcess;

        static void Main(string[] args)
        {
            Console.WriteLine("How many DataServers do you want?");
            int nrOfDataServerToLaunch = Convert.ToInt32(Console.ReadLine());
            launchMasterServer(1000);
            launchDataServers(nrOfDataServerToLaunch);
            thisProcess = Process.GetCurrentProcess();
            thisProcess.EnableRaisingEvents = true;
            thisProcess.Exited += (sender, e) =>
            {
                foreach (Process p in dataProcesses)
                {
                    Console.WriteLine("deleting processes");
                    p.Kill();
                }
            };
            Console.ReadLine();
        }

        static void launchMasterServer(int port)
        {
            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(Master),
                "MasterServer",
                WellKnownObjectMode.Singleton);

            Console.WriteLine("master server launched on port " + port);
        }

        static void launchDataServers(int nrOfServers)
        {
            int port = 1001;
            for (int i = 0; i < nrOfServers; i++)
            {
                dataProcesses.Add(Process.Start(dataServerExeLocation, (port + i).ToString()));
            }
        }

        public void sendTableToDataServers()
        {
            int dataServerCounter = dataServers.getNumberOfServers();

            Dictionary<int, string>.ValueCollection UrlColl = dataServers.getAllUrls();
            foreach (string url in UrlColl)
            {
                DataServer dataServer = (DataServer)Activator.GetObject(
                            typeof(DataServer),
                            url);
                if(dataServer != null)
                    dataServer.receiveDataServersTable(dataServers);
            }
        }

        public long GetTimeStamp()
        {
            return dataServers.GetTimeStamp();
        } 

        public ServerHashTable requestHashTable()
        {
            return dataServers;
        }

        public int addDataServer(string url)
        {
          lock (lockDataServers)
          {
            Console.WriteLine("Master Adding data server");
            int serverId = dataServers.addServer(url);
            sendTableToDataServers();
            return serverId;
          }
        }


    }
}