using MASZ.Enums;
using Discord.Interactions;
using Discord.WebSocket;
using MASZ.Models;
using MASZ.Services;
using Discord;

namespace MASZ.Commands;

public abstract class BaseModalInteraction<T>
{
    public Identity CurrentIdentity;
    public Translator Translator { get; set; }
    public IdentityManager IdentityManager { get; set; }
    public InternalConfiguration Config { get; set; }
    public IServiceProvider ServiceProvider { get; set; }
    private IUser User { get; set; }

    public BaseModalInteraction(IServiceProvider serviceProvider, IUser user)
    {
        ServiceProvider = serviceProvider;
        Translator = serviceProvider.GetService<Translator>();
        IdentityManager = serviceProvider.GetService<IdentityManager>();
        Config = serviceProvider.GetService<InternalConfiguration>();
        User = user;
    }

    public async Task BeforeExecute()
    {
        CurrentIdentity = await IdentityManager.GetIdentity(User);
    }

    public abstract RequireCheckEnum[] _checks { get; }
    public abstract Task<PreconditionResult> CheckRequirementsAsync(SocketModal modal, IServiceProvider services);
    public abstract Task HandleModal(SocketModal modal);
}