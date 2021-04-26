package tracker.discord;

import net.dv8tion.jda.api.JDA;
import net.dv8tion.jda.api.JDABuilder;
import net.dv8tion.jda.api.entities.Guild;
import net.dv8tion.jda.api.entities.Invite;
import net.dv8tion.jda.api.events.guild.invite.GuildInviteCreateEvent;
import net.dv8tion.jda.api.events.guild.invite.GuildInviteDeleteEvent;
import net.dv8tion.jda.api.events.guild.member.GuildMemberJoinEvent;
import net.dv8tion.jda.api.hooks.ListenerAdapter;
import net.dv8tion.jda.api.requests.GatewayIntent;
import net.dv8tion.jda.api.utils.cache.CacheFlag;

import javax.security.auth.login.LoginException;
import java.time.OffsetDateTime;
import java.util.*;
import java.util.stream.Collectors;

import lombok.extern.log4j.Log4j;
import tracker.cli.ICommandNotify;
import tracker.cli.IExitNotify;
import tracker.database.SqlConnector;
import tracker.database.UserInvite;


@Log4j
public class InvitationTracker extends ListenerAdapter implements ICommandNotify, IExitNotify
{
    private final Map<String, Map<String, Invite>> invites = new HashMap<>();

    private final JDA jda;

    public InvitationTracker(String token) throws LoginException, InterruptedException
    {
        jda = JDABuilder.create(token, GatewayIntent.GUILD_MEMBERS, GatewayIntent.GUILD_INVITES)
                .disableCache(CacheFlag.ACTIVITY, CacheFlag.VOICE_STATE, CacheFlag.EMOTE, CacheFlag.CLIENT_STATUS)
                .addEventListeners(this)
                .build();
        jda.awaitReady();
        for (var guild : jda.getGuilds())
        {
            fetchInvites(guild.getId());
        }
    }

    @Override
    public void onGuildMemberJoin(GuildMemberJoinEvent event)
    {
        log.info("Fetch which invite the member used.");
        var guild = Optional.of(event.getGuild());
        guild.ifPresent(e -> handleMemberJoin(event, e));
    }

    private void handleMemberJoin(GuildMemberJoinEvent event, Guild e)
    {
        if (!invites.containsKey(e.getId()))
        {
            log.warn("New GuildId found, probably if was not configure before runtime, first join will be dropped");
            fetchInvites(e.getId());
        } else if (SqlConnector.isGuildRegistered(e.getId()))
        {
            e.retrieveInvites()
                    .queue(fetchedInvites -> getUsedInvite(fetchedInvites, event.getGuild().getId())
                                    .ifPresent(i -> storeInDB(i, event.getMember().getId()))
                            , failure -> log.warn("Could not fetch invites", failure.getCause()));
        } else
        {
            log.warn("Could not registered, ignoring event: " + e.getId());
        }
    }

    private void storeInDB(Invite usedInvite, String joinedUser)
    {
        var guild = usedInvite.getGuild();
        if (guild != null && usedInvite.getInviter() != null && usedInvite.getChannel() != null)
        {
            var currentDate = new java.sql.Timestamp(Calendar.getInstance().getTime().getTime());
            var timeCreated = new java.sql.Timestamp(usedInvite.getTimeCreated().toInstant().toEpochMilli());

            log.info("Invite link: " + usedInvite.getUrl() +
                    " User joined: " + guild.getName() +
                    " Invited by " + usedInvite.getInviter());
            var userInvite = new UserInvite(guild.getId(), usedInvite.getChannel().getId(), joinedUser, usedInvite.getUrl(),
                    usedInvite.getInviter().getId(), currentDate, timeCreated);
            SqlConnector.writeInviteEntry(userInvite);
        } else
        {
            log.error("Somehow an invalid join link has been used.");
        }

    }

    @Override
    public void onGuildInviteCreate(GuildInviteCreateEvent event)
    {
        log.info("Invite " + event.getUrl() + " created for guild " + event.getGuild().getName());
        fetchInvites(event.getGuild().getId());
    }

    @Override
    public void onGuildInviteDelete(GuildInviteDeleteEvent event)
    {
        log.info("Invite " + event.getUrl() + " deleted for guild " + event.getGuild().getName());
        fetchInvites(event.getGuild().getId());
    }

    public void fetchInvites(String id)
    {
        invites.putIfAbsent(id, new HashMap<>());

        var guild = Optional.ofNullable(jda.getGuildById(id));
        guild.ifPresent(e -> e.retrieveInvites().queue(success ->
        {
            if (success != null)
            {
                invites.get(id).clear();
                for (Invite i : success)
                {
                    invites.get(id).put(i.getCode(), i);
                }
            }
        }, failure -> log.warn("Could not fetch invites", failure.getCause())));

    }

    public Optional<Invite> getUsedInvite(List<Invite> newInvites, String id)
    {
        // easy way to find, invite is already registered
        Optional<Invite> usedInvite = newInvites
                .stream()
                .filter(e -> invites.containsKey(id))
                .filter(e -> invites.get(id).get(e.getCode()) != null)
                .filter(e -> e.getUses() != invites.get(id).get(e.getCode()).getUses())
                .findFirst();

        if (usedInvite.isEmpty())
        {
            // if not found yet, this means the invite expired
            Set<String> expiredInvites = getDiff(newInvites, id);
            if (expiredInvites.size() == 1)
            {
                var i = expiredInvites.toArray()[0].toString();
                usedInvite = Optional.of(invites.get(id).get(i));
            } else if (expiredInvites.size() > 1)
            {
                removeExpiredInvites(expiredInvites, id);
                if (expiredInvites.size() == 1)
                {
                    var i = expiredInvites.toArray()[0].toString();
                    usedInvite = Optional.of(invites.get(id).get(i));
                } else
                {
                    return Optional.empty();
                }
            }
        }
        return usedInvite;
    }

    private void removeExpiredInvites(Set<String> diff, String id)
    {
        for (String i : diff)
        {
            Invite tempInv = invites.get(id).get(i);
            if (tempInv.getMaxAge() != 0
                    && tempInv.getTimeCreated().plusSeconds(tempInv.getMaxAge())
                    .isBefore(OffsetDateTime.now()))
            {
                diff.remove(i);  // if invite expired by time already
            }
        }
    }

    private Set<String> getDiff(List<Invite> newInvites, String id)
    {
        // find difference between old and new invite list, difference are expired invites
        Set<String> diff = invites.get(id).keySet();
        Set<String> updatedInvites = newInvites.stream().map(Invite::getCode).collect(Collectors.toSet());
        diff.removeAll(updatedInvites);
        return diff;
    }

    @Override
    public String fetchInvitationLinks()
    {
        var sb = new StringBuilder();
        for (var invitesPerGuild : invites.entrySet())
        {
            sb.append(invitesPerGuild.getKey())
                    .append(":\n");
            for (var link : invitesPerGuild.getValue().entrySet())
            {
                sb.append("    - ")
                        .append(link.getValue().getUrl())
                        .append(" creator:")
                        .append(link.getValue().getInviter())
                        .append(" uses:")
                        .append(link.getValue().getUses())
                        .append("\n");
            }
        }
        return sb.toString();
    }

    @Override
    public void exit()
    {
        jda.shutdown();
    }
}
