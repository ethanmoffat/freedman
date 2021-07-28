using AutomaticTypeMapper;
using Microsoft.Extensions.Configuration;
using System;

namespace freedman.Configuration
{
    [AutoMappedType]
    public class ConfigurationProvider : IConfigurationProvider
    {
        public IConfigurationRoot Configuration { get; }

        public ConfigurationProvider()
        {
            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(Environment.GetEnvironmentVariable("ConnectionString"));
            Configuration = builder.Build();
        }
    }
}
