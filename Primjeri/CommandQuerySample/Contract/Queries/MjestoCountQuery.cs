using CommandQueryCore;

namespace Contract.Queries
{
  public class MjestoCountQuery : IQuery<int>
  {
    public string SearchText { get; set; }
  }
}
