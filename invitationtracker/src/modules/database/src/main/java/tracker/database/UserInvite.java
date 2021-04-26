package tracker.database;

import lombok.AllArgsConstructor;
import lombok.Value;

import java.sql.Timestamp;

@Value
@AllArgsConstructor
public class UserInvite
{
    String guildId;
    String targetChannelId;
    String joinedUserID;
    String usedInvite;
    String inviteIssuerId;
    Timestamp joinedAt;
    Timestamp inviteCreatedAt;
}
