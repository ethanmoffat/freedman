using AutomaticTypeMapper;
using freedman.Configuration;

namespace freedman.Converters.Currency
{
    [AutoMappedType]
    public class MXNConverter : CurrencyConverter
    {
        protected override string Currency => "MXN";

        public MXNConverter(IConfigurationProvider configurationProvider)
            : base(configurationProvider, "^MXN$")
        {
        }
    }
}
