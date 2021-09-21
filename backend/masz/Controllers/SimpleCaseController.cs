using System;
using System.Threading.Tasks;
using masz.Exceptions;
using masz.Models;
using masz.Repositories;
using masz.Enums;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    public class SimpleCaseController : SimpleController
    {
        private readonly ILogger<SimpleCaseController> _logger;

        public SimpleCaseController(IServiceProvider serviceProvider, ILogger<SimpleCaseController> logger) : base(serviceProvider) {
            _logger = logger;
        }
        public async Task RequirePermission(ulong guildId, int caseId, APIActionPermission permission)
        {
            GuildConfig guild = await GetRegisteredGuild(guildId);
            Identity currentIdentity = await GetIdentity();
            ModCase modCase = await ModCaseRepository.CreateDefault(_serviceProvider, currentIdentity).GetModCase(guildId, caseId);
            if (modCase == null)
            {
                throw new ResourceNotFoundException();
            }
            if (!await currentIdentity.IsAllowedTo(permission, modCase))
            {
                throw new UnauthorizedException();
            }
            if (modCase.MarkedToDeleteAt != null && permission == APIActionPermission.Edit)
            {
                throw new CaseMarkedToBeDeletedException();
            }
        }
    }
}