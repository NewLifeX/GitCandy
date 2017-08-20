using System;

namespace GitCandy.Git
{
    [Serializable]
    public class RevisionSummaryCacheItem
    {
        public String Name;
        public String Path;
        public String TargetSha;
        public String CommitSha;
        public String MessageShort;
        public String AuthorName;
        public String AuthorEmail;
        public DateTimeOffset AuthorWhen;
        public String CommitterName;
        public String CommitterEmail;
        public DateTimeOffset CommitterWhen;
        public Int32 Ahead;
        public Int32 Behind;
    }
}