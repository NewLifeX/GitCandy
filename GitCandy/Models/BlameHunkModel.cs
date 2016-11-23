using System;

namespace GitCandy.Models
{
    [Serializable]
    public class BlameHunkModel
    {
        public String Code { get; set; }
        public String MessageShort { get; set; }
        public String Sha { get; set; }
        public String Author { get; set; }
        public String AuthorEmail { get; set; }
        public DateTimeOffset AuthorDate { get; set; }
    }
}