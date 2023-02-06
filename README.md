# currency-converter

Programming Exercise
Write a simple currency converter in any programming language. The list of allowed currencies and exchange rates are
stored in a single database table with the following fields: CurrencyID (int), CurrencyCode (3 letter string), ExchangeRate
(double, stored as the rate from US dollars to the given currency).
Example database rows: {1, “USD”, 1.0}, {2, “PHP”, 43.1232}
For simplicity, write everything in one class. Write separate private functions as needed, and include the following public
function, which returns the converted currency amount:
public decimal ConvertCurrency(string currencyFrom, string currencyTo, decimal amount);
The following are items you might consider while programming:
● Data validation
● Database availability
● Comments
● Variable names
● Reusability
● Unit Testing
Extra Credit
Extend the functionality of your currency class in some way.
Here are some ideas. Extra points for coming up with your own.
● Write a method that gets the exchange rate for a specific currency
● Write CRUD methods for keeping your exchange rates up to date in the database
● Redesign the table in the database to account for the fact the exchange rates are constantly fluctuating
● Add an option for using a public API to get the exchange rate instead of the database
