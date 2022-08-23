using MagicVilla_CouponMinimalAPI.Models;

namespace MagicVilla_CouponMinimalAPI.Data
{
    public static class CouponStore
    {
        public static List<Coupon> couponList = new List<Coupon>
        {
            new Coupon{ Id = 1, Name = "10OFF", Percent = 10, IsActive = true },
            new Coupon{ Id = 2, Name = "20OFF", Percent = 20, IsActive = false },
            new Coupon{ Id = 3, Name = "30OFF", Percent = 30, IsActive = false, Created = DateTime.Now }
        };
    }
}
