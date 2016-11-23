using System;
using System.Collections.Generic;

namespace GitCandy.Models
{
    public class TreeModel : RepositoryModelBase
    {
        public String ReferenceName { get; set; }
        public String Path { get; set; }
        public CommitModel Commit { get; set; }
        public IEnumerable<TreeEntryModel> Entries { get; set; }
        public TreeEntryModel Readme { get; set; }
        public bool IsRoot { get { return String.IsNullOrEmpty(Path) || Path == "\\" || Path == "/"; } }
        public RepositoryScope Scope { get; set; }
        public GitUrl[] GitUrls { get; set; }
        public String Description { get; set; }
        public PathBarModel PathBar { get; set; }
    }
}