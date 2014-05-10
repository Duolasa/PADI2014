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
        int addDataServer(string url);

    }


    public interface IData
    {
        PadInt CreatePadInt(int uid);
        PadInt AccessPadInt(int uid);
    }

    public interface IPADIDSTM
    {
        bool Init();
        bool TxBegin();
        bool TxCommit();
        bool TxAbort();
        bool Status();
        bool Fail(string url);
        bool Freeze(string url);
        bool Recover(string url);
    }

}
