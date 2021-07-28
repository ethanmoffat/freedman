using AutomaticTypeMapper;
using freedman.Configuration;

namespace freedman.Converters.Currency
{
    [AutoMappedType]
    public class USDConverter : CurrencyConverter
    {
        protected override string Currency => "USD";

        public USDConverter(IConfigurationProvider configurationProvider)
            : base(configurationProvider, "^USD$")
        {
        }
    }
}
