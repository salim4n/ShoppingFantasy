using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Models;

namespace ShoppingFantasy.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<ShippingService> ShippingServices { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ShoppingCartConfiguration());
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(x => new { x.LoginProvider, x.ProviderKey });

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Picture)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId);


            //modelBuilder.Entity<CartItem>()
            //    .HasOne(ci => ci.AppUser)
            //    .WithMany(u => u.CartItems)
            //    .HasForeignKey(ci => ci.AppUserId);

            //modelBuilder.Entity<CartItem>()
            //    .HasMany(ci => ci.Products)
            //    .WithOne(p => p.CartItem)
            //    .HasForeignKey(p => p.CartItemId);
        }

    }
}