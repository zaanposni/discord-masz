using System.ComponentModel.DataAnnotations;

namespace MASZ.Dtos.VerifiedEvidence
{
    public class CreateVerifiedEvidenceDto
    {
        [Required(ErrorMessage = "Channel ID is required")]
        [RegularExpression(@"^\d{17,19}$", ErrorMessage = "the channel id can only consist of digits and be between 17 and 19 characters long")]
        public string ChannelId { get; set; }
        [Required(ErrorMessage = "Message ID is required")]
        [RegularExpression(@"^\d{17,19}$", ErrorMessage = "the message id can only consist of digits and be between 17 and 19 characters long")]
        public string MessageId { get; set; }
    }
}
