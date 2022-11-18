using LuxoftPOS.Model.CashOptions;
using LuxoftPOS.Model.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxoftPOS.Model
{
    public class Parity:ICommit,IPrototype<Parity>
    {
        private IDBContext dbcontext;
        public Parity() { }
        public Parity(IDBContext dbcontext)
        { 
            this.dbcontext = dbcontext;
            this.Country = dbcontext.GetCountry();
            POSId = ((POS)dbcontext).Id;
        }
        public Parity(IDBContext dbcontext, int id, CountryOption country, CurrencyType currency, double value, DateTime parityDate) : this(dbcontext)
        {
            Id = id;
            Country = country;
            Currency = currency;
            Value = value;
            ParityDate = parityDate;

        }

        [Key]
        public int Id { get; set; }
        public int POSId { get; set; }
        public CountryOption Country { get; set; }
        public CurrencyType Currency { get; set; }
        
        public double Value { get; set; }
        [Required(ErrorMessage ="Parity Date Must be added")]
        public DateTime ParityDate { get; set; }
  


        public void Commit()
        {
            Func<Parity, bool> predicate = x => x.POSId == this.POSId && x.ParityDate == this.ParityDate && x.Currency == this.Currency;
            if (this.dbcontext.GetDbContext().Parities.Where(predicate).FirstOrDefault() == null)
            {
                //create new record and if the currency have been tracked before, we verify that currency its not going to change or trying to be added.
                var x =dbcontext.GetDbContext().Entry<CurrencyType>(this.Currency).State;                
                Currency.DefaultLookup(x => x.CurrencyName == Currency.CurrencyName && x.Country == Currency.Country && x.IsBaseCurrency == Currency.IsBaseCurrency);
                dbcontext.GetDbContext().Entry<CurrencyType>(Currency).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                this.dbcontext.GetDbContext().Parities.Add(this);
            }

            //Parity.Value can be changed
            this.dbcontext.GetDbContext().SaveChanges();
        }
        public void DefaultLookup(Func<Parity, bool> predicate)
        {
            var result = dbcontext.GetDbContext().Parities.Where(predicate).FirstOrDefault();
            if (result != null)
                this.Id = result.Id;
            dbcontext.GetDbContext().Entry<Parity>(result).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            dbcontext.GetDbContext().Entry<Parity>(this).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
        }  

        public Parity DeepCopy()
        {
            return new Parity(dbcontext, Id, Country, Currency, Value, ParityDate); 
        }


    }
}
