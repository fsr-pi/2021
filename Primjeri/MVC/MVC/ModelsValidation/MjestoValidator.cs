using FluentValidation;
using MVC.Models;

namespace MVC.ModelsValidation
{
  public class MjestoValidator : AbstractValidator<Mjesto>
  {
    public MjestoValidator()
    {
      RuleFor(m => m.OznDrzave)
        .NotEmpty().WithMessage("Potrebno je unijeti oznaku države");           

      RuleFor(m => m.NazMjesta)        
        .NotEmpty().WithMessage("Potrebno je unijeti naziv mjesta");

      RuleFor(m => m.PostBrMjesta)
        .NotEmpty().WithMessage("Potrebno je unijeti poštanski broj mjesta (10-60000)")
        .GreaterThanOrEqualTo(10).WithMessage("Dozvoljeni raspon: 10-60000")
        .LessThanOrEqualTo(60000).WithMessage("Dozvoljeni raspon: 10-60000");


    }
  }
}
