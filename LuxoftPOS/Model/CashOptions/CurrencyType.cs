using LuxoftPOS.Model.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxoftPOS.Model.CashOptions
{
    public class CurrencyType :ICommit,IPrototype<CurrencyType>
    {
        private IDBContext dbcontext;
        public CurrencyType() { }
        public CurrencyType(IDBContext dbcontext) { 
            this.dbcontext = dbcontext;
            Country = dbcontext.GetCountry();
            POSId = ((POS)dbcontext).Id;
        }
        public CurrencyType(IDBContext dbcontext, int id, string currencyName, CountryOption country,  bool isBaseCurrency) : this(dbcontext)
        {
            Id = id;
            CurrencyName = currencyName;
            Country = country;
            IsBaseCurrency = isBaseCurrency;           
        }
        [Key]
        public int Id { get; set; }
        public int POSId { get; set; }
        public CountryOption Country { get; set; }
        public string CurrencyName { get; set; }
        
        
        private bool _IsBaseCurrency = false;
        public bool IsBaseCurrency { get { return _IsBaseCurrency; } set { _IsBaseCurrency = value; dbcontext.SetBaseCurrency(this); } }

        public void DefaultLookup(Func<CurrencyType,bool> predicate)
        {
            var result = dbcontext.GetDbContext().Currencies.Where(predicate).FirstOrDefault();
            if (result != null)            
                this.Id = result.Id;
                var x = dbcontext.GetDbContext().Entry<CurrencyType>(result).State;
                dbcontext.GetDbContext().Entry<CurrencyType>(result).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                dbcontext.GetDbContext().Entry<CurrencyType>(this).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
        }
        public void Commit()
        {
            Func<CurrencyType, bool> predicate = (x => x.CurrencyName == this.CurrencyName && x.POSId == this.POSId && x.IsBaseCurrency == this.IsBaseCurrency);
            if (dbcontext.GetDbContext().Currencies.Where(predicate).FirstOrDefault() == null)
            {
                dbcontext.GetDbContext().Currencies.Add(this);
                dbcontext.GetDbContext().SaveChanges();
            }
          //  else DefaultLookup(predicate);//throw new Exception("Currency already exists");
            dbcontext.GetDbContext().Entry<CurrencyType>(this).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        }

        public CurrencyType DeepCopy()
        {
            return new CurrencyType(dbcontext, Id, CurrencyName, Country, IsBaseCurrency); 
        }


    }
}
