-- Database: toyana_identity
CREATE DATABASE toyana_identity;
\c toyana_identity;

CREATE TABLE Users (
    Id UUID PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PhoneNumber VARCHAR(20) NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt TIMESTAMPTZ DEFAULT NOW(),
    LastLogin TIMESTAMPTZ
);

CREATE INDEX idx_users_username ON Users(Username);
CREATE INDEX idx_users_phone ON Users(PhoneNumber);
