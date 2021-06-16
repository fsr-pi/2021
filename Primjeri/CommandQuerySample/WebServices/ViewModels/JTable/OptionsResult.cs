using System.Collections.Generic;

namespace WebServices.ViewModels.JTable
{
  public class OptionsResult : JTableAjaxResult
  {
    public OptionsResult(List<TextValue> options)
    {
      Options = options;
    }
    public List<TextValue> Options { get; set; }
  }

  public class TextValue
  {
    public string DisplayText { get; set; }
    public string Value { get; set; }    
  }
}
