using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace PADIDSTM {
    public class PadiDstm {

        static IMaster masterServer;
        static ServerHashTable dataServersPorts;
        static private List<PadInt> updatedPadInts = new List<PadInt>();


        public static bool Init() {

          TcpChannel channel = new TcpChannel(Utils.CLIENT_PORT);
          ChannelServices.RegisterChannel(channel, true);
            masterServer = (IMaster)Activator.GetObject(typeof(IMaster),"tcp://localhost:1000/MasterServer");
            Console.WriteLine("Praise the sun");

            RequestHash();
            Console.WriteLine("REQUEST");
            return true;
        }

        public static bool TxBegin() {
            updatedPadInts.Clear();
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
                foreach (PadInt padint in updatedPadInts) {
                    padint.writeCommit();
                }
                foreach (PadInt padint in updatedPadInts) {
                    padint.unlockPadInt();
                }
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public static bool TxAbort() {
            try {
                foreach (PadInt padint in updatedPadInts) {
                    padint.unlockPadInt();
                }
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public static bool Status() {
            try {
                Dictionary<int, string>.ValueCollection UrlCollection = dataServersPorts.getAllUrls();
                foreach (string url in UrlCollection) {
                    DataServer dataServer = (DataServer)Activator.GetObject(typeof(DataServer), url);
                    DataServer.State state = dataServer.getStatus();

                    Console.Write("Data Server [" + dataServer.getId() + "] is now" + state.ToString());
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
                dataServer.setStatus(DataServer.State.Failed);
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public static bool Freeze(string url) {
            try {
                DataServer dataServer = (DataServer)Activator.GetObject(typeof(DataServer), url);
                dataServer.setStatus(DataServer.State.Frozen);
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }


        public static bool Recover(string url) {
            try {
                DataServer dataServer = (DataServer)Activator.GetObject(typeof(DataServer), url);
                dataServer.setStatus(DataServer.State.Working);
                return true;
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private static void RequestHash() {
            dataServersPorts = masterServer.requestHashTable();
        }

        public static PadInt CreatePadInt(int uid) {
            string url = dataServersPorts.getServerByPadiIntID(uid);
            IData dataServer = (IData)Activator.GetObject(typeof(IData), url);
            PadInt p = dataServer.CreatePadInt(uid);
            if (p != null)
                updatedPadInts.Add(p);
            return p;
        }

        public static PadInt AccessPadInt(int uid) {
            string url = dataServersPorts.getServerByPadiIntID(uid);
            IData dataServer = (IData)Activator.GetObject(typeof(IData), url);
            PadInt p = dataServer.AccessPadInt(uid);
            return p;
        }
    }
}
