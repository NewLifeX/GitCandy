
using System;

namespace GitCandy.Models
{
    public class BlameModel : RepositoryModelBase
    {
        public String ReferenceName { get; set; }
        public String Path { get; set; }
        public String Sha { get; set; }
        public BlameHunkModel[] Hunks { get; set; }
        public String Brush { get; set; }
        public PathBarModel PathBar { get; set; }
        public String SizeString { get; set; }
    }
}