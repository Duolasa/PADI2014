using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PADIDSTM
{
  public class PadInt : MarshalByRefObject
  {
    private bool waitingForWrite;
    private int newValue;
    private int value;
    private int id;

    public bool WaitingForWrite
    {
      get { return waitingForWrite; }
    }

    public PadInt(int uid)
    {
      waitingForWrite = false;
      newValue = 0;
      id = uid;
      value = 0;
      lockPadInt();
    }

    public void Write(int val)
    {
      try
      {
        lockPadInt();
        this.newValue = val;
        this.waitingForWrite = true;
      }
      catch (Exception e)
      {
        throw e;
      }

    }

    public void writeCommit()
    {
      this.value = this.newValue;
      this.waitingForWrite = false;
    }

    public int Read()
    {
      try
      {
        lockPadInt();
        if (this.waitingForWrite)
        {
          return this.newValue;
        }
        else
        {
          return this.value;
        }
      }
      catch (Exception e)
      {
        throw e;
      }

    }


    private void lockPadInt()
    {
      if(Monitor.IsEntered(this)) return;

      bool sucess = false;
      Monitor.TryEnter(this, Utils.TIME_TO_WAIT_FOR_TRANSACTION,  ref sucess);
      if (!sucess)
      {
        throw new Exception("Unsuccessfully tried to get lock for this object for " + Utils.TIME_TO_WAIT_FOR_TRANSACTION);
      }
    }

    public void unlockPadInt()
    {
      Monitor.Exit(this);
      Monitor.Pulse(this);
    }

  }
}
