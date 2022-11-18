using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuxoftPOS.Model.CashOptions;
using LuxoftPOS.Model.Contracts;
namespace LuxoftPOS.Model
{
    public  class PaymentCurrencyDenominations: IPrototype<PaymentCurrencyDenominations>
    {
        private IDBContext dbcontext;
        public PaymentCurrencyDenominations() { }
        public PaymentCurrencyDenominations(IDBContext dbcontext)
        {
            this.dbcontext = dbcontext;
           // this.Country = dbcontext.GetCountry();
          //  POSId = ((POS)dbcontext).Id;
        }
        public PaymentCurrencyDenominations(IDBContext dbcontext, int id, int paymentId, ExchangeType exchangeType, int quantity, PaymentType paymentType) : this(dbcontext)
        {
            Id = id;
            PaymentId = paymentId;
            ExchangeType = exchangeType;
            Quantity = quantity;
            PaymentType = paymentType;
        }

        [Key]
        public int Id { get; set; }
        [ForeignKey("PaymentId")]
        public int PaymentId { get; set; }
        public ExchangeType ExchangeType { get; set; }
        public int Quantity { get; set; }
        public PaymentType PaymentType { get; set; }

        public PaymentCurrencyDenominations DeepCopy()
        {
            return new PaymentCurrencyDenominations(dbcontext,Id,PaymentId,ExchangeType,Quantity,PaymentType);
        }
    }
}
