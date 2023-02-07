using System.Linq;
using System.Text.Json;
using Dapper;
using MySql.Data.MySqlClient;

public class CurrencyConverter
{
    static HttpClient client = new HttpClient();
    public static async Task<decimal> ConvertCurrency(string currencyFrom, string currencyTo, decimal amount)
    {
        try
        {
            string connectionString = "server=SG-currency-converter-7204-mysql-master.servers.mongodirector.com;port=3306;database=currencies;user id=client;password=Notag00d#Password";
            // Set variable that will be returned
            decimal convertedAmount = 0;

            // Db connection string. Live db for fun.
            
            // Connect to the database
            using (MySqlConnection? connection = new MySqlConnection(connectionString))
            {
                // Open our database connection.
                await connection.OpenAsync();

                using (MySqlCommand? command = connection.CreateCommand())
                {
                    // Let's check if the table exists in the db.
                    long tableExists = 0;
                    command.CommandText = "SELECT COUNT(*) FROM information_schema.tables WHERE table_name = 'currencies'";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tableExists = reader.GetInt64(0);
                        }
                    }
                    
                    // If the table does not exist we create one
                    if(tableExists < 1)
                    {
                        command.CommandText = "CREATE TABLE currencies (CurrencyId int, CurrencyCode CHAR(3), ExchangeRate double)";
                        await command.ExecuteNonQueryAsync();

                        // For the sake of simplicity we add 3 currencies to the db.
                        command.CommandText = @"INSERT INTO currencies (CurrencyId, CurrencyCode, ExchangeRate) VALUES (@CurrencyId1, @CurrencyCode1, @ExchangeRate1),(@CurrencyId2, @CurrencyCode2, @ExchangeRate2),(@CurrencyId3, @CurrencyCode3, @ExchangeRate3)";
                        command.Parameters.AddWithValue("@CurrencyId1", 1);
                        command.Parameters.AddWithValue("@CurrencyCode1", "EUR");
                        command.Parameters.AddWithValue("@ExchangeRate1", 0.93);
                        command.Parameters.AddWithValue("@CurrencyId2", 2);
                        command.Parameters.AddWithValue("@CurrencyCode2", "JPY");
                        command.Parameters.AddWithValue("@ExchangeRate2", 131.83);
                        command.Parameters.AddWithValue("@CurrencyId3", 3);
                        command.Parameters.AddWithValue("@CurrencyCode3", "GBP");
                        command.Parameters.AddWithValue("@ExchangeRate3", 0.83);
                        int rowCount = await command.ExecuteNonQueryAsync();
                    }
                    
                    // Get available currencies from the db
                    List<Currency> availableCurrencies = connection.Query<Currency>("SELECT * FROM currencies").ToList();

                    // Validate that user input matches an available currency and set to detailed variable if it does
                    Currency detailedCurrencyFrom = null;
                    Currency detailedCurrencyTo = null;

                    foreach (Currency item in availableCurrencies)
                    {
                        if(item.CurrencyCode == currencyFrom)
                        {
                            detailedCurrencyFrom = item;
                        }else if(item.CurrencyCode == currencyTo)
                        {
                            detailedCurrencyTo = item;
                        }
                    }

                    // If a detailed variable does not exist it is not valid
                    if(detailedCurrencyFrom == null || detailedCurrencyTo == null)
                    {
                        return 0;
                    }else
                    {
                        // Calculate Conversion
                        decimal usdAmount = amount / (decimal)detailedCurrencyFrom.ExchangeRate;
                        convertedAmount = usdAmount * (decimal)detailedCurrencyTo.ExchangeRate;
                    }
                }
            }
            // Return
            return convertedAmount;
        }
        catch (System.Exception)
        {
            throw;
            return 0;
        }
    }

    // Optional Convert with api instead
    public static async Task<decimal> ConvertWithApi(string convertFrom, string convertTo, decimal amount)
    {
        try
        {
            string url = $"https://marketdata.tradermade.com/api/v1/convert?api_key=9-p9HE6QJ8lhEcLkAKdr&from={convertFrom}&to={convertTo}&amount={amount}";
            string response = await client.GetStringAsync(url);
            ApiConversionCurrency apiCurrency = JsonSerializer.Deserialize<ApiConversionCurrency>(response);
            decimal conversionResult = apiCurrency.total;
            return conversionResult;
        }
        catch (System.Exception)
        {
            return 0;
        }
    }

    // Optional live rates from api
    public static async Task<int> GetLiveRate(string quote)
    {
        try
        {
            string url = $"https://marketdata.tradermade.com/api/v1/live?currency={quote}&api_key=9-p9HE6QJ8lhEcLkAKdr";
         
            string response = await client.GetStringAsync(url);
            LiveRate? liveRate = JsonSerializer.Deserialize<LiveRate>(response);

            Console.WriteLine($"Live Rate: {liveRate.Mid}");
            return 1;
        }
        catch (System.Exception)
        {
            return 0;
        }
    }

    // List available currencies (db)
    public static async Task ListAvailableCurrencies()
    {
        string connectionString = "server=SG-currency-converter-7204-mysql-master.servers.mongodirector.com;port=3306;database=currencies;user id=client;password=Notag00d#Password";
        // Connect to the database
        using (MySqlConnection? connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();
    
            using (MySqlCommand? command = connection.CreateCommand())
            {
                // Get available currencies from the db
                List<Currency> availableCurrencies = connection.Query<Currency>("SELECT * FROM currencies").ToList();
                foreach (Currency item in availableCurrencies)
                {
                    Console.WriteLine(item.CurrencyCode);
                }
            }
        }
    }

    // Records in place of models to keep in one class
    public record Currency()
    {
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public double ExchangeRate { get; set; }
    }

    public record ApiConversionCurrency()
    {
        public string Base_Currency { get; set; }
        public string Quote_Currency { get; set; }
        public decimal total { get; set; }
    }
    public record LiveRate()
    {
        public decimal Ask { get; set; }
        public string Base_Currency { get; set; }
        public decimal Bid { get; set; }
        public decimal Mid { get; set; }
        public string QuoteCurrency { get; set; }
    }
}