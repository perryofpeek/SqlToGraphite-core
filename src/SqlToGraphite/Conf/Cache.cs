using System;

using log4net;

namespace SqlToGraphite.Conf
{
    public class Cache : ICache
    {
        private readonly TimeSpan cacheLength;

        private readonly ILog log;

        private DateTime nextExpireTime;

        public Cache(TimeSpan cacheLength, ILog log)
        {
            this.cacheLength = cacheLength;
            this.log = log;
        }

        public void ResetCache()
        {
            nextExpireTime = DateTime.Now.Add(this.cacheLength);
            log.Debug(string.Format("Next cache expire time {0}", nextExpireTime));
        }

        public bool HasExpired()
        {
            var hasExpired = DateTime.Now >= this.nextExpireTime;
            log.Debug(string.Format("Cache expired {0}", hasExpired));
            return hasExpired;
        }
    }
}