using MASZ.Enums;

namespace MASZ.Services
{
    public interface IInternalConfiguration
    {
        void Init();
        string GetBotToken();
        string GetClientId();
        string GetClientSecret();
        string GetFileUploadPath();
        string GetHostName();
        string GetServiceDomain();
        string GetBaseUrl();
        List<ulong> GetSiteAdmins();
        Language GetDefaultLanguage();
        string GetAuditLogWebhook();
        bool IsPublicFileEnabled();
        bool IsDemoModeEnabled();
        bool IsCustomPluginModeEnabled();
        bool IsCorsEnabled();
    }
}
