namespace MASZ.Events
{
    public class InternalCachingDoneEventArgs : EventArgs
    {
        private readonly int _count;
        private readonly DateTime _nextCache;

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