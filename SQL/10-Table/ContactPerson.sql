/*
聯絡人
*/
CREATE TABLE [dbo].[ContactPerson] (
    [ContactPersonID] BIGINT        IDENTITY (1, 1) NOT NULL PRIMARY KEY,
    [Name]            VARCHAR (20)  NOT NULL,
    [NickName]        VARCHAR (20)  NULL,
    [ContactNumber]   VARCHAR (10)  NOT NULL,
    [CreateUser]      NVARCHAR (20) NOT NULL,
    [CreateTime]      DATETIME      DEFAULT (getdate()) NOT NULL,
    [ModifyUser]      NVARCHAR (20) NOT NULL,
    [ModifyTime]      DATETIME      DEFAULT (getdate()) NOT NULL    
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'暱稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ContactPerson', @level2type = N'COLUMN', @level2name = N'NickName';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'名字', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ContactPerson', @level2type = N'COLUMN', @level2name = N'Name';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'連絡電話', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ContactPerson', @level2type = N'COLUMN', @level2name = N'ContactNumber';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'聯絡人管理 - 存放寵物飼主及相關聯絡人的基本資訊', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ContactPerson';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'聯絡人唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ContactPerson', @level2type = N'COLUMN', @level2name = N'ContactPersonID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ContactPerson', @level2type = N'COLUMN', @level2name = N'CreateUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ContactPerson', @level2type = N'COLUMN', @level2name = N'CreateTime';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ContactPerson', @level2type = N'COLUMN', @level2name = N'ModifyUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ContactPerson', @level2type = N'COLUMN', @level2name = N'ModifyTime';

