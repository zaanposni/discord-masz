namespace masz.Exceptions
{
    public class UnregisteredGuildException : BaseAPIException
    {
        public ulong GuildId { get; set; }
        public UnregisteredGuildException(string message, ulong guildId) : base(message)
        {
            GuildId = guildId;
        }
        public UnregisteredGuildException(ulong guildId) : base("Guild is not registered.")
        {
            GuildId = guildId;
        }
        public UnregisteredGuildException() : base("Guild is not registered.")
        {
        }
    }
}