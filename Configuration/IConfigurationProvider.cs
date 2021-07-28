using Microsoft.Extensions.Configuration;

namespace freedman.Configuration
{
    public interface IConfigurationProvider
    {
        IConfigurationRoot Configuration { get; }
    }
}
