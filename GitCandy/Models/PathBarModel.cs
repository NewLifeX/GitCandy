
using System;

namespace GitCandy.Models
{
    public class PathBarModel
    {
        public String Name { get; set; }
        public String ReferenceName { get; set; }
        public String ReferenceSha { get; set; }
        public String Path { get; set; }
        public String Action { get; set; }
        public bool HideLastSlash { get; set; }
    }
}