namespace MASZ.Models.Views
{
    public class AppealTable
    {
        public List<AppealView> AppealViews { get; set; }
        public int FullSize { get; set; }

        public AppealTable(List<AppealView> appealViews, int fullSize)
        {
            AppealViews = appealViews;
            FullSize = fullSize;
        }
    }
}
