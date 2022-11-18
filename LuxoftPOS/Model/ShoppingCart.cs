using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuxoftPOS.Model.CashOptions;
using LuxoftPOS.Model.Contracts;
namespace LuxoftPOS.Model
{
    public class ShoppingCart:ICommit, IPrototype<ShoppingCart>
    {
        private IDBContext dbcontext;
        public ShoppingCart() { }
        public ShoppingCart(IDBContext dbcontext)
        {
            this.dbcontext = dbcontext;
            this.POSId = ((POS)dbcontext).Id;
            this.ShoppingCartDate = DateTime.UtcNow;
        }
        public ShoppingCart(IDBContext dbcontext, int id, int pOSId, DateTime shoppingCartDate, int totalProducts, HashSet<ShoppingCartDetail> shoppingCartDetails, HashSet<Product> products, Payment payment) : this(dbcontext)
        {
            Id = id;
            POSId = pOSId;
            ShoppingCartDate = shoppingCartDate;
            TotalProducts = totalProducts;
            ShoppingCartDetails = shoppingCartDetails;
            _products = products;
            this.payment = payment;
        }

        public int Id { get; set; }
        public int POSId { get; set; }
        public DateTime ShoppingCartDate { get; set; }
        //public double BaseCurrencyTotalAmount { get; set; }
        //public double ConvertionCurrencyTotalAmout { get; set; }
        public int TotalProducts { get; set; }
        public HashSet<ShoppingCartDetail> ShoppingCartDetails { get; set; }
      //  public Payment Payment { get; set; }
       private HashSet<Product> _products = new HashSet<Product>();
        private Payment payment;
        public void AddProductToCart(Product product,int quantity)
        {            
            if(this.ShoppingCartDetails==null)
                this.ShoppingCartDetails = new HashSet<ShoppingCartDetail>();
             ShoppingCartDetail shoppingCartDetail = new ShoppingCartDetail(dbcontext) { 
             Product = product,
             Quantity = quantity,
            };
            _products.Add(product);
            TotalProducts += quantity;
            ShoppingCartDetails.Add(shoppingCartDetail);           
        }
        /// <summary>
        /// this methos must be executed after the shopping cart sale is close/committed
        /// </summary>
        /// <param name="paymentCurrency"></param>
        /// <param name="totalPayment"></param>
        /// <param name="LstPaymentDenominations"></param>
        /// <exception cref="Exception"></exception>
        public void AddPayment(CurrencyType paymentCurrency,double totalPayment,HashSet<PaymentCurrencyDenominations> LstPaymentDenominations) {
            if (this.Id == 0)
                throw new Exception("ShoppingCart Id mustn't be empty");
             Payment Payment = new Payment(dbcontext)
            {
                  PaymentCurrency = paymentCurrency,
                   TotalPayment = totalPayment,
                    PaymentCurrencyDenominations = LstPaymentDenominations,
                     ShoppingCartId = this.Id
             };
            Payment.Commit();
            payment = Payment;
        }
        public Payment GetPayment() => payment;

        public double GetCurrentTotal() {
            double total = 0;
            foreach (var cartDetail in ShoppingCartDetails) {
               var lastprice = cartDetail.Product.Prices.OrderBy(x=>x.Id).LastOrDefault();
                total +=lastprice.BasePriceValue * cartDetail.Quantity;
            }
            return total;
        }
        public void Commit()
        {
            foreach (var product in _products)
            {
                var stock= dbcontext.GetDbContext().Stocks.OrderBy(x=>x.Id).LastOrDefault(x => x.POSId == this.POSId && x.ProductId == product.Id);
                var detail =ShoppingCartDetails.FirstOrDefault(x =>  x.POSId == this.POSId && x.Product.Id == product.Id); //shoppingcartId is Imply
                stock.InStock-= detail.Quantity;
                stock.Selled += detail.Quantity;
                dbcontext.GetDbContext().Stocks.Update(stock);
            }
            dbcontext.GetDbContext().ShoppingCarts.Add(this);
            dbcontext.GetDbContext().SaveChanges();
        }

        public ShoppingCart DeepCopy()
        {
            return new ShoppingCart(dbcontext, Id, POSId, ShoppingCartDate, TotalProducts, ShoppingCartDetails, _products, payment);
        }
    }
}
