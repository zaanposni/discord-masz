using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;

namespace MASZ.Repositories
{

    public class AppSettingsRepository : BaseRepository<AppSettingsRepository>
    {
        private AppSettingsRepository(IServiceProvider serviceProvider) : base(serviceProvider) { }
        public static AppSettingsRepository CreateDefault(IServiceProvider serviceProvider) => new(serviceProvider);

        public async Task<AppSettings> GetAppSettings()
        {
            AppSettings settings = await Database.GetAppSetting();
            if (settings == null)
            {
                return AppSettings.CreateDefault();
            }
            return settings;
        }

        public async Task<AppSettings> UpdateAppSettings(AppSettings appSettings)
        {
            AppSettings existing = await GetAppSettings();

            existing.EmbedTitle = appSettings.EmbedTitle;
            existing.EmbedContent = appSettings.EmbedContent;
            existing.EmbedShowIcon = appSettings.EmbedShowIcon;
            existing.AuditLogWebhookURL = appSettings.AuditLogWebhookURL;
            existing.DefaultLanguage = appSettings.DefaultLanguage;
            existing.PublicFileMode = appSettings.PublicFileMode;

            Database.PutAppSetting(existing);
            await Database.SaveChangesAsync();

            ApplyAppSettings(existing);

            return existing;
        }

        public async Task ApplyAppSettings()
        {
            ApplyAppSettings(await GetAppSettings());
        }

        public void ApplyAppSettings(AppSettings settings)
        {
            _config.SetAuditLogWebhook(settings.AuditLogWebhookURL);
            _config.SetDefaultLanguage(settings.DefaultLanguage);
            _config.SetPublicFileModeEnabled(settings.PublicFileMode);
            _translator.SetContext(settings.DefaultLanguage);
        }
    }
}