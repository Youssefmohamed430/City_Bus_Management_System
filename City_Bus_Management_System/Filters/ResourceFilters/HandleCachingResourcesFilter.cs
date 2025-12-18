using City_Bus_Management_System.Attributes;
using City_Bus_Management_System.DataLayer.Entities;
using Data_Access_Layer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace City_Bus_Management_System.Filters.ResourceFilters
{
    public class HandleCachingResourcesFilter(IMemoryCache cache,ILogger<HandleCachingResourcesFilter> logger) : IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            var endpoint = context.HttpContext.GetEndpoint();
            if (endpoint == null) return;

            var cacheAttr = endpoint.Metadata.GetMetadata<CacheableAttribute>();
            if (cacheAttr == null) return;

            if (context.Result is OkObjectResult okResult)
            {
                cache.Set(cacheAttr.Key, okResult.Value, TimeSpan.FromMinutes(10));
            }
            logger.LogInformation("Resource executed and cached with key: {CacheKey}", cacheAttr.Key);
        }


        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var endpoint = context.HttpContext.GetEndpoint();
            if (endpoint == null)
                return;

            var cacheAttr = endpoint.Metadata.GetMetadata<CacheableAttribute>();
            if (cacheAttr == null) return;

            if (cache.TryGetValue(cacheAttr.Key, out object cachedData))
            {
                context.Result = new OkObjectResult(cachedData);
            }

            logger.LogInformation("Checked cache for key: {CacheKey}", cacheAttr.Key);
        }
    }
}
