using System.Collections.Generic;

namespace Discogs.Entity
{
    public class DiscogsSearchResults:DiscogsPaginatedResult<DiscogsSearchResult>
    {
        public IEnumerable<DiscogsSearchResult> Results { get; set; }
        public override IEnumerable<DiscogsSearchResult> GetData() => Results;

    }
}
