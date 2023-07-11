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

