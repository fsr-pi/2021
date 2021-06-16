using Contract.Queries;
using Contract.QueryHandlers;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.QueryHandlers
{
  public class MjestoQueryHandler : IMjestoQueryHandler
  {
    private readonly FirmaContext ctx;

    public MjestoQueryHandler(FirmaContext ctx)
    {
      this.ctx = ctx;
    }

    public async Task<Contract.DTOs.Mjesto> Handle(MjestoQuery query)
    {
      var mjesto = await ctx.Mjesto
                            .Where(m => m.IdMjesta == query.Id)
                            .Select(m => new Contract.DTOs.Mjesto
                            {
                              IdMjesta = m.IdMjesta,
                              NazivDrzave = m.OznDrzaveNavigation.NazDrzave,
                              NazivMjesta = m.NazMjesta,
                              OznDrzave = m.OznDrzave,
                              PostBrojMjesta = m.PostBrMjesta,
                              PostNazivMjesta = m.PostNazMjesta
                            })
                            .FirstOrDefaultAsync();
      return mjesto;
    }
  }
}
