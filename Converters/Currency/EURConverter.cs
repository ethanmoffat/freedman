using AutomaticTypeMapper;
using freedman.Configuration;

namespace freedman.Converters.Currency
{
    [AutoMappedType]
    public class EURConverter : CurrencyConverter
    {
        protected override string Currency => "EUR";

        public EURConverter(IConfigurationProvider configurationProvider)
            : base(configurationProvider, "^EUR$")
        {
        }
    }
}
