using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebServices.ViewModels;

namespace WebServices.Controllers
{
  public interface ICustomController<TKey, TModel>
  {
    public Task<int> Count([FromQuery] string filter);
    public Task<IEnumerable<TModel>> GetAll([FromQuery] LoadParams loadParams);
    public Task<ActionResult<TModel>> Get(TKey id);
    public Task<IActionResult> Create(TModel model);
    public Task<IActionResult> Update(TKey id, TModel model);
    public Task<IActionResult> Delete(TKey id);
  }
}
