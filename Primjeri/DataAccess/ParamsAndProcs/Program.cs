using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.Common;

namespace ParamsAndProcs
{
  public class Program
  {
    static IConfigurationRoot config;
    static DbProviderFactory factory = SqlClientFactory.Instance;
    //static DbProviderFactory factory = DbProviderFactories.GetFactory("Microsoft.Data.SqlClient"); //nije dio .Net Core, potrebno eksplicitno ranije registrirati  s   DbProviderFactories.RegisterFactory(factoryName, MySQLProviderFactory.Instance), npr. DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance); (jednom, u glavnom programu ili sl.)

    public static void Main(string[] args)
    {
      config = new ConfigurationBuilder()
                  //.SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json")
                  .Build();

      ParametrizedQueryDemo();
      Console.ReadLine();
      decimal threshold = decimal.Parse(config["PriceThreshold"]);
      ProcedureDemo(threshold);
    }

    /// <summary>
    /// Primjer stvaranja priključka na bazu korištenjem razreda DBProviderFactory
    /// Upit se postavlja korištenjem parametara i vraća 2 skupa rezultata
    /// </summary>
    private static void ParametrizedQueryDemo()
    {     
      using (DbConnection conn = factory.CreateConnection())
      {
        conn.ConnectionString = config.GetConnectionString("Firma");
        using (DbCommand command = factory.CreateCommand())
        {
          command.Connection = conn;
          command.CommandText = "SELECT TOP 3 * FROM Artikl WHERE JedMjere = @JedMjere ORDER BY CijArtikla DESC;" +
            "SELECT TOP 3 * FROM Artikl WHERE JedMjere = @JedMjere AND CijArtikla > @Cijena ORDER BY CijArtikla";
          DbParameter param = command.CreateParameter();
          param.ParameterName = "JedMjere";
          param.DbType = System.Data.DbType.String;
          param.Value = "kom";
          command.Parameters.Add(param);

          param = factory.CreateParameter();
          param.ParameterName = "Cijena";
          param.DbType = System.Data.DbType.Decimal;
          param.Value = 100m;
          command.Parameters.Add(param);
          conn.Open();
          using (DbDataReader reader = command.ExecuteReader())
          {
            bool firstResult = true;
            do
            {
              Console.WriteLine(firstResult ? "The most expensive products sold in pieces:" : "The cheapest products sold in pieces");
              Console.WriteLine("-----------------------------");
              while (reader.Read())
              {
                Console.WriteLine("\t{0} - {1}", reader["NazArtikla"], reader["CijArtikla"]);
              }
              Console.WriteLine();
              firstResult = false;
            }
            while (reader.NextResult());
          }
        }
      }
    }

    private static void ProcedureDemo(decimal priceThreshold)
    {      
      using (DbConnection conn = factory.CreateConnection())
      {
        conn.ConnectionString = config.GetConnectionString("Firma");
        using (DbCommand command = factory.CreateCommand())
        {
          command.Connection = conn;
          command.CommandText = "ap_ArtikliSkupljiOd";
          command.CommandType = System.Data.CommandType.StoredProcedure;

          DbParameter param = command.CreateParameter();
          param.ParameterName = "Prag";
          param.DbType = System.Data.DbType.Decimal;
          param.Value = priceThreshold;
          command.Parameters.Add(param);

          param = command.CreateParameter();
          param.ParameterName = "BrojSkupljih";
          param.DbType = System.Data.DbType.Int32;
          param.Direction = System.Data.ParameterDirection.Output;

          command.Parameters.Add(param);

          param = command.CreateParameter();
          param.ParameterName = "BrojJeftinijih";
          param.DbType = System.Data.DbType.Int32;
          param.Direction = System.Data.ParameterDirection.Output;
          command.Parameters.Add(param);

          conn.Open();
          using (DbDataReader reader = command.ExecuteReader())
          {
            Console.WriteLine("Products with the price above the threashold ({0})", priceThreshold);
            Console.WriteLine("---------------------------------");
            while (reader.Read())
            {
              Console.WriteLine("\t{0} - {1}", reader["NazArtikla"], reader["CijArtikla"]);
            }
            Console.WriteLine("---------");
          }
          //we are finished with reader -> read parameters vakues
          Console.WriteLine("Above threashold: {0}, Below threashold: {1}",
              command.Parameters["BrojSkupljih"].Value, 
              command.Parameters["BrojJeftinijih"].Value);
        }
      }
    }
  }
}
