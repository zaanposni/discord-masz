namespace masz.Enums
{
    public enum APIError
    {
        Unknown = 0,
        InvalidDiscordUser = 1,
        ProtectedModCaseSuspect = 2,
        ProtectedModCaseSuspectIsBot = 3,
        ProtectedModCaseSuspectIsSiteAdmin = 4,
        ProtectedModCaseSuspectIsTeam = 5,
        ResourceNotFound = 6,
        InvalidIdentity = 7,
        GuildUnregistered = 8,
    }
}