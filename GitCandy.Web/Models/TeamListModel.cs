
namespace GitCandy.Models
{
    public class TeamListModel
    {
        public TeamModel[] Teams { get; set; }
        public System.Int32 CurrentPage { get; set; }
        public System.Int32 ItemCount { get; set; }
    }
}