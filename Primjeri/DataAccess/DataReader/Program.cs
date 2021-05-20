using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;

namespace DataReader
{
  class Program
  {
    public static void Main(string[] args)
    {
      //ShowProducts();
      Console.WriteLine();      
      ShowProductsBetterVersion();     
    }

    private static void ShowProducts()
    {
      string connString = "Data Source=rppp.fer.hr,3000;Initial Catalog=Firma;User Id=rppp;Password=do-not-put-password-in-source-code";

      IDbConnection conn = new SqlConnection(connString);

      IDbCommand command = new SqlCommand();
      command.CommandText = "SELECT TOP 10 * FROM Artikl ORDER BY CijArtikla DESC";
      command.Connection = conn;

      conn.Open();

      IDataReader reader = command.ExecuteReader();
      while (reader.Read())
      {
        object productCode = reader["SifArtikla"];
        object productName = reader["NazArtikla"];
        object isService = reader["ZastUsluga"];                
        Console.WriteLine("{0} - {1}  {2}", productCode.ToString(), productName.ToString(), (bool)isService ? "(Service)" : "");
      }
      reader.Close();

      conn.Close();
    }    

    private static void ShowProductsBetterVersion()
    {
      string connString = GetConnectionString();
      try
      {
        using (var conn = new SqlConnection(connString))
        {
          using (var command = conn.CreateCommand())
          {
            command.CommandText = "SELECT TOP 10 * FROM Artikl ORDER BY CijArtikla DESC";
            command.Connection = conn;

            conn.Open();

            using (var reader = command.ExecuteReader())
            {
              while (reader.Read())
              {
                object productCode = reader["SifArtikla"];
                object productName = reader["NazArtikla"];
                object isService = reader["ZastUsluga"];
                Console.WriteLine("{0} - {1}  {2}", productCode.ToString(), productName.ToString(), (bool)isService ? "(Service)" : "");
              }
            }
          }
        }
      }
      catch (Exception exc)
      {
        Console.WriteLine("Pogreška: " + exc.Message + exc.StackTrace);
      }
    }

    private static string GetConnectionString()
    {            
      var configBuilder = new ConfigurationBuilder()                              
                              .AddJsonFile("appsettings.json");

      #region Variant with environment name and secret file                              
      var environmentName = Environment.GetEnvironmentVariable("ENVIRONMENT");
      configBuilder = configBuilder.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

      bool useSecretFile = "Development".Equals(environmentName, StringComparison.OrdinalIgnoreCase);
      if (useSecretFile)
      {
        configBuilder = configBuilder.AddUserSecrets("Firma");
      }
      #endregion

      var config = configBuilder.Build();

      //string connString = config["ConnectionStrings:Firma"];
      string connString = config.GetConnectionString("Firma");

      #region if using secret file...
      if (useSecretFile)
      {
        connString = connString.Replace("sifra", config["FirmaSqlPassword"]);
      }
      #endregion

      return connString;
    }
  }
}
