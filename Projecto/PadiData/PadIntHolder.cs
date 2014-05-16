﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADIDSTM {
    public class PadIntHolder : PadInt {

        private bool waitingForWrite = false;
        private int txID;
        private RealPadInt realPadInt;

        public RealPadInt RealPadInt {
            get { return realPadInt; }
        }

        public bool WaitingForWrite {
            get { return waitingForWrite; }
        }

        public PadIntHolder(int txID, RealPadInt realPadi) {
            this.txID = txID;
            this.realPadInt = realPadi;
        }

        public int Read() {
            try {
                return realPadInt.Read(txID);
            } catch (Exception e) {
                throw new TxException("Read!!", e);
            }


        }

        public void Write(int value) {
            try {
                realPadInt.Write(value, txID);
                waitingForWrite = true;
            } catch (Exception e) {
                throw new TxException("Write", e);
            }
        }
    }
}
