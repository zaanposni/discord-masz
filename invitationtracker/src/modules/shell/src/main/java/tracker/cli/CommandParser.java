package tracker.cli;

import java.util.ArrayList;
import java.util.List;

public class CommandParser
{
    private final ICommandNotify commandObserver;
    private final List<IExitNotify> exitNotifyList = new ArrayList<>();

    public CommandParser(ICommandNotify commandObserver)
    {
        this.commandObserver = commandObserver;
    }

    public void addExitNotify(IExitNotify notify)
    {
        exitNotifyList.add(notify);
    }


    public String parseAndExecute(String line)
    {
        if ("help".equals(line))
        {
            return
                    "\navailable commands:\n" +
                            "    help:                  show this text\n" +
                            "    fetch_invitations:     prints all tracked invitation links\n" +
                            "    exit:                  terminate application\n";
        } else if ("fetch_invitations".equals(line))
        {
            return commandObserver.fetchInvitationLinks();
        } else if ("exit".equals(line))
        {
            exitNotifyList.forEach(IExitNotify::exit);
            return "Exiting...";
        } else
        {
            return "<invalid command>";
        }
    }
}
