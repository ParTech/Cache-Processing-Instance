namespace CachePoc.Events.PublishEnd
{
    using System;
    using System.Collections;
    
    public class RebuildCache
    {
        public RebuildCache()
        {
            this.Sites = new ArrayList();
        }

        public ArrayList Sites { get; set; }

        public void OnRebuildCache(object sender, EventArgs args)
        {
            CacheManager.OnRebuildCache();
        }
    }
}