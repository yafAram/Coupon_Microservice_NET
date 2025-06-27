using Microsoft.EntityFrameworkCore;
using Uttt.Micro.Cupon.Models;

namespace Uttt.Micro.Cupon.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId = 1,
                CouponCode = "100FF",
                DiscountAmount = "10",
                MinAmount = 20,
                AmountType = "PERCENTAGE",
                LimitUse = 100,
                DateInt = new DateTime(2025, 1, 1),
                DateEnd = new DateTime(2025, 12, 31),
                Category = "GENERAL",
                StateCoupon = true
            });

            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId = 2,
                CouponCode = "200FF",
                DiscountAmount = "20",
                MinAmount = 40,
                AmountType = "PERCENTAGE",
                LimitUse = 50,
                DateInt = new DateTime(2025, 6, 1),
                DateEnd = new DateTime(2025, 12, 31),
                Category = "PREMIUM",
                StateCoupon = true
            });
        }
    }
}