using System;

namespace GitCandy.Models
{
    public class TagModel
    {
        public String ReferenceName { get; set; }
        public String Sha { get; set; }
        public DateTimeOffset When { get; set; }
        public String MessageShort { get; set; }
    }
}