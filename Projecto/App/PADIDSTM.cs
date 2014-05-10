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

        static void CreatePadInt()
        {
          Random rnd = new Random();
          int num = rnd.Next(0, 666);
          string url = dataServersPorts.getServerByPadiIntID(num);
          IData dataServer = (IData)Activator.GetObject(
              typeof(IData), url);
          PadInt p = dataServer.createPadInt(2);
          p.write(9999);

          PadInt j = dataServer.acessPadInt(2);

          Console.WriteLine(j.read());

        }
    }
}
