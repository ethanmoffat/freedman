using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
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
        private static TableClient _tableClient;

        private static IDictionary<ulong, int> _precision;

        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(Environment.GetEnvironmentVariable("ConnectionString"));
            _config = builder.Build();

            _httpClient = new HttpClient();

            _lastFetchUsdCad = DateTime.Now;
            _lastFetchCadUsd = DateTime.Now;

            _precision = new Dictionary<ulong, int>();

            DiscordClient discordClient = null;
            try
            {
                _tableClient = new TableClient(_config["freedman-storage"], "freedmanprecision");

                discordClient = new DiscordClient(new DiscordConfiguration
                {
                    Token = _config["freeman-token"],
                    TokenType = TokenType.Bot
                });

                discordClient.MessageCreated += OnMessageCreated;

                await discordClient.ConnectAsync();
                await Task.Delay(-1);
            }
            finally
            {
                _httpClient.Dispose();
                await discordClient?.DisconnectAsync();
                discordClient?.Dispose();
            }
        }

        private static async Task OnMessageCreated(MessageCreateEventArgs e)
        {
            if (e.Author.IsBot)
                return;

            List<string> messageParts;

            if (e.Message.Content.StartsWith("!precision ", StringComparison.OrdinalIgnoreCase))
            {
                messageParts = e.Message.Content.Split(" ").ToList();
                if (messageParts.Count <= 1)
                    return;

                if (!int.TryParse(messageParts[1], out var precision) || precision < 0 || precision > 10)
                {
                    await e.Message.RespondAsync("Precision must be an integer between 0 and 10");
                    return;
                }

                // todo: connect to cosmos and store precision for a given server
                // todo: cache precision values per server instead of fetching them every time
                PrecisionTableRecord precisionRecord;
                try
                {
                    precisionRecord = await _tableClient.GetEntityAsync<PrecisionTableRecord>($"{e.Guild.Id}", "precision");
                    precisionRecord.Precision = precision;
                    await _tableClient.UpdateEntityAsync(precisionRecord, ETag.All, TableUpdateMode.Replace);
                }
                catch (RequestFailedException)
                {
                    // entity doesn't exist
                    precisionRecord = new PrecisionTableRecord
                    {
                        ETag = ETag.All,
                        PartitionKey = $"{e.Guild.Id}",
                        Timestamp = DateTime.Now,
                        Precision = precision
                    };
                    await _tableClient.AddEntityAsync(precisionRecord);
                }

                _precision[e.Guild.Id] = precision;

                await e.Message.RespondAsync($"Precision for {e.Guild.Name} set to {precision}");
                return;
            }
            else if (e.Message.Content.Equals("!help", StringComparison.OrdinalIgnoreCase))
            {
                await e.Message.RespondAsync("Use `!convert {value} {units}` to convert units or `!precision {value}` to set precision of values");
                return;
            }
            else if (!e.Message.Content.StartsWith("!convert ", StringComparison.OrdinalIgnoreCase))
                return;

            messageParts = e.Message.Content.Split(" ").ToList();
            if (messageParts.Count <= 1)
                return;

            if (!_precision.ContainsKey(e.Guild.Id))
            {
                try
                {
                    PrecisionTableRecord precisionRecord = await _tableClient.GetEntityAsync<PrecisionTableRecord>($"{e.Guild.Id}", "precision");
                    _precision[e.Guild.Id] = precisionRecord.Precision;
                }
                catch (RequestFailedException)
                {
                    // entity doesn't exist
                    _precision[e.Guild.Id] = 4;
                }
            }

            double quantity = 0;
            var unit = string.Empty;
            if (!double.TryParse(messageParts[1], out quantity))
            {
                var filtered = new string(messageParts[1].Where(c => char.IsDigit(c) || c == '.').ToArray());
                quantity = double.Parse(filtered);
                unit = messageParts[1].Substring(messageParts[1].IndexOf(filtered) + filtered.Length);
                if (messageParts.Count > 2 && IsValidSecondWordUnit(unit, messageParts[2]))
                {
                    unit += " " + messageParts[2];
                }
            }
            else
            {
                if (messageParts.Count <= 2)
                    return;
                unit = messageParts[2];
                if (messageParts.Count > 3 && IsValidSecondWordUnit(unit, messageParts[3]))
                {
                    unit += " " + messageParts[3];
                }
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
                : $"{quantity}{extra} {unit} is {Math.Round(converted.Value.Quantity, _precision[e.Guild.Id])}{extra2} {converted?.Unit}";

            await e.Message.RespondAsync(message);
        }

        private static async Task<(double Quantity, string Unit)?> Convert(double quantity, string unit)
        {
            // machine learning is just if statements
            switch (unit.ToLowerInvariant())
            {
                case "c":
                case "celsius":
                case "degrees c":
                case "degrees celsius":
                    return (quantity * 1.8 + 32.0, "farenheit");

                case "f":
                case "farenheit":
                case "degrees f":
                case "degrees farenheit":
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

                case "washroom":
                    return (quantity, "bathroom");
                case "bathroom":
                    return (quantity, "washroom");

                case "ly":
                case "light years":
                case "lightyears":
                    return (quantity * 63241.0771, "AU");

                case "au":
                case "astronomical units":
                    return (quantity / 63241.0771, "light years");

                case "football field":
                case "football fields":
                    return (quantity * 1570.90909090, "big macs");

                case "big mac":
                case "big macs":
                    return (quantity / 1570.90909090, "football fields");
            }

            return null;
        }

        private static bool IsValidSecondWordUnit(string firstPart, string secondPart)
        {
            var secondPartLower = secondPart.ToLower();
            switch (firstPart.ToLower())
            {
                case "degrees":
                    return secondPartLower == "f" || secondPartLower == "c" || secondPartLower == "farenheit" || secondPartLower == "celsius";
                case "fl":
                case "fluid":
                    return secondPartLower == "oz" || secondPartLower == "ozs" || secondPartLower == "ounce" || secondPartLower == "ounces";
                case "light":
                    return secondPartLower == "years";
                case "astronomical":
                    return secondPartLower == "units";
                case "big":
                    return secondPartLower == "mac" || secondPartLower == "macs";
                case "football":
                    return secondPartLower == "field" || secondPartLower == "fields";
            }

            return false;
        }
    }
}
