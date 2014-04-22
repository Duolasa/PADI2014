using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;

namespace PADIDSTM
{

    public class PadInt : MarshalByRefObject
    {
        private int value;
        private int id;

        public PadInt(int uid)
        {
            id = uid;
            value = 0;
        }

        public void write(int val)
        {
            this.value = val;
        }

        public int read()
        {
            return value;
        }


        public void lockPadInt()
        {
            Monitor.Enter(this);
        }

        public void unlockPadInt()
        {
            Monitor.Exit(this);
            Monitor.Pulse(this);
        }

    }

    public interface IMaster
    {
        ServerHashTable requestHashTable();
        void addDataServer(string url);

    }


    public interface IData
    {
        PadInt createPadInt(int uid);
        PadInt acessPadInt(int uid);
    }

    public interface IApp
    {
        bool init();
        bool txBegin();
        bool txCommit();
        bool txAbort();
        bool status();
        bool fail(string url);
        bool freeze(string url);
        bool recover(string url);
    }

}
