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
    public class Stock: IPrototype<Stock>
    {
        private IDBContext dbcontext;
        public Stock() { }
        public Stock(IDBContext dbcontext) {
            this.dbcontext = dbcontext;
            this.POSId = ((POS)dbcontext).Id;
        }
        public Stock(IDBContext dbcontext, int id, int productId, CountryOption country, int inStock, DateTime stockDate) : this(dbcontext)
        {
            Id = id;
            //ProductId = productId;
            Country = country;
            InStock = inStock;
            StockDate = stockDate;
        }
        [Key]
        public int Id { get; set; }

        
        public CountryOption Country { get; set; }
        public int POSId { get; set; }
        public int InStock { get; set; }
        public int Selled { get; set; }
        public DateTime StockDate { get; set; }
        public int ProductId { get; set; }

        public Stock DeepCopy()
        {
            return new Stock(dbcontext,Id,ProductId,Country,InStock,StockDate);
        }
    }
}
