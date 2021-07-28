using Azure;
using freedman.Configuration;
using freedman.Unit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace freedman.Converters.Currency
{
    public abstract class CurrencyConverter : IUnitConverter, IDisposable
    {
        private const string CurrencyConvertUrl = "https://free.currconv.com/api/v7/convert?q={0}&compact=ultra&apiKey={1}";

        private static readonly IDictionary<string, double> _cachedRates;
        private static readonly IDictionary<string, DateTime> _fetchTimes;

        static CurrencyConverter()
        {
            _cachedRates = new Dictionary<string, double>();
            _fetchTimes = new Dictionary<string, DateTime>();
        }

        private readonly IConfigurationProvider _configurationProvider;
        private readonly Regex _matcher;
        private readonly HttpClient _httpClient;

        public IUnit DefaultTarget => new Unit.Currency(0, "USD");

        protected abstract string Currency { get; }

        protected CurrencyConverter(IConfigurationProvider configurationProvider, string pattern)
        {
            _matcher = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            _httpClient = new HttpClient();
            _configurationProvider = configurationProvider;
        }

        public bool IsConverterFor(IUnit unit)
            => _matcher.IsMatch(unit.Units);

        public IUnit FromSIUnit(IUnit source)
        {
            if (Currency == "USD")
                return source;

            // todo: convert everything to async
            return ConvertCurrency(source.Value, "USD", Currency).GetAwaiter().GetResult();
        }

        public IUnit ToSIUnit(IUnit source)
        {
            if (Currency == "USD")
                return source;

            return ConvertCurrency(source.Value, source.Units, "USD").GetAwaiter().GetResult();
        }

        public IUnit UnitFactory(double value, string units)
        {
            return new Unit.Currency(value, units);
        }

        private async Task<IUnit> ConvertCurrency(double value, string sourceCurrency, string targetCurrency)
        {
            sourceCurrency = sourceCurrency.ToUpper();
            targetCurrency = targetCurrency.ToUpper();

            var currencyString = sourceCurrency == "USD"
                ? $"{sourceCurrency}_{targetCurrency}"
                : $"{targetCurrency}_{sourceCurrency}";
            var invert = sourceCurrency != "USD";

            if (!_cachedRates.ContainsKey(currencyString) || !_fetchTimes.ContainsKey(currencyString) || (DateTime.Now - _fetchTimes[currencyString]).TotalHours > 1)
            {
                using var response = await _httpClient.GetAsync(string.Format(CurrencyConvertUrl, currencyString, _configurationProvider.Configuration["currency-api-key"]));
                if (!response.IsSuccessStatusCode)
                    throw new RequestFailedException($"Unable to contact currency conversion API (code {response.StatusCode})\nService status: https://www.currencyconverterapi.com/server-status");
                var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                _cachedRates[currencyString] = json[currencyString].Value<double>();
                _fetchTimes[currencyString] = DateTime.Now;
            }

            var convertedValue = invert
                ? value / _cachedRates[currencyString]
                : value * _cachedRates[currencyString];
            return UnitFactory(convertedValue, targetCurrency);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
