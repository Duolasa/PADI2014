using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PADIDSTM
{
  class PADIDSTM : IPADIDSTM, IData
    {

      static IMaster masterServer;
      static ServerHashTable dataServersPorts;
      private List<PadInt> updatedPadInts = new List<PadInt>();

        public static bool Init()
        {
            return true;
        }

        public bool TxBegin()
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

        public bool TxCommit()
        {
            return true;
        }
        public bool TxAbort()
        {
            return true;
        }
        public bool Status()
        {
            return true;
        }
        public bool Fail(string url)
        {
            return true;
        }
        public bool Freeze(string url)
        {
            return true;
        }
        public bool Recover(string url)
        {
            return true;
        }

        public void RequestHash()
        {
          dataServersPorts = masterServer.requestHashTable();
        }

        static PadInt CreatePadInt(int uid)
        {
          string url = dataServersPorts.getServerByPadiIntID(uid);
          IData dataServer = (IData)Activator.GetObject(
              typeof(IData), url);
          PadInt p = dataServer.CreatePadInt(uid);
          return p;
        }

      static PadInt AccessPadInt(int uid){
        string url = dataServersPorts.getServerByPadiIntID(uid);
        IData dataServer = (IData)Activator.GetObject(
                typeof(IData), url);

        PadInt p = dataServer.AccessPadInt(uid);
        return p;

    }


    }
}
