using AutoMapper;
using MagicVilla_CouponMinimalAPI.Models;
using MagicVilla_CouponMinimalAPI.Models.DTO;

namespace MagicVilla_CouponMinimalAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Coupon, CouponCreateDTO>().ReverseMap();
            CreateMap<Coupon, CouponDTO>().ReverseMap();
        }
    }
}
