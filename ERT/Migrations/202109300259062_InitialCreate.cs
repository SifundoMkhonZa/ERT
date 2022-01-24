namespace ERT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Beneficiary_Signature",
                c => new
                    {
                        Signaturee_ID = c.Int(nullable: false, identity: true),
                        Sign_Date = c.DateTime(nullable: false),
                        MySignature = c.Binary(),
                        SignedBy = c.String(),
                        OrderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Signaturee_ID)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        OrderDate = c.DateTime(nullable: false),
                        Username = c.String(),
                        FirstName = c.String(nullable: false, maxLength: 160),
                        LastName = c.String(nullable: false, maxLength: 160),
                        Address = c.String(nullable: false, maxLength: 70),
                        Phone = c.String(nullable: false, maxLength: 24),
                        Email = c.String(nullable: false),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Payment = c.Boolean(nullable: false),
                        OrderStatus_ID = c.Int(nullable: false),
                        Client_Id = c.String(maxLength: 128),
                        Reference = c.String(),
                        Distance = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.Clients", t => t.Client_Id)
                .ForeignKey("dbo.OrderStatus", t => t.OrderStatus_ID, cascadeDelete: true)
                .Index(t => t.OrderStatus_ID)
                .Index(t => t.Client_Id);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Client_Id = c.String(nullable: false, maxLength: 128),
                        Client_IDNo = c.String(maxLength: 13),
                        Client_Name = c.String(),
                        Client_Surname = c.String(),
                        Client_Cellnumber = c.String(),
                        Client_Address = c.String(),
                        Client_Email = c.String(),
                        Client_Tellnum = c.String(),
                    })
                .PrimaryKey(t => t.Client_Id);
            
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        OrderDetailId = c.Int(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Product_id = c.Int(nullable: false),
                        OrderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderDetailId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.Product_id, cascadeDelete: true)
                .Index(t => t.Product_id)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Product_id = c.Int(nullable: false, identity: true),
                        Product_Name = c.String(nullable: false, maxLength: 160),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Image = c.Binary(),
                        CategoryId = c.Int(nullable: false),
                        Shop_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Product_id)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Eshops", t => t.Shop_Id, cascadeDelete: true)
                .Index(t => t.CategoryId)
                .Index(t => t.Shop_Id);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(maxLength: 250),
                        Supplier_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CategoryId)
                .ForeignKey("dbo.Suppliers", t => t.Supplier_Id)
                .Index(t => t.Supplier_Id);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        Supplier_Id = c.String(nullable: false, maxLength: 128),
                        Supplier_Name = c.String(nullable: false),
                        Supplier_Image = c.Binary(),
                        Supplier_LastName = c.String(nullable: false),
                        Supplier_Address = c.String(nullable: false),
                        Supplier_ContactNumber = c.String(nullable: false),
                        Supplier_Email = c.String(nullable: false),
                        Supplier_Contract = c.Boolean(nullable: false),
                        Supplier_Password = c.String(nullable: false, maxLength: 100),
                        Supplier_Ducuments = c.Binary(),
                    })
                .PrimaryKey(t => t.Supplier_Id);
            
            CreateTable(
                "dbo.Eshops",
                c => new
                    {
                        Shop_Id = c.Int(nullable: false, identity: true),
                        Shop_Name = c.String(nullable: false),
                        Shop_Image = c.Binary(),
                        Supplier_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Shop_Id)
                .ForeignKey("dbo.Suppliers", t => t.Supplier_Id)
                .Index(t => t.Supplier_Id);
            
            CreateTable(
                "dbo.OrderStatus",
                c => new
                    {
                        OrderStatus_ID = c.Int(nullable: false, identity: true),
                        Status_Name = c.String(),
                    })
                .PrimaryKey(t => t.OrderStatus_ID);
            
            CreateTable(
                "dbo.CartCopies",
                c => new
                    {
                        RecordId = c.Int(nullable: false, identity: true),
                        CartId = c.String(),
                        Count = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        OrderId = c.Int(nullable: false),
                        Product_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RecordId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.Product_id, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.Product_id);
            
            CreateTable(
                "dbo.Carts",
                c => new
                    {
                        RecordId = c.Int(nullable: false, identity: true),
                        CartId = c.String(),
                        Count = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        Product_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RecordId)
                .ForeignKey("dbo.Products", t => t.Product_id, cascadeDelete: true)
                .Index(t => t.Product_id);
            
            CreateTable(
                "dbo.Drivers",
                c => new
                    {
                        Driver_ID = c.String(nullable: false, maxLength: 128),
                        Driver_IDNo = c.String(),
                        Driver_Name = c.String(),
                        Driver_Surname = c.String(),
                        Driver_CellNo = c.String(),
                        Driver_Address = c.String(),
                        Driver_Email = c.String(),
                        Diver_Image = c.Binary(),
                    })
                .PrimaryKey(t => t.Driver_ID);
            
            CreateTable(
                "dbo.Schedules",
                c => new
                    {
                        Schedule_ID = c.Int(nullable: false, identity: true),
                        Schedule_Date = c.DateTime(nullable: false),
                        OrderId = c.Int(nullable: false),
                        Driver_ID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Schedule_ID)
                .ForeignKey("dbo.Drivers", t => t.Driver_ID)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.Driver_ID);
            
            CreateTable(
                "dbo.QRCodes",
                c => new
                    {
                        QRId = c.Int(nullable: false, identity: true),
                        QRCodeText = c.String(),
                        QRCodeImagePath = c.String(),
                        OrderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QRId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.QRCodes", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Schedules", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Schedules", "Driver_ID", "dbo.Drivers");
            DropForeignKey("dbo.Carts", "Product_id", "dbo.Products");
            DropForeignKey("dbo.CartCopies", "Product_id", "dbo.Products");
            DropForeignKey("dbo.CartCopies", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Beneficiary_Signature", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "OrderStatus_ID", "dbo.OrderStatus");
            DropForeignKey("dbo.OrderDetails", "Product_id", "dbo.Products");
            DropForeignKey("dbo.Products", "Shop_Id", "dbo.Eshops");
            DropForeignKey("dbo.Categories", "Supplier_Id", "dbo.Suppliers");
            DropForeignKey("dbo.Eshops", "Supplier_Id", "dbo.Suppliers");
            DropForeignKey("dbo.Products", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.OrderDetails", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "Client_Id", "dbo.Clients");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.QRCodes", new[] { "OrderId" });
            DropIndex("dbo.Schedules", new[] { "Driver_ID" });
            DropIndex("dbo.Schedules", new[] { "OrderId" });
            DropIndex("dbo.Carts", new[] { "Product_id" });
            DropIndex("dbo.CartCopies", new[] { "Product_id" });
            DropIndex("dbo.CartCopies", new[] { "OrderId" });
            DropIndex("dbo.Eshops", new[] { "Supplier_Id" });
            DropIndex("dbo.Categories", new[] { "Supplier_Id" });
            DropIndex("dbo.Products", new[] { "Shop_Id" });
            DropIndex("dbo.Products", new[] { "CategoryId" });
            DropIndex("dbo.OrderDetails", new[] { "OrderId" });
            DropIndex("dbo.OrderDetails", new[] { "Product_id" });
            DropIndex("dbo.Orders", new[] { "Client_Id" });
            DropIndex("dbo.Orders", new[] { "OrderStatus_ID" });
            DropIndex("dbo.Beneficiary_Signature", new[] { "OrderId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.QRCodes");
            DropTable("dbo.Schedules");
            DropTable("dbo.Drivers");
            DropTable("dbo.Carts");
            DropTable("dbo.CartCopies");
            DropTable("dbo.OrderStatus");
            DropTable("dbo.Eshops");
            DropTable("dbo.Suppliers");
            DropTable("dbo.Categories");
            DropTable("dbo.Products");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.Clients");
            DropTable("dbo.Orders");
            DropTable("dbo.Beneficiary_Signature");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
        }
    }
}
