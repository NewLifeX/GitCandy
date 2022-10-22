
namespace GitCandy.Models
{
    public class AheadBehindModel
    {
        public Int32 Ahead { get; set; }
        public Int32 Behind { get; set; }
        public CommitModel Commit { get; set; }
    }
}