/*
預約紀錄明細表 - ReserveRecordDetail
存放寵物美容預約記錄，明細資訊
*/

-- 如果表存在則先刪除
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ReserveRecordDetail')
BEGIN
    DROP TABLE [dbo].[ReserveRecordDetail]
END
GO

CREATE TABLE [dbo].[ReserveRecordDetail] (
    [ReserveRecordDetailID]    BIGINT            IDENTITY (1, 1) NOT NULL,
    [ReserveRecordID]          BIGINT            NOT NULL,
    [ServiceType]            VARCHAR(100)     NOT NULL,
    [Price]                     MONEY             NOT NULL,
    [CreateUser]                NVARCHAR(50)      NOT NULL,
    [CreateTime]                DATETIME2         DEFAULT (GETDATE()) NOT NULL,
    [ModifyUser]                NVARCHAR(50)      NOT NULL,
    [ModifyTime]                DATETIME2         DEFAULT (GETDATE()) NOT NULL,

    -- 主鍵約束
    CONSTRAINT [PK_ReserveRecordDetail] PRIMARY KEY CLUSTERED ([ReserveRecordDetailID] ASC),
    CONSTRAINT [FK_ReserveRecordDetail_ReserveRecord] FOREIGN KEY ([ReserveRecordID]) REFERENCES [dbo].[ReserveRecord] ([ReserveRecordID])
);
-- 建立索引以優化常見查詢
CREATE UNIQUE NONCLUSTERED INDEX IX_ReserveRecordDetail_ReserveRecordDetailID
ON dbo.ReserveRecordDetail (ReserveRecordDetailID);

CREATE NONCLUSTERED INDEX IX_ReserveRecordDetail_ReserveRecordID_CreateTime
ON dbo.ReserveRecordDetail (ReserveRecordID, CreateTime);

CREATE NONCLUSTERED INDEX IX_ReserveRecordDetail_CreateTime_Includes
ON dbo.ReserveRecordDetail (CreateTime) INCLUDE (ReserveRecordID, CreateUser);

CREATE NONCLUSTERED INDEX IX_ReserveRecordDetail_ModifyTime_Includes
ON dbo.ReserveRecordDetail (ModifyTime) INCLUDE (ModifyUser);

GO

-- 表說明
EXECUTE sp_addextendedproperty
    @name = N'MS_Description',
    @value = N'預約管理 - 存放寵物美容預約記錄，明細資訊',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'ReserveRecordDetail';

-- 欄位說明
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預約記錄唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecordDetail', @level2type = N'COLUMN', @level2name = N'ReserveRecordDetailID';

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預約記錄識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecordDetail', @level2type = N'COLUMN', @level2name = N'ReserveRecordID';

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'服務類型', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecordDetail', @level2type = N'COLUMN', @level2name = N'ServiceType';

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecordDetail', @level2type = N'COLUMN', @level2name = N'CreateUser';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecordDetail', @level2type = N'COLUMN', @level2name = N'CreateTime';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecordDetail', @level2type = N'COLUMN', @level2name = N'ModifyUser';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ReserveRecordDetail', @level2type = N'COLUMN', @level2name = N'ModifyTime';

GO