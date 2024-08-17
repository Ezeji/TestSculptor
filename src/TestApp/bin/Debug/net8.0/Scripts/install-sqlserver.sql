-- install-sqlserver.sql

-- Variable to store the database name
:setvar DatabaseName "TestSculptorDb"

-- Check if the database exists, and if not, create it
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '$(DatabaseName)')
BEGIN
    CREATE DATABASE [$(DatabaseName)];
END
GO

-- Switch context to the new database
USE [$(DatabaseName)];
GO

-- Execute other scripts to set up database schema
-- Change the paths to point to your actual script files
:r .\schema\Orders.sql
:r .\schema\Products.sql
