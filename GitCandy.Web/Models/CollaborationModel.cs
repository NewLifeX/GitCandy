using System;

namespace GitCandy.Models
{
    public class CollaborationModel : RepositoryModelBase
    {
        public UserRole[] Users { get; set; }
        public TeamRole[] Teams { get; set; }

        public class UserRole
        {
            public String Name { get; set; }
            public Boolean AllowRead { get; set; }
            public Boolean AllowWrite { get; set; }
            public Boolean IsOwner { get; set; }
        }

        public class TeamRole
        {
            public String Name { get; set; }
            public Boolean AllowRead { get; set; }
            public Boolean AllowWrite { get; set; }
        }
    }
}