using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace PADIDSTM
{
    public class Master : MarshalByRefObject, IMaster
    {
        private ServerHashTable dataServers = new ServerHashTable();
        private int dataServeCounter = 0;

        static void Main(string[] args)
        {
            launchMasterServer(1000);
            Console.ReadLine();

        }
        static void launchMasterServer(int port)
        {
            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(Master),
                "MasterServer",
                WellKnownObjectMode.Singleton);

            Console.WriteLine("master server launched on port " + port);
        }

        public ServerHashTable requestHashTable()
        {
            return dataServers;
        }

        private void addDataServer(string url)
        {
            dataServers.addServer(url);           
        }


    }
}