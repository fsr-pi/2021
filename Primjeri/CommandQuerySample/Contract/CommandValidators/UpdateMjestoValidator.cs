using CommandQueryCore;
using Contract.Commands;
using Contract.DTOs;
using Contract.Queries;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Contract.CommandValidators
{
  public class UpdateMjestoValidator : AbstractValidator<UpdateMjesto>
  {
    private readonly IQueryHandler<SearchMjestoQuery, IEnumerable<Mjesto>> searchMjestoQueryHandler;

    public UpdateMjestoValidator(IQueryHandler<SearchMjestoQuery, IEnumerable<Mjesto>> searchMjestoQueryHandler)
    {
      this.searchMjestoQueryHandler = searchMjestoQueryHandler;

      RuleFor(m => m.OznDrzave)
        .NotEmpty().WithMessage("Potrebno je unijeti oznaku države");

      RuleFor(m => m.NazivMjesta)
        .NotEmpty().WithMessage("Potrebno je unijeti naziv mjesta");

      RuleFor(m => m.PostBrojMjesta)
        .NotEmpty().WithMessage("Potrebno je unijeti poštanski broj mjesta (10-60000)")
        .GreaterThanOrEqualTo(10).WithMessage("Dozvoljeni raspon: 10-60000")
        .LessThanOrEqualTo(60000).WithMessage("Dozvoljeni raspon: 10-60000")
        //Npr. neka u istoj državi ne mogu postojati 2 mjesta s istim poštanskim brojem
        .MustAsync(CheckUniqueIndex).WithMessage("Poštanski broj mora biti jedinstven na razini države");
    }

    private async Task<bool> CheckUniqueIndex(UpdateMjesto command, int pbr, CancellationToken cancellationToken)
    {
      var searchQuery = new SearchMjestoQuery
      {
        OznDrzave = command.OznDrzave,
        PostanskiBroj = pbr
      };
      var mjesta = await searchMjestoQueryHandler.Handle(searchQuery);
      if (mjesta.Count() > 0)
      {
        var mjesto = mjesta.First();
        return mjesto.IdMjesta == command.IdMjesta; 
      }
      else
      {
        return true;
      }
    }
  }
}
