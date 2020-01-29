using System.Collections.Generic;

namespace Discogs.Entity
{
    /// <summary>
    /// Basic class for classes accepting pagination.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DiscogsPaginatedResult<T> where T:DiscogsEntity
    {
        public DiscogsPagination Pagination { get; set; }
        public abstract IEnumerable<T> GetData();
    }
}
