using Contract.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebServices.Util.ExceptionFilters;
using WebServices.ViewModels.JTable;

namespace WebServices.Controllers.JTable
{
  [Route("jtable/mjesto/[action]")]
  [TypeFilter(typeof(ErrorStatusTo200WithErrorMessage))]
  public class MjestoJTableController : JTableController<MjestoController, int, Mjesto>
  {
    public MjestoJTableController(MjestoController controller) : base(controller)
    {

    }

    [HttpPost]
    public async Task<JTableAjaxResult> Update([FromForm] Mjesto model)
    {
      return await base.UpdateItem(model.IdMjesta, model);
    }

    [HttpPost]
    public async Task<JTableAjaxResult> Delete([FromForm] int IdMjesta)
    {
      return await base.DeleteItem(IdMjesta);
    }
  }
}