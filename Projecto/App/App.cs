using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PADIDSTM
{
    class App
    {
        static void Main(string[] args)
        {

        }

        public static bool init()
        {
            return true;
        }

        public bool txBegin()
        {
            return true;
        }
        public bool txCommit()
        {
            return true;
        }
        public bool txAbort()
        {
            return true;
        }
        public bool status()
        {
            return true;
        }
        public bool fail(string url)
        {
            return true;
        }
        public bool freeze(string url)
        {
            return true;
        }
        public bool recover(string url)
        {
            return true;
        }
    }
}
