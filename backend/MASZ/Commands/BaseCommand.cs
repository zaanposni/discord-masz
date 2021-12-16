using Discord;
using Discord.Interactions;
using MASZ.Models;
using MASZ.Services;

namespace MASZ.Commands
{

    public class BaseCommand<T> : InteractionModuleBase<SocketInteractionContext>
    {

        protected ILogger<T> Logger { get; set; }
        protected Translator Translator { get; set; }
        protected IdentityManager IdentityManager { get; set; }
        protected Identity CurrentIdentity { get; set; }
        protected InternalConfiguration Config { get; set; }
        protected DiscordAPIInterface DiscordAPI { get; set; }
        protected IServiceProvider ServiceProvider { get; set; }

        public BaseCommand(IServiceProvider serviceProvider)
        {
            Logger = (ILogger<T>)serviceProvider.GetRequiredService(typeof(ILogger<T>));
            Translator = (Translator)serviceProvider.GetRequiredService(typeof(Translator));
            IdentityManager = (IdentityManager)serviceProvider.GetRequiredService(typeof(IdentityManager));
            Config = (InternalConfiguration)serviceProvider.GetRequiredService(typeof(InternalConfiguration));
            DiscordAPI = (DiscordAPIInterface)serviceProvider.GetRequiredService(typeof(DiscordAPIInterface));
            ServiceProvider = serviceProvider;
        }

        public override async void BeforeExecute(ICommandInfo command)
        {
            if (Context.Channel is ITextChannel)
            {
                Logger.LogInformation($"{Context.User.Id} used {command.Name} in {Context.Channel.Id} | {Context.Guild.Id} {Context.Guild.Name}");
            }
            else
            {
                Logger.LogInformation($"{Context.User.Id} used {command.Name} in DM");
            }

            CurrentIdentity = await IdentityManager.GetIdentity(Context.User);

            if (CurrentIdentity == null)
            {
                Logger.LogError($"Failed to register command identity for '{Context.User.Id}'.");
                return;
            }

            if (Context.Guild != null)
            {
                await Translator.SetContext(Context.Guild.Id);
            }
        }

    }
}