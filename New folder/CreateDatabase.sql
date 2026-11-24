
-- Create database
IF DB_ID('SevenElevenDB') IS NULL
BEGIN
    CREATE DATABASE SevenElevenDB;
END
GO

USE SevenElevenDB;
GO

-- Create ProductCategory
IF OBJECT_ID('dbo.ProductCategory', 'U') IS NULL
BEGIN
    CREATE TABLE ProductCategory (
        ProductCategoryId INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL UNIQUE,
        Description NVARCHAR(255)
    );
END
GO

-- Create Product
IF OBJECT_ID('dbo.Product', 'U') IS NULL
BEGIN
    CREATE TABLE Product (
        ProductId INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL UNIQUE,
        Barcode NVARCHAR(50),
        PurchasePrice DECIMAL(10,2),
        SellingPrice DECIMAL(10,2),
        ShelfLife INT,
        IsSeasonal BIT,
        ProductCategoryId INT,

        CONSTRAINT FK_Product_ProductCategory
            FOREIGN KEY (ProductCategoryId)
            REFERENCES ProductCategory(ProductCategoryId)
    );
END
GO

-- Create Store
IF OBJECT_ID('dbo.Store', 'U') IS NULL
BEGIN
    CREATE TABLE Store (
        StoreId INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL UNIQUE,
        Address NVARCHAR(200) NOT NULL
    );
END
GO

-- Create Inventory
IF OBJECT_ID('dbo.Inventory', 'U') IS NULL
BEGIN
    CREATE TABLE Inventory (
        InventoryId INT PRIMARY KEY IDENTITY(1,1),
        ProductId INT NOT NULL,
        StoreId INT NOT NULL,
        Quantity INT NOT NULL DEFAULT 0,
        MinimumStock INT NOT NULL DEFAULT 0,
        LastUpdated DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

        CONSTRAINT FK_Inventory_Product
            FOREIGN KEY (ProductId)
            REFERENCES Product(ProductId),

        CONSTRAINT FK_Inventory_Store
            FOREIGN KEY (StoreId)
            REFERENCES Store(StoreId),

        CONSTRAINT UQ_Inventory_Product_Store
            UNIQUE (ProductId, StoreId),

        CONSTRAINT CK_Inventory_Quantity
            CHECK (Quantity >= 0),

        CONSTRAINT CK_Inventory_MinimumStock
            CHECK (MinimumStock >= 0)
    );
END
GO
