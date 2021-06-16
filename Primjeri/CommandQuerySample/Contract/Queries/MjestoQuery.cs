using CommandQueryCore;
using Contract.DTOs;

namespace Contract.Queries
{
  public class MjestoQuery : IQuery<Mjesto>
  {
    public int Id { get; set; }
  }
}
