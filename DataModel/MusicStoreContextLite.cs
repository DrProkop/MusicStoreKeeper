using MusicStoreKeeper.Model;
using SQLite.CodeFirst;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SQLite.EF6.Migrations;

namespace MusicStoreKeeper.DataModel
{
    public class MusicStoreContextLite : DbContext
    {
        /// <summary>
        /// Constructs a new context instance using conventions to create the name of the database to
        /// which a connection will be made.  The by-convention name is the full name (namespace + class name)
        /// of the derived context class.
        /// See the class remarks for how this is used to create a connection.
        /// </summary>
        public MusicStoreContextLite() : base("name = MusicStoreContextLiteConnection")
        {
        }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<ImageData> ImagesData { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var initializer = new SqliteCreateDatabaseIfNotExists<MusicStoreContextLite>(modelBuilder);
            Database.SetInitializer(initializer);
        }
    }

    internal sealed class SQLiteContextMigrationConfiguration : DbMigrationsConfiguration<MusicStoreContextLite>
    {
        public SQLiteContextMigrationConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator());
        }
    }
}