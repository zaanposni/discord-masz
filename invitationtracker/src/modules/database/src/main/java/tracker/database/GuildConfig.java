package tracker.database;

import lombok.AllArgsConstructor;
import lombok.Value;

@Value
@AllArgsConstructor
public class GuildConfig
{
    int id;
    String guildId;
    String modRoles;
    String adminRoles;
    String mutedRoles;
    String internalWebhook;
    String publicWebhook;
    Boolean strictModPermissionCheck;
    Boolean executeWhoisOnJoin;
}
