
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

-- Supplier and order tables from the ordering use case / DCD
CREATE TABLE Supplier (
    SupplierId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    ContactInformation NVARCHAR(255) NULL
);

CREATE TABLE [Order] (
    OrderId INT PRIMARY KEY IDENTITY(1,1),
    SupplierId INT NOT NULL,
    StoreId INT NOT NULL,
    OrderDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    ExpectedDeliveryDate DATETIME2 NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT N'Oprettet',
    CONSTRAINT FK_Order_Supplier FOREIGN KEY (SupplierId) REFERENCES Supplier(SupplierId),
    CONSTRAINT FK_Order_Store FOREIGN KEY (StoreId) REFERENCES Store(StoreId),
    CONSTRAINT CK_Order_Status CHECK (Status IN (N'Oprettet', N'Bekræftet', N'Leveret', N'Annulleret'))
);

CREATE TABLE OrderLine (
    OrderLineId INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_OrderLine_Order FOREIGN KEY (OrderId) REFERENCES [Order](OrderId),
    CONSTRAINT FK_OrderLine_Product FOREIGN KEY (ProductId) REFERENCES Product(ProductId),
    CONSTRAINT CK_OrderLine_Quantity CHECK (Quantity > 0)
);

IF NOT EXISTS (SELECT 1 FROM Supplier WHERE Name = N'7-Eleven Leverandør')
    INSERT INTO Supplier (Name, ContactInformation) VALUES (N'7-Eleven Leverandør', N'Demo-leverandør');
