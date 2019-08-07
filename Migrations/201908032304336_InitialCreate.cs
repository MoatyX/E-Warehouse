namespace E_Warehouse.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsSourceCompany = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PartNumber = c.String(),
                        BuyPrice = c.Single(nullable: false),
                        SellPrice = c.Single(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ItemCompanies",
                c => new
                    {
                        Item_Id = c.Int(nullable: false),
                        Company_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Item_Id, t.Company_Id })
                .ForeignKey("dbo.Items", t => t.Item_Id, cascadeDelete: true)
                .ForeignKey("dbo.Companies", t => t.Company_Id, cascadeDelete: true)
                .Index(t => t.Item_Id)
                .Index(t => t.Company_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemCompanies", "Company_Id", "dbo.Companies");
            DropForeignKey("dbo.ItemCompanies", "Item_Id", "dbo.Items");
            DropIndex("dbo.ItemCompanies", new[] { "Company_Id" });
            DropIndex("dbo.ItemCompanies", new[] { "Item_Id" });
            DropTable("dbo.ItemCompanies");
            DropTable("dbo.Items");
            DropTable("dbo.Companies");
        }
    }
}
