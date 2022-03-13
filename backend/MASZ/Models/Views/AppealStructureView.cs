namespace MASZ.Models.Views
{
    public class AppealStructureView
    {
        public int Id { get; set; }
        public string GuildId { get; set; }
        public int SortOrder { get; set; }
        public string Question { get; set; }

        public AppealStructureView(AppealStructure structure)
        {
            Id = structure.Id;
            GuildId = structure.GuildId.ToString();
            SortOrder = structure.SortOrder;
            Question = structure.Question;
        }
    }
}
