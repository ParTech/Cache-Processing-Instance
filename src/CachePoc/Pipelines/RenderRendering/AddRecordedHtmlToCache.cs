namespace CachePoc.Pipelines.RenderRendering
{
    using System;
    using Sitecore;
    using Sitecore.Mvc.Pipelines.Response.RenderRendering;
    
    public class AddRecordedHtmlToCache : Sitecore.Mvc.Pipelines.Response.RenderRendering.AddRecordedHtmlToCache
    {
        protected override void AddHtmlToCache(string cacheKey, string html, RenderRenderingArgs args)
        {
            var cache = CacheManager.GetHtmlCache(Context.Site?.Name);

            if (cache == null)
            {
                return;
            }

            TimeSpan timeout = this.GetTimeout(args);

            cache.Add(html, cacheKey, timeout);
        }
    }
}
