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
    public class DataServer : MarshalByRefObject, IData
    {

        static IMaster masterServer;
        static private int port;
        static private string url;
        static int staticPort;
        static private Hashtable padIntStorage = new Hashtable();
        static void Main(string[] args)
        {
            Console.WriteLine("Data server port?");
            staticPort = Convert.ToInt32(Console.ReadLine());
            launchServer(staticPort);
            getMasterServer();
            registerDataServer();
            Console.ReadLine();

        }

        static void launchServer(int url)
        {
            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(DataServer),
                "Server",
                WellKnownObjectMode.Singleton);

            Console.WriteLine("data server launched on port " + url);

        }

        static void getMasterServer()
        {
            masterServer = (IMaster)Activator.GetObject(
                typeof(IMaster),
                "tcp://localhost:1000/MasterServer");
        }

        static void registerDataServer()
        {
            masterServer.addDataServer(port);
        }

        public PadInt createPadInt(int uid)
        {
            if( padIntStorage.ContainsKey(uid))
            {
                return null;
            }

            PadInt pad = new PadInt(uid);
            padIntStorage.Add(uid, pad);
            return pad;

        }
        public PadInt acessPadInt(int uid)
        {
            if (!(padIntStorage.ContainsKey(uid)))
            {
                return null;
            }

            return (PadInt) padIntStorage[uid];

        }

       
    }
}
