namespace GitCandy.Models
{
    public class TagsModel : RepositoryModelBase
    {
        public TagModel[] Tags { get; set; }
        public Boolean HasTags => Tags != null && Tags.Length != 0;

        public Boolean CanDelete { get; set; }
    }
}