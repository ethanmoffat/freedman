using AutomaticTypeMapper;
using freedman.Configuration;

namespace freedman.Converters.Currency
{
    [AutoMappedType]
    public class GBPConverter : CurrencyConverter
    {
        protected override string Currency => "GBP";

        public GBPConverter(IConfigurationProvider configurationProvider)
            : base(configurationProvider, "^GBP$")
        {
        }
    }
}
