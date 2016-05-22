namespace CachePoc.Events.EventQueue
{
    using System;
    using Sitecore.Diagnostics;
    using Sitecore.Events.Hooks;

    public class CacheReadyHook : IHook
    {
        public void Initialize()
        {
            Log.Info("[CachePoc]: CacheReadyHook", typeof(CacheReadyHook));

            Sitecore.Eventing.EventManager.Subscribe(new Action<CacheReadyEvent>(CacheManager.UpdateCache));
        }
    }
}