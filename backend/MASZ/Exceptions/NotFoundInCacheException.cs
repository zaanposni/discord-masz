namespace MASZ.Exceptions
{
    public class NotFoundInCacheException : BaseException
    {
        public string CacheKey { get; set; }
        public NotFoundInCacheException(string message, string cacheKey) : base(message)
        {
            CacheKey = cacheKey;
        }
        public NotFoundInCacheException(string cacheKey) : base($"'{cacheKey}' is not cached.")
        {
            CacheKey = cacheKey;
        }
        public NotFoundInCacheException() : base("Failed to find key in local cache.")
        {
        }
    }
}