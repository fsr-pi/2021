using System;
using System.Collections.Generic;

namespace Logging
{
  public interface IDataLoader
  {
    List<Tuple<string, DateTime>> LoadData(string filename);
  }
}
