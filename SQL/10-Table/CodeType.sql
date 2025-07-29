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

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'代碼類型管理 - 定義系統中使用的各種代碼類型分類', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'CodeType';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'代碼類型唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'CodeType', @level2type = N'COLUMN', @level2name = N'ID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'代碼類型代碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'CodeType', @level2type = N'COLUMN', @level2name = N'CodeType';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'代碼類型名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'CodeType', @level2type = N'COLUMN', @level2name = N'Name';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'描述說明', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'CodeType', @level2type = N'COLUMN', @level2name = N'Description';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'CodeType', @level2type = N'COLUMN', @level2name = N'CreateUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'CodeType', @level2type = N'COLUMN', @level2name = N'CreateTime';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'CodeType', @level2type = N'COLUMN', @level2name = N'ModifyUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'CodeType', @level2type = N'COLUMN', @level2name = N'ModifyTime';


