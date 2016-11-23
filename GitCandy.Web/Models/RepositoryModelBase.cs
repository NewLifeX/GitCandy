using System;

namespace GitCandy.Models
{
    public abstract class RepositoryModelBase
    {
        public String RepositoryName { get; set; }
        public BranchSelectorModel BranchSelector { get; set; }
    }
}