namespace E_Warehouse.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Transactions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemTransactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Int(nullable: false),
                        CompanyId = c.Int(nullable: false),
                        ItemTransactionType = c.Int(nullable: false),
                        Price = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.ItemId)
                .Index(t => t.CompanyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemTransactions", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ItemTransactions", "CompanyId", "dbo.Companies");
            DropIndex("dbo.ItemTransactions", new[] { "CompanyId" });
            DropIndex("dbo.ItemTransactions", new[] { "ItemId" });
            DropTable("dbo.ItemTransactions");
        }
    }
}
