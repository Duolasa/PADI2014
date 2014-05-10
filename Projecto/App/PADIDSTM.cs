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

        public static bool Init()
        {
            return true;
        }

        public bool TxBegin()
        {
            return true;
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

        static void RequestHash()
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
