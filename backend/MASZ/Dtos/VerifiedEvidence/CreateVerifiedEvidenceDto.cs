using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.VerifiedEvidence
{
    public class CreateVerifiedEvidenceDto
    {
        [Required(ErrorMessage = "Channel ID is required")]
        public ulong ChannelId { get; set; }
        [Required(ErrorMessage = "Message ID is required")]
        public ulong MessageId { get; set; }
    }
}
