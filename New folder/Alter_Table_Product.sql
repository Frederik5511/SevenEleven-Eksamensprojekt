USE SevenElevenDB;
GO

EXEC sp_helpconstraint 'dbo.Product';

ALTER TABLE dbo.Product
ALTER COLUMN Barcode NVARCHAR(50) NOT NULL;

ALTER TABLE dbo.Product
ADD CONSTRAINT UQ_Product_Barcode UNIQUE (Barcode);

ALTER TABLE dbo.Product
DROP CONSTRAINT [UQ__Product__737584F6710FCC54];

USE SevenElevenDB;

SELECT * FROM dbo.ProductCategory;
SELECT * FROM dbo.Product;
SELECT * FROM dbo.Store;
SELECT * FROM dbo.Inventory;