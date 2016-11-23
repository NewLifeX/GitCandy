using System;
using System.Collections.Generic;

namespace GitCandy.Models
{
    public class CommitsModel : RepositoryModelBase
    {
        public String ReferenceName { get; set; }
        public String Sha { get; set; }
        public String Path { get; set; }
        public IEnumerable<CommitModel> Commits { get; set; }
        public int CurrentPage { get; set; }
        public int ItemCount { get; set; }
        public PathBarModel PathBar { get; set; }
    }
}