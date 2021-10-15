using System;

namespace masz.Events
{
    public class InternalCachingDoneEventArgs : EventArgs
    {
        private int _count;
        private DateTime _nextCache;

        public InternalCachingDoneEventArgs(int count, DateTime nextCache)
        {
             _count = count;
             _nextCache = nextCache;
        }

        public int GetUserEntriesCount()
        {
            return _count;
        }

        public DateTime GetNextCache()
        {
            return _nextCache;
        }
    }
}