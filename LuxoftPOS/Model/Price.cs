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
    public class Price:ICommit,IPrototype<Price>
    {
        private IDBContext dbcontext;
        public Price() { }
        public Price(IDBContext dbcontext)
        {
            this.dbcontext = dbcontext;
            this.Country = dbcontext.GetCountry();
            POSId = ((POS)dbcontext).Id;
        }
        public Price(IDBContext dbcontext, int id, CountryOption country, DateTime priceDate, double basePraceValue) : this(dbcontext)
        {
            this.Id = id;
            this.Country = country;
            //this.ParityApraisals = parityApraisals;
            this.PriceDate = priceDate;
            this.BasePriceValue = basePraceValue;
        }

        [Key]
        public int Id { get; set; }
        public int POSId { get; set; }
        public CountryOption Country { get; set; }
        
        //public HashSet<ParityApraisal> ParityApraisals { get; set; }
        [Required(ErrorMessage = "Price Date is Required")]
        public DateTime PriceDate { get; set; }
        public double BasePriceValue { get; set; }

        public void Commit()
        {
            if (dbcontext.GetDbContext().Prices.Where(x => x.PriceDate == this.PriceDate && x.Country == dbcontext.GetCountry()).FirstOrDefault() == null)
            {
                dbcontext.GetDbContext().Prices.Add(this);
            }
            dbcontext.GetDbContext().SaveChanges();
        }

        public Price DeepCopy()
        {
            return new Price(dbcontext, Id, Country, PriceDate, BasePriceValue);
        }
    }
}
