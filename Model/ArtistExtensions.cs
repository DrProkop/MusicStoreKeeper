using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStoreKeeper.Model
{
    public static class ArtistExtensions
    {
        public static void Merge(this Artist artistA, Artist artistB)
        {
            if (artistA == null || artistB == null) return;

            if (artistB.Name!=null)
            {
                artistA.Name = artistB.Name;
            }
            if (artistB.Profile != null)
            {
                artistA.Profile = artistB.Profile;
            }
            if (artistB.RealName != null)
            {
                artistA.RealName = artistB.RealName;
            }
            if (artistB.StoragePath != null)
            {
                artistA.StoragePath = artistB.StoragePath;
            }
        }
    }
}
