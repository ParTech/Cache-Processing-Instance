namespace CachePoc.Providers
{
    using System;

    /// <summary>
    /// Interface for the cache provider.
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Gets the cached object from specified system with specified cache key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The cache key.</param>
        /// <returns>Cached object or null if not found.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Gets the cached object from specified system with specified cache key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The cache key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Cached object or null if not found.</returns>
        T Get<T>(string key, T defaultValue)
            where T : class;

        /// <summary>
        /// Adds the specified object to the cache indefinitely.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="key">The cache key.</param>
        void Add(object obj, string key);

        /// <summary>
        /// Adds the specified object to the cache with a set expiration.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="key">The cache key.</param>
        /// <param name="lifeTimeInMinutes">The cache life time in minutes.</param>
        void Add(object obj, string key, int lifeTimeInMinutes);

        /// <summary>
        /// Adds the specified object to the cache with a set expiration.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="key">The cache key.</param>
        /// <param name="lifeTime">The cache life time.</param>
        void Add(object obj, string key, TimeSpan lifeTime);

        /// <summary>
        /// Adds the specified object to the cache with a set expiration.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="key">The cache key.</param>
        /// <param name="expirationUtcDate">The expiration date.</param>
        void Add(object obj, string key, DateTime expirationUtcDate);

        /// <summary>
        /// Clears all the cache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Serializes the cache to JSON.
        /// </summary>
        /// <returns></returns>
        string Serialize();

        /// <summary>
        /// Overrides the cache with the deserialized JSON.
        /// </summary>
        /// <param name="json">The JSON.</param>
        void Deserialize(string json);
    }
}