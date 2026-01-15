-- Database: toyana_vendor
CREATE DATABASE toyana_vendor;
\c toyana_vendor;

CREATE TABLE Vendors (
    Id UUID PRIMARY KEY,
    BusinessName VARCHAR(255) NOT NULL,
    TaxId VARCHAR(50) NOT NULL UNIQUE, -- STIR/INN
    LegalType VARCHAR(50) NOT NULL, -- LLC, Self-Employed, etc.
    IsVerified BOOLEAN DEFAULT FALSE,
    CreatedAt TIMESTAMPTZ DEFAULT NOW(),
    ContactEmail VARCHAR(255),
    PhoneNumber VARCHAR(50)
);

CREATE TABLE ServicePackages (
    Id UUID PRIMARY KEY,
    VendorId UUID NOT NULL REFERENCES Vendors(Id),
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    BasePrice DECIMAL(18,2) NOT NULL,
    Currency VARCHAR(3) DEFAULT 'UZS',
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE AvailabilityCalendar (
    Id UUID PRIMARY KEY,
    VendorId UUID NOT NULL REFERENCES Vendors(Id),
    Date DATE NOT NULL,
    Status VARCHAR(50) NOT NULL, -- Available, Booked, Blackout
    Notes TEXT,
    UNIQUE(VendorId, Date)
);

CREATE INDEX idx_vendor_availability ON AvailabilityCalendar(VendorId, Date);
