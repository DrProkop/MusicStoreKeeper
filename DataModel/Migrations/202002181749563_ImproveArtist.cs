namespace MusicStoreKeeper.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImproveArtist : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Artists", "RealName", c => c.String());
            AddColumn("dbo.Artists", "Profile", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Artists", "Profile");
            DropColumn("dbo.Artists", "RealName");
        }
    }
}
