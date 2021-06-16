using Contract.DTOs;
using Contract.Queries;
using Contract.QueryHandlers;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.QueryHandlers
{
  public class DrzaveLookupQueryHandler : IDrzaveLookupQueryHandler
  {    
    private readonly FirmaContext ctx;


    public DrzaveLookupQueryHandler(FirmaContext ctx)
    {
      this.ctx = ctx;
    } 

    public async Task<IEnumerable<TextValue<string>>> Handle(DrzaveLookupQuery query)
    {
      var list = await ctx.Drzava
                          .OrderBy(a => a.NazDrzave)
                          .Select(a => new TextValue<string>
                          {
                            Value = a.OznDrzave,
                            Text = a.NazDrzave
                          })
                          .ToListAsync();
      return list;
    }
  }
}
