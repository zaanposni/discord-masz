using MASZ.Enums;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Models.Database;
using MASZ.Repositories;

namespace MASZ.Controllers
{
    public class SimpleEvidenceController : SimpleController
    {
        public SimpleEvidenceController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected async Task RequirePermission(ulong guildId, int evidenceId, APIActionPermission permission)
        {
            Identity currentIdentity = await GetIdentity();
            VerifiedEvidence evidence = await VerifiedEvidenceRepository.CreateDefault(_serviceProvider, currentIdentity).GetEvidence(guildId, evidenceId);
            if (!await currentIdentity.IsAllowedTo(permission, evidence))
            {
                throw new UnauthorizedException();
            }
        }
    }
}
