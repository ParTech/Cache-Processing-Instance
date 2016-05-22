namespace CachePoc.Events.EventQueue
{
    using System.Runtime.Serialization;

    [DataContract]
    public class CacheReadyEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheReadyEvent"/> class.
        /// </summary>
        /// <param name="data">The cache data.</param>
        public CacheReadyEvent(string data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Gets or sets the cache data.
        /// </summary>
        [DataMember]
        public string Data { get; set; }
    }
}
