namespace E_Warehouse.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Main : DbMigration
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
                        Description = c.String(),
                        SellPrice = c.Double(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ItemStatistics",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        AvgSellingPrice = c.Double(nullable: false),
                        AvgBuyingPrice = c.Double(nullable: false),
                        LowestBuyingPrice = c.Double(nullable: false),
                        LowestBuyingCompany_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.Id)
                .ForeignKey("dbo.Companies", t => t.LowestBuyingCompany_Id)
                .Index(t => t.Id)
                .Index(t => t.LowestBuyingCompany_Id);
            
            CreateTable(
                "dbo.ItemTransactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(nullable: false),
                        CompanyId = c.Int(nullable: false),
                        ItemTransactionType = c.Int(nullable: false),
                        TransactionDate = c.DateTime(nullable: false),
                        Price = c.Double(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.ItemId)
                .Index(t => t.CompanyId);
            
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
            DropForeignKey("dbo.ItemTransactions", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ItemTransactions", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.ItemCompanies", "Company_Id", "dbo.Companies");
            DropForeignKey("dbo.ItemCompanies", "Item_Id", "dbo.Items");
            DropForeignKey("dbo.ItemStatistics", "LowestBuyingCompany_Id", "dbo.Companies");
            DropForeignKey("dbo.ItemStatistics", "Id", "dbo.Items");
            DropIndex("dbo.ItemCompanies", new[] { "Company_Id" });
            DropIndex("dbo.ItemCompanies", new[] { "Item_Id" });
            DropIndex("dbo.ItemTransactions", new[] { "CompanyId" });
            DropIndex("dbo.ItemTransactions", new[] { "ItemId" });
            DropIndex("dbo.ItemStatistics", new[] { "LowestBuyingCompany_Id" });
            DropIndex("dbo.ItemStatistics", new[] { "Id" });
            DropTable("dbo.ItemCompanies");
            DropTable("dbo.ItemTransactions");
            DropTable("dbo.ItemStatistics");
            DropTable("dbo.Items");
            DropTable("dbo.Companies");
        }
    }
}
