using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuxoftPOS.Model.CashOptions;
using LuxoftPOS.Model.Contracts;
namespace LuxoftPOS.Model.CashOptions
{
    public class ExchangeType:ICommit,IPrototype<ExchangeType>
    {
        private IDBContext dbcontext;
        public ExchangeType() { }
        public ExchangeType(IDBContext dbcontext) {
            this.dbcontext = dbcontext;
            this.POSId = ((POS)dbcontext).Id;
            this.Country = dbcontext.GetCountry();
        }
        public ExchangeType(IDBContext dbcontext, int id, int pOSId, CountryOption country, CurrencyType currency, double value, Denomination denomination) : this(dbcontext)
        {
            Id = id;
            POSId = pOSId;
            Country = country;
            Currency = currency;
            Value = value;
            Denomination = denomination;
        }

        [Key]
        public int Id { get; set; }

        public int POSId { get; set; }
        public CountryOption Country { get; set; }
        public CurrencyType Currency { get; set; }
        public double Value { get; set; }
        public Denomination Denomination { get; set; }

  
        public void Commit()
        {
            if (dbcontext.GetDbContext().ExchangeTypes.Where(x => x.POSId == POSId && x.Value == Value && x.Denomination == Denomination && x.Currency == Currency).FirstOrDefault() == null)
            {
                dbcontext.GetDbContext().ExchangeTypes.Add(this);
                dbcontext.GetDbContext().SaveChanges();
                dbcontext.GetDbContext().Entry<ExchangeType>(this).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
        }

        public ExchangeType DeepCopy()
        {
            return new ExchangeType( dbcontext,Id,POSId,Country,Currency,Value,Denomination);
        }
    }
}
