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
                res = PadiDstm.TxBegin();
                PadInt pi_a = PadiDstm.CreatePadInt(0);
                Console.WriteLine("Created Paint 0");
                pi_a.Write(36);
                Console.WriteLine("Wrote Padint 0");
                Console.ReadLine();
                PadiDstm.TxAbort();
                Console.WriteLine("Aborted");
                //res = PadiDstm.TxCommit();
                Console.ReadLine();
                res = PadiDstm.TxBegin();
                PadiDstm.Status();
                Console.WriteLine("Testing recover server 1");
               // res = PadiDstm.Fail("tcp://localhost:1001/Server");
                PadiDstm.Status();
                Console.ReadLine();
                pi_a = PadiDstm.AccessPadInt(0);
                if (pi_a == null)
                    Console.WriteLine("NULL");
                Console.WriteLine("a = " + pi_a.Read());
                Console.ReadLine();
                res = PadiDstm.Recover("tcp://localhost:1001/Server");
                pi_a = PadiDstm.AccessPadInt(0);
                Console.WriteLine("a = " + pi_a.Read());

                //PadInt pi_d = PadiDstm.CreatePadInt(3);
                //pi_d.Write(55);
                //Console.WriteLine("d = " + pi_d.Read());

                //PadInt pi_c = PadiDstm.CreatePadInt(2);
                //pi_c.Write(50);
                //Console.WriteLine("c = " + pi_c.Read());


                Console.ReadLine();
            //    res = PadiDstm.TxCommit();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }

            Console.ReadLine();
        }


    }
}
