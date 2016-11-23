using System;
using LibGit2Sharp;

namespace GitCandy.Models
{
    public class CommitChangeModel
    {
        public String OldPath { get; set; }
        public String Path { get; set; }
        public ChangeKind ChangeKind { get; set; }
        public int LinesAdded { get; set; }
        public int LinesDeleted { get; set; }
        public String Patch { get; set; }
    }
}