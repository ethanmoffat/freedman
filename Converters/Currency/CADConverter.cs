using AutomaticTypeMapper;
using freedman.Configuration;

namespace freedman.Converters.Currency
{
    [AutoMappedType]
    public class CADConverter : CurrencyConverter
    {
        protected override string Currency => "CAD";

        public CADConverter(IConfigurationProvider configurationProvider)
            : base(configurationProvider, "^CAD$")
        {
        }
    }
}
