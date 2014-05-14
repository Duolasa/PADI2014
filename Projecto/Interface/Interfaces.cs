using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;

namespace PADIDSTM
{

    public interface IMaster
    {
        ServerHashTable requestHashTable();
        int addDataServer(string url);

        int getNewTransactionId();
        long GetTimeStamp();

    }


    public interface IData
    {
        PadInt CreatePadInt(int uid);
        PadInt AccessPadInt(int uid);
    }

  

}
