/*
寵物
*/
CREATE TABLE [dbo].[Pet] (
    [PetID]             BIGINT        IDENTITY (1, 1) NOT NULL,
    [PetName]           NVARCHAR(50) NOT NULL,
    [Gender]            VARCHAR(100)  NOT NULL,
    [Breed]             VARCHAR(100)  NOT NULL,
    [BirthDay]          DATE          NULL,
    [CoatColor]        VARCHAR(100)  NULL,
    [BodyWeight]       DECIMAL (5, 1) NULL,
    [NormalPrice]       MONEY         NULL,
    [SubscriptionPrice] MONEY         NULL,
    [CreateUser]        NVARCHAR (20) NOT NULL,
    [CreateTime]        DATETIME      DEFAULT (getdate()) NOT NULL,
    [ModifyUser]        NVARCHAR (20) NOT NULL,
    [ModifyTime]        DATETIME      DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([PetID] ASC)
);

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'名字', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pet', @level2type = N'COLUMN', @level2name = N'PetName';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'性別', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pet', @level2type = N'COLUMN', @level2name = N'Gender';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'品種', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pet', @level2type = N'COLUMN', @level2name = N'Breed';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'生日', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pet', @level2type = N'COLUMN', @level2name = N'BirthDay';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'毛色', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pet', @level2type = N'COLUMN', @level2name = N'CoatColor';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'體重(公斤)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pet', @level2type = N'COLUMN', @level2name = N'BodyWeight';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'單次價格', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pet', @level2type = N'COLUMN', @level2name = N'NormalPrice';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'包月價格', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pet', @level2type = N'COLUMN', @level2name = N'SubscriptionPrice';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寵物管理 - 存放寵物基本資訊，包含名稱、品種、性別、生日及定價資訊', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pet';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寵物唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pet', @level2type = N'COLUMN', @level2name = N'PetID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pet', @level2type = N'COLUMN', @level2name = N'CreateUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pet', @level2type = N'COLUMN', @level2name = N'CreateTime';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pet', @level2type = N'COLUMN', @level2name = N'ModifyUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pet', @level2type = N'COLUMN', @level2name = N'ModifyTime';