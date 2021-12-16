namespace MASZ.Models
{
    public class CacheApiResponse
    {
        private object Content { get; set; }
        private DateTime ExpiresAt { get; set; }

        public CacheApiResponse(object content, int cacheMinutes = 30)
        {
            Content = content;
            ExpiresAt = DateTime.Now.AddMinutes(cacheMinutes);
        }

        public T GetContent<T>()
        {
            return (T)Content;
        }
        public bool IsExpired()
        {
            return DateTime.Now > ExpiresAt;
        }
    }
}
