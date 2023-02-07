bool running = true;

while (running)
{
    Console.WriteLine("Choose 'Convert', 'Api', 'LiveQuote'");
    string? selection = Console.ReadLine();
    decimal convertedAmount = 0;

    if(selection == "Convert" || selection == "Api")
    {
        Console.WriteLine("Available Currencies:");
        CurrencyConverter.ListAvailableCurrencies();

        Console.WriteLine("What currency would you like to convert?");
        string? convertFrom = Console.ReadLine();

        Console.WriteLine($"What currency would you like to convert {convertFrom} to?");
        string? convertTo = Console.ReadLine();

        Console.WriteLine($"How much {convertFrom} would you like to convert to {convertTo}?");
        decimal amount;
        string? amountStr = Console.ReadLine();
        Decimal.TryParse(amountStr, out amount);

        if(convertFrom != null && convertTo != null)
        {
            convertedAmount = selection == "Convert" ? CurrencyConverter.ConvertCurrency(convertFrom, convertTo, amount).Result : CurrencyConverter.ConvertWithApi(convertFrom, convertTo, amount).Result;
        }
    }else if(selection == "LiveQuote")
    {
        Console.WriteLine("Available Currencies:");
        CurrencyConverter.ListAvailableCurrencies();

        Console.WriteLine("What currency would you like to quote?");
        string? quote = Console.ReadLine();

        CurrencyConverter.GetLiveRate(quote);
    }


    if(convertedAmount != 0)
    {
        Console.WriteLine($"{convertedAmount}");
    }else
    {
        Console.WriteLine("Invalid Input");
    }
    running = false;
}