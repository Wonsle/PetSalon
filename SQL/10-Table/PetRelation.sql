/*
聯絡人與寵物關聯
*/
CREATE TABLE [dbo].[PetRelation] (
    [PetRelationID]   BIGINT          IDENTITY (1, 1) NOT NULL,
    [PetID]           BIGINT           NOT NULL,
    [ContactPersonID] BIGINT           NOT NULL,
    [Sort]            INT           DEFAULT ((0)) NOT NULL,
    [CreateUser]      NVARCHAR (20) NOT NULL,
    [CreateTime]      DATETIME      DEFAULT (getdate()) NOT NULL,
    [ModifyUser]      NVARCHAR (20) NOT NULL,
    [ModifyTime]      DATETIME      DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([PetRelationID] ASC),
    CONSTRAINT [FK_PetRelation_ContactPerson] FOREIGN KEY ([ContactPersonID]) REFERENCES [dbo].[ContactPerson] ([ContactPersonID]),
    CONSTRAINT [FK_PetRelation_Pet] FOREIGN KEY ([PetID]) REFERENCES [dbo].[Pet] ([PetID])
);


