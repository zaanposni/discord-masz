namespace MASZ.InviteTracking
{
    public static class InviteTracker
    {
        private static readonly Dictionary<ulong, List<TrackedInvite>> GuildInvites = new();

        public static TrackedInvite GetUsedInvite(ulong guildId, List<TrackedInvite> currentInvites)
        {
            if (GuildInvites.ContainsKey(guildId))
            {
                List<TrackedInvite> invites = GuildInvites[guildId];
                List<TrackedInvite> changedInvites = invites.Where(x =>
                    (currentInvites.Find(c => c.Code == x.Code) != null && x.HasNewUses(currentInvites.Find(c => c.Code == x.Code).Uses)) ||  // where invite is in current invites and has new uses
                    (x.MaxUses.GetValueOrDefault(0) - 1 == x.Uses && currentInvites.Find(c => c.Code == x.Code) == null)  // where invite is not in current invites and maybe expired via max uses
                ).ToList();

                if (changedInvites.Count == 1)
                {
                    return changedInvites.First();
                }

                List<TrackedInvite> notExpiredInvites = changedInvites.Where(x => !x.IsExpired()).ToList();
                if (notExpiredInvites.Count == 1)
                {
                    return notExpiredInvites.First();
                }

                return null;
            }
            return null;
        }

        public static void AddInvites(ulong guildId, List<TrackedInvite> invites)
        {
            GuildInvites[guildId] = invites;
        }

        public static void AddInvite(ulong guildId, TrackedInvite invite)
        {
            if (!GuildInvites.ContainsKey(guildId))
            {
                GuildInvites[guildId] = new List<TrackedInvite>() { invite };
            }
            else
            {
                List<TrackedInvite> invites = GuildInvites[guildId];
                List<TrackedInvite> filteredInvites = invites.Where(x => x.Code != invite.Code).ToList();
                filteredInvites.Add(invite);
                GuildInvites[guildId] = filteredInvites;
            }
        }

        public static List<TrackedInvite> RemoveInvite(ulong guildId, string code)
        {
            if (GuildInvites.ContainsKey(guildId))
            {
                var invites = GuildInvites[guildId].TakeWhile(x => x.Code == code);
                GuildInvites[guildId].RemoveAll(x => x.Code == code);
                return invites.ToList();
            }
            return new List<TrackedInvite>();
        }
    }
}