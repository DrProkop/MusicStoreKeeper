﻿using Discogs.Entity;
using Discogs.Enums;
using RateLimiter;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discogs
{
    public class DiscogsClient
    {
        private static readonly string userAgent = "MusicStoreKeeper/0.1";
        private static readonly RestClient client= new RestClient("https://api.discogs.com/") { UserAgent = userAgent };
        private static readonly string token = "SXxVxPpSgbTxskTqVrwFzqrbcLvMNtnAKDyIbuub";

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

        public async Task<DiscogsArtist> GetArtistById(int artistId)
        {
            var request = new RestRequest(ArtistPath).AddUrlSegment(nameof(artistId), artistId.ToString());
            return await client.GetAsync<DiscogsArtist>(request);
        }

        public async Task<DiscogsArtist> GetArtistByName(string artistName)
        {
            var searchParameters = new DiscogsSearchParameters() { query = artistName, type = DiscogsSearchObjectType.artist };
            var results = await DiscogsSearch(searchParameters);
            var firstResult = results.FirstOrDefault();
            if (firstResult == null) return null;
            return await GetArtistById(firstResult.id);
        }

        #endregion [ artist ]

        #region [ release ]

        public async Task<IEnumerable<DiscogsArtistRelease>> GetArtistReleases(int artistId)
        {
            var request = new RestRequest(ArtistReleasesPath).AddUrlSegment(nameof(artistId), artistId.ToString());
            var response = await client.GetAsync<DiscogsArtistReleases>(request);
            return response.GetData();
        }

        public async Task<DiscogsRelease> GetReleaseById(int releaseId)
        {
            var request = new RestRequest(ReleasePath).AddUrlSegment(nameof(releaseId), releaseId.ToString());
            return await client.GetAsync<DiscogsRelease>(request);
        }

        public async Task<DiscogsMasterRelease> GetMaterReleaseById(int masterId)
        {
            var request = new RestRequest(MasterReleasePath).AddUrlSegment(nameof(masterId), masterId.ToString());
            return await client.GetAsync<DiscogsMasterRelease>(request);
        }

        public async Task<DiscogsRelease> GetReleaseByTitle(string releaseTitle, string trackName)
        {
            var releaseId = 0;
            DiscogsRelease release = null;
            var searchParameters = new DiscogsSearchParameters() { query = releaseTitle, type = DiscogsSearchObjectType.release };
            var discogsSearchResults = await DiscogsSearch(searchParameters);
            var results = discogsSearchResults.ToList();
            foreach (var result in results)
            {
                //TODO: Add check for master releases
                release = GetReleaseById(result.id).Result;
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

        public async Task<IEnumerable<DiscogsSearchResult>> DiscogsSearch(DiscogsSearchParameters arg)
        {
            var prm = arg.GetSearchString();
            var request = new RestRequest(prm);
            var response = await client.GetAsync<DiscogsSearchResults>(request);
            return response.GetData();
        }

        public object GetRawData(string uri)
        {
            var request = new RestRequest(uri);
            var response = client.Get(request);
            return response.Content;
        }

        #endregion [methods]
    }
}