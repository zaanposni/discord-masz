package tracker;

import tracker.cli.Shell;
import lombok.extern.log4j.Log4j;
import tracker.database.SqlConnector;
import tracker.discord.InvitationTracker;

import javax.security.auth.login.LoginException;


@Log4j
public class Main
{
    public static void main(String[] args)
    {
        log.info("Init System");
        SqlConnector.setLoginName(System.getenv("MYSQL_USER"));
        SqlConnector.setLoginPw(System.getenv("MYSQL_PASSWORD"));
        SqlConnector.setDbName(System.getenv("MYSQL_DATABASE"));
        SqlConnector.setBaseUrl("jdbc:mysql://" + System.getenv("MYSQL_HOST") + ":" + System.getenv("MYSQL_PORT") + "/");
        if (!SqlConnector.testConnection())
        {
            log.warn("Could not connect to DB, did you set MYSQL_USER and MYSQL_PASSWORD?");
            return;
        }

        try
        {
            var invitationTracker = new InvitationTracker(System.getenv("DISCORD_BOT_TOKEN"));
            var shell = new Shell(invitationTracker, invitationTracker);
            shell.start();
        } catch (LoginException e)
        {
            log.warn("Could not connect to Discord, did you set DISCORD_BOT_TOKEN?", e);
            return;
        } catch (InterruptedException e)
        {
            Thread.currentThread().interrupt();
            log.error("JDA interrupted", e);
            return;
        } catch(IllegalStateException e)
        {
            
        }

        log.info("System is running fine");
    }
}
