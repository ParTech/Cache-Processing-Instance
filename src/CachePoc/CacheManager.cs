namespace CachePoc
{
    using System.Collections.Generic;
    using System.Net;
    using CachePoc.Events.EventQueue;
    using CachePoc.Providers;
    using Sitecore.Diagnostics;
    using Sitecore.Mvc.Extensions;

    public class CacheManager
    {
        private static readonly Dictionary<string, ICacheProvider> Caches = new Dictionary<string, ICacheProvider>();

        /// <summary>
        /// Gets the HTML cache provider.
        /// </summary>
        /// <param name="siteName">Name of the site.</param>
        /// <returns></returns>
        public static ICacheProvider GetHtmlCache(string siteName)
        {
            if (siteName == null)
            {
                return null;
            }

            if (!Caches.ContainsKey(siteName))
            {
                // TODO: This can be extended to use configuration to determine what cache provider to use.
                var provider = Sitecore.Context.Site.ValueOrDefault(site => new HtmlCacheProvider(site));

                if (provider == null)
                {
                    return null;
                }

                Caches.Add(siteName, provider);
            }

            return Caches[siteName];
        }

        /// <summary>
        /// Called when cache must be rebuild (i.e. after publishing has finished).
        /// </summary>
        public static void OnRebuildCache()
        {
            Log.Info("[CachePoc]: CacheManager.OnRebuildCache is called.", typeof(CacheManager));

            // Add rebuild cache event to the EventQueue so that the Cache Processing instance can handle it.
            Sitecore.Eventing.EventManager.QueueEvent(new RebuildCacheEvent());
        }

        /// <summary>
        /// Rebuilds the cache and publishes it to other instances using the event queue.
        /// </summary>
        /// <param name="evt">The evt.</param>
        public static void RebuildCache(RebuildCacheEvent evt)
        {
            // This will only be called on instances that have subscribed to the RebuildCacheEvent.
            // That should only be the Cache Processing instance.

            Log.Info("[CachePoc]: CacheManager.RebuildCache is called.", typeof(CacheManager));

            var cache = GetHtmlCache("website");

            if (cache == null)
            {
                return;
            }

            // Rebuild the cache by first clearing it and then requesting warmup pages using HTTP requests.
            cache.Clear();
            new WebClient().DownloadString("http://cp.cache-poc/cached");

            // TODO: Call other warmup pages.
            // TODO: Add logic to configure or automatically detect warmup pages.

            // Cache for warmup pages is now rebuilt.
            // Serialize it and submit it to the event queue.
            string serialized = GetHtmlCache("website")?.Serialize();

            if (serialized != null)
            {
                Sitecore.Eventing.EventManager.QueueEvent(new CacheReadyEvent(serialized));
            }
        }

        /// <summary>
        /// Updates the cache using the published cache from the event queue.
        /// </summary>
        /// <param name="evt">The evt.</param>
        public static void UpdateCache(CacheReadyEvent evt)
        {
            // This will only be called on instances that have subscribed to the CacheReadyEvent.
            // That should only be the Content Management & Delivery instances.

            Log.Info("[CachePoc]: CacheManager.UpdateCache is called.", typeof(CacheManager));

            GetHtmlCache("website")?.Deserialize(evt.Data);
        }
    }
}