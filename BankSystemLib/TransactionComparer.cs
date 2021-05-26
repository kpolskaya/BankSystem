using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystemLib
{
    internal class TransactionComparer : IEqualityComparer<Transaction>
    {
        public bool Equals(Transaction x, Transaction y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.Id == y.Id && x.TimeStamp == y.TimeStamp;
        }

        public int GetHashCode(Transaction t)
        {
            if (Object.ReferenceEquals(t, null)) return 0;

            int hashID = t.Id.GetHashCode();
            int hashTimeStamp = t.TimeStamp.GetHashCode();

            return hashID ^ hashTimeStamp;
        }
    }
}
