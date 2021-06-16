using CommandQueryCore;
using Contract.Queries;

namespace Contract.QueryHandlers
{
  public interface IMjestoCountQueryHandler : IQueryHandler<MjestoCountQuery, int>
  {
  }
}
