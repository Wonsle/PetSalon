/*
移除 Subscription 表的 Status 欄位
執行日期: 2025-07-30
說明: 移除包月訂閱服務的狀態管理欄位，改由前端或業務邏輯計算狀態
*/

USE [PetSalon]
GO

-- 先檢查是否存在 Status 欄位
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Subscription' 
    AND COLUMN_NAME = 'Status'
)
BEGIN
    PRINT '開始移除 Subscription 表的 Status 相關元素...'
    
    -- 1. 刪除 Status 欄位的索引
    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Subscription_Status')
    BEGIN
        PRINT '刪除索引 IX_Subscription_Status'
        DROP INDEX [IX_Subscription_Status] ON [dbo].[Subscription]
        PRINT '索引 IX_Subscription_Status 已刪除'
    END
    
    -- 2. 刪除 Status 欄位的檢查約束
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.CHECK_CONSTRAINTS WHERE CONSTRAINT_NAME = 'CK_Subscription_Status')
    BEGIN
        PRINT '刪除檢查約束 CK_Subscription_Status'
        ALTER TABLE [dbo].[Subscription] DROP CONSTRAINT [CK_Subscription_Status]
        PRINT '檢查約束 CK_Subscription_Status 已刪除'
    END
    
    -- 3. 刪除 Status 欄位的說明
    IF EXISTS (
        SELECT * FROM sys.extended_properties ep
        INNER JOIN sys.tables t ON ep.major_id = t.object_id
        INNER JOIN sys.columns c ON ep.major_id = c.object_id AND ep.minor_id = c.column_id
        WHERE t.name = 'Subscription' AND c.name = 'Status' AND ep.name = 'MS_Description'
    )
    BEGIN
        PRINT '刪除 Status 欄位說明'
        EXEC sp_dropextendedproperty 
            @name = N'MS_Description', 
            @level0type = N'SCHEMA', @level0name = N'dbo', 
            @level1type = N'TABLE', @level1name = N'Subscription', 
            @level2type = N'COLUMN', @level2name = N'Status'
        PRINT 'Status 欄位說明已刪除'
    END
    
    -- 4. 最後刪除 Status 欄位
    PRINT '刪除 Status 欄位'
    ALTER TABLE [dbo].[Subscription] DROP COLUMN [Status]
    PRINT 'Status 欄位已成功刪除'
    
    PRINT '移除 Subscription 表的 Status 相關元素完成!'
END
ELSE
BEGIN
    PRINT 'Subscription 表中不存在 Status 欄位，無需執行移除操作'
END

GO