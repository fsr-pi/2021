using System.Collections.Generic;

namespace WebServices.ViewModels.JTable
{
  public class TableRecords<T> : JTableAjaxResult
  {
    public IEnumerable<T> Records { get; set; }
    public int TotalRecordCount { get; set; }
    public TableRecords(int totalCount, IEnumerable<T> records) : base()
    {
      TotalRecordCount = totalCount;
      Records = records;
    }    
  }
}
