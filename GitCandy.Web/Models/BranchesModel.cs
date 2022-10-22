﻿
namespace GitCandy.Models
{
    public class BranchesModel : RepositoryModelBase
    {
        public CommitModel Commit { get; set; }
        public AheadBehindModel[] AheadBehinds { get; set; }
        public Boolean CanDelete { get; set; }
    }
}