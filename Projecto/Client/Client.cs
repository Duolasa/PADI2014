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
                pi_a.Write(36);
                Console.ReadLine();
                res = PadiDstm.TxCommit();

                res = PadiDstm.Fail("tcp://localhost:1001/Server");

                Console.WriteLine("Next TX...");
                Console.ReadLine();
                res = PadiDstm.TxBegin();
                PadiDstm.Status();;
                pi_a = PadiDstm.AccessPadInt(0);
                Console.WriteLine("a = " + pi_a.Read());
                Console.ReadLine();
                res = PadiDstm.TxCommit();

                res = PadiDstm.Fail("tcp://localhost:1002/Server");

                Console.WriteLine("Next TX...");
                Console.ReadLine();
                res = PadiDstm.TxBegin();
                PadiDstm.Status(); ;
                pi_a = PadiDstm.AccessPadInt(0);
                Console.WriteLine("a = " + pi_a.Read());
                Console.ReadLine();
                res = PadiDstm.TxCommit();

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
