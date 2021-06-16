using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServices.ViewModels.JTable
{
  public class JTableAjaxResult
  {
    public string Result { get; set; }
    public string Message { get; set; }

    protected JTableAjaxResult()
    {
      Result = "OK";      
    }

    protected JTableAjaxResult(string errorMessage)
    {
      Result = "ERROR";
      Message = errorMessage;
    }

    public static JTableAjaxResult OK => new JTableAjaxResult { Result = "OK" };
    public static JTableAjaxResult Error(string errorMessage) => new JTableAjaxResult { Result = "ERROR", Message = errorMessage };
      
  }
}
