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
        static private int id;
        private ServerHashTable dataServersTable = new ServerHashTable();
        static private Hashtable padIntStorage = new Hashtable();
        public enum State { Working, Failed, Frozen };
        private State state = State.Working;

        static void Main(string[] args)
        {
            //Console.WriteLine(args);
            if (args.Length > 0)
            {
                port = Convert.ToInt32(args[0]);
            }
            else 
            {
                Console.WriteLine("Port Not Given, write port manually");
                port = Convert.ToInt32(Console.ReadLine());
            }
            launchServer(port);
            getMasterServer();
            registerDataServer();
            Console.ReadLine();

        }

        static void launchServer(int port)
        {
            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(DataServer),
                "Server",
                WellKnownObjectMode.Singleton);

            Console.WriteLine("data server launched on port " + port);

        }

        static void getMasterServer()
        {
            masterServer = (IMaster)Activator.GetObject(
                typeof(IMaster),
                "tcp://localhost:1000/MasterServer");
            Console.WriteLine("got Master Server");
        }

        static void registerDataServer()
        {
            Console.WriteLine("Registering on Master");
            id = masterServer.addDataServer(Utils.getDataServerUrl(port));
            Console.WriteLine("Registered on Master Server with id " + id);
        }

        public void receiveDataServersTable(ServerHashTable dataServers)
        {
            dataServersTable = dataServers;
        }

        public PadInt CreatePadInt(int uid)
        {
            if( padIntStorage.ContainsKey(uid))
            {
                return null;
            }

            PadInt pad = new PadInt(uid);
            padIntStorage.Add(uid, pad);
            return pad;

        }
        public PadInt AccessPadInt(int uid)
        {
            if (!(padIntStorage.ContainsKey(uid)))
            {
                return null;
            }

            return (PadInt) padIntStorage[uid];

        }

        public State getStatus() {
            return state;
        }

        public void setStatus(State state) {
             this.state = state;
        }

        public int getId() {
            return id;
        }       
    }
}
