using System;
using System.Threading.Tasks;
using Discogs.Enums;

namespace DiscogsApiTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var dq = new DiscogsClient();
            var id = 152835;
            // var id = 0;
            var searchArg = "Randomajestiq";
            
            // code = dq.GetArtistId("Bluetech");
            //  var yolo = dq.GetArtistById(id);
            //  dq.GetArtistReleases(id);
            // dq.DiscogsSearch(searchArg);
            //  Console.WriteLine($"Return code: {code.Result}.");
            var dsp= new DiscogsSearchParameters()
            {
                query = searchArg,
                
                type = DiscogsSearchObjectType.artist
            };

            //var result = dq.GetArtistByName(searchArg);
            //Console.WriteLine($"{result.Result.Name} - {result.Result.id}");
            //var result = dq.GetArtistReleases(id);
            //var releases = result.Result;
            //foreach (var release in releases)
            //{
            //    Console.WriteLine(release.Title);
            //    Console.WriteLine();
            //}
            // var result = dq.GetRawData("https://api.discogs.com/releases/212529");
           // var result = dq.GetRawData("https://api.discogs.com/artists/152835/releases");
            var release = dq.GetReleaseByTitle("Prima Materia", "Leaving Babylon");

            var result = release.Result;
            Console.WriteLine(result.title);
            Console.ReadLine();
        }
    }
}