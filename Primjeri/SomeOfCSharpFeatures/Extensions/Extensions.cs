using System.Collections.Generic;

namespace Extensions
{
  public static class Extensions
  {
    public static V GetOrCreate<K, V>(this Dictionary<K, V> dict, K key) where V : new()
    {
      if (!dict.TryGetValue(key, out V value))
      {
        value = new V();
        dict[key] = value;        
      }
      return value;            
    }
  }
}
