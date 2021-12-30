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

            Database.PutAppSetting(existing);
            await Database.SaveChangesAsync();

            return existing;
        }
    }
}