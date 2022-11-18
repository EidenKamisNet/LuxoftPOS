using LuxoftPOS.Model.CashOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxoftPOS.Model.Contracts
{
    public interface IDBContext
    {
        EFCashMastersDbContext GetDbContext();
        CountryOption GetCountry();
        void SetBaseCurrency(CurrencyType currency);
        CurrencyType GetBaseCurrency();
        Parity GetBaseParity();
        void SetExchangeTypes(string Exchanges, CurrencyType ExchangeCurrency);
        IEnumerable<ExchangeType> GetPOSExchangeTypes(Func<ExchangeType, bool> predicate);
        ExchangeType GetPOSExchangeType(Func<ExchangeType, bool> predicate);
    }
}
