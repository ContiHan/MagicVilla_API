using FluentValidation;
using MagicVilla_CouponMinimalAPI.Models.DTO;

namespace MagicVilla_CouponMinimalAPI.Validations
{
    public class CouponCreateValidation : AbstractValidator<CouponCreateDTO>
    {
        public CouponCreateValidation()
        {
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).InclusiveBetween(1, 100);
        }
    }
}
