using Discogs.Entity;
using MusicStoreKeeper.Model;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    //TODO: Rename DiscogsConverter class and its methods
    public class DiscogsConverter
    {
        public DiscogsConverter()
        {
        }

        #region [  public methods  ]

        public Artist CreateArtist(DiscogsArtist dArtist)
        {
            var artist = new Artist();
            artist.Name = dArtist.name;
            artist.DiscogsId = dArtist.id;

            return artist;
        }

        public Album CreateAlbum(DiscogsRelease dRelease)
        {
            var album = new Album();
            album.Title = dRelease.title;
            album.ReleaseDate = dRelease.year;
            album.DiscogsId = dRelease.id;
            foreach (var dTrack in dRelease.tracklist)
            {
                var track = CreateTrack(dTrack);
                album.Tracks.Add(track);
            }
            return album;
        }

        public Track CreateTrack(DiscogsTrack dTrack)
        {
            var track = new Track();
            track.Name = dTrack.title;
            if (int.TryParse(dTrack.position, out var pos))
            {
                track.Number = pos;
            }
            track.Duration = dTrack.duration;
            return track;
        }

        #endregion [  public methods  ]
    }
}