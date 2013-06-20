USE PortfolioViewer
GO

IF OBJECT_ID('dbo.usp_GetUserByName') IS NOT NULL DROP PROCEDURE dbo.usp_GetUserByName
GO
CREATE PROCEDURE dbo.usp_GetUserByName
    @Username VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON
    SELECT USER_ID, USERNAME, PWD_HASH, FULL_NAME, ROLE_CD, ACTIVE_FLG, LAST_LOGIN_DT
    FROM APP_USER
    WHERE USERNAME = UPPER(LTRIM(RTRIM(@Username)))
END
GO

IF OBJECT_ID('dbo.usp_SetLastLogin') IS NOT NULL DROP PROCEDURE dbo.usp_SetLastLogin
GO
CREATE PROCEDURE dbo.usp_SetLastLogin
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON
    UPDATE APP_USER SET LAST_LOGIN_DT = GETDATE() WHERE USER_ID = @UserId
END
GO

IF OBJECT_ID('dbo.usp_ClientSearch') IS NOT NULL DROP PROCEDURE dbo.usp_ClientSearch
GO
-- RJM 2013-07. Search by name or ref. Wildcards both sides, name search is slow but table is small.
-- 2015-11 DW: include closed accounts in count (ops asked)
CREATE PROCEDURE dbo.usp_ClientSearch
    @SearchText VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON
    SELECT c.CLIENT_ID, c.CLIENT_REF, c.CLIENT_NAME, c.STATUS_CD,
           u.USERNAME AS ADVISER_USERNAME, u.FULL_NAME AS ADVISER_NAME,
           (SELECT COUNT(*) FROM ACCOUNT a WHERE a.CLIENT_ID = c.CLIENT_ID) AS ACCT_COUNT
    FROM CLIENT c
    LEFT JOIN APP_USER u ON u.USER_ID = c.ADVISER_USER_ID
    WHERE c.CLIENT_NAME LIKE '%' + @SearchText + '%'
       OR c.CLIENT_REF LIKE @SearchText + '%'
    ORDER BY c.CLIENT_NAME
END
GO
