using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using System.Diagnostics;

namespace PADIDSTM {
    public class Master : MarshalByRefObject, IMaster {
        private const string dataServerExeLocation = "DataServer";
        private static Thread heartBeatWorkerThread;
        private static ServerHashTable dataServers = new ServerHashTable();
        private System.Object lockDataServers = new System.Object();
        private System.Object lockTransactionId = new System.Object();
        private static List<Process> dataProcesses = new List<Process>();
        private static Process thisProcess;
        private static int transactionsId = 0;
        private static int nrOfDataServers = 0;

        static void Main(string[] args) {
            Console.WriteLine("How many DataServers do you want?");
            nrOfDataServers = Convert.ToInt32(Console.ReadLine());
            launchMasterServer(1000);
            launchDataServers(nrOfDataServers);
            while (nrOfDataServers != dataServers.getNumberOfServers()) { }
            sendTableToDataServers();
            initiateDataServersCopy();
            launchHeartBeatWorker();
            thisProcess = Process.GetCurrentProcess();
            thisProcess.EnableRaisingEvents = true;
            thisProcess.Exited += (sender, e) => {
                foreach (Process p in dataProcesses) {
                    Console.WriteLine("deleting processes");
                    p.Kill();
                }
            };
            Console.ReadLine();
        }

        static void launchMasterServer(int port) {
            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(Master),
                "MasterServer",
                WellKnownObjectMode.Singleton);

            Console.WriteLine("master server launched on port " + port);
        }

        static void launchHeartBeatWorker() {
            MasterServerWorker heartBeatWorker = new MasterServerWorker();
            heartBeatWorkerThread = new Thread(heartBeatWorker.checkServersAliveWorker);
            heartBeatWorkerThread.Start();
        }

        public int getNewTransactionId() {
            lock (lockTransactionId) {
                return transactionsId++;
            }
        }

        public void iAmAlive(int uid) {
            dataServers.serverIsAlive(uid);
        }

        static void launchDataServers(int nrOfServers) {
            int port = 1001;
            for (int i = 0; i < nrOfServers; i++) {
                dataProcesses.Add(Process.Start(dataServerExeLocation, (port + i).ToString()));
            }
        }

        static public void sendTableToDataServers() {
            int dataServerCounter = dataServers.getNumberOfServers();

            Dictionary<int, string>.ValueCollection UrlColl = dataServers.getAllUrls();
            foreach (string url in UrlColl) {
                DataServer dataServer = (DataServer)Activator.GetObject(
                            typeof(DataServer),
                            url);
                if (dataServer != null)
                    dataServer.receiveDataServersTable(dataServers);
            }
        }

        static void initiateDataServersCopy() {
            Console.WriteLine("Data servers getting reference to safe copy");
            int dataServerCounter = dataServers.getNumberOfServers();

            Dictionary<int, string>.ValueCollection UrlColl = dataServers.getAllUrls();
            foreach (string url in UrlColl) {
                DataServer dataServer = (DataServer)Activator.GetObject(
                            typeof(DataServer),
                            url);
                if (dataServer != null)
                    dataServer.getRefToMySafeCopy();
            }
        }

        public long GetTimeStamp() {
            return dataServers.GetTimeStamp();
        }

        public ServerHashTable requestHashTable() {
            return dataServers;
        }

        public int addDataServer(int port) {
            lock (lockDataServers) {
                Console.WriteLine("Master Adding data server");
                int serverId = dataServers.addServer(port);
                sendTableToDataServers();
                return serverId;
            }
        }

        private class MasterServerWorker {
            public void checkServersAliveWorker() {
                while (true) {
                    Thread.Sleep(Utils.HEARTBEAT_INTERVAL_CHECK);
                    Console.WriteLine("=======================================");
                    Dictionary<int, string> deadServers = dataServers.getDeadServers();
                    if (deadServers.Count > 0) {
                        foreach (KeyValuePair<int, string> pair in deadServers) {
                            Console.WriteLine("OMFG MAN(SERVER) " + pair.Key + " IS DOWN");

                        }

                    } else {
                        Console.WriteLine("Everyone is alive..boring...");
                    }
                    Console.WriteLine("=======================================");
                }
            }
        }


    }
}