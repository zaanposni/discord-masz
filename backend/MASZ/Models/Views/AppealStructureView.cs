namespace MASZ.Models.Views
{
    public class AppealStructureView
    {
        public string GuildId { get; set; }
        public int SortOrder { get; set; }
        public string Question { get; set; }

        private AppealStructureView(AppealStructure structure)
        {
            GuildId = structure.GuildId.ToString();
            SortOrder = structure.SortOrder;
            Question = structure.Question;
        }

        public static AppealStructureView CreateOrDefault(AppealStructure structure)
        {
            if (structure == null) return null;
            return new(structure);
        }
    }
}
