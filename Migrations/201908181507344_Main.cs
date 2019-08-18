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
                        PartNumber = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Description = c.String(),
                        SellPrice = c.Double(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PartNumber);
            
            CreateTable(
                "dbo.ItemStatistics",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ItemStatisticId = c.Int(nullable: false),
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
                        Item_PartNumber = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.Items", t => t.Item_PartNumber)
                .Index(t => t.CompanyId)
                .Index(t => t.Item_PartNumber);
            
            CreateTable(
                "dbo.ItemCompanies",
                c => new
                    {
                        Item_PartNumber = c.String(nullable: false, maxLength: 128),
                        Company_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Item_PartNumber, t.Company_Id })
                .ForeignKey("dbo.Items", t => t.Item_PartNumber, cascadeDelete: true)
                .ForeignKey("dbo.Companies", t => t.Company_Id, cascadeDelete: true)
                .Index(t => t.Item_PartNumber)
                .Index(t => t.Company_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemTransactions", "Item_PartNumber", "dbo.Items");
            DropForeignKey("dbo.ItemTransactions", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.ItemCompanies", "Company_Id", "dbo.Companies");
            DropForeignKey("dbo.ItemCompanies", "Item_PartNumber", "dbo.Items");
            DropForeignKey("dbo.ItemStatistics", "LowestBuyingCompany_Id", "dbo.Companies");
            DropForeignKey("dbo.ItemStatistics", "Id", "dbo.Items");
            DropIndex("dbo.ItemCompanies", new[] { "Company_Id" });
            DropIndex("dbo.ItemCompanies", new[] { "Item_PartNumber" });
            DropIndex("dbo.ItemTransactions", new[] { "Item_PartNumber" });
            DropIndex("dbo.ItemTransactions", new[] { "CompanyId" });
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
