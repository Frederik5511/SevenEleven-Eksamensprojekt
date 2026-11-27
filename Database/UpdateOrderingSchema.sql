USE SevenElevenDB;
GO

IF OBJECT_ID(N'dbo.Supplier', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Supplier (
        SupplierId INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        ContactInformation NVARCHAR(255) NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.[Order]', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.[Order] (
        OrderId INT PRIMARY KEY IDENTITY(1,1),
        SupplierId INT NOT NULL,
        StoreId INT NOT NULL,
        OrderDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
        ExpectedDeliveryDate DATETIME2 NULL,
        Status NVARCHAR(20) NOT NULL DEFAULT N'Oprettet',
        CONSTRAINT FK_Order_Supplier FOREIGN KEY (SupplierId) REFERENCES dbo.Supplier(SupplierId),
        CONSTRAINT FK_Order_Store FOREIGN KEY (StoreId) REFERENCES dbo.Store(StoreId),
        CONSTRAINT CK_Order_Status CHECK (Status IN (N'Oprettet', N'Bekræftet', N'Leveret', N'Annulleret'))
    );
END;
GO

IF OBJECT_ID(N'dbo.OrderLine', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrderLine (
        OrderLineId INT PRIMARY KEY IDENTITY(1,1),
        OrderId INT NOT NULL,
        ProductId INT NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(10,2) NOT NULL,
        CONSTRAINT FK_OrderLine_Order FOREIGN KEY (OrderId) REFERENCES dbo.[Order](OrderId),
        CONSTRAINT FK_OrderLine_Product FOREIGN KEY (ProductId) REFERENCES dbo.Product(ProductId),
        CONSTRAINT CK_OrderLine_Quantity CHECK (Quantity > 0)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Supplier WHERE Name = N'7-Eleven Leverandør')
    INSERT INTO dbo.Supplier (Name, ContactInformation)
    VALUES (N'7-Eleven Leverandør', N'Demo-leverandør');
GO
