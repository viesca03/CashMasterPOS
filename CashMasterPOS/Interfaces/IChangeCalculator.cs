using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashMasterPOS.Interfaces
{
    public interface IChangeCalculator
    {
        Dictionary<double, int> CalculateChange(double price, Dictionary<double, int> payment);

        string GetTotalChange(Dictionary<double, int> change, string mainSymbol);
    }
}
