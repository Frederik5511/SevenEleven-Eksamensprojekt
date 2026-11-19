
-- Opret databasen
CREATE DATABASE SevenElevenDB;
GO

USE SevenElevenDB;
GO

-- Opret Produkt
CREATE TABLE Produkt (
    ProduktId INT PRIMARY KEY IDENTITY(1,1),
    Navn NVARCHAR(100) NOT NULL,
    Stregkode NVARCHAR(50),
    Indkøbspris DECIMAL(10,2),
    Salgspris DECIMAL(10,2),
    Holdbarhed INT,
    ErSæsonvare BIT
);

-- Opret Produktgruppe
CREATE TABLE Produktgruppe (
    ProduktgruppeId INT PRIMARY KEY IDENTITY(1,1),
    Navn NVARCHAR(100) NOT NULL,
    Beskrivelse NVARCHAR(255)
);

-- Tilføj relation mellem Produkt og Produktgruppe
ALTER TABLE Produkt
ADD ProduktgruppeId INT;

ALTER TABLE Produkt
ADD CONSTRAINT FK_Produkt_Produktgruppe
FOREIGN KEY (ProduktgruppeId)
REFERENCES Produktgruppe(ProduktgruppeId);


-- Opret Butik
CREATE TABLE Butik (
    ButikId INT PRIMARY KEY IDENTITY(1,1),
    Navn NVARCHAR(100) NOT NULL,
    Adresse NVARCHAR(200) NOT NULL
);


-- Opret Lagerbeholdning
CREATE TABLE Lagerbeholdning (
    LagerbeholdningId INT PRIMARY KEY IDENTITY(1,1),
    ProduktId INT NOT NULL,
    ButikId INT NOT NULL,
    Antal INT NOT NULL DEFAULT 0,
    Minimumsbeholdning INT NOT NULL DEFAULT 0,
    SidstOpdateret DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_Lagerbeholdning_Produkt
        FOREIGN KEY (ProduktId)
        REFERENCES Produkt(ProduktId),

    CONSTRAINT FK_Lagerbeholdning_Butik
        FOREIGN KEY (ButikId)
        REFERENCES Butik(ButikId),

    CONSTRAINT UQ_Lagerbeholdning_Produkt_Butik
        UNIQUE (ProduktId, ButikId),

    CONSTRAINT CK_Lagerbeholdning_Antal
        CHECK (Antal >= 0),

    CONSTRAINT CK_Lagerbeholdning_Minimum
        CHECK (Minimumsbeholdning >= 0)
);

