/*
移除 ReserveRecord 表的 ServiceType 欄位
此欄位會造成資料庫約束衝突，且不需要儲存
*/

-- 1. 移除相關的 CHECK 約束
IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_ReserveRecord_ServiceType')
BEGIN
    ALTER TABLE [dbo].[ReserveRecord] DROP CONSTRAINT [CK_ReserveRecord_ServiceType];
    PRINT 'Dropped CHECK constraint: CK_ReserveRecord_ServiceType';
END
GO

-- 2. 移除索引
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ReserveRecord_ServiceType' AND object_id = OBJECT_ID('dbo.ReserveRecord'))
BEGIN
    DROP INDEX [IX_ReserveRecord_ServiceType] ON [dbo].[ReserveRecord];
    PRINT 'Dropped index: IX_ReserveRecord_ServiceType';
END
GO

-- 3. 移除欄位
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
           WHERE TABLE_NAME = 'ReserveRecord'
           AND COLUMN_NAME = 'ServiceType')
BEGIN
    ALTER TABLE [dbo].[ReserveRecord] DROP COLUMN [ServiceType];
    PRINT 'Dropped column: ServiceType from ReserveRecord';
END
GO

PRINT 'ServiceType 欄位已成功從 ReserveRecord 表中移除';
GO
