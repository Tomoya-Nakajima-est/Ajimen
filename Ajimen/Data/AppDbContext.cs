using Ajimen.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Ajimen.Data
{    
    public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

            public DbSet<Item> Items { get; set; }
            public DbSet<OrderLog> OrderLogs { get; set; }
            public DbSet<StockLog> StockLogs { get; set; }
        }
}
