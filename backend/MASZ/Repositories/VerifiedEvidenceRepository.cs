using Discord;
using MASZ.Exceptions;
using MASZ.Models;
using MASZ.Models.Database;

namespace MASZ.Repositories
{
    public class VerifiedEvidenceRepository : BaseRepository<VerifiedEvidenceRepository>
    {
        IUser _currentUser;

        private VerifiedEvidenceRepository(IServiceProvider serviceProvider, IUser currentUser) : base(serviceProvider)
        {
            _currentUser = currentUser;
        }

        private VerifiedEvidenceRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _currentUser = DiscordAPI.GetCurrentBotInfo();
        }

        public static VerifiedEvidenceRepository CreateDefault(IServiceProvider serviceProvider, Identity identity) => new(serviceProvider, identity.GetCurrentUser());
        public static VerifiedEvidenceRepository CreateDefault(IServiceProvider serviceProvider, IUser user) => new(serviceProvider, user);
        public static VerifiedEvidenceRepository CreateWithBotIdentity(IServiceProvider serviceProvider) => new(serviceProvider);

        public async Task<List<VerifiedEvidence>> GetEvidencePagination(ulong guildId, int page = 0, int pageSize = 20)
        {
            return await Database.GetEvidencePagination(guildId, page, pageSize);
        }

        public async Task<List<VerifiedEvidence>> GetEvidencePaginationForUser(ulong guildId, ulong userId, int page = 0, int pageSize = 20)
        {
            return await Database.GetEvidencePaginationForUser(guildId, userId, page, pageSize);
        }

        public async Task<VerifiedEvidence> GetEvidence(ulong guildId, int evidenceId)
        {
            VerifiedEvidence evidence =  await Database.GetEvidence(guildId, evidenceId);
            if (evidence == null)
            {
                throw new ResourceNotFoundException();
            }
            return evidence;
        }

        public async Task<int> CountEvidenceForGuild(ulong guildId)
        {
            return await Database.CountTrackedInvitesForGuild(guildId);
        }

        public async Task<VerifiedEvidence> DeleteEvidence(ulong guildId, int evidenceId)
        {
            VerifiedEvidence evidence = await GetEvidence(guildId, evidenceId);

            if (evidence == null)
            {
                throw new ResourceNotFoundException();
            }

            Database.DeleteEvidence(evidence);
            await Database.SaveChangesAsync();

            return evidence;
        }

        public async Task<VerifiedEvidence> CreateEvidence(VerifiedEvidence evidence)
        {
            // TODO: handle existing evidence?

            if (string.IsNullOrWhiteSpace(evidence.ReportedContent))
            {
                throw new BaseAPIException("Reported content cannot be empty.");
            }

            evidence.ModId = _currentUser.Id;

            Database.CreateEvidence(evidence);
            await Database.SaveChangesAsync();
            return evidence;
        }

        public async Task Link(ulong guildId, int evidenceId, int caseId)
        {
            ModCaseEvidenceMapping existing = await Database.GetModCaseEvidenceMapping(evidenceId, caseId);
            if (existing != null)
            {
                throw new BaseAPIException("Cases are already linked.");
            }

            VerifiedEvidence evidence = await GetEvidence(guildId, evidenceId);
            ModCase modCase = await ModCaseRepository.CreateWithBotIdentity(_serviceProvider).GetModCase(guildId, caseId);

            ModCaseEvidenceMapping newMapping = new()
            {
                Evidence = evidence,
                ModCase = modCase
            };

            Database.CreateModCaseEvidenceMapping(newMapping);
            await Database.SaveChangesAsync();
        }

        public async Task Unlink(ulong guildId, int evidenceId, int caseId)
        {


            VerifiedEvidence evidence = await GetEvidence(guildId, evidenceId);
            ModCase modCase = await ModCaseRepository.CreateWithBotIdentity(_serviceProvider).GetModCase(guildId, caseId);

            ModCaseEvidenceMapping mapping = await Database.GetModCaseEvidenceMapping(evidence.Id, modCase.Id);

            if (mapping == null)
            {
                throw new BaseAPIException("Cases are not linked.");
            }

            Database.DeleteModCaseEvidenceMapping(mapping);
            await Database.SaveChangesAsync();
        }
    }
}
