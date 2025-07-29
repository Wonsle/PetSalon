/*
聯絡人與寵物關聯
*/
DROP TABLE IF EXISTS [dbo].[PetRelation];
CREATE TABLE [dbo].[PetRelation] (
    [PetRelationID]   BIGINT          IDENTITY (1, 1) NOT NULL,
    [PetID]           BIGINT           NOT NULL,
    [ContactPersonID] BIGINT           NOT NULL,
    [RelationshipType] VARCHAR(20)     NOT NULL DEFAULT('OWNER'),
    [Sort]            INT           DEFAULT ((0)) NOT NULL,
    [CreateUser]      NVARCHAR (20) NOT NULL,
    [CreateTime]      DATETIME      DEFAULT (getdate()) NOT NULL,
    [ModifyUser]      NVARCHAR (20) NOT NULL,
    [ModifyTime]      DATETIME      DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([PetRelationID] ASC),
    CONSTRAINT [FK_PetRelation_ContactPerson] FOREIGN KEY ([ContactPersonID]) REFERENCES [dbo].[ContactPerson] ([ContactPersonID]),
    CONSTRAINT [FK_PetRelation_Pet] FOREIGN KEY ([PetID]) REFERENCES [dbo].[Pet] ([PetID])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'關係類型', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetRelation', @level2type = N'COLUMN', @level2name = N'RelationshipType';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寵物關聯管理 - 建立寵物與聯絡人之間的關係，支援多種關係類型', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetRelation';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'關聯記錄唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetRelation', @level2type = N'COLUMN', @level2name = N'PetRelationID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'寵物ID，關聯至Pet表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetRelation', @level2type = N'COLUMN', @level2name = N'PetID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'聯絡人ID，關聯至ContactPerson表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetRelation', @level2type = N'COLUMN', @level2name = N'ContactPersonID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'顯示排序', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetRelation', @level2type = N'COLUMN', @level2name = N'Sort';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetRelation', @level2type = N'COLUMN', @level2name = N'CreateUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetRelation', @level2type = N'COLUMN', @level2name = N'CreateTime';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetRelation', @level2type = N'COLUMN', @level2name = N'ModifyUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PetRelation', @level2type = N'COLUMN', @level2name = N'ModifyTime';

