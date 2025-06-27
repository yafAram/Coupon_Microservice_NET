using AutoMapper;
using Uttt.Micro.Cupon.Models;
using Uttt.Micro.Cupon.Models.Dto;

namespace Uttt.Micro.Cupon
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}