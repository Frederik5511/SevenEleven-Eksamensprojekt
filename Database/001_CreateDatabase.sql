
-- Create database
CREATE DATABASE SevenElevenDB;
GO

USE SevenElevenDB;
GO

-- Create ProductCategory
CREATE TABLE ProductCategory (
    ProductCategoryId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255)
);

-- Create Product
CREATE TABLE Product (
    ProductId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
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

-- Create Store
CREATE TABLE Store (
    StoreId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Address NVARCHAR(200) NOT NULL
);

-- Create Inventory
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
