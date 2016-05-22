using System.Linq;
using Newtonsoft.Json;

namespace CachePoc.Providers
{
    using System;
    using System.Collections.Generic;
    using Sitecore.Caching;
    using Sitecore.Diagnostics;
    using Sitecore.Mvc.Extensions;
    using Sitecore.Sites;

    /// <summary>
    /// Wrapper around Sitecore HtmlCache object.
    /// </summary>
    public class HtmlCacheProvider : ICacheProvider
    {
        /// <summary>
        /// The HTML cache storage.
        /// </summary>
        private readonly HtmlCache htmlCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlCacheProvider" /> class.
        /// </summary>
        /// <param name="site">The site context.</param>
        public HtmlCacheProvider(SiteContext site)
        {
            this.htmlCache = site.ValueOrDefault(Sitecore.Caching.CacheManager.GetHtmlCache);

            Assert.IsNotNull(this.htmlCache, "htmlCache can not be null");
        }

        /// <summary>
        /// Gets the cached object with specified cache key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns>Cached object or null if not found.</returns>
        public T Get<T>(string key)
        {
            Log.Info($"[CachePoc]: Get from HtmlCache: {key}", this);

            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(this.htmlCache.GetHtml(key), typeof(T));
            }

            return default(T);
        }

        /// <summary>
        /// Gets the cached object with specified cache key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// Cached object or null if not found.
        /// </returns>
        public T Get<T>(string key, T defaultValue)
            where T : class
        {
            return this.Get<T>(key) ?? defaultValue;
        }

        /// <summary>
        /// Adds the specified object to the cache indefinitely.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="key">The cache key.</param>
        public void Add(object obj, string key)
        {
            this.Add(obj, key, DateTime.MinValue);
        }

        /// <summary>
        /// Adds the specified object to the cache with a set expiration.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="key">The cache key.</param>
        /// <param name="lifeTimeInMinutes">The cache life time in minutes.</param>
        public void Add(object obj, string key, int lifeTimeInMinutes)
        {
            this.Add(obj, key, DateTime.Now.AddMinutes(lifeTimeInMinutes));
        }
        
        /// <summary>
        /// Adds the specified object to the cache with a set expiration.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="key">The cache key.</param>
        /// <param name="lifeTime">The cache life time.</param>
        public void Add(object obj, string key, TimeSpan lifeTime)
        {
            if (lifeTime == TimeSpan.Zero)
            {
                this.Add(obj, key);
            }
            else
            {
                this.Add(obj, key, DateTime.UtcNow.Add(lifeTime));
            }
        }

        /// <summary>
        /// Adds the specified object to the cache with a set expiration.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="key">The cache key.</param>
        /// <param name="expirationUtcDate">The expiration date.</param>
        public void Add(object obj, string key, DateTime expirationUtcDate)
        {
            Log.Info($"[CachePoc]: Add to HtmlCache: {key} - expire: {expirationUtcDate}", this);

            if (expirationUtcDate == DateTime.MinValue)
            {
                this.htmlCache.SetHtml(key, (string)obj);
            }
            else
            {
                this.htmlCache.SetHtml(key, (string)obj, expirationUtcDate);
            }
        }

        /// <summary>
        /// Clears the cache for the specified system identifier.
        /// </summary>
        public void Clear()
        {
            this.htmlCache.Clear(true);
        }

        /// <summary>
        /// Serializes the cache to JSON.
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            // Convert cache to dictionary so it can be serialized.
            var dictionary = this.htmlCache.InnerCache.GetCacheKeys()
                .ToArray()
                .ToList()
                .ToDictionary(key => (string)key, key => new CacheEntry(this.htmlCache.InnerCache.GetEntry((string)key, false)));

            try
            {
                return JsonConvert.SerializeObject(dictionary);
            }
            catch (Exception ex)
            {
                Log.Error("[CachePoc]: Could not serialize cache to JSON.", ex, this);
            }

            return null;
        }

        /// <summary>
        /// Overrides the cache with the deserialized JSON.
        /// </summary>
        /// <param name="json">The JSON.</param>
        public void Deserialize(string json)
        {
            try
            {
                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, CacheEntry>>(json);

                this.Clear();

                dictionary.Where(x => !x.Value.Expired)
                    .ToList()
                    .ForEach(x => this.Add(x.Value.Data, x.Key, x.Value.Expiration));
            }
            catch (Exception ex)
            {
                Log.Error("[CachePoc]: Could not deserialize cache JSON.", ex, this);
            }
        }

        /// <summary>
        /// CacheEntry with parameterless constructor for the purpose of serialization.
        /// </summary>
        private class CacheEntry
        {
            public CacheEntry()
            {
            }

            public CacheEntry(Sitecore.Caching.Cache.CacheEntry entry)
            {
                this.Data = entry.Data;
                this.Expired = entry.Expired;
                this.Expiration = entry.AbsoluteExpiration;
            }

            public object Data { get; set; }

            public bool Expired { get; set; }

            public DateTime Expiration { get; set; }
        }
    }
}