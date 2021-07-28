using AutomaticTypeMapper;
using freedman.Configuration;

namespace freedman.Converters.Currency
{
    [AutoMappedType]
    public class JPYConverter : CurrencyConverter
    {
        protected override string Currency => "JPY";

        public JPYConverter(IConfigurationProvider configurationProvider)
            : base(configurationProvider, "^JPY$")
        {
        }
    }
}
