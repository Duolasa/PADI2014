using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADIDSTM
{
  [Serializable]
  public class PadIntSafeCopy
  {
    private int owner;
    private Hashtable padIntStorage = new Hashtable();


    public int Owner
    {
      get { return owner; }
    }

    public Hashtable PadIntStorage
    {
      get { return padIntStorage; }
    }

    public PadIntSafeCopy(int serverId)
    {
      owner = serverId;
    }

  }
}
