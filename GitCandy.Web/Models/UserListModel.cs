
namespace GitCandy.Models
{
    public class UserListModel
    {
        public UserModel[] Users { get; set; }
        public Int32 CurrentPage { get; set; }
        public Int32 ItemCount { get; set; }
    }
}