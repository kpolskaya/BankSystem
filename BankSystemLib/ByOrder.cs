using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystemLib
{
    public class ByOrder : IComparer<Transaction>
    {
        public int Compare(Transaction x, Transaction y)
        {
            if (x.TimeStamp.CompareTo(y.TimeStamp) != 0)
                return x.TimeStamp.CompareTo(y.TimeStamp);
            else if (x.Id.CompareTo(y.Id) != 0)
                return x.Id.CompareTo(y.Id);
            else return 0;
        }
    }
}
