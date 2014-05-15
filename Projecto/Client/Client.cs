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
    public class Client {
        static void Main(string[] args) {
            bool res;

            try {
                Console.ReadLine();
                PadiDstm.Init();
                Console.WriteLine("INIT");

                res = PadiDstm.TxBegin();
                Console.WriteLine("BEGIN");
                PadInt pi_a = PadiDstm.CreatePadInt(0);
                Console.WriteLine("CREATE 0");
                PadInt pi_b = PadiDstm.CreatePadInt(1);
                res = PadiDstm.TxCommit();
                Console.WriteLine("COMMIT");

                res = PadiDstm.TxBegin();
                Console.WriteLine("BEGIN");
                pi_a = PadiDstm.AccessPadInt(0);
                Console.WriteLine("ACCESS 0");
                pi_b = PadiDstm.AccessPadInt(1);
                pi_a.Write(36);
                Console.ReadLine();
                Console.WriteLine("a = " + pi_a.Read());
                Console.WriteLine("WRITE");
                pi_b.Write(37);
                Console.WriteLine("WRITE");
                Console.WriteLine("a = " + pi_a.Read());
                Console.WriteLine("b = " + pi_b.Read());
          /*      PadiDstm.Status();
                Console.WriteLine("Testing recover server 1");
                res = PadiDstm.Fail("tcp://localhost:1002/Server");
                PadiDstm.Status();
                res = PadiDstm.Recover("tcp://localhost:1002/Server");
                PadiDstm.Status();

                Console.WriteLine("Testing recover server 0");
                res = PadiDstm.Fail("tcp://localhost:1001/Server");
                PadiDstm.Status();
                res = PadiDstm.Recover("tcp://localhost:1001/Server");
                PadiDstm.Status();

                Console.WriteLine("Testing recover server 1");
                res = PadiDstm.Fail("tcp://localhost:1002/Server");
                PadiDstm.Status();
                res = PadiDstm.Recover("tcp://localhost:1002/Server");
                PadiDstm.Status();*/
                res = PadiDstm.TxCommit();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }


    }
}
