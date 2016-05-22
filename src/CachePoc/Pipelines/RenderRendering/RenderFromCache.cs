namespace CachePoc.Pipelines.RenderRendering
{
    using System.IO;
    using Sitecore;
    using Sitecore.Mvc.Pipelines.Response.RenderRendering;

    public class RenderFromCache : Sitecore.Mvc.Pipelines.Response.RenderRendering.RenderFromCache
    {
        protected override bool Render(string cacheKey, TextWriter writer, RenderRenderingArgs args)
        {
            var cache = CacheManager.GetHtmlCache(Context.Site?.Name);
            
            string html = cache?.Get<string>(cacheKey);

            if (html == null)
            {
                return false;
            }

            writer.Write(html);

            return true;
        }
    }
}