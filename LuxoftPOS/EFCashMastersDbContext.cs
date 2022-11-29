using LuxoftPOS.Model;
using LuxoftPOS.Model.CashOptions;
using Microsoft.EntityFrameworkCore;

using System;

using Microsoft.Extensions.Configuration;


namespace LuxoftPOS
{

    public class EFCashMastersDbContext : DbContext
    {

        public DbSet<POS> POS { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Parity> Parities { get; set; }
        public DbSet<CurrencyType> Currencies { get; set; }
        public DbSet<ParityApraisal> ParityApraisals { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<ExchangeType> ExchangeTypes { get; set; }
    
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartDetail> ShoppingCartDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentCurrencyDenominations> PaymentCurrencyDenominations{ get; set; }
        public EFCashMastersDbContext() { }
        public EFCashMastersDbContext(DbContextOptions<EFCashMastersDbContext> options):base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Parity>().Property(p => p.ParityDate).IsRequired();
            modelBuilder.Entity<Product>().Property(p => p.Name).HasMaxLength(30).IsRequired();
            modelBuilder.Entity<Stock>().Property(p=>p.InStock).HasPrecision(precision:4);
            modelBuilder.Entity<CurrencyType>().Property(p => p.CurrencyName).HasMaxLength(10);
          //  modelBuilder.Entity<Payment>().HasMany(x => x.PaybackCurrencyDenominations).WithOne("Payment").HasForeignKey(x=>x.PaymentId).HasConstraintName($"FK_Payment__IdAddress").OnDelete(DeleteBehavior.Cascade); ;
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
           
            var configuration = new ConfigurationBuilder()
                      .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                      .AddXmlFile("App.config")
                      .Build();
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(configuration.GetValue<string>("connectionStrings:add:CashMasters_dbConnection:connectionString"));

            // optionsBuilder.UseSqlServer(x.GetConnectionString("Default"));
            //if(!optionsBuilder.IsConfigured)
            //   optionsBuilder.UseSqlServer(@"Data Source=EIDENKAMIS-PC\SQLEXPRESS2019;Initial catalog=Prueba3;Integrated Security=true");
            //"Data Source=EIDENKAMIS-PC;Initial catalog=CashMasters;Integrated Security=true"
            //Server =EIDENKAMIS-PC\SQLEXPRESS2019;Database=CashMasters;User Id=sa;Password=EidenKamis18235;
            base.OnConfiguring(optionsBuilder);
        }



    }
}
