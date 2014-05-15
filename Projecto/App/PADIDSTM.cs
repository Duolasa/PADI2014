using System;
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
        static int currentTXID = 0;


        public static bool Init() {
          TcpChannel channel = new TcpChannel();
          ChannelServices.RegisterChannel(channel, true);
            masterServer = (IMaster)Activator.GetObject(typeof(IMaster),"tcp://localhost:1000/MasterServer");
            RequestHash();
            return true;
        }

        public static bool TxBegin() {
            updatedPadInts.Clear();
            currentTXID = masterServer.getNewTransactionId();
            try {
                if (dataServersPorts.GetTimeStamp() < masterServer.GetTimeStamp())
                    RequestHash();
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public static bool TxCommit() {
            try {
                foreach (PadIntHolder padint in updatedPadInts) {
                    if (padint.WaitingForWrite)
                        padint.RealPadInt.writeCommit();
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
                throw e;
            }
        }
        public static bool TxAbort() {
            try {
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
                throw e;
            }
        }
        public static bool Status() {
            try {
                Dictionary<int, string>.ValueCollection UrlCollection = dataServersPorts.getAllUrls();
                foreach (string url in UrlCollection) {
                    DataServer dataServer = (DataServer)Activator.GetObject(typeof(DataServer), url);
                    DataServer.State state = dataServer.getStatus();

                    Console.WriteLine("Data Server [" + dataServer.getId() + "] is now" + state.ToString());
                }
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public static bool Fail(string url) {
            try {
                DataServer dataServer = (DataServer)Activator.GetObject(typeof(DataServer), url);
                dataServer.fail();
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public static bool Freeze(string url) {
            try {
                DataServer dataServer = (DataServer)Activator.GetObject(typeof(DataServer), url);
                dataServer.freeze();
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
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
            IData dataServer = (IData)Activator.GetObject(typeof(IData), url);
            RealPadInt p = dataServer.CreatePadInt(uid);
            PadIntHolder pHolder = null;
            if (p != null) {
                pHolder = new PadIntHolder(currentTXID, p);
                updatedPadInts.Add(pHolder);
            }

            return (PadInt) pHolder;
        }

        public static PadInt AccessPadInt(int uid) {
            string url = dataServersPorts.getServerByPadiIntID(uid);
            IData dataServer = (IData)Activator.GetObject(typeof(IData), url);
            RealPadInt p = dataServer.AccessPadInt(uid);
            PadIntHolder pHolder = new PadIntHolder(currentTXID, p);
            updatedPadInts.Add(pHolder);
            return (PadInt)pHolder;
        }
    }
}
