using Discogs.Entity;
using Discogs.Enums;
using RateLimiter;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Discogs
{
    public class DiscogsClient
    {
        private static readonly string userAgent = "MusicStoreKeeper/0.1";
        private static readonly RestClient client= new RestClient("https://api.discogs.com/") { UserAgent = userAgent };
        private static readonly string dToken = "SXxVxPpSgbTxskTqVrwFzqrbcLvMNtnAKDyIbuub";

        //TODO: Use some OAuth1.0(a) library for proper authentication

        private const string ConsumerKey = "aqlnairRjUNWFcLeOXSe";
        private const string ConsumerSecret = "nUpCtMncknQjtZIzVuzNYuUgUqAJROpO";

        //path

        private const string ArtistPath = "artists/{artistId}";
        private const string ArtistReleasesPath = "artists/{artistId}/releases";
        private const string SearchStringPath = "database/search?";
        private const string ReleasePath = "releases/{releaseId}";
        private const string MasterReleasePath = "/masters/{masterId}";

        private TimeLimiter timeConstraint;

        public DiscogsClient()
        {
            client.UseSerializer(() => new JsonNetSerializer());
            client.Authenticator = new SimpleAuthenticator("key", ConsumerKey, "secret", ConsumerSecret);
            timeConstraint = TimeLimiter.GetFromMaxCountByInterval(60, TimeSpan.FromSeconds(1));
        }

        #region [methods]

        #region [ artist ]

        public async Task<DiscogsArtist> GetArtistById(int artistId, CancellationToken token)
        {
            var request = new RestRequest(ArtistPath).AddUrlSegment(nameof(artistId), artistId.ToString());
            return await ExecuteRequest<DiscogsArtist>(request, token);
        }

        public async Task<DiscogsArtist> GetArtistByName(string artistName, CancellationToken token)
        {
            var searchParameters = new DiscogsSearchParameters() { query = artistName, type = DiscogsSearchObjectType.artist };
            var results = await DiscogsSearch(searchParameters, token);
            var firstResult = results.FirstOrDefault();
            if (firstResult == null) return null;
            return await GetArtistById(firstResult.id, token);
        }

        #endregion [ artist ]

        #region [ release ]

        public async Task<IEnumerable<DiscogsArtistRelease>> GetArtistReleases(int artistId, CancellationToken token)
        {
            var request = new RestRequest(ArtistReleasesPath).AddUrlSegment(nameof(artistId), artistId.ToString());
            var responseData= await ExecuteRequest<DiscogsArtistReleases>(request, token);
            return responseData.GetData();
        }

        public async Task<DiscogsRelease> GetReleaseById(int releaseId, CancellationToken token)
        {
            var request = new RestRequest(ReleasePath).AddUrlSegment(nameof(releaseId), releaseId.ToString());
            return await ExecuteRequest<DiscogsRelease>(request, token);
        }

        public async Task<DiscogsMasterRelease> GetMatserReleaseById(int masterId, CancellationToken token)
        {
            var request = new RestRequest(MasterReleasePath).AddUrlSegment(nameof(masterId), masterId.ToString());
            return await ExecuteRequest<DiscogsMasterRelease>(request, token);
        }

        public async Task<DiscogsRelease> GetReleaseByTitle(string releaseTitle, string trackName, CancellationToken token)
        {
            var releaseId = 0;
            DiscogsRelease release = null;
            var searchParameters = new DiscogsSearchParameters() { query = releaseTitle, type = DiscogsSearchObjectType.release };
            var discogsSearchResults = await DiscogsSearch(searchParameters, token);
            var results = discogsSearchResults.ToList();
            foreach (var result in results)
            {
                //TODO: Add check for master releases
                release = GetReleaseById(result.id, token).Result;
                var tracks = release.tracklist;
                if (tracks.Any(arg => arg.title.Equals(trackName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    releaseId = release.id;
                    break;
                }
            }
            return releaseId == 0 ? null : release;
        }

        #endregion [ release ]

        #region [  image  ]

        public void SaveImage(DiscogsImage dImage, string path, string fileName, DiscogsImageFormatType type = DiscogsImageFormatType.Normal)
        {
            var uri = (type==DiscogsImageFormatType.Normal) ? dImage.uri : dImage.uri150;
            var request=new RestRequest(uri, Method.GET);
            var bytes = client.DownloadData(request);
            File.WriteAllBytes(Path.Combine(path, fileName),bytes);
        }

        #endregion

        public async Task<IEnumerable<DiscogsSearchResult>> DiscogsSearch(DiscogsSearchParameters arg, CancellationToken token)
        {
            var prm = arg.GetSearchString();
            var request = new RestRequest(prm);
            var response = await ExecuteRequest<DiscogsSearchResults>(request, token);
            return response.GetData();
        }

        public object GetRawData(string uri)
        {
            var request = new RestRequest(uri);
            var response = client.Get(request);
            return response.Content;
        }

        #endregion [methods]

        #region [  private methods  ]

        private async Task<T> ExecuteRequest<T>(IRestRequest request, CancellationToken token)
        {
            var response = await client.ExecuteGetAsync<T>(request, token);
            return response.Data;
        }

        #endregion
    }
}