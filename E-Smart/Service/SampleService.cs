using Microsoft.Extensions.Caching.Distributed;

namespace E_Smart.Service
{
   
    public class SampleService
    {
        private readonly IDistributedCache _cache;
        public SampleService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<string>GetCacheDataAsync(string key)
        {
            var cachedData = await _cache.GetStringAsync(key);
            if (cachedData == null)
            {
                // Nếu không có dữ liệu trong cache, thực hiện logic khác
                cachedData = "Data from other source";
                await _cache.SetStringAsync(key, cachedData);
            }
            return cachedData;
        }
    }
}
