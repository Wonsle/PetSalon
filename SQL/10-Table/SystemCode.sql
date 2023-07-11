/*
	系統代碼
*/
CREATE TABLE [dbo].[SystemCode]
(
	[CodeID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[CodeType] VARCHAR(100) NOT NULL,    
	[Code] VARCHAR(100) DEFAULT ('') NOT NULL,    
	[Name] VARCHAR(100) DEFAULT ('') NOT NULL,    
	[StartDate] DATE NOT NULL DEFAULT (getdate()),    
	[EndDate] DATE,
	[Sort] INT DEFAULT(0),
	[Description] NVARCHAR(200) DEFAULT ('') ,
    [CreateUser] NVARCHAR(20) NOT NULL,
    [CreateTime] DATETIME NOT NULL DEFAULT getdate(), 
    [ModifyUser] NVARCHAR(20) NOT NULL,
    [ModifyTime] DATETIME NOT NULL DEFAULT getdate(),
)
ALTER TABLE SystemCode 
ADD CONSTRAINT UQ_CodeType UNIQUE (CodeType, Code);  

