using AutoMapper;
using MagicVilla_CouponMinimalAPI.Models;
using MagicVilla_CouponMinimalAPI.Models.DTO;
using Microsoft.Extensions.FileProviders;

namespace MagicVilla_CouponMinimalAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Coupon, CouponDTO>().ReverseMap();
            CreateMap<Coupon, CouponCreateDTO>().ReverseMap();
            CreateMap<Coupon, CouponUpdateDTO>().ReverseMap();
        }
    }
}
