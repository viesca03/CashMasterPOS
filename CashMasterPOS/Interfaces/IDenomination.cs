using CashMasterPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashMasterPOS.Interfaces
{
    public interface IDenomination
    {
        DenominationRootModel GetDenominations();
    }
}
