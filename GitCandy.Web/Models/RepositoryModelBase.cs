using System;

namespace GitCandy.Models
{
    public abstract class RepositoryModelBase
    {
        /// <summary>仓库编号</summary>
        public Int32 Id { get; set; }

        public String Owner { get; set; }
        public String Name { get; set; }
        public BranchSelectorModel BranchSelector { get; set; }
    }
}