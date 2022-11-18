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
    public class Payment:ICommit, IPrototype<Payment>
    {
        private IDBContext dbcontext;
        public Payment() { }
        public Payment(IDBContext dbcontext)
        {
            this.dbcontext = dbcontext;
            this.Country = dbcontext.GetCountry();
            POSId = ((POS)dbcontext).Id;
        }
        public Payment(IDBContext dbcontext, int id, int pOSId, CountryOption country, int shoppingCartId, CurrencyType paymentCurrency, double totalPayment, HashSet<PaymentCurrencyDenominations> paymentCurrencyDenominations, double baseCurrencyTotalPayed, double baseCurrencyTotalToPay) : this(dbcontext)
        {
            Id = id;
            POSId = pOSId;
            Country = country;
            ShoppingCartId = shoppingCartId;
            PaymentCurrency = paymentCurrency;
            TotalPayment = totalPayment;
            PaymentCurrencyDenominations = paymentCurrencyDenominations;
            BaseCurrencyTotalPayed = baseCurrencyTotalPayed;
            BaseCurrencyTotalToPay = baseCurrencyTotalToPay;
        }

        [Key]
        public int Id { get; set; }
        public int POSId { get; set; }
        public CountryOption Country { get; set; }
        public int ShoppingCartId { get; set; }
        public CurrencyType PaymentCurrency { get; set; }
        public double TotalPayment { get; set; }
        public HashSet<PaymentCurrencyDenominations> PaymentCurrencyDenominations { get; set; }
        //private HashSet<PaymentCurrencyDenominations> PaybackCurrencyDenominations { get; set; }
        public HashSet<PaymentCurrencyDenominations> GetPaybackCurrencyDenominations() => PaymentCurrencyDenominations.Where(x=>x.PaymentType== PaymentType.Payback).ToHashSet();
        private double BaseCurrencyTotalPayed = 0;
        private double BaseCurrencyTotalToPay = 0;
        public double GetBaseCurrencyTotalPayed() => BaseCurrencyTotalPayed;
        public double GetBaseCurrencyTotalToPay() => BaseCurrencyTotalToPay;
        public double CalculateTotalPayedByTheClient() {
            
            
            var parityApraisal = dbcontext.GetDbContext().ParityApraisals.Where(x => x.BaseParity == dbcontext.GetBaseParity() && x.ConvertionParity.Currency == PaymentCurrency).OrderBy(x => x.Id).LastOrDefault();
            foreach (var DenominationPayed in PaymentCurrencyDenominations.Where(x => x.PaymentType == PaymentType.Payment))
            {
                if (dbcontext.GetBaseCurrency() == this.PaymentCurrency)
                {
                    BaseCurrencyTotalPayed += DenominationPayed.ExchangeType.Value * DenominationPayed.Quantity; //Exchange Value * # of Unities of an specific Exchange Type
                }
                else
                {
                    //cast payment to base currency                    
                    if (parityApraisal != null)
                    {
                        if (parityApraisal.BaseParity.Value < parityApraisal.ConvertionParity.Value)
                            BaseCurrencyTotalPayed += (DenominationPayed.ExchangeType.Value * DenominationPayed.Quantity) * parityApraisal.ConvertionParity.Value;
                        else
                            BaseCurrencyTotalPayed += (DenominationPayed.ExchangeType.Value * DenominationPayed.Quantity) / parityApraisal.ConvertionParity.Value;
                    }
                    else throw new Exception("Parity Apraisal doesn't exists and is required to calculate Payback for client");
                }
            }
            if (dbcontext.GetBaseCurrency() == this.PaymentCurrency)
            {
                BaseCurrencyTotalToPay = TotalPayment;
            }
            else
            {
                if (parityApraisal != null)
                {
                    if (parityApraisal.BaseParity.Value < parityApraisal.ConvertionParity.Value)
                        BaseCurrencyTotalToPay = (TotalPayment * parityApraisal.ConvertionParity.Value);
                    else
                    {
                        BaseCurrencyTotalToPay = (TotalPayment / parityApraisal.ConvertionParity.Value);
                    }
                }
                else
                    throw new Exception("Parity Apraisal doesn't exists and is required to calculate Payback for client");
            }
            return BaseCurrencyTotalPayed;
        }
        public double GetTotalPayback()
        { 
            double totalPaybackAmount = 0;
            var parityApraisal = dbcontext.GetDbContext().ParityApraisals.Where(x => x.BaseParity == dbcontext.GetBaseParity() && x.ConvertionParity.Currency == PaymentCurrency).OrderBy(x => x.Id).LastOrDefault();
            foreach (var DenominationPayback in GetPaybackCurrencyDenominations())
            {
                if (dbcontext.GetBaseCurrency() == this.PaymentCurrency)
                {
                    totalPaybackAmount += DenominationPayback.ExchangeType.Value * DenominationPayback.Quantity; //Exchange Value * # of Unities of an specific Exchange Type
                }
                else
                {
                    //cast payment to base currency                    
                    if (parityApraisal != null)
                    {
                        if (parityApraisal.BaseParity.Value < parityApraisal.ConvertionParity.Value)
                            totalPaybackAmount += (DenominationPayback.ExchangeType.Value * DenominationPayback.Quantity) * parityApraisal.ConvertionParity.Value;
                        else
                            totalPaybackAmount += (DenominationPayback.ExchangeType.Value * DenominationPayback.Quantity) / parityApraisal.ConvertionParity.Value;
                    }
                    else throw new Exception("Parity Apraisal doesn't exists and is required to calculate Payback for client");
                }
            }
            return totalPaybackAmount;
        }
        public void Commit()
        {
            double BaseCurrencyTotalToPayback = 0;
            CalculateTotalPayedByTheClient();
            /*double BaseCurrencyTotalPayed = 0;            
            double BaseCurrencyTotalToPay = 0;
            var parityApraisal = dbcontext.GetDbContext().ParityApraisals.Where(x => x.BaseParity == dbcontext.GetBaseParity() && x.ConvertionParity.Currency == PaymentCurrency).OrderBy(x => x.Id).LastOrDefault();
            foreach (var DenominationPayed in PaymentCurrencyDenominations.Where(x=>x.PaymentType == PaymentType.Payment)) {
                if (dbcontext.GetBaseCurrency() == this.PaymentCurrency)
                {
                    BaseCurrencyTotalPayed += DenominationPayed.ExchangeType.Value * DenominationPayed.Quantity; //Exchange Value * # of Unities of an specific Exchange Type
                }
                else 
                {                    
                    //cast payment to base currency                    
                    if (parityApraisal != null)
                    {
                        if(parityApraisal.BaseParity.Value < parityApraisal.ConvertionParity.Value)
                        BaseCurrencyTotalPayed += (DenominationPayed.ExchangeType.Value * DenominationPayed.Quantity)* parityApraisal.ConvertionParity.Value;
                        else
                            BaseCurrencyTotalPayed += (DenominationPayed.ExchangeType.Value * DenominationPayed.Quantity) / parityApraisal.ConvertionParity.Value;
                    }
                    else throw new Exception("Parity Apraisal doesn't exists and is required to calculate Payback for client");
                }
            }
            if (dbcontext.GetBaseCurrency() == this.PaymentCurrency)
            {
                BaseCurrencyTotalToPay = TotalPayment;
            }
            else {
                if (parityApraisal != null)
                {
                    if (parityApraisal.BaseParity.Value < parityApraisal.ConvertionParity.Value)
                        BaseCurrencyTotalToPay = (TotalPayment * parityApraisal.ConvertionParity.Value);
                    else {
                        BaseCurrencyTotalToPay = (TotalPayment / parityApraisal.ConvertionParity.Value);
                    }
                }
                else
                     throw new Exception("Parity Apraisal doesn't exists and is required to calculate Payback for client");
            }*/
            if(BaseCurrencyTotalPayed < BaseCurrencyTotalToPay)
                throw new Exception("Total Paid must be equal or greater than the Total to Pay");
            BaseCurrencyTotalToPayback =  BaseCurrencyTotalPayed - BaseCurrencyTotalToPay;
            double totalAmountGetted = 0;
            foreach (ExchangeType PaybackExchange in dbcontext.GetDbContext().ExchangeTypes.Where(x => x.POSId == POSId && x.Currency == dbcontext.GetBaseCurrency()).OrderByDescending(x=>x.Value)) {
                bool isLessThanTotalToPayback = true;
                bool CheckedAtLeastOneTime = false;
                int totalUnities = 1;
                
                while (isLessThanTotalToPayback)
                {
                    var tempCalc = (PaybackExchange.Value * totalUnities);
                    if ((totalAmountGetted + tempCalc) <= BaseCurrencyTotalToPayback)
                    {
                        totalAmountGetted += tempCalc;
                        totalUnities++;
                        CheckedAtLeastOneTime = true;
                    }
                    else {
                        totalUnities = 1;
                        isLessThanTotalToPayback=false;
                    }
                    
                }
                if (CheckedAtLeastOneTime) {
                    PaymentCurrencyDenominations paybackCurrencyDenominations = new PaymentCurrencyDenominations(dbcontext)
                    {
                        PaymentType = PaymentType.Payback,
                        //PaymentId = this.Id
                        Quantity = totalUnities,
                        ExchangeType = PaybackExchange
                    };
                    if (PaymentCurrencyDenominations == null)
                        PaymentCurrencyDenominations = new HashSet<PaymentCurrencyDenominations>();
                    PaymentCurrencyDenominations.Add(paybackCurrencyDenominations);
                    if (totalAmountGetted == BaseCurrencyTotalToPayback)
                        break;
                }
            }
            dbcontext.GetDbContext().Payments.Add(this);
            dbcontext.GetDbContext().SaveChanges();

            //throw new NotImplementedException();
        }

        public Payment DeepCopy()
        {
            return new Payment(dbcontext, Id, POSId, Country, ShoppingCartId, PaymentCurrency, TotalPayment, PaymentCurrencyDenominations, BaseCurrencyTotalPayed, BaseCurrencyTotalToPay);
        }
    }
}
