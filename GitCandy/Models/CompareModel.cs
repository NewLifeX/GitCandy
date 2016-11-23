
using System;

namespace GitCandy.Models
{
    public class CompareModel : RepositoryModelBase
    {
        public String[] Branches { get; set; }
        public BranchSelectorModel BaseBranchSelector { get; set; }
        public BranchSelectorModel CompareBranchSelector { get; set; }
        public CommitModel CompareResult { get; set; }
        public CommitModel[] Walks { get; set; }
    }
}