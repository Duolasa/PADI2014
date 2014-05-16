using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;


namespace PADIDSTM {
    public class Program {
        static void Main(string[] args) {
            bool res;

            try {
                Console.ReadLine();
                PadiDstm.Init();
                Console.WriteLine("INIT");

                res = PadiDstm.Recover("tcp://localhost:1001/Server");
                /*res = PadiDstm.TxBegin();
                Console.WriteLine("BEGIN");
                PadInt pi_a = PadiDstm.AccessPadInt(0);
                Console.WriteLine("ACCESS 0");
                PadInt pi_b = PadiDstm.AccessPadInt(1);
                Console.WriteLine("ACCESS 1");
              //  pi_a.Write(36);
                Console.ReadLine();
                Console.WriteLine("a = " + pi_a.Read());
                Console.WriteLine("b = " + pi_b.Read());
                pi_a.Write(40);
                pi_b.Write(41);
                Console.WriteLine("a = " + pi_a.Read());
                Console.WriteLine("b = " + pi_b.Read());
                PadiDstm.Status();
                // The following 3 lines assume we have 2 servers: one at port 2001 and another at port 2002
                res = PadiDstm.Freeze("tcp://localhost:1001/Server");
                res = PadiDstm.Recover("tcp://localhost:1001/Server");
                res = PadiDstm.Fail("tcp://localhost:1002/Server");
                PadiDstm.Status();
                res = PadiDstm.TxCommit();*/
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }


    }
}
