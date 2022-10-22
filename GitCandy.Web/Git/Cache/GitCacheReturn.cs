
namespace GitCandy.Git.Cache
{
    public struct GitCacheReturn<T>
    {
        public T Value { get; set; }
        public Boolean Done { get; set; }
    }
}
