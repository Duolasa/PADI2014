﻿using System;
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
        static DataServerWorker worker;
        static DataServerWorker heartBeatWorker;
        static TcpChannel remoteChannel;
        static TcpChannel adminChannel;
        static IMaster masterServer;
        static private int port;
        static private int adminPort;
        static private int id;
        static private ServerHashTable dataServersTable = new ServerHashTable();
        static private Dictionary<int,Hashtable> padIntStorage = new Dictionary<int, Hashtable>();
        static private object statusChangeLock = new object();
        static private object requestQueueLock = new object();
        public enum State { Working, Failed, Frozen };
        static private State state = State.Working;
        static private Dictionary<int, Hashtable> myPadIntSafeCopy = new Dictionary<int, Hashtable>();
        static private Dictionary<int, Dictionary<int, Hashtable>> otherSafeCopies = new Dictionary<int, Dictionary<int, Hashtable>>();

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
            launchRecoverCommandThread();
            launchHeartBeatThread();
            Console.ReadLine();

        }


        static void launchHeartBeatThread() {
            heartBeatWorker = new DataServerWorker();
            Thread heartBeatWorkerThread = new Thread(heartBeatWorker.heartBeatWorker);
            heartBeatWorkerThread.Start();
        }
        static void launchRecoverCommandThread() {
            worker = new DataServerWorker();
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
            padIntStorage.Add(id, new Hashtable());
            Console.WriteLine("Registered on Master Server with id " + id);
        }

        public void receiveDataServersTable(ServerHashTable dataServers) {
            dataServersTable = dataServers;
        }

        public bool ServerHasDied(int id)
        {
          if (otherSafeCopies.ContainsKey(id))
          {
            Dictionary<int, Hashtable> safeCopy;
            otherSafeCopies.TryGetValue(id, out safeCopy);


          }

          return false;
        } 
      
        public Dictionary<int, Hashtable> getPadIntSafeCopy(int serverId)
        {
          Dictionary<int, Hashtable> pisc;
          if (otherSafeCopies.ContainsKey(serverId))
          {
            otherSafeCopies.TryGetValue(serverId, out pisc);
            return pisc;
          }
          pisc = new Dictionary<int, Hashtable>();
          pisc.Add(serverId, new Hashtable());
          otherSafeCopies.Add(serverId, pisc);
          return pisc;
        }

        public void getRefToMySafeCopy()
        {
          Dictionary<int, string> dic = dataServersTable.getDictionary();
          String url;
          int backupServerId = (id + 1) % dataServersTable.getNumberOfServers();
          dic.TryGetValue(backupServerId, out url);

            DataServer copyHolder = (DataServer)Activator.GetObject(typeof(DataServer), url); ;
            myPadIntSafeCopy = copyHolder.getPadIntSafeCopy(id);

            Console.WriteLine("Got my safe copy from server " + copyHolder.getId());
        }

        public RealPadInt CreatePadIntSafeCopy(int uid) {
            int correspondingServer = (uid) % dataServersTable.getNumberOfServers();
            RealPadInt pad = new RealPadInt(uid);
            Hashtable safeCopy;
            myPadIntSafeCopy.TryGetValue(correspondingServer, out safeCopy);
            safeCopy.Add(uid, pad);
            return pad;
        }

        public RealPadInt AccessPadIntSafeCopy(int uid)
        {
          int correspondingServer = (uid) % dataServersTable.getNumberOfServers();
          Hashtable safeCopy;
          myPadIntSafeCopy.TryGetValue(correspondingServer, out safeCopy);
          return (RealPadInt) safeCopy[uid];
        }

        public void DeletePadIntSafeCopy(int uid)
        {
          int correspondingServer = (uid) % dataServersTable.getNumberOfServers();
          Hashtable safeCopy;
          myPadIntSafeCopy.TryGetValue(correspondingServer, out safeCopy);
          safeCopy.Remove(uid);

        }

        public void checkFreezeStatus() {
            if (state == State.Frozen) {
                Console.WriteLine("freezing");
                Object lockedObject = new Object();
                lock (requestQueueLock) {
                    requestQueue.Add(lockedObject);
                }
                lock (lockedObject) {
                    Monitor.Wait(lockedObject);
                }
            }
        }

        public RealPadInt CreatePadInt(int uid) {
			 checkFreezeStatus();
            int correspondingServer = (uid) % dataServersTable.getNumberOfServers();

          if (!padIntStorage.ContainsKey(correspondingServer)) {
                return null;
            }
            Hashtable safeCopy;
            padIntStorage.TryGetValue(correspondingServer, out safeCopy);

            if (safeCopy.ContainsKey(uid)) {
                return null;
            }

            RealPadInt pad = new RealPadInt(uid);
            safeCopy.Add(uid, pad);
            return pad;

        }
        public RealPadInt AccessPadInt(int uid) {
checkFreezeStatus();
          int correspondingServer = (uid) % dataServersTable.getNumberOfServers();

          if (!padIntStorage.ContainsKey(correspondingServer))
          {
            return null;
          }

          Hashtable safeCopy;
          padIntStorage.TryGetValue(correspondingServer, out safeCopy);

          if (!safeCopy.ContainsKey(uid))
          {
            return null;
          }

          return (RealPadInt)safeCopy[uid];

        }

        public void DeletePadInt(int uid)
        {
          int correspondingServer = (uid) % dataServersTable.getNumberOfServers();

          if (!padIntStorage.ContainsKey(correspondingServer))
          {
          }

          Hashtable safeCopy;
          padIntStorage.TryGetValue(correspondingServer, out safeCopy);
          safeCopy.Remove(uid);
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
        }

        public void freeze() {
            setStatus(State.Frozen);


        }

        static void recover() {
            Console.WriteLine("entered request QueueLock");
            setStatus(State.Working);
            lock (requestQueueLock) {
                Console.WriteLine("entered request QueueLock");
                foreach (object obj in requestQueue) {
                    lock (obj) {
                        Monitor.Pulse(obj);
                    }
                }
                requestQueue.Clear();
            }
        }

        private class DataServerWorker {
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
                    if (state != State.Working)
                        recover();

                }
            }

            public void heartBeatWorker() {
                while (true) {
                    if (state == State.Failed) {
                        Thread.Sleep(1000);
                        continue;
                    }
                    Thread.Sleep(Utils.HEARTBEAT_INTERVAL);
                    masterServer.iAmAlive(id);
                }
            }
        }
    }
}
