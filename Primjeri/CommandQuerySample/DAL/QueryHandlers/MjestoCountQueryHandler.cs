using Contract.Queries;
using Contract.QueryHandlers;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.QueryHandlers
{
  public class MjestoCountQueryHandler : IMjestoCountQueryHandler
  {
    private readonly FirmaContext ctx;

    public MjestoCountQueryHandler(FirmaContext ctx)
    {
      this.ctx = ctx;
    }

    public async Task<int> Handle(MjestoCountQuery query)
    {
      var dbQuery = ctx.Mjesto.AsQueryable();
      if (!string.IsNullOrWhiteSpace(query.SearchText))
      {
        dbQuery = dbQuery.Where(m => m.NazMjesta.Contains(query.SearchText));
      }
      int count = await dbQuery.CountAsync();
      return count;
    }
  }
}
