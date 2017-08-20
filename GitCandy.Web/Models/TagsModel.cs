namespace GitCandy.Models
{
    public class TagsModel : RepositoryModelBase
    {
        public TagModel[] Tags { get; set; }
        public System.Boolean HasTags { get { return Tags != null && Tags.Length != 0; } }

        public System.Boolean CanDelete { get; set; }
    }
}