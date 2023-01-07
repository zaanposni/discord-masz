using Discord;
using Discord.Interactions;
using MASZ.Enums;
using MASZ.Models;
using MASZ.Services;
using MASZ.Utils;
using MASZ.Extensions;

namespace MASZ.Commands
{

    public class BaseCommand<T> : InteractionModuleBase<SocketInteractionContext>
    {

        public ILogger<T> Logger { get; set; }
        public Translator Translator { get; set; }
        public IdentityManager IdentityManager { get; set; }
        public InternalConfiguration Config { get; set; }
        public DiscordAPIInterface DiscordAPI { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
        public InternalEventHandler eventHandler { get; set; }

        public Identity CurrentIdentity;

        public override void BeforeExecute(ICommandInfo command)
        {
            if (Context.Channel is ITextChannel)
            {
                Logger.LogInformation($"{Context.User.Id} used {command.Name} in {Context.Channel.Id} | {Context.Guild.Id} {Context.Guild.Name}");
            }
            else
            {
                Logger.LogInformation($"{Context.User.Id} used {command.Name} in DM");
            }

            RunCMDSetup().GetAwaiter().GetResult();
        }

        public override void AfterExecute(ICommandInfo command)
        {
            eventHandler.OnApplicationCommandUsedEvent.InvokeAsync(command, Context.User, Context.Guild);
        }

        private async Task RunCMDSetup() {
            Language? userLocale = LocaleHelper.DiscordLocaleToMASZLocale(Context.Interaction.UserLocale);
            if (userLocale.HasValue)
            {
                Translator.SetContext(userLocale.Value);
            } else {
                if (Context.Guild != null)
                {
                    await Translator.SetContext(Context.Guild.Id);
                }
            }

            CurrentIdentity = await IdentityManager.GetIdentity(Context.User);

            if (CurrentIdentity == null)
            {
                Logger.LogError($"Failed to register command identity for '{Context.User.Id}'.");
                return;
            }
        }
    }
}