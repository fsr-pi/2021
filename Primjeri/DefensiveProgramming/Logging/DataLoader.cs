using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Logging
{
  public class DataLoader : IDataLoader
  {
    private readonly ILogger<DataLoader> logger;

    public DataLoader(ILogger<DataLoader> logger)
    {
      this.logger = logger;
    }

    public List<Tuple<string, DateTime>> LoadData(string filename)
    {
      List<Tuple<string, DateTime>> list = new List<Tuple<string, DateTime>>();
      if (!File.Exists(filename))
      {
        string message = $"File {filename} does not exist";
        logger.LogError(message);
        throw new FileNotFoundException(message);
      }
      else
      {
        //use File.ReadAllLines for small files
        using (var reader = new StreamReader(File.OpenRead(filename)))
        {
          string line;
          while((line = reader.ReadLine()) != null) {
            string pattern = "[0-3]?[0-9].[0-3]?[0-9].[0-9]{4}.";
            var match = Regex.Match(line, pattern);
            if (match.Success)
            {
              string datetext = line.Substring(match.Index, match.Length);
              if (DateTime.TryParseExact(datetext, "d.M.yyyy.", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
              {
                var tuple = new Tuple<string, DateTime>(line.Remove(match.Index, match.Length).Trim(), date);
                logger.LogTrace(tuple.ToString());
                list.Add(tuple);
              }
              else
              {
                logger.LogWarning("Invalid date in line: " + line);
              }
            }
            else
            {
              logger.LogWarning("No date found in line: " + line);
            }
          }
        }
      }
      return list;
    }  
  }
}
