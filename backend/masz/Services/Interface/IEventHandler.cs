using System.Threading.Tasks;
using masz.Events;

namespace masz.Services
{
    public interface IEventHandler
    {
        event AsyncEventHandler<IdentityRegisteredEventArgs> OnIdentityRegistered;
        event AsyncEventHandler<TokenCreatedEventArgs> OnTokenCreated;
        event AsyncEventHandler<TokenDeletedEventArgs> OnTokenDeleted;
        event AsyncEventHandler<AutoModerationConfigUpdatedEventArgs> OnAutoModerationConfigUpdated;
        event AsyncEventHandler<AutoModerationConfigDeletedEventArgs> OnAutoModerationConfigDeleted;
        event AsyncEventHandler<AutoModerationEventRegisteredEventArgs> OnAutoModerationEventRegistered;
        event AsyncEventHandler<CaseTemplateCreatedEventArgs> OnCaseTemplateCreated;
        event AsyncEventHandler<CaseTemplateDeletedEventArgs> OnCaseTemplateDeleted;
        Task InvokeIdentityRegistered(IdentityRegisteredEventArgs eventArgs);
        Task InvokeTokenCreated(TokenCreatedEventArgs eventArgs);
        Task InvokeTokenDeleted(TokenDeletedEventArgs eventArgs);
        Task InvokeAutoModerationConfigUpdated(AutoModerationConfigUpdatedEventArgs eventArgs);
        Task InvokeAutoModerationConfigDeleted(AutoModerationConfigDeletedEventArgs eventArgs);
        Task InvokeAutoModerationEventRegistered(AutoModerationEventRegisteredEventArgs eventArgs);
        Task InvokeCaseTemplateCreated(CaseTemplateCreatedEventArgs eventArgs);
        Task InvokeCaseTemplateDeleted(CaseTemplateDeletedEventArgs eventArgs);
    }
}
