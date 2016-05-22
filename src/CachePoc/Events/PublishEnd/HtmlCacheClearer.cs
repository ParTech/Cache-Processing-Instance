using System.Collections;
using Sitecore.Diagnostics;

namespace CachePoc.Events.PublishEnd
{
    using System;

    public class HtmlCacheClearer
    {
        public HtmlCacheClearer()
        {
            this.Sites = new ArrayList();
        }

        public ArrayList Sites { get; set; }

        public void ClearCache(object sender, EventArgs args)
        {
            foreach (object site in this.Sites)
            {
                string siteName = site as string;

                if (siteName == null)
                {
                    continue;
                }

                Log.Info($"[CachePoc]: Clear HTML cache for {siteName}", this);

                CacheManager.GetHtmlCache(siteName)?.Clear();
            }
        }
    }
}