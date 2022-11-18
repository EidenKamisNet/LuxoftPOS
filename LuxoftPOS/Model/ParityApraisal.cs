using LuxoftPOS.Model.CashOptions;
using LuxoftPOS.Model.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace LuxoftPOS.Model
{
    public class ParityApraisal:ICommit,IPrototype<ParityApraisal>
    {
        private IDBContext dbcontext;
        public ParityApraisal() { }
        public ParityApraisal(IDBContext dbcontext) {
            this.dbcontext = dbcontext;
            this.Country = dbcontext.GetCountry();
            POSId = ((POS)dbcontext).Id;
        }
        public ParityApraisal(IDBContext dbcontext, int id, CountryOption country, Parity baseParity, Parity convertionParity, DateTime apraisalDate) : this(dbcontext)
        {
            Id = id;
            Country = country;
            BaseParity = baseParity;
            ConvertionParity = convertionParity;
            ApraisalDate = apraisalDate;
        }
        [Key]
        public int Id { get; set; }
        public int POSId { get; set; }
        public CountryOption Country { get; set; }
        
        public Parity BaseParity { get; set; }
        public Parity ConvertionParity { get; set; }
        public DateTime ApraisalDate { get; set; }

        public void Commit()
        {
            if (dbcontext.GetDbContext().ParityApraisals.Where(x => x.POSId == POSId && x.ApraisalDate == this.ApraisalDate && x.BaseParity == this.BaseParity && x.ConvertionParity == this.ConvertionParity).FirstOrDefault() == null)
            {                
                BaseParity.DefaultLookup(x => x.Country == BaseParity.Country && x.Currency == BaseParity.Currency && x.ParityDate == BaseParity.ParityDate);
                ConvertionParity.DefaultLookup(x => x.Country == ConvertionParity.Country && x.Currency == ConvertionParity.Currency && x.ParityDate == ConvertionParity.ParityDate);
                dbcontext.GetDbContext().Entry<Parity>(BaseParity).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                dbcontext.GetDbContext().Entry<Parity>(ConvertionParity).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                dbcontext.GetDbContext().ParityApraisals.Add(this);
                dbcontext.GetDbContext().SaveChanges();
            }
            
        }
        public void DefaultLookup(Func<ParityApraisal, bool> predicate)
        {
            var result = dbcontext.GetDbContext().ParityApraisals.Where(predicate).FirstOrDefault();
            if (result != null)
                this.Id = result.Id;
            dbcontext.GetDbContext().Entry<ParityApraisal>(result).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            dbcontext.GetDbContext().Entry<ParityApraisal>(this).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
        }

        public ParityApraisal DeepCopy()
        {
            return new ParityApraisal(dbcontext, Id, Country, BaseParity, ConvertionParity, ApraisalDate);
        }
    }
}
