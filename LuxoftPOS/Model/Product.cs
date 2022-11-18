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
    public class Product:ICommit, IPrototype<Product>
    {
        private IDBContext dbcontext;
        //private CountryOption countryOption;
        public Product() { }
        public Product(IDBContext dbcontext)
        {
            this.dbcontext = dbcontext;
             this.Country = dbcontext.GetCountry();
            POSId = ((POS)dbcontext).Id;
        }
        public Product(IDBContext dbcontext, int id, string name, string description, DateTime productDate, HashSet<Price> prices) : this(dbcontext)
        {
            Id = id;
            Name = name;
            Description = description;
            ProductDate = productDate;
            Prices = prices;
        }
        [Key]
        public int Id { get; set; }
        [Required( ErrorMessage ="Name is required for the Product")]
        public int POSId { get; set; }
        public CountryOption Country { get; set; }
        public string Name { get; set; }
        
        
        public string Description { get; set; }
        public DateTime ProductDate { get; set; }
        public HashSet<Price> Prices { get; set; }
        public HashSet<Stock> Stocks { get; set; }
        public void AddNewPrice(double BasePriceValue,DateTime PriceDate)
        {
           var result =  dbcontext.GetDbContext().Prices.Where(x => x.Country== this.dbcontext.GetCountry() && x.PriceDate == PriceDate && x.BasePriceValue == BasePriceValue).FirstOrDefault();
            if (result == null) { 
                if(Prices==null)
                    Prices = new HashSet<Price>();
                Price price = new Price(dbcontext) { 
                     BasePriceValue = BasePriceValue,
                      PriceDate = PriceDate                       
                };
                Prices.Add(price);
                //price.ParityApraisals= dbcontext.GetDbContext().ParityApraisals.Where(x=>x.Country==dbcontext.GetCountry() && x.BaseParity== dbcontext.GetBaseParity()).ToHashSet();
            }           
        }
        public void AddStocks(int totalStocks)
        {
            //var result = dbcontext.GetDbContext().Stocks.Where(x => x.ProductId == Id && x.StockDate == DateTime.UtcNow && x.Country == dbcontext.GetCountry()).FirstOrDefault();
            //if (result == null)
            //{
                if (Stocks == null)
                    Stocks = new HashSet<Stock>();
                Stock stock = new Stock(dbcontext)
                {
                    Country = dbcontext.GetCountry(),
                    StockDate = DateTime.UtcNow,
                    InStock = totalStocks,
                     
                };
                var result = dbcontext.GetDbContext().Stocks.OrderBy(x=>x.Id).LastOrDefault();
            if (result != null)
            {
                stock.InStock += result.InStock;
            }
                Stocks.Add(stock);              
        }
        public void Commit()
        {
            if (dbcontext.GetDbContext().Products.Where(x => x.Name == Name && x.ProductDate == ProductDate && x.Country == dbcontext.GetCountry()).FirstOrDefault() == null)
            {

                dbcontext.GetDbContext().Products.Add(this);
            }
            else {
                dbcontext.GetDbContext().Products.Update(this);
            }
            dbcontext.GetDbContext().SaveChanges();
        }

        public Product DeepCopy()
        {
            return new Product(dbcontext, Id, Name, Description, ProductDate, Prices);
        }

        public void DefaultLookup(Func<Product, bool> predicate)
        {
            var result = dbcontext.GetDbContext().Products.Where(predicate).FirstOrDefault();
            if (result != null)
                this.Id = result.Id;
            dbcontext.GetDbContext().Entry<Product>(result).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            dbcontext.GetDbContext().Entry<Product>(this).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
        }
    }
}
