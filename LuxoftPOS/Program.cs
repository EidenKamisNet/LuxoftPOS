using System;
using LuxoftPOS.Model;
using LuxoftPOS.Model.CashOptions;
using LuxoftPOS.Model.Contracts;
using System.Configuration;
using System.Collections.Specialized;
using static System.Console;
using System.Collections.Generic;

namespace LuxoftPOS
{
    public class Program
    {
        static void Main(string[] args)
        {

            try
            {
                WriteLine($"//////////////////////SET POS CONFIGURATION////////////////////////////////");
                IDBContext pos = new POS(Model.CashOptions.CountryOption.Mexico,new EFCashMastersDbContext()).CreateNewPOS();
                //IDBContext pos = new POS(Model.CashOptions.CountryOption.Mexico);
                WriteLine($"POS Id: {((POS)pos).Id.ToString()}  |  POS Country: {pos.GetCountry()}");
                //Configure Currencies handle by the current POS
                WriteLine("\r\n===============================Configure Currencies===============================");
                CurrencyType CurrencyMXN;
                CurrencyMXN = new CurrencyType(pos) { CurrencyName = "MXN", IsBaseCurrency = true };
                CurrencyMXN.Commit();
                WriteLine($"CurrencyName:{CurrencyMXN.CurrencyName}  |  Country:{CurrencyMXN.Country}  |  IsBaseCurrency: {CurrencyMXN.IsBaseCurrency}");
                var CurrencyUSD = new CurrencyType(pos) { CurrencyName = "USD", IsBaseCurrency = false };
                CurrencyUSD.Commit();
                WriteLine($"CurrencyName:{CurrencyUSD.CurrencyName}  |  Country:{CurrencyUSD.Country}  |  IsBaseCurrency: {CurrencyUSD.IsBaseCurrency}");
                //Configure Parities for the current POS

                WriteLine("\r\n================================Configure  Parities================================");
                Parity parityMXN = new Parity(pos)
                {
                    Currency = CurrencyMXN,//new CurrencyType(pos) { CurrencyName = "MXN", IsBaseCurrency = true },CurrencyMXN, 
                    ParityDate = DateTime.UtcNow,
                    Value = 1
                }; //the value of 1 peso in MXN Currency for a POS from mexico it will be 1 MXN
                WriteLine($"Currency:{parityMXN.Currency.CurrencyName}  |  Value:{parityMXN.Value.ToString()}");
                parityMXN.Commit();
                Parity parityUSD = new Parity(pos)
                {
                    Currency = CurrencyUSD,//new CurrencyType(pos) { CurrencyName = "USD", IsBaseCurrency = false },//CurrencyUSD,
                    ParityDate = DateTime.UtcNow,
                    Value = 19.39
                }; //the value of 1 USD in MXN Currency for a POS from mexico it will be 19.39 MXN
                WriteLine($"Currency:{parityUSD.Currency.CurrencyName}  |  Value:{parityUSD.Value.ToString()}");
                parityUSD.Commit();
                WriteLine("\r\n=================Parity Apraisal of  the day between MXN and USD==================");
                //Configure Parity Apraisal of the day between MXN and USD in case we need to do a convertion for payments that are done in other than the current POS Currency
                ParityApraisal parityApraisal = new ParityApraisal(pos) { ApraisalDate = DateTime.UtcNow, BaseParity = parityMXN, ConvertionParity = parityUSD };

                parityApraisal.Commit();
                WriteLine($"BaseParity:{parityApraisal.BaseParity.Value} {parityApraisal.BaseParity.Currency.CurrencyName}  |  ConvertionParity:{parityApraisal.ConvertionParity.Value} {parityApraisal.ConvertionParity.Currency.CurrencyName}");

                ParityApraisal parityApraisalDefault = new ParityApraisal(pos) { ApraisalDate = DateTime.UtcNow, BaseParity = parityMXN, ConvertionParity = parityMXN };

                parityApraisalDefault.Commit();
                WriteLine($"BaseParity:{parityApraisalDefault.BaseParity.Value} {parityApraisalDefault.BaseParity.Currency.CurrencyName}  |  ConvertionParity:{parityApraisalDefault.ConvertionParity.Value} {parityApraisalDefault.ConvertionParity.Currency.CurrencyName}");

                //capture Product and price (prices can be changed with the time if is required by the client)

                WriteLine("\r\n===============================Add Product to POS================================");
                Product product = new Product(pos)
                {
                    Name = "HeadSet Vx248'",
                    Description = "Headset for gaming porupuses",
                    ProductDate = DateTime.Today,
                    Country = pos.GetCountry()
                };
                product.AddNewPrice(120, DateTime.UtcNow);
                product.AddStocks(5);
                product.Commit();
                Product product2 = new Product(pos)
                {
                    Name = "Mouse Logitech 348'",
                    Description = "Mouse for gaming porupuses",
                    ProductDate = DateTime.Today,
                    Country = pos.GetCountry()
                };
                product2.AddNewPrice(87, DateTime.UtcNow);
                product2.AddStocks(10);
                product2.Commit();
                WriteLine($"ProductName:{product.Name} ");
                WriteLine($"ProductName:{product2.Name} ");
                //configure exchagetypes
                WriteLine("\r\n============================Configure  Exchange Types=============================");
                string USDExchages = "0.01|Coin,0.05|Coin,0.10|Coin,0.25|Coin,0.50|Coin,1.00|Bill,2.00|Bill,5.00|Bill,10.00|Bill,20.00|Bill,50.00|Bill,100.00|Bill";
                string MXNExchages = "0.05|Coin, 0.10|Coin, 0.20|Coin, 0.50|Coin, 1.00|Coin, 2.00|Coin, 5.00|Coin, 10.00|Coin, 20.00|Bill, 50.00|Bill, 100.00|Bill";
                pos.SetExchangeTypes(USDExchages, CurrencyUSD);
                pos.SetExchangeTypes(MXNExchages, CurrencyMXN);
                WriteLine($"USD Exchanges:{USDExchages}");
                WriteLine($"MXN Exchanges:{MXNExchages}");

                ///////////////////////END POS CONFIGURATION/////////////////////////////


                WriteLine($"\r\n////////////////////////        SHOPPINGCART          //////////////////////////////");
                WriteLine("=================================Products Added==================================");
                /// add articles to shopping cart
                ShoppingCart shoppingCart = new ShoppingCart(pos);
                shoppingCart.AddProductToCart(product, 2);
                shoppingCart.AddProductToCart(product2, 1);
                WriteLine($"ProductName:{product.Name}  Total Unities: 2");
                WriteLine($"ProductName:{product2.Name} Total Unities: 1 ");
                //get a tracking of the current balance in shopping cart
                WriteLine("\r\n==================================Current Total===================================");
                var currenttotal = shoppingCart.GetCurrentTotal();
                WriteLine($"Total in POS BaseCurrency of the ShoppingCart:{currenttotal} ");
                ///Commit and finish buying products
                shoppingCart.Commit();
                ////proceed to payment,here you will pay all the products in shopping cart in just one payment,currency is needed in case we want to do a payment with a currency diferent from Base Currency of the POS
                HashSet<PaymentCurrencyDenominations> paymentCurrencyDenominations = new HashSet<PaymentCurrencyDenominations>();
                //PAYMENT WITH EXCHANGE TYPE 1
                var POSExchange = pos.GetPOSExchangeType(x => x.Currency == CurrencyMXN && x.Value == 100.00);
                PaymentCurrencyDenominations PaymentForHeadset1 = new PaymentCurrencyDenominations(pos)
                { PaymentType = PaymentType.Payment, ExchangeType = POSExchange, Quantity = 3 };
                paymentCurrencyDenominations.Add(PaymentForHeadset1);
                //PAYMENT WITH EXCHANGE TYPE 2
                var POSExchange2 = pos.GetPOSExchangeType(x => x.Currency == CurrencyMXN && x.Value == 10.00);
                PaymentCurrencyDenominations PaymentForHeadset2 = new PaymentCurrencyDenominations(pos)
                { PaymentType = PaymentType.Payment, ExchangeType = POSExchange2, Quantity = 5 };
                paymentCurrencyDenominations.Add(PaymentForHeadset2);
                //1 full payment per Shopping Cart
                shoppingCart.AddPayment(CurrencyMXN, currenttotal, paymentCurrencyDenominations);
                //get exchange estructure
                shoppingCart.GetPayment().GetPaybackCurrencyDenominations();

                WriteLine("\r\n=========================get Total to payback to the client==========================");
                var result = shoppingCart.GetPayment().GetTotalPayback();
                WriteLine($"result-> Total Payed By the client:{shoppingCart.GetPayment().GetBaseCurrencyTotalPayed()}  |  total to payback to the client:{result.ToString()}   |  Total To Pay in POS BaseCurrency: {shoppingCart.GetPayment().TotalPayment.ToString()}");
                ReadLine();
            }
            catch (Exception ex)
            {
                
                WriteLine("\r\n=====================================ERROR======================================");
                WriteLine($"ERROR MSG:{ex.Message}");
                ReadLine();
            }
        }
        
    }
}
