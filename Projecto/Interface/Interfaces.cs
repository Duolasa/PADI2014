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
        int addDataServer(int port);

        int getNewTransactionId();
        long GetTimeStamp();

    }


    public interface IData
    {
        RealPadInt CreatePadInt(int uid);
        RealPadInt AccessPadInt(int uid);
    }

  

}
