using AutomaticTypeMapper;
using Azure.Data.Tables;
using freedman.Configuration;

namespace freedman.Precision
{
    [AutoMappedType(IsSingleton = true)]
    public class PrecisionTableProvider : IPrecisionTableProvider
    {
        public TableClient TableClient { get; }

        public PrecisionTableProvider(IConfigurationProvider configurationProvider)
        {
            TableClient = new TableClient(configurationProvider.Configuration["freedman-storage"], configurationProvider.Configuration["freedman-precisiontablename"]);
        }
    }

    public interface IPrecisionTableProvider
    {
        TableClient TableClient { get; }
    }
}
