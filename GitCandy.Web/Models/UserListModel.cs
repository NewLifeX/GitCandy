
namespace GitCandy.Models
{
    public class UserListModel
    {
        public UserModel[] Users { get; set; }
        public System.Int32 CurrentPage { get; set; }
        public System.Int32 ItemCount { get; set; }
    }
}