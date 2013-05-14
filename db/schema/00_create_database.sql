-- Portfolio Viewer reference database
-- RJM 2013-05: created on PVSQL01. Recreate with this script on a new box.
-- 2018-02 KP: moved to SQL Express, keep compatibility at 2008 (100) - stored procs not tested on anything newer.
USE master
GO

IF DB_ID('PortfolioViewer') IS NULL
    CREATE DATABASE PortfolioViewer
GO

ALTER DATABASE PortfolioViewer SET COMPATIBILITY_LEVEL = 100
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'pv_app')
    CREATE LOGIN pv_app WITH PASSWORD = 'pvapp2013', CHECK_POLICY = OFF
GO

USE PortfolioViewer
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'pv_app')
BEGIN
    CREATE USER pv_app FOR LOGIN pv_app
    EXEC sp_addrolemember 'db_datareader', 'pv_app'
    EXEC sp_addrolemember 'db_datawriter', 'pv_app'
END
GO
GRANT EXECUTE TO pv_app
GO
