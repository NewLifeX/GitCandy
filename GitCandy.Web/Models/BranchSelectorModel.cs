using System;
using System.Collections.Generic;

namespace GitCandy.Models
{
    public class BranchSelectorModel
    {
        public IEnumerable<String> Branches { get; set; }
        public IEnumerable<String> Tags { get; set; }
        public String Current { get; set; }
        public bool CurrentIsBranch { get; set; }
        public String Path { get; set; }
    }
}