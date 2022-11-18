using LuxoftPOS.Model.CashOptions;
using LuxoftPOS.Model.Contracts;
using LuxoftPOS.Model;
using System;
using Xunit;
using Moq;
using LuxoftPOS;
using Moq.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace xUnitTestPOS
{
   
    public class POSUnitTest
    {

        EFCashMastersDbContext context;
        public POSUnitTest() {

            //MockEFCashMastersDbContext = new Mock<EFCashMastersDbContext>();
            // userContextMock.Setup(x => x.POS).ReturnsDbSet(LstPOS);
            var options = new DbContextOptionsBuilder<EFCashMastersDbContext>()
           .UseInMemoryDatabase(databaseName: "CashMastersTest")
           .Options;
            
            context = new EFCashMastersDbContext(options);
        }
        //[Fact]
        [Theory]
        [InlineData(CountryOption.Mexico)]
        [InlineData(CountryOption.Canada)]
        [InlineData(CountryOption.UnitedState)]
        public void EF_1_POS_CreateNewPOS(CountryOption country) {           
            IDBContext pos = new POS(country, context).CreateNewPOS();
            Assert.True(((POS)pos).Id > 0);
            var exception = Assert.Throws(typeof(Exception), () => ((POS)pos).CreateNewPOS());
            Assert.True(exception.Message == $"POS {country.ToString()} already exists");

        }
        [Theory]
        [InlineData(CountryOption.Mexico)]
        [InlineData(CountryOption.Canada)]
        [InlineData(CountryOption.UnitedState)]
        public void EF_2_POS_SetConfigurations_CreateShoppingCart(CountryOption country)
        {
            IDBContext pos = new POS(country, context);
            ((POS)pos).SetBaseCurrency(null);
            Exception exception = Assert.Throws(typeof(Exception), () => ((POS)pos).GetBaseCurrency());
            Assert.True(exception.Message == $"Base Currency is required for the country '{country.ToString()}' ,please verify the settings were configure");
            CurrencyType CurrencyMXN;
            CurrencyMXN = new CurrencyType(pos) { CurrencyName = "MXN", IsBaseCurrency = true };
            CurrencyMXN.Commit();
            Assert.True(CurrencyMXN.Id > 0 && CurrencyMXN.POSId > 0);
            var CurrencyUSD = CurrencyMXN.DeepCopy();

            CurrencyUSD.Id = 0;
            CurrencyUSD.IsBaseCurrency = false;
            CurrencyUSD.CurrencyName = "USD";
            CurrencyUSD.Commit();
            
            Assert.True(((POS)pos).GetBaseCurrency().CurrencyName == CurrencyMXN.CurrencyName);
            Parity parityMXN = new Parity(pos)
            {
                Currency = CurrencyMXN,
                ParityDate = DateTime.UtcNow,
                Value = 1
            };
            parityMXN.Commit();
            Assert.True(parityMXN.Id > 0 && parityMXN.POSId > 0);
            Parity parityUSD = new Parity(pos)
            {
                Currency = CurrencyUSD,
                ParityDate = DateTime.UtcNow,
                Value = 19.39
            };
            parityUSD.Commit();
            Assert.True(parityUSD.Id > 0 && parityUSD.POSId > 0);
            ParityApraisal parityApraisal = new ParityApraisal(pos) { ApraisalDate = DateTime.UtcNow, BaseParity = parityMXN, ConvertionParity = parityUSD };
            parityApraisal.Commit();
            Assert.True(parityApraisal.Id > 0 && parityApraisal.POSId > 0);
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
            Assert.True(product.Id > 0 && product.POSId > 0 && product.Stocks.Count > 0);
            string USDExchages = "0.01|Coin,0.05|Coin,0.10|Coin,0.25|Coin,0.50|Coin,1.00|Bill,2.00|Bill,5.00|Bill,10.00|Bill,20.00|Bill,50.00|Bill,100.00|Bill";
            string MXNExchages = "0.05|Coin, 0.10|Coin, 0.20|Coin, 0.50|Coin, 1.00|Coin, 2.00|Coin, 5.00|Coin, 10.00|Coin, 20.00|Bill, 50.00|Bill, 100.00|Bill";
            pos.SetExchangeTypes(USDExchages, CurrencyUSD);
            pos.SetExchangeTypes(MXNExchages, CurrencyMXN);
            Assert.True(pos.GetPOSExchangeTypes(x => x.Country == country) != null);
            ShoppingCart shoppingCart = new ShoppingCart(pos);
           
            Assert.NotNull(product);
            shoppingCart.AddProductToCart(product, 2);
            shoppingCart.Commit();
            Assert.True(shoppingCart.Id > 0 && shoppingCart.POSId > 0 && shoppingCart.ShoppingCartDetails.Count > 0);
            var currenttotal = shoppingCart.GetCurrentTotal();
            HashSet<PaymentCurrencyDenominations> paymentCurrencyDenominations = new HashSet<PaymentCurrencyDenominations>();
   
            var POSExchange = pos.GetPOSExchangeType(x => x.Currency == CurrencyMXN && x.Value == 100.00);
            PaymentCurrencyDenominations PaymentForHeadset1 = new PaymentCurrencyDenominations(pos)
            { PaymentType = PaymentType.Payment, ExchangeType = POSExchange, Quantity = 3 };
            paymentCurrencyDenominations.Add(PaymentForHeadset1);            

            shoppingCart.AddPayment(CurrencyMXN, currenttotal, paymentCurrencyDenominations);

            Assert.True(shoppingCart.GetPayment().GetPaybackCurrencyDenominations().Count>0);
            Assert.True(shoppingCart.GetPayment().GetTotalPayback()>0);
            Assert.True(shoppingCart.GetPayment().GetBaseCurrencyTotalPayed()>0);
        }
    }
}
