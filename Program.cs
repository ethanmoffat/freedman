using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace freedman
{
    public class Program
    {
        private static readonly string CurrencyConvertUrl = "https://free.currconv.com/api/v7/convert?q={0}&compact=ultra&apiKey={1}";
        private static HttpClient _httpClient;

        private static double USD_CAD = 0.0;
        private static DateTime _lastFetchUsdCad;

        private static double CAD_USD = 0.0;
        private static DateTime _lastFetchCadUsd;

        private static IConfigurationRoot _config;

        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(Environment.GetEnvironmentVariable("ConnectionString"));
            _config = builder.Build();

            _httpClient = new HttpClient();

            _lastFetchUsdCad = DateTime.Now;
            _lastFetchCadUsd = DateTime.Now;

            try
            {
                var discordClient = new DiscordClient(new DiscordConfiguration
                {
                    Token = _config["freeman-token"],
                    TokenType = TokenType.Bot
                });

                discordClient.MessageCreated += OnMessageCreated;

                await discordClient.ConnectAsync();
                await Task.Delay(-1);
            }
            catch
            {
                _httpClient.Dispose();
            }
        }

        private static async Task OnMessageCreated(MessageCreateEventArgs e)
        {
            if (!e.Message.Content.StartsWith("!convert ", StringComparison.OrdinalIgnoreCase))
                return;

            var messageParts = e.Message.Content.Split(" ").ToList();
            if (messageParts.Count <= 1)
                return;

            double quantity = 0;
            var unit = string.Empty;
            if (!double.TryParse(messageParts[1], out quantity))
            {
                var filtered = new string(messageParts[1].Where(c => char.IsDigit(c) || c == '.').ToArray());
                quantity = double.Parse(filtered);
                unit = messageParts[1].Substring(messageParts[1].IndexOf(filtered) + filtered.Length);
            }
            else
            {
                if (messageParts.Count <= 2)
                    return;
                unit = messageParts[2];
            }

            var converted = await Convert(quantity, unit);

            var extra = quantity.ToString().Split(".")[0] == "69"
                ? " (nice)"
                : string.Empty;

            var extra2 = converted?.Quantity.ToString().Split(".")[0] == "69"
                ? " (nice)"
                : string.Empty;

            var message = converted == null
                ? $"I don't know how to convert {unit} into something"
                : $"{quantity}{extra} {unit} is {converted?.Quantity}{extra2} {converted?.Unit}";

            await e.Message.RespondAsync(message);
        }

        private static async Task<(double Quantity, string Unit)?> Convert(double quantity, string unit)
        {
            // machine learning is just if statements
            switch (unit.ToLowerInvariant())
            {
                case "c":
                case "celsius":
                    return (quantity * 1.8 + 32.0, "farenheit");
                case "f":
                case "farenheit":
                    return ((quantity - 32.0) / 1.8, "celsius");

                case "gal":
                case "gallon":
                case "gallons":
                    return ((quantity * 3.78541178), "liters");

                case "liter":
                case "litre":
                case "liters":
                case "litres":
                    return ((quantity / 3.78541178), "gallons");

                case "usd":
                {
                    try
                    {
                        if ((DateTime.Now - _lastFetchUsdCad).TotalHours > 1 || USD_CAD == 0.0)
                        {
                            using var response = await _httpClient.GetAsync(string.Format(CurrencyConvertUrl, "USD_CAD", _config["currency-api-key"]));
                            var json = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), new { USD_CAD = 0.0 });
                            USD_CAD = json.USD_CAD;
                            _lastFetchUsdCad = DateTime.Now;
                        }

                        return (quantity * USD_CAD, "CAD");
                    }
                    catch
                    {
                        break;
                    }
                }

                case "cad":
                {
                    try
                    {
                        if ((DateTime.Now - _lastFetchCadUsd).TotalHours > 1 || CAD_USD == 0.0)
                        {
                            using var response = await _httpClient.GetAsync(string.Format(CurrencyConvertUrl, "CAD_USD", _config["currency-api-key"]));
                            var json = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), new { CAD_USD = 0.0 });
                            CAD_USD = json.CAD_USD;
                            _lastFetchCadUsd = DateTime.Now;
                        }

                        return (quantity * CAD_USD, "USD");
                    }
                    catch
                    {
                        break;
                    }
                }

                case "km":
                case "kilometer":
                case "kilometers":
                case "kilmetre":
                case "kilmetres":
                    return (quantity / 1.609344, "miles");

                case "mi":
                case "miles":
                case "mile":
                    return (quantity * 1.609344, "kilometers");

                case "m":
                case "meter":
                case "meters":
                case "metres":
                case "metre":
                    return (quantity * 3.2808399, "feet");

                case "ft":
                case "foot":
                case "feet":
                    return (quantity / 3.2808399, "meters");

                case "cm":
                case "centimeter":
                case "centimeters":
                    return (quantity / 2.54, "inches");

                case "in":
                case "inches":
                case "inch":
                    return (quantity * 2.54, "centimeters");

                case "#":
                case "lbs":
                case "lb":
                case "pound":
                case "pounds":
                    return (quantity / 2.20462262, "kilograms");

                case "kg":
                case "kilogram":
                case "kilograms":
                    return (quantity * 2.20462262, "pounds");
            }

            return null;
        }
    }
}
