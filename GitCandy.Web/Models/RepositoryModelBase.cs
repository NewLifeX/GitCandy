using System;

namespace GitCandy.Models
{
    public abstract class RepositoryModelBase
    {
        public String Owner { get; set; }
        public String Name { get; set; }
        public BranchSelectorModel BranchSelector { get; set; }
    }
}