using Discord;
using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Extensions;
using MASZ.Models;

namespace MASZ.Repositories
{

    public class CaseTemplateRepository : BaseRepository<CaseTemplateRepository>
    {
        private readonly bool _isBot;
        private readonly Identity _identity;
        private readonly IUser _currentUser;
        private readonly int MAX_ALLOWED_CASE_TEMPLATES_PER_USER = 20;
        private CaseTemplateRepository(IServiceProvider serviceProvider, Identity identity) : base(serviceProvider)
        {
            _currentUser = identity.GetCurrentUser();
            _identity = identity;
            _isBot = false;
        }
        private CaseTemplateRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _currentUser = DiscordAPI.GetCurrentBotInfo();
            _isBot = true;
        }

        public static CaseTemplateRepository CreateDefault(IServiceProvider serviceProvider, Identity identity) => new(serviceProvider, identity);
        public static CaseTemplateRepository CreateWithBotIdentity(IServiceProvider serviceProvider) => new(serviceProvider);

        public async Task<int> CountTemplates()
        {
            return await Database.CountAllCaseTemplates();
        }

        public async Task<CaseTemplate> CreateTemplate(CaseTemplate template)
        {
            var existingTemplates = await Database.GetAllTemplatesFromUser(template.UserId);
            if (existingTemplates.Count >= MAX_ALLOWED_CASE_TEMPLATES_PER_USER)
            {
                throw new TooManyTemplatesCreatedException();
            }

            template.CreatedAt = DateTime.UtcNow;
            template.UserId = _currentUser.Id;

            await Database.SaveCaseTemplate(template);
            await Database.SaveChangesAsync();

            _eventHandler.OnCaseTemplateCreatedEvent.InvokeAsync(template);

            return template;
        }

        public async Task<CaseTemplate> GetTemplate(int id)
        {
            CaseTemplate template = await Database.GetSpecificCaseTemplate(id);
            if (template == null)
            {
                throw new ResourceNotFoundException();
            }
            return template;
        }

        public async Task DeleteTemplate(CaseTemplate template)
        {
            Database.DeleteSpecificCaseTemplate(template);
            await Database.SaveChangesAsync();

            _eventHandler.OnCaseTemplateDeletedEvent.InvokeAsync(template);
        }

        public async Task<List<CaseTemplate>> GetTemplatesBasedOnPermissions()
        {
            List<CaseTemplate> templates = await Database.GetAllCaseTemplates();
            List<CaseTemplate> filteredTemplates = new();
            foreach (CaseTemplate template in templates)
            {
                if (await AllowedToView(template))
                {
                    filteredTemplates.Add(template);
                }
            }
            return filteredTemplates;
        }

        private async Task<bool> AllowedToView(CaseTemplate template)
        {
            if (_isBot)
            {
                return true;
            }

            if (_identity.IsSiteAdmin())
            {
                return true;
            }

            if (template.UserId == _currentUser.Id)
            {
                return true;
            }

            if (template.ViewPermission == ViewPermission.Self)
            {
                return false;
            }

            if (template.ViewPermission == ViewPermission.Global)
            {
                return true;
            }

            return await _identity.HasPermissionOnGuild(DiscordPermission.Moderator, template.CreatedForGuildId);
        }
    }
}