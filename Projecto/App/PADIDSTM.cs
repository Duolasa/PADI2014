using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PADIDSTM
{
  class PADIDSTM
    {

      static IMaster masterServer;
      static ServerHashTable dataServersPorts;
      private List<PadInt> updatedPadInts = new List<PadInt>();

        public static bool Init()
        {
            return true;
        }

        public static bool TxBegin()
        {
            try
            {
                if (dataServersPorts.GetTimeStamp() < masterServer.GetTimeStamp())
                    RequestHash();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public static bool TxCommit()
        { 
            return true;
        }
        public static bool TxAbort()
        {
            return true;
        }
        public static bool Status()
        {
            return true;
        }
        public static bool Fail(string url)
        {
            return true;
        }
        public static bool Freeze(string url)
        {
            return true;
        }
        public static bool Recover(string url)
        {
            return true;
        }

        private static void RequestHash()
        {
          dataServersPorts = masterServer.requestHashTable();
        }

        public static PadInt CreatePadInt(int uid)
        {
          string url = dataServersPorts.getServerByPadiIntID(uid);
          IData dataServer = (IData)Activator.GetObject(
              typeof(IData), url);
          PadInt p = dataServer.CreatePadInt(uid);
          return p;
        }

      public static PadInt AccessPadInt(int uid){
        string url = dataServersPorts.getServerByPadiIntID(uid);
        IData dataServer = (IData)Activator.GetObject(
                typeof(IData), url);

        PadInt p = dataServer.AccessPadInt(uid);
        return p;

    }


    }
}
