using AutomaticTypeMapper;
using System.Collections.Generic;

namespace freedman.Precision
{
    [AutoMappedType(IsSingleton = true)]
    public class PrecisionCacheRepository : IPrecisionCacheRepository
    {
        public IDictionary<ulong, int> PrecisionCache { get; set; } = new Dictionary<ulong, int>();
    }

    public interface IPrecisionCacheRepository
    {
        IDictionary<ulong, int> PrecisionCache { get; set; }
    }
}
