package tracker.database;

import lombok.Setter;
import lombok.extern.log4j.Log4j;

import java.sql.*;

@Log4j
public class SqlConnector
{
    @Setter
    private static String dbName = "masz";
    @Setter
    private static String tableName = "UserInvites";
    @Setter
    private static String tableNameGuildConfig = "GuildConfigs";
    @Setter
    private static String baseUrl = "jdbc:mysql://localhost/";
    @Setter
    private static String urlConfigs = "?useUnicode=true&useJDBCCompliantTimezoneShift=true&useLegacyDatetimeCode=false&serverTimezone=UTC";
    @Setter
    private static String loginName;
    @Setter
    private static String loginPw;

    private SqlConnector()
    {
        // hide public constructor
    }

    public static boolean testConnection()
    {
        try
        {
            DriverManager.getConnection(baseUrl + dbName + urlConfigs, loginName, loginPw);
            return true;
        } catch (SQLException e)
        {
            log.error("Could not connect to Database", e);
        }
        return false;
    }

    public static boolean isGuildRegistered(String guildId)
    {
        try (var connect = DriverManager.getConnection(baseUrl + dbName + urlConfigs, loginName, loginPw);
             var statement = connect.createStatement())
        {
            var query = "select * from " + tableNameGuildConfig + " where GuildId = \"" + guildId + "\"";
            var result = statement.executeQuery(query);
            if (result == null)
            {
                return false;
            }
            result.beforeFirst();
            result.last();
            var size = result.getRow();
            if (size < 1)
            {
                return false;
            }
            return result.getString("GuildId").equals(guildId);
        } catch (Exception e)
        {
            log.error("Could not update database", e);
        }
        return false;
    }

    public static GuildConfig getGuildConfig(String guildId)
    {
        try (var connect = DriverManager.getConnection(baseUrl + dbName + urlConfigs, loginName, loginPw);
             var statement = connect.createStatement())
        {
            var query = "select * from " + tableNameGuildConfig + " where GuildId = \"" + guildId + "\"";
            var result = statement.executeQuery(query);
            if (result == null)
            {
                return null;
            }
            result.beforeFirst();
            result.last();
            var size = result.getRow();
            if (size < 1)
            {
                return null;
            }
            return new GuildConfig(
                result.getInt("Id"),
                result.getString("GuildId"),
                result.getString("ModRoles"),
                result.getString("AdminRoles"),
                result.getString("MutedRoles"),
                result.getString("ModInternalNotificationWebhook"),
                result.getString("ModPublicNotificationWebhook"),
                result.getBoolean("StrictModPermissionCheck"),
                result.getBoolean("ExecuteWhoisOnJoin")
            );
        } catch (Exception e)
        {
            log.error("Could not update database", e);
        }
        return null;
    }

    public static void writeInviteEntry(UserInvite invite)
    {
        try (var connect = DriverManager.getConnection(baseUrl + dbName + urlConfigs, loginName, loginPw);
             var preparedStatement = connect.prepareStatement("insert into " + dbName + "." + tableName
                     + " values (default, ?, ?, ?, ? , ?, ?, ?)"))
        {
            preparedStatement.setString(1, invite.getGuildId());
            preparedStatement.setString(2, invite.getTargetChannelId());
            preparedStatement.setString(3, invite.getJoinedUserID());
            preparedStatement.setString(4, invite.getUsedInvite());
            preparedStatement.setString(5, invite.getInviteIssuerId());
            preparedStatement.setTimestamp(6, invite.getJoinedAt());
            preparedStatement.setTimestamp(7, invite.getInviteCreatedAt());
            preparedStatement.executeUpdate();
        } catch (Exception e)
        {
            log.error("Could not update database", e);
        }
    }
}