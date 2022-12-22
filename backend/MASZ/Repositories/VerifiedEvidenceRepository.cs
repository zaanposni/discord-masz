using MASZ.Models.Database;

namespace MASZ.Repositories
{
    public class VerifiedEvidenceRepository : BaseRepository<VerifiedEvidenceRepository>
    {
        private VerifiedEvidenceRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public static VerifiedEvidenceRepository CreateDefault(IServiceProvider serviceProvider) => new(serviceProvider);

        public async Task<List<VerifiedEvidence>> GetAllEvidence(ulong guildId)
        {
            return await Database.GetAllEvidence(guildId);
        }

        public async Task<VerifiedEvidence> GetEvidence(ulong guildId, int evidenceId)
        {
            return await Database.GetEvidence(guildId, evidenceId);
        }

        public async Task<VerifiedEvidence> DeleteEvidence(ulong guildId, int evidenceId)
        {
            VerifiedEvidence evidence = await GetEvidence(guildId, evidenceId);
            if (evidence != default)
            {
                Database.DeleteEvidence(evidence);
                await Database.SaveChangesAsync();
            }
            return evidence;
        }

        public async Task<VerifiedEvidence> CreateEvidence(VerifiedEvidence evidence)
        {
            Database.CreateEvidence(evidence);
            await Database.SaveChangesAsync();
            return evidence;
        }
    }
}
