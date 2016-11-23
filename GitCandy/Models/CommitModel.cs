using System;
using System.Collections.Generic;
using LibGit2Sharp;

namespace GitCandy.Models
{
    public class CommitModel : RepositoryModelBase
    {
        public String Sha { get; set; }
        public String ReferenceName { get; set; }
        public String CommitMessageShort { get; set; }
        public String CommitMessage { get; set; }
        public Signature Author { get; set; }
        public Signature Committer { get; set; }
        public String[] Parents { get; set; }
        public IEnumerable<CommitChangeModel> Changes { get; set; }
        public PathBarModel PathBar { get; set; }
    }
}