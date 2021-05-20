using System.Text.Json.Serialization;

namespace MVC.ViewModels
{  
  public class AutoCompleteArtikl
  {
    [JsonPropertyName("label")]
    public string Label { get; set; }
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("cijena")]
    public decimal Cijena { get; set; }
    public AutoCompleteArtikl() { }
    public AutoCompleteArtikl(int id, string label, decimal cijena)
    {
      Id = id;
      Label = label;
      Cijena = cijena;
    }
  }
}
