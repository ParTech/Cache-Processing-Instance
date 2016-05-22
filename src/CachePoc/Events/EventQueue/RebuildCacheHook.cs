namespace CachePoc.Events.EventQueue
{
    using System;
    using Sitecore.Diagnostics;
    using Sitecore.Events.Hooks;

    public class RebuildCacheHook : IHook
    {
        public void Initialize()
        {
            Log.Info("[CachePoc]: RebuildCacheHook", typeof(RebuildCacheHook));

            Sitecore.Eventing.EventManager.Subscribe(new Action<RebuildCacheEvent>(CacheManager.RebuildCache));
        }
    }
}