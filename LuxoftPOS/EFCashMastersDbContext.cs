using LuxoftPOS.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxoftPOS
{
    public class EFCashMastersDbContext:DbContext
    {
        public DbSet<Product> Products { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=EIDENKAMIS-PC\SQLEXPRESS2019;Initial catalog=CashMasters;Integrated Security=true");
            //"Data Source=EIDENKAMIS-PC;Initial catalog=CashMasters;Integrated Security=true"
            //Server =EIDENKAMIS-PC\SQLEXPRESS2019;Database=CashMasters;User Id=sa;Password=EidenKamis18235;
            // base.OnConfiguring(optionsBuilder);
        }
    }
}
