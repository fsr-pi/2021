using FluentValidation;
using MVC.Models;

namespace MVC.ModelsValidation
{
  public class ArtiklValidator : AbstractValidator<Artikl>
  {
    public ArtiklValidator()
    {
      //RuleFor(m => m.OznDrzave)
      //  .NotEmpty().WithMessage("Potrebno je unijeti oznaku države");           

      //RuleFor(m => m.NazMjesta)        
      //  .NotEmpty().WithMessage("Potrebno je unijeti naziv mjesta");

      //RuleFor(m => m.PostBrMjesta)
      //  .NotEmpty().WithMessage("Potrebno je unijeti poštanski broj mjesta (10-60000)")
      //  .GreaterThanOrEqualTo(10).WithMessage("Dozvoljeni raspon: 10-60000")
      //  .LessThanOrEqualTo(6000).WithMessage("Dozvoljeni raspon: 10-60000");


    }
  }
}
