namespace Uttt.Micro.Cupon.Models.Dto
{
    public class CouponDto
    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; } = "";
        public string DiscountAmount { get; set; } = "";
        public int MinAmount { get; set; }
        public string AmountType { get; set; } = "";
        public int LimitUse { get; set; }
        public DateTime DateInt { get; set; }
        public DateTime DateEnd { get; set; }
        public string Category { get; set; } = "";
        public bool StateCoupon { get; set; }
    }
}