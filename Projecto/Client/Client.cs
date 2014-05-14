using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;


namespace PADIDSTM
{
  public class Client
  {
    static void Main(string[] args) {
          bool res;

          try
          {
            PadiDstm.Init();
            Console.WriteLine("INIT");

            res = PadiDstm.TxBegin();
            Console.WriteLine("BEGIN");
            PadInt pi_a = PadiDstm.CreatePadInt(0);
            Console.WriteLine("CREATE 0");
            PadInt pi_b = PadiDstm.CreatePadInt(1);
            res = PadiDstm.TxCommit();

            res = PadiDstm.TxBegin();
            pi_a = PadiDstm.AccessPadInt(0);
            pi_b = PadiDstm.AccessPadInt(1);
            pi_a.Write(36);
            pi_b.Write(37);
            Console.WriteLine("a = " + pi_a.Read());
            Console.WriteLine("b = " + pi_b.Read());
            PadiDstm.Status();
            // The following 3 lines assume we have 2 servers: one at port 2001 and another at port 2002
            res = PadiDstm.Freeze("tcp://localhost:2001/Server");
            res = PadiDstm.Recover("tcp://localhost:2001/Server");
            res = PadiDstm.Fail("tcp://localhost:2002/Server");
            res = PadiDstm.TxCommit();
          }
          catch (Exception e)
          {
            Console.WriteLine(e.Message);
          }
      }


  }
}
