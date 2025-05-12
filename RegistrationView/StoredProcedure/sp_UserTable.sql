USE [aspnet-53bc9b9d-9d6a-45d4-8429-2a2761773502]
GO
/****** Object:  StoredProcedure [dbo].[sp_UserTable]    Script Date: 2025/05/12 21:04:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_UserTable]
    @Fullname NVARCHAR(250) = NULL,
    @EmailAddress NVARCHAR(250) = NULL,
    @Passwords NVARCHAR(255) = NULL,
    @Action NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Action = 'ADD'
    BEGIN
        INSERT INTO [dbo].[Users] ([Fullname], [EmailAddress], [Passwords])
        VALUES (@Fullname, @EmailAddress, @Passwords)
    END

	IF @Action = 'CHECK_EMAIL'
    BEGIN
        SELECT *
        FROM [dbo].[Users]
        WHERE EmailAddress = @EmailAddress
    END

    ELSE IF @Action = 'RESET'
    BEGIN
        UPDATE [dbo].[Users]
        SET [Passwords] = @Passwords
        WHERE EmailAddress = @EmailAddress
    END

    ELSE IF @Action = 'LOGIN'
    BEGIN
        SELECT *
        FROM [dbo].[Users]
        WHERE EmailAddress = @EmailAddress
    END
END