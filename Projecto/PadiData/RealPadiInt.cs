using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PADIDSTM
{
  public class RealPadInt : MarshalByRefObject, PadInt
  {
    private int value;
    private int newValue;
    private int id;
    private int writeTXID = 0;
    private HashSet<int> readingTXID = new HashSet<int>();
    private bool beingWrited = false;
    private int numReaders = 0;


    public RealPadInt(int uid)
    {
      id = uid;
      value = 0;
    }

    public int ID
    {
      get { return id; }
    }

    public void Write(int val, int txID)
    {
      try
      {
          lock (this) {
              if (txID == this.writeTXID || (readingTXID.Count == 1 && readingTXID.Contains(txID))) {
                  newValue = val;
                  this.writeTXID = txID;
                  readingTXID.Add(txID);
              } else {
                  while (beingWrited && readingTXID.Count > 0)
                      Monitor.Wait(this);
                  beingWrited = true;
                  this.writeTXID = txID;
                  newValue = val;
                  readingTXID.Add(txID);
              }
          }
      }
      catch (Exception e)
      {
        throw e;
      }

    }

    public int writeCommit()
    {
      this.value = this.newValue;
      return this.value;
    }

    public int Read(int txID) {
        try {
            lock (this) {
                if (txID == this.writeTXID) {
                    readingTXID.Add(txID);
                    return newValue;
                }
                while (beingWrited)
                    Monitor.Wait(this);
                readingTXID.Add(txID);
                return value;
            }
        } catch (Exception e) {
            throw e;
        }

    }

    public void removeMeFromReadersList(int txID) {
        lock (this) {
            readingTXID.Remove(txID);
        }
    }

      //never called

    public int Read() { return 0; }

    public void Write(int value) { }

    public void unlockPadInt()
    {
        lock (this) {
            beingWrited = false;
            Monitor.PulseAll(this);
        }
    }

    public void DirectWrite(int v)
    {
      value = v;
    }

  }
}
