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

        void iAmAlive(int uid);

    }


    public interface IData
    {
        RealPadInt CreatePadInt(int uid);
        RealPadInt AccessPadInt(int uid);
        void DeletePadInt(int uid);

        void CreatePadIntSafeCopy(RealPadInt p);
        void WritePadIntSafeCopy(RealPadInt p);
        void DeletePadIntSafeCopy(RealPadInt p);




    }

  

}
