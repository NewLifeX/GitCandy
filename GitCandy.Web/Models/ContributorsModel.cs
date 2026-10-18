
namespace GitCandy.Models
{
    public class ContributorsModel : RepositoryModelBase
    {
        public RepositoryStatisticsModel Statistics { get; set; }
        public Boolean IsLoading { get; set; }
    }
}