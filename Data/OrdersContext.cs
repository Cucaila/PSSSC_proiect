using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Data
{
    public class OrdersContext: DbContext
    {
        public OrdersContext(DbContextOptions<OrdersContext> options) : base(options)
        {

        }

        public DbSet<OrderHeaderDbo> OrdersHeader { get; set; }
        public DbSet<OrderLineDbo> OrdersLine { get; set; }
        public DbSet<ProductDbo> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderHeaderDbo>().ToTable("OrderHeader").HasKey(s => s.OrderId);
            modelBuilder.Entity<OrderLineDbo>().ToTable("OrderLine").HasKey(s => s.OrderLineId);
            modelBuilder.Entity<ProductDbo>().ToTable("Product").HasKey(s => s.ProductId);

        }
    }
}
