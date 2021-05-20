using FluentValidation;
using MVC.Models;

namespace MVC.ModelsValidation
{
  public class DrzavaValidator : AbstractValidator<Drzava>
  {
    public DrzavaValidator()
    {
      RuleFor(d => d.OznDrzave)
        .NotEmpty().WithMessage("Oznaka države je obvezno polje")        
        .MaximumLength(3).WithMessage("Oznaka države može sadržavati maksimalno 3 znaka");

      RuleFor(d => d.NazDrzave)        
        .NotEmpty().WithMessage("Naziv države je obvezno polje");       

      RuleFor(d => d.Iso3drzave)        
        .MaximumLength(3).WithMessage("ISO3 oznaka može sadržavati maksimalno 3 znaka");      

      
    }
  }
}
