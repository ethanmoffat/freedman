using AutomaticTypeMapper;
using Microsoft.Extensions.Configuration;
using System;

namespace freedman.Configuration
{
    [AutoMappedType(IsSingleton = true)]
    public class ConfigurationProvider : IConfigurationProvider
    {
        public IConfigurationRoot Configuration { get; }

        public ConfigurationProvider()
        {
            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(Environment.GetEnvironmentVariable("FreedmanConnectionString", EnvironmentVariableTarget.User) ?? Environment.GetEnvironmentVariable("FreedmanConnectionString"));
            Configuration = builder.Build();
        }
    }
}
