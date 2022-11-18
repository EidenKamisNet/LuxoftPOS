using LuxoftPOS.Model.CashOptions;
using LuxoftPOS.Model.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxoftPOS.Model
{
    public class POS:ICommit,IDBContext
    {
        private EFCashMastersDbContext dbcontext = new EFCashMastersDbContext();
        [Key]
        public int Id { get; set; }
        public string POSName { get; set; }
        
        private CountryOption Country { get; set; }
        private CurrencyType POSBaseCurrency;
        public POS() { }
        public POS(CountryOption country) {
            Country = country;
            POSName = Country.ToString();
            DefaultLookup(x =>  x.POSName== POSName);
        }
        public POS(EFCashMastersDbContext dbcontext, int id, CountryOption country, CurrencyType pOSBaseCurrency)
        {
            this.dbcontext = dbcontext;
            Id = id;
            Country = country;
            POSBaseCurrency = pOSBaseCurrency;
        }
        public POS CreateNewPOS() {
            POSName = Country.ToString();
            Commit();
            return this;
        }
        public void DefaultLookup(Func<POS, bool> predicate)
        {
            var result = dbcontext.POS.Where(predicate).FirstOrDefault();
            if (result != null)
            {
                this.Id = result.Id;

                dbcontext.Entry<POS>(result).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                dbcontext.Entry<POS>(this).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
            }
        }

        public int AddProduct(Product product) {
            int id = 0;
            var result = dbcontext.Products.Where(x=>x.Name==product.Name).FirstOrDefault();
            if (result.Equals(null))
            {
                dbcontext.Products.Add(product);
                id = dbcontext.SaveChanges();
            }
            else { id = result.Id; }
            return id;
        }
        
        
        public EFCashMastersDbContext GetDbContext() => dbcontext;
        public void SetExchangeTypes(string Exchanges,CurrencyType ExchangeCurrency)
        {
            foreach (string Value in Exchanges.Split(new char[] { ',' }))
            {
                var Tuple = Value.Split(new char[] { '|' });
                ExchangeType exchangeType = new ExchangeType(this)
                {
                    Denomination = (Denomination)Enum.Parse(typeof(Denomination), Tuple[1]),
                    Currency = ExchangeCurrency,
                    Value = double.Parse(Tuple[0])
                };
                exchangeType.Commit();
            }
        }
        public CountryOption GetCountry()
        {
            return Country;
        }

        public void SetBaseCurrency(CurrencyType currency)
        {
            POSBaseCurrency = currency;
        }

        public CurrencyType GetBaseCurrency( )
        {
            POSBaseCurrency = dbcontext.Currencies.Where(x => x.IsBaseCurrency && x.Country == this.GetCountry()).FirstOrDefault();
            if (POSBaseCurrency == null) { throw new Exception($"Base Currency is required for the country '{this.GetCountry()}' ,please verify the settings were configure"); }
            
            return POSBaseCurrency;
        }

        public Parity GetBaseParity()
        {
            var parity = dbcontext.Parities.Where(x => x.Country == GetCountry() && x.Currency.IsBaseCurrency == true).OrderByDescending(x=>x.ParityDate).FirstOrDefault();
            if (parity == null)
                throw new Exception($"there are not parities captured for the country '{GetCountry()}',please verify the settings were configure");
            return parity;
        }
        public IEnumerable<ExchangeType> GetPOSExchangeTypes(Func<ExchangeType,bool> predicate 
        ) {

            if (this.Id == 0)
                throw new Exception("POS Id Shouldn't be Empty");
            return dbcontext.ExchangeTypes.Where(x => x.POSId == this.Id).Where(predicate);
        }
        public ExchangeType GetPOSExchangeType(Func<ExchangeType, bool> predicate
        )
        {

            if (this.Id == 0)
                throw new Exception("POS Id Shouldn't be Empty");
            return dbcontext.ExchangeTypes.Where(x => x.POSId == this.Id).FirstOrDefault(predicate);
        }

        public void Commit()
        {
            var result = dbcontext.POS.Where(x => x.POSName == POSName).FirstOrDefault();
            if (result == null)
            {
                dbcontext.POS.Add(this);
                dbcontext.SaveChanges();
                
                dbcontext.Entry<POS>(this).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
            }
            else {
                dbcontext.Entry<POS>(result).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                throw new Exception($"POS {POSName} already exists");
            }
            
        }
    }
}
