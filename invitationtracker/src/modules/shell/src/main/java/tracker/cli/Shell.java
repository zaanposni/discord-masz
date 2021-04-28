package tracker.cli;


import lombok.extern.log4j.Log4j;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;

@Log4j
public class Shell implements Runnable, IExitNotify
{
    private final Thread thread;
    private boolean running = true;

    private CommandParser parser;

    public Shell(ICommandNotify commandObserver, IExitNotify notify)
    {
        parser = new CommandParser(commandObserver);
        parser.addExitNotify(this);
        parser.addExitNotify(notify);
        thread = new Thread(this);
        log.info("Shell ready, type \"help\" for more information");
    }

    public void start()
    {
        thread.start();
    }

    @Override
    public void run()
    {
        try (var reader = new BufferedReader(new InputStreamReader(System.in));)
        {
            String line;
            while ((line = reader.readLine()) != null)
            {
                String ret = parser.parseAndExecute(line);
                System.out.print("$" + line + "\n" + ret + "\n\n");
            }
        } catch (IOException e)
        {
            e.printStackTrace();
        }
    }

    public void setRunning(boolean running)
    {
        this.running = running;
    }

    @Override
    public void exit()
    {
        setRunning(false);
        parser = null;
        System.out.println("closing shell");
    }
}
