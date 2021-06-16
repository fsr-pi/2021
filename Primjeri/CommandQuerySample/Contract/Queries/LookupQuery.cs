using CommandQueryCore;
using Contract.DTOs;
using System.Collections.Generic;

namespace Contract.Queries
{
  public abstract class LookupQuery<V> : IQuery<IEnumerable<TextValue<V>>>
  {
      
  }  
}
