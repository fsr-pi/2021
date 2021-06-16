namespace WebServices.Util.Extensions
{
  public static class StringExtensions
  {
    public static string ControllerName(this string className)
    {
      if (className.EndsWith("Controller"))
      {
        return className.Substring(0, className.Length - "Controller".Length);
      }
      else
      {
        return className;
      }
    }

    public static string FirstN(this string s, int n)
    {
      if (s == null)
      {
        return string.Empty;
      }
      if (s.Length > n)
      {
        return s.Substring(0, n);
      }
      else
      {
        return s;
      }
    }
  }
}
