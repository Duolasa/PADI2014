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
        static IMaster masterServer;
        static ServerHashTable dataServersPorts;
        
        static void Main(string[] args)
        {
            Console.WriteLine("Client port?");
            int port = Convert.ToInt32(Console.ReadLine());
            launchClient(port);
            requestHash();
            createPadInt();
            Console.ReadLine();
        }

        static void launchClient(int port)
        {
            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, true);

            masterServer = (IMaster)Activator.GetObject(
                typeof(IMaster),
                "tcp://localhost:1000/MasterServer");

        }

        static void requestHash()
        {
            dataServersPorts = masterServer.requestHashTable();
        }

        static void createPadInt()
        {
            Random rnd = new Random();
            int num = rnd.Next(0,666);
            string url = dataServersPorts.getServerByPadiIntID(num);
            IData dataServer = (IData)Activator.GetObject(
                typeof(IData),url);
            PadInt p = dataServer.createPadInt(2);
            p.write(9999);

            PadInt j = dataServer.acessPadInt(2);

            Console.WriteLine(j.read());
            
        }

    }

}
