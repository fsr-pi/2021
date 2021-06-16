using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServices.Util.Extensions;
using WebServices.ViewModels;
using WebServices.ViewModels.JTable;

namespace WebServices.Controllers.JTable
{
  [ApiExplorerSettings(IgnoreApi = true)]
  public abstract class JTableController<TController, TKey, TModel> : ControllerBase where TController : ICustomController<TKey, TModel> 
  {    
    private readonly TController controller;

    public JTableController(TController controller)
    {      
      this.controller = controller;
    }

    /// <summary>
    /// Wrapper za učitavanje podataka koristeći jTable 
    /// </summary>
    /// <param name="loadParams"></param>
    /// <param name="search"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual async Task<TableRecords<TModel>> GetAll([FromQuery] LoadParams loadParams, [FromForm] string search)
    {
      int count = await controller.Count(search);
      loadParams.Filter = search;
      var data = await controller.GetAll(loadParams);
      return new TableRecords<TModel>(count, data);      
    }

    [HttpPost]
    public virtual async Task<JTableAjaxResult> Create([FromForm] TModel model)
    {
      if (model == null)
      {
        return JTableAjaxResult.Error("Model is null");
      }
      else if (!ModelState.IsValid)
      {
        return JTableAjaxResult.Error(ModelState.GetErrorsString());
      }

      var result = await controller.Create(model);
      if (result is CreatedAtActionResult created)
      {
        return new CreateResult(created.Value);
      }
      else 
      {        
        return JTableAjaxResult.Error(result.ToString());
      }
    }   

    protected async Task<JTableAjaxResult> UpdateItem(TKey id, TModel model)
    {

      if (model == null)
      {
        return JTableAjaxResult.Error("Model is null");
      }
      else if (!ModelState.IsValid)
      {
        return JTableAjaxResult.Error(ModelState.GetErrorsString());
      }


      var result = await controller.Update(id, model);
      if (result is NoContentResult)
      {
        return JTableAjaxResult.OK;
      }    
      else
      {
        return JTableAjaxResult.Error("Not found");
      }
    }
    
    protected async Task<JTableAjaxResult> DeleteItem(TKey id)
    {
      var result = await controller.Delete(id);
      if (result is NoContentResult)
      {
        return JTableAjaxResult.OK;
      }
      else
      {
        return JTableAjaxResult.Error("Not found");
      }      
    }
  }
}
