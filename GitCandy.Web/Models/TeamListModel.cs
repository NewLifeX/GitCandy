
namespace GitCandy.Models
{
    public class TeamListModel
    {
        public TeamModel[] Teams { get; set; }
        public Int32 CurrentPage { get; set; }
        public Int32 ItemCount { get; set; }
    }
}