using AutomaticTypeMapper;
using freedman.Configuration;

namespace freedman.Converters.Currency
{
    [AutoMappedType]
    public class ISKConverter : CurrencyConverter
    {
        protected override string Currency => "ISK";

        public ISKConverter(IConfigurationProvider configurationProvider)
            : base(configurationProvider, "^ISK$")
        {
        }
    }
}
