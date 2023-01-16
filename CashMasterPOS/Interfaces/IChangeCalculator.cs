using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashMasterPOS.Interfaces
{
    public interface IChangeCalculator
    {
        List<decimal> CalculateChange(decimal price, Dictionary<decimal, int> payment);

        string GetTotalChange(List<decimal> change, string mainSymbol);
    }
}
