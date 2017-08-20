using System;

namespace GitCandy.Models
{
    [Serializable]
    public class RepositoryScope
    {
        public Int32 Commits { get; set; }
        public Int32 Branches { get; set; }
        public Int32 Tags { get; set; }
        public Int32 Contributors { get; set; }
    }
}