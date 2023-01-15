using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashMasterPOS.Interfaces;
using CashMasterPOS.Models;
using Newtonsoft.Json;

namespace CashMasterPOS.Services
{
    public class DenominationService : IDenomination
    {
        private readonly string _country;

        public DenominationService()
        {
            _country = ConfigurationManager.AppSettings["CurrentDenomination"];
        }

        public DenominationRootModel GetDenominations()
        {
            var filePath = @"Denominations/" + _country + ".json";
            var json = File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<DenominationRootModel>(json);
            return result;
        }
    }
}
