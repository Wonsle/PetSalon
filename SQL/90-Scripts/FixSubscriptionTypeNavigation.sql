/*
修復 Subscription 表的 SubscriptionTypeNavigation 導航屬性問題
解決錯誤：無效的資料行名稱 'SubscriptionTypeNavigationSubscriptionTypeId'
*/

-- 檢查並添加 SubscriptionTypeID 欄位到 Subscription 表
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Subscription' AND COLUMN_NAME = 'SubscriptionTypeID')
BEGIN
    PRINT '正在添加 SubscriptionTypeID 欄位到 Subscription 表...'
    
    ALTER TABLE [dbo].[Subscription]
    ADD [SubscriptionTypeID] BIGINT NULL
    
    -- 添加欄位說明
    EXECUTE sp_addextendedproperty 
        @name = N'MS_Description', 
        @value = N'包月類型ID，關聯至SubscriptionType表', 
        @level0type = N'SCHEMA', @level0name = N'dbo', 
        @level1type = N'TABLE', @level1name = N'Subscription', 
        @level2type = N'COLUMN', @level2name = N'SubscriptionTypeID'
    
    PRINT 'SubscriptionTypeID 欄位已成功添加'
END
ELSE
BEGIN
    PRINT 'SubscriptionTypeID 欄位已存在，跳過添加'
END

-- 檢查並添加外鍵約束
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
               WHERE CONSTRAINT_NAME = 'FK_Subscription_SubscriptionType')
BEGIN
    -- 確保 SubscriptionType 表存在
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SubscriptionType')
    BEGIN
        PRINT '正在添加 FK_Subscription_SubscriptionType 外鍵約束...'
        
        ALTER TABLE [dbo].[Subscription]
        ADD CONSTRAINT [FK_Subscription_SubscriptionType]
        FOREIGN KEY ([SubscriptionTypeID]) REFERENCES [dbo].[SubscriptionType] ([SubscriptionTypeID])
        
        PRINT 'FK_Subscription_SubscriptionType 外鍵約束已成功添加'
    END
    ELSE
    BEGIN
        PRINT '警告：SubscriptionType 表不存在，無法建立外鍵約束'
        PRINT '請先執行 SubscriptionType.sql 建立 SubscriptionType 表'
    END
END
ELSE
BEGIN
    PRINT 'FK_Subscription_SubscriptionType 外鍵約束已存在，跳過添加'
END

-- 建立索引以提升查詢效能
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Subscription_SubscriptionTypeID')
BEGIN
    PRINT '正在建立 SubscriptionTypeID 索引...'
    
    CREATE NONCLUSTERED INDEX [IX_Subscription_SubscriptionTypeID]
    ON [dbo].[Subscription] ([SubscriptionTypeID])
    
    PRINT 'SubscriptionTypeID 索引已成功建立'
END
ELSE
BEGIN
    PRINT 'SubscriptionTypeID 索引已存在，跳過建立'
END

PRINT '修復完成！'
PRINT '現在 Entity Framework 應該能夠正確處理 SubscriptionTypeNavigation 導航屬性'