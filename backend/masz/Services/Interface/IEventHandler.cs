using System.Threading.Tasks;
using masz.Events;

namespace masz.Services
{
    public interface IEventHandler
    {
        event AsyncEventHandler<IdentityRegisteredEventArgs> OnIdentityRegistered;
        event AsyncEventHandler<TokenCreatedEventArgs> OnTokenCreated;
        event AsyncEventHandler<TokenDeletedEventArgs> OnTokenDeleted;
        Task InvokeIdentityRegistered(IdentityRegisteredEventArgs eventArgs);
        Task InvokeTokenCreated(TokenCreatedEventArgs eventArgs);
        Task InvokeTokenDeleted(TokenDeletedEventArgs eventArgs);
    }
}
