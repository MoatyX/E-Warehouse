namespace E_Warehouse.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemFix : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Items", "PartNumber", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Items", "PartNumber", c => c.String());
        }
    }
}
