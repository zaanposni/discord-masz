namespace MASZ.Models
{
    public class CommentsView
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }

        public CommentsView(ModCaseComment comment)
        {
            Id = comment.Id;
            Message = comment.Message;
            CreatedAt = comment.CreatedAt;
            UserId = comment.UserId.ToString();
        }

        public void RemoveModeratorInfo(string suspectId)
        {
            if (UserId != suspectId)
            {
                UserId = null;
            }
        }
    }
}
