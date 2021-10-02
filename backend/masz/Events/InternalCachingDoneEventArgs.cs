using System;

namespace masz.Events
{
    public class InternalCachingDoneEventArgs : EventArgs
    {
        private int _count;

        public InternalCachingDoneEventArgs(int count)
        {
             _count = count;
        }

        public int GetEntriesCount()
        {
            return _count;
        }
    }
}