USE SevenElevenDB;
GO

INSERT INTO dbo.ProductCategory (Name)
SELECT v.Name
FROM (VALUES
    (N'Beverages'),
    (N'Ready Meals'),
    (N'Snacks'),
    (N'Candy')
) AS v(Name)
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.ProductCategory AS pc
    WHERE pc.Name = v.Name
);
-- Insert products

IF NOT EXISTS (SELECT 1 FROM Product WHERE Barcode = '5700000000011')
BEGIN
    INSERT INTO Product
        (Name, Barcode, PurchasePrice, SellingPrice,
         ShelfLife, IsSeasonal, ProductCategoryId)
    VALUES
        ('Coca-Cola 0.5L', '5700000000011', 8.00, 22.00,
         180, 0,
         (SELECT ProductCategoryId FROM ProductCategory
          WHERE Name = 'Beverages'));
END

IF NOT EXISTS (SELECT 1 FROM Product WHERE Barcode = '5700000000028')
BEGIN
    INSERT INTO Product
        (Name, Barcode, PurchasePrice, SellingPrice,
         ShelfLife, IsSeasonal, ProductCategoryId)
    VALUES
        ('Red Bull 250ml', '5700000000028', 10.00, 25.00,
         365, 0,
         (SELECT ProductCategoryId FROM ProductCategory
          WHERE Name = 'Beverages'));
END

IF NOT EXISTS (SELECT 1 FROM Product WHERE Barcode = '5700000000035')
BEGIN
    INSERT INTO Product
        (Name, Barcode, PurchasePrice, SellingPrice,
         ShelfLife, IsSeasonal, ProductCategoryId)
    VALUES
        ('Chicken Sandwich', '5700000000035', 18.00, 39.00,
         3, 0,
         (SELECT ProductCategoryId FROM ProductCategory
          WHERE Name = 'Ready Meals'));
END

IF NOT EXISTS (SELECT 1 FROM Product WHERE Barcode = '5700000000042')
BEGIN
    INSERT INTO Product
        (Name, Barcode, PurchasePrice, SellingPrice,
         ShelfLife, IsSeasonal, ProductCategoryId)
    VALUES
        ('Potato Chips', '5700000000042', 9.00, 24.00,
         120, 0,
         (SELECT ProductCategoryId FROM ProductCategory
          WHERE Name = 'Snacks'));
END

IF NOT EXISTS (SELECT 1 FROM Product WHERE Barcode = '5700000000059')
BEGIN
    INSERT INTO Product
        (Name, Barcode, PurchasePrice, SellingPrice,
         ShelfLife, IsSeasonal, ProductCategoryId)
    VALUES
        ('Chocolate Bar', '5700000000059', 6.00, 18.00,
         240, 0,
         (SELECT ProductCategoryId FROM ProductCategory
          WHERE Name = 'Candy'));
END


-- Insert stores

IF NOT EXISTS (SELECT 1 FROM Store WHERE Name = '7-Eleven Store 1')
BEGIN
    INSERT INTO Store (Name, Address)
    VALUES ('7-Eleven Store 1', 'Test Address 1');
END

IF NOT EXISTS (SELECT 1 FROM Store WHERE Name = '7-Eleven Store 2')
BEGIN
    INSERT INTO Store (Name, Address)
    VALUES ('7-Eleven Store 2', 'Test Address 2');
END

IF NOT EXISTS (SELECT 1 FROM Store WHERE Name = '7-Eleven Store 3')
BEGIN
    INSERT INTO Store (Name, Address)
    VALUES ('7-Eleven Store 3', 'Test Address 3');
END


-- Insert inventory test data

INSERT INTO Inventory
    (ProductId, StoreId, Quantity, MinimumStock)
SELECT
    p.ProductId,
    s.StoreId,
    v.Quantity,
    v.MinimumStock
FROM (VALUES
    ('5700000000011', '7-Eleven Store 1', 24, 10),
    ('5700000000028', '7-Eleven Store 1', 5, 12),
    ('5700000000035', '7-Eleven Store 1', 0, 6),
    ('5700000000042', '7-Eleven Store 1', 30, 10),
    ('5700000000059', '7-Eleven Store 1', 15, 8),

    ('5700000000011', '7-Eleven Store 2', 8, 10),
    ('5700000000028', '7-Eleven Store 2', 20, 12),
    ('5700000000035', '7-Eleven Store 2', 7, 6),
    ('5700000000042', '7-Eleven Store 2', 18, 10),
    ('5700000000059', '7-Eleven Store 2', 4, 8),

    ('5700000000011', '7-Eleven Store 3', 40, 10),
    ('5700000000028', '7-Eleven Store 3', 15, 12),
    ('5700000000035', '7-Eleven Store 3', 3, 6),
    ('5700000000042', '7-Eleven Store 3', 25, 10),
    ('5700000000059', '7-Eleven Store 3', 12, 8)
) AS v(Barcode, StoreName, Quantity, MinimumStock)
INNER JOIN Product p
    ON p.Barcode = v.Barcode
INNER JOIN Store s
    ON s.Name = v.StoreName
WHERE NOT EXISTS (
    SELECT 1
    FROM Inventory i
    WHERE i.ProductId = p.ProductId
      AND i.StoreId = s.StoreId
);
