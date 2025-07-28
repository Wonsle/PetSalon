/*
    代碼類型
*/
CREATE TABLE [dbo].[CodeType]
(
    [ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CodeType] VARCHAR(100) NOT NULL,
    [Name] NVARCHAR(100) DEFAULT ('') NOT NULL,
    [Description] NVARCHAR(200) DEFAULT ('') ,
    [CreateUser] NVARCHAR(20) NOT NULL,
    [CreateTime] DATETIME NOT NULL DEFAULT getdate(), 
    [ModifyUser] NVARCHAR(20) NOT NULL,
    [ModifyTime] DATETIME NOT NULL DEFAULT getdate(),
)


