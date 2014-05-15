using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace PADIDSTM {
    public class DataServer : MarshalByRefObject, IData {
        static WaitingForRecover worker;
        static TcpChannel remoteChannel;
        static TcpChannel adminChannel;
        static IMaster masterServer;
        static private int port;
        static private int adminPort;
        static private int id;
        static private ServerHashTable dataServersTable = new ServerHashTable();
        static private Hashtable padIntStorage = new Hashtable();
        static private object statusChangeLock = new object();
        public enum State { Working, Failed, Frozen };
        static private State state = State.Working;
        static private PadIntSafeCopy myPadIntSafeCopy;
        static private Dictionary<int, PadIntSafeCopy> otherSafeCopies = new Dictionary<int,PadIntSafeCopy>();

        static void Main(string[] args) {
            //Console.WriteLine(args);
            if (args.Length > 0) {
                port = Convert.ToInt32(args[0]);
            } else {
                Console.WriteLine("Port Not Given, write port manually");
                port = Convert.ToInt32(Console.ReadLine());
            }
            launchServer(port);
            adminPort = port + Utils.ADMIN_PORT;
            getMasterServer();
            registerDataServer();
            Console.ReadLine();

        }

        public PadIntSafeCopy getPadIntSafeCopy(int serverId)
        {
          PadIntSafeCopy pisc;
          if (otherSafeCopies.ContainsKey(serverId))
          {
            otherSafeCopies.TryGetValue(serverId, out pisc);
            return pisc;
          }
          pisc = new PadIntSafeCopy(serverId);
          otherSafeCopies.Add(serverId, pisc);
          return pisc;
        }


        public void getRefToMySafeCopy()
        {
          Dictionary<int, string> dic = dataServersTable.getDictionary();
          String url;
          dic.TryGetValue((id + 1) % dataServersTable.getNumberOfServers(), out url);

          DataServer copyHolder = (DataServer) Activator.GetObject(typeof(DataServer),url); ;
          myPadIntSafeCopy = copyHolder.getPadIntSafeCopy(id);

          Console.WriteLine("Got my safe copy from server " + copyHolder.getId());
        }

        static void launchRecoverCommandThread() {
            worker = new WaitingForRecover();
            Thread workerThread = new Thread(worker.waitForRecover);
            workerThread.Start();
        }
        static void launchServer(int port) {
            remoteChannel = new TcpChannel(port);
            adminChannel = new TcpChannel(port + Utils.ADMIN_PORT);
            ChannelServices.RegisterChannel(remoteChannel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(DataServer),
                "Server",
                WellKnownObjectMode.Singleton);

            Console.WriteLine("data server launched on port " + port);

        }

        static void getMasterServer() {
            masterServer = (IMaster)Activator.GetObject(
                typeof(IMaster),
                "tcp://localhost:1000/MasterServer");
            Console.WriteLine("got Master Server");
        }

        static void registerDataServer() {
            Console.WriteLine("Registering on Master with port: " + port);
            id = masterServer.addDataServer(port);
            Console.WriteLine("Registered on Master Server with id " + id);
        }

        public void receiveDataServersTable(ServerHashTable dataServers) {
            dataServersTable = dataServers;
        }

        public RealPadInt CreatePadInt(int uid) {
            if (padIntStorage.ContainsKey(uid)) {
                return null;
            }

            RealPadInt pad = new RealPadInt(uid);
            padIntStorage.Add(uid, pad);
            return pad;

        }
        public RealPadInt AccessPadInt(int uid) {
            if (!(padIntStorage.ContainsKey(uid))) {
                return null;
            }

            return (RealPadInt)padIntStorage[uid];

        }

        public State getStatus() {
            lock (statusChangeLock) {
                return state;
            }
        }

        static private void setStatus(State newState) {
            lock (statusChangeLock) {
                state = newState;
            }
        }

        public int getId() {
            return id;
        }

        public void fail() {
            setStatus(State.Failed);
            ChannelServices.UnregisterChannel(remoteChannel);
        }

        public void freeze() {
            setStatus(State.Frozen);
            ChannelServices.UnregisterChannel(remoteChannel);
        }

        static void recover() {
            ChannelServices.RegisterChannel(remoteChannel, true);
            setStatus(State.Working);
        }

        private class WaitingForRecover {
            public void waitForRecover() {
                TcpListener server = null;
                // Set the TcpListener on port 13000.
                IPAddress localAddr = Dns.GetHostEntry("localhost").AddressList[0];
                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, adminPort);

                // Start listening for client requests.
                server.Start();


                // Enter the listening loop. 
                while (true) {
                    Console.WriteLine("Waiting for a recover... ");

                    // Perform a blocking call to accept requests. 
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    client.Close();
                    Console.WriteLine("Going To Recover!");
                    if(state == State.Failed)
                        recover();

                }
            }
        }
    }
}
