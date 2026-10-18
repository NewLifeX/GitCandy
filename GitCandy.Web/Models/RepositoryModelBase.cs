using System;

namespace GitCandy.Models
{
    public abstract class RepositoryModelBase
    {
        /// <summary>仓库编号</summary>
        public Int32 Id { get; set; }

        public String Owner { get; set; }
        public String Name { get; set; }

        /// <summary>完整仓库名（拥有者/仓库名）</summary>
        public String FullName => String.IsNullOrEmpty(Owner) ? Name : $"{Owner}/{Name}";

        public BranchSelectorModel BranchSelector { get; set; }
    }
}