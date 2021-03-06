﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Net;
using System.Net.Sockets;

namespace PADIDSTM {
    public class PadiDstm {

        static IMaster masterServer;
        static ServerHashTable dataServersPorts;
        static private List<PadIntHolder> updatedPadInts = new List<PadIntHolder>();
        static private List<int> createdPadInts = new List<int>();

        static int currentTXID = 0;


        public static bool Init() {
          TcpChannel channel = new TcpChannel();
          ChannelServices.RegisterChannel(channel, false);
            masterServer = (IMaster)Activator.GetObject(typeof(IMaster),"tcp://localhost:1000/MasterServer");
            RequestHash();
            return true;
        }

        public static bool TxBegin() {
            updatedPadInts.Clear();
            createdPadInts.Clear();
            currentTXID = masterServer.getNewTransactionId();
            try {
                
                    RequestHash();
                return true;
            } catch (Exception e) {
                throw new TxException("TxBegin", e);
            }
        }

        public static bool TxCommit() {
            try {
                foreach (PadIntHolder padint in updatedPadInts) {
                  if (padint.WaitingForWrite)
                  {
                    string url = dataServersPorts.getServerByPadiIntID(padint.RealPadInt.ID);
                    IData dataServer = (IData)Activator.GetObject(typeof(IData), url);
                    padint.RealPadInt.writeCommit();
                    dataServer.WriteCommit(padint.RealPadInt.ID, padint.RealPadInt.DirectRead());

                  }
                }
                foreach (PadIntHolder padint in updatedPadInts) {
                    RealPadInt realPadInt = padint.RealPadInt;
                    lock (realPadInt) {
                        realPadInt.removeMeFromReadersList(currentTXID);
                        if (padint.WaitingForWrite) 
                            realPadInt.unlockPadInt();
                    }
                }
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
                throw new TxException("TxCommit: " + e.Message) ;
            }
        }
        public static bool TxAbort() {
            try {
                foreach (PadIntHolder padint in updatedPadInts) {
                    RealPadInt realPadInt = padint.RealPadInt;
                    lock (realPadInt) {
                        realPadInt.removeMeFromReadersList(currentTXID);
                        if (padint.Created) {
                            int pInt = realPadInt.ID;
                            string url = dataServersPorts.getServerByPadiIntID(pInt);
                            IData dataServer = (IData)Activator.GetObject(typeof(IData), url);
                            dataServer.DeletePadInt(pInt);
                            int correspondingServer = (pInt) % dataServersPorts.getNumberOfServers();
                            dataServer.DeleteOnMySafeCopy(correspondingServer, pInt);
                        }
                        else if (padint.WaitingForWrite)
                            realPadInt.unlockPadInt();
                    }
                }

                foreach (int i in createdPadInts)
                {
                  string url = dataServersPorts.getServerByPadiIntID(i);
                  IData dataServer = (IData)Activator.GetObject(typeof(IData), url);
                  dataServer.DeletePadInt(i);
                  int correspondingServer = (i) % dataServersPorts.getNumberOfServers();

                  dataServer.DeleteOnMySafeCopy(correspondingServer, i);

                }
                return true;
            } catch (Exception e) {
                throw new TxException("TxAbort", e);
            }
        }
        public static bool Status() {
                Dictionary<int, string>.ValueCollection UrlCollection = dataServersPorts.getAllUrls();
                foreach (string url in UrlCollection) {
                    DataServer dataServer = (DataServer)Activator.GetObject(typeof(DataServer), url);
                    DataServer.State state = dataServer.getStatus();

                    Console.WriteLine("Data Server [" + dataServer.getId() + "] is now" + state.ToString());
                }
                return true;
        }

        public static bool Fail(string url) {
                DataServer dataServer = (DataServer)Activator.GetObject(typeof(DataServer), url);
                dataServer.fail();
                return true;
        }

        public static bool Freeze(string url) {
                DataServer dataServer = (DataServer)Activator.GetObject(typeof(DataServer), url);
                dataServer.freeze();

                return true;
        }


        /*public static bool Recover(string url) {
            try {
                DataServer dataServer = (DataServer)Activator.GetObject(typeof(DataServer), url);
                dataServer.setStatus(DataServer.State.Working);
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }*/

        public static bool Recover(string url) {
            Int32 port = dataServersPorts.getAdminPortByUrl(url);
            TcpClient client = new TcpClient(Dns.GetHostEntry("localhost").AddressList[0].ToString(), port);
            client.Close();
            Console.WriteLine("connect to port");
            return true;
        }

        private static void RequestHash() {
            dataServersPorts = masterServer.requestHashTable();
        }

        public static PadInt CreatePadInt(int uid) {
            string url = dataServersPorts.getServerByPadiIntID(uid);
            DataServer dataServer = (DataServer)Activator.GetObject(typeof(DataServer), url);
            if (dataServer.getStatus() == DataServer.State.Failed) {
                return null;
            }
            RealPadInt p = dataServer.CreatePadInt(uid);
            PadIntHolder pHolder = null;
            if (p != null) {
                createdPadInts.Add(uid);
                int correspondingServer = (p.ID) % dataServersPorts.getNumberOfServers();
                dataServer.CreateOnMySafeCopy(correspondingServer, p.ID, p.DirectRead());
                pHolder = new PadIntHolder(currentTXID, p);
                pHolder.Created = true;
                updatedPadInts.Add(pHolder);
            }

            return (PadInt) pHolder;
        }

        public static PadInt AccessPadInt(int uid) {
            string url = dataServersPorts.getServerByPadiIntID(uid);
            DataServer dataServer = (DataServer)Activator.GetObject(typeof(DataServer), url);
            if (dataServer.getStatus() == DataServer.State.Failed) {
                return null;
            }
            RealPadInt p = dataServer.AccessPadInt(uid);
            PadIntHolder pHolder = new PadIntHolder(currentTXID, p);
            updatedPadInts.Add(pHolder);
            return (PadInt)pHolder;
        }
    }
}
