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

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'系統代碼管理 - 存放系統各種類型的代碼配置，如品種、性別、服務類型等', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SystemCode';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'代碼唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SystemCode', @level2type = N'COLUMN', @level2name = N'CodeID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'代碼類型，如Breed、Gender、ServiceType等', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SystemCode', @level2type = N'COLUMN', @level2name = N'CodeType';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'代碼值', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SystemCode', @level2type = N'COLUMN', @level2name = N'Code';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'代碼顯示名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SystemCode', @level2type = N'COLUMN', @level2name = N'Name';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'生效日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SystemCode', @level2type = N'COLUMN', @level2name = N'StartDate';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'失效日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SystemCode', @level2type = N'COLUMN', @level2name = N'EndDate';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'顯示排序', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SystemCode', @level2type = N'COLUMN', @level2name = N'Sort';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'描述說明', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SystemCode', @level2type = N'COLUMN', @level2name = N'Description';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SystemCode', @level2type = N'COLUMN', @level2name = N'CreateUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SystemCode', @level2type = N'COLUMN', @level2name = N'CreateTime';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SystemCode', @level2type = N'COLUMN', @level2name = N'ModifyUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SystemCode', @level2type = N'COLUMN', @level2name = N'ModifyTime';  

