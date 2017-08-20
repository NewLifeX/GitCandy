using System;

namespace GitCandy.Models
{
    public class RepositoryStatisticsModel
    {
        public Statistics Default { get; set; }
        public Statistics Current { get; set; }
        public Int64 RepositorySize { get; set; }

        [Serializable]
        public class Statistics
        {
            public String Branch { get; set; }
            public Int32 NumberOfFiles { get; set; }
            public Int32 NumberOfCommits { get; set; }
            public Int64 SizeOfSource { get; set; }
            public Int32 NumberOfContributors { get; set; }
            public ContributorCommits[] OrderedCommits { get; set; }
        }

        [Serializable]
        public class ContributorCommits
        {
            public String Author { get; set; }
            public Int32 CommitsCount { get; set; }
        }
    }
}