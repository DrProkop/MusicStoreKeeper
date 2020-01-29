using System.Collections.Generic;

namespace Discogs.Entity
{
    public class DiscogsArtistReleases:DiscogsPaginatedResult<DiscogsArtistRelease>
    {
        public IEnumerable<DiscogsArtistRelease> Releases { get; set; }
        public override IEnumerable<DiscogsArtistRelease> GetData() => Releases;
    }
}