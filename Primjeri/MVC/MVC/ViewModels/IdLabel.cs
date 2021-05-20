using System.Text.Json.Serialization;

namespace MVC.ViewModels
{  
  public class IdLabel
  {
    [JsonPropertyName("label")]
    public string Label { get; set; }
    [JsonPropertyName("id")]
    public int Id { get; set; }
    public IdLabel() { }
    public IdLabel(int id, string label)
    {
      Id = id;
      Label = label;
    }
  }
}
