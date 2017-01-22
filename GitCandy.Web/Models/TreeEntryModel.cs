using LibGit2Sharp;
using System.Text;
using System;

namespace GitCandy.Models
{
    public class TreeEntryModel : RepositoryModelBase
    {
        //public String Name { get; set; }
        public String Path { get; set; }
        public String ReferenceName { get; set; }
        public CommitModel Commit { get; set; }
        public String Sha { get; set; }
        public TreeEntryTargetType EntryType { get; set; }
        public byte[] RawData { get; set; }
        public String SizeString { get; set; }
        public String TextContent { get; set; }
        public String TextBrush { get; set; }
        public BlobType BlobType { get; set; }
        public Encoding BlobEncoding { get; set; }
        public PathBarModel PathBar { get; set; }
    }
}