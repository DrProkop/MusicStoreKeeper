using Discogs.Enums;
using DiscogsApiTest;
using NUnit.Framework;

namespace Discogs.Tests
{
    [TestFixture]
    public class DiscogsSearchParametersTest
    {
        private const string ExpectedSearchString = @"/database/search?q=query&type=artist&title=title&release_title=release_title&credit=credit&artist=artist&anv=anv&label=label&genre=genre&style=style&country=country&year=year&format=format&catno=catno&barcode=barcode&track=track&submitter=submitter&contributor=contributor";


        [Test]
        public void GetSearchStringShouldReturnValidString()
        {
            var discogsParameters = new DiscogsSearchParameters()
            {
                query = "query",
                type = DiscogsSearchObjectType.artist,
                title = "title",
                releaseTitle = "release_title",
                credit = "credit",
                artist = "artist",
                anv = "anv",
                label = "label",
                genre = "genre",
                style = "style",
                country = "country",
                year = "year",
                format = "format",
                catno = "catno",
                barcode = "barcode",
                track = "track",
                submitter = "submitter",
                contributor = "contributor"
            };

            StringAssert.AreEqualIgnoringCase(ExpectedSearchString, discogsParameters.GetSearchString());
        }
    }
}