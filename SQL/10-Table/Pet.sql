/*
寵物
*/
CREATE TABLE [dbo].[Pet] (
    [PetID]             BIGINT        IDENTITY (1, 1) NOT NULL,
    [PetName]           NVARCHAR(50) NOT NULL,
    [Gender]            VARCHAR(100)  NOT NULL,
    [Breed]             VARCHAR(100)  NOT NULL,
    [BirthDay]          DATE          NULL,
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
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'單次價格', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pet', @level2type = N'COLUMN', @level2name = N'NormalPrice';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'包月價格', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Pet', @level2type = N'COLUMN', @level2name = N'SubscriptionPrice';