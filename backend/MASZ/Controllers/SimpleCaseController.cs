using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Repositories;

namespace MASZ.Controllers
{
    public class SimpleCaseController : SimpleController
    {

        public SimpleCaseController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        protected async Task RequirePermission(ulong guildId, int caseId, APIActionPermission permission)
        {
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