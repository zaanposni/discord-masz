using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Exceptions;
using masz.Models;
namespace masz.Repositories
{

    public class CaseTemplateRepository : BaseRepository<CaseTemplateRepository>
    {
        private readonly bool _isBot;
        private readonly Identity _identity;
        private readonly DiscordUser _currentUser;
        private CaseTemplateRepository(IServiceProvider serviceProvider, Identity identity) : base(serviceProvider)
        {
            _currentUser = identity.GetCurrentUser();
            _identity = identity;
            _isBot = false;
        }
        private CaseTemplateRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _currentUser = _discordAPI.GetCurrentBotInfo(CacheBehavior.Default);
            _isBot = true;
        }
        public static CaseTemplateRepository CreateDefault(IServiceProvider serviceProvider, Identity identity) => new CaseTemplateRepository(serviceProvider, identity);
        public static CaseTemplateRepository CreateWithBotIdentity(IServiceProvider serviceProvider) => new CaseTemplateRepository(serviceProvider);

        public async Task<int> CountTemplates()
        {
            return await _database.CountAllCaseTemplates();
        }

        public async Task<CaseTemplate> CreateTemplate(CaseTemplate template)
        {
            template.CreatedAt = DateTime.UtcNow;
            template.UserId = _currentUser.Id;

            await _database.SaveCaseTemplate(template);
            await _database.SaveChangesAsync();

            return template;
        }

        public async Task<CaseTemplate> GetTemplate(int id)
        {
            CaseTemplate template = await _database.GetSpecificCaseTemplate(id);
            if (template == null)
            {
                throw new ResourceNotFoundException();
            }
            return template;
        }

        public async Task DeleteTemplate(CaseTemplate template)
        {
            _database.DeleteSpecificCaseTemplate(template);
            await _database.SaveChangesAsync();
        }

        public async Task<List<CaseTemplate>> GetTemplatesBasedOnPermissions()
        {
            List<CaseTemplate> templates = await _database.GetAllCaseTemplates();
            List<CaseTemplate> filteredTemplates = new List<CaseTemplate>();
            foreach (CaseTemplate template in templates)
            {
                if (await allowedToView(template))
                {
                    filteredTemplates.Add(template);
                }
            }
            return filteredTemplates;
        }
        private async Task<bool> allowedToView(CaseTemplate template) {
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