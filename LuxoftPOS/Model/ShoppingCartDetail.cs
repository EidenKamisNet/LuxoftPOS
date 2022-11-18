using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuxoftPOS.Model.CashOptions;
using LuxoftPOS.Model.Contracts;

namespace LuxoftPOS.Model
{
    public class ShoppingCartDetail: IPrototype<ShoppingCartDetail>
    {
        private IDBContext dbcontext;
        public ShoppingCartDetail() { }
        public ShoppingCartDetail(IDBContext dbcontext) {
            this.dbcontext = dbcontext;
            this.POSId = ((POS)dbcontext).Id;
            this.ShoppingCartDetailDate = DateTime.UtcNow;
            this.Country = dbcontext.GetCountry();
        }
        public ShoppingCartDetail(IDBContext dbcontext, int id, int pOSId, CountryOption country, DateTime shoppingCartDetailDate, Product product, int quantity) : this(dbcontext)
        {
            Id = id;
            POSId = pOSId;
            Country = country;
            ShoppingCartDetailDate = shoppingCartDetailDate;
            Product = product;
            Quantity = quantity;
        }

        [Key]
        public int Id { get; set; }
        public int POSId { get; set; }
        public CountryOption Country { get; set; }
        public DateTime ShoppingCartDetailDate { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public ShoppingCartDetail DeepCopy()
        {
            return new ShoppingCartDetail(dbcontext,Id,POSId,Country,ShoppingCartDetailDate,Product,Quantity);
        }
    }
}
