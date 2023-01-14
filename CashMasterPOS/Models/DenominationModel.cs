using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashMasterPOS.Models
{
    public class DenominationRootModel
    {
        public string CountryName { get; set; }
        public string MainSymbol { get; set; }
        public List<DenominationModel> Denominations { get; set; }
    }

    public class DenominationModel
    {
        public string Image { get; set; }
        public string Symbol { get; set; }
        public string CurrencyName { get; set; }
        public double Value { get; set; }
    }
}
