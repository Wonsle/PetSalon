/*
檔案附件管理 - 統一檔案上傳和關聯管理
*/
CREATE TABLE [dbo].[FileAttachment] (
    [FileID]           BIGINT IDENTITY(1,1) NOT NULL,
    [OriginalFileName] NVARCHAR(255) NOT NULL,
    [StoredFileName]   NVARCHAR(255) NOT NULL,
    [FilePath]         NVARCHAR(500) NOT NULL,
    [FileSize]         BIGINT NOT NULL,
    [MimeType]         VARCHAR(100) NOT NULL,
    [Extension]        VARCHAR(10) NOT NULL,
    [FileHash]         VARCHAR(64) NOT NULL,
    [EntityType]       VARCHAR(50) NOT NULL,
    [EntityID]         BIGINT NOT NULL,
    [AttachmentType]   VARCHAR(50) NOT NULL,
    [DisplayOrder]     INT DEFAULT 0,
    [IsActive]         BIT DEFAULT 1,
    [CreateUser]       NVARCHAR(20) NOT NULL,
    [CreateTime]       DATETIME DEFAULT (GETDATE()) NOT NULL,
    [ModifyUser]       NVARCHAR(20) NOT NULL,
    [ModifyTime]       DATETIME DEFAULT (GETDATE()) NOT NULL,
    PRIMARY KEY CLUSTERED ([FileID] ASC)
);

GO
CREATE NONCLUSTERED INDEX [IX_FileAttachment_Entity]
    ON [dbo].[FileAttachment]([EntityType] ASC, [EntityID] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_FileAttachment_Hash]
    ON [dbo].[FileAttachment]([FileHash] ASC);

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'檔案唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FileAttachment', @level2type = N'COLUMN', @level2name = N'FileID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'原始檔案名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FileAttachment', @level2type = N'COLUMN', @level2name = N'OriginalFileName';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'儲存的檔案名稱（含GUID）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FileAttachment', @level2type = N'COLUMN', @level2name = N'StoredFileName';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'檔案相對路徑', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FileAttachment', @level2type = N'COLUMN', @level2name = N'FilePath';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'檔案大小（bytes）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FileAttachment', @level2type = N'COLUMN', @level2name = N'FileSize';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'MIME類型', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FileAttachment', @level2type = N'COLUMN', @level2name = N'MimeType';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'副檔名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FileAttachment', @level2type = N'COLUMN', @level2name = N'Extension';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'檔案SHA256 Hash值（用於檢測重複檔案）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FileAttachment', @level2type = N'COLUMN', @level2name = N'FileHash';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'實體類型（Pet, ContactPerson, Service等）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FileAttachment', @level2type = N'COLUMN', @level2name = N'EntityType';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'實體ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FileAttachment', @level2type = N'COLUMN', @level2name = N'EntityID';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'附件類型（Avatar, Photo, Document等）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FileAttachment', @level2type = N'COLUMN', @level2name = N'AttachmentType';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'顯示順序', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FileAttachment', @level2type = N'COLUMN', @level2name = N'DisplayOrder';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否啟用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FileAttachment', @level2type = N'COLUMN', @level2name = N'IsActive';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FileAttachment', @level2type = N'COLUMN', @level2name = N'CreateUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FileAttachment', @level2type = N'COLUMN', @level2name = N'CreateTime';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FileAttachment', @level2type = N'COLUMN', @level2name = N'ModifyUser';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FileAttachment', @level2type = N'COLUMN', @level2name = N'ModifyTime';

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'檔案附件管理 - 統一管理檔案上傳、儲存和關聯，支援多種實體類型的檔案附件', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'FileAttachment';
