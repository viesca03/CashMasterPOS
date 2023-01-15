using CashMasterPOS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashMasterPOS.Services
{
    public static class GlobalService
    {
        public static ILogger LogService { get; set; }
        public static IDenomination DenominationService { get; set; }
        public static IChangeCalculator ChangeCalculatorService { get; set; }
    }
}
