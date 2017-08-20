using System;
using LibGit2Sharp;

namespace GitCandy.Models
{
    public class CommitChangeModel
    {
        public String OldPath { get; set; }
        public String Path { get; set; }
        public ChangeKind ChangeKind { get; set; }
        public Int32 LinesAdded { get; set; }
        public Int32 LinesDeleted { get; set; }
        public String Patch { get; set; }
    }
}