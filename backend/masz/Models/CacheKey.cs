namespace masz.Models
{
    public class CacheKey
    {
        private string _key;
        private CacheKey(string key)
        {
            _key = key;
        }
        public string GetValue()
        {
            return _key;
        }
        public static CacheKey User(ulong userId) => new CacheKey($"u:{userId}");
        public static CacheKey Guild(ulong guildId) => new CacheKey($"g:{guildId}");
        public static CacheKey GuildBans(ulong guildId) => new CacheKey($"g:{guildId}:b");
        public static CacheKey GuildBan(ulong guildId, ulong userId) => new CacheKey($"g:{guildId}:b:{userId}");
        public static CacheKey GuildMembers(ulong guildId) => new CacheKey($"g:{guildId}:m");
        public static CacheKey GuildMember(ulong guildId, ulong userId) => new CacheKey($"g:{guildId}:m:{userId}");
        public static CacheKey GuildChannels(ulong guildId) => new CacheKey($"g:{guildId}:c");
        public static CacheKey DMChannel(ulong userId) => new CacheKey($"c:{userId}");
        public static CacheKey TokenUser(string token) => new CacheKey($"t:{token}");
        public static CacheKey TokenUserGuilds(string token) => new CacheKey($"t:{token}:g");
    }
}