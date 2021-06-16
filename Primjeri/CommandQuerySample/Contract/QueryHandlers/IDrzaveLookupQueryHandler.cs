using CommandQueryCore;
using Contract.DTOs;
using Contract.Queries;
using System.Collections.Generic;

namespace Contract.QueryHandlers
{
  public interface IDrzaveLookupQueryHandler : IQueryHandler<DrzaveLookupQuery, IEnumerable<TextValue<string>>>
  {
  }
}
