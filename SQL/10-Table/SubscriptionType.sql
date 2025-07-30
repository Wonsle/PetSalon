/*
包月類型配置表
支援動態的包月方案設定，便於新增或修改包月服務類型
*/

-- 如果表存在則先刪除
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SubscriptionType')
BEGIN
    DROP TABLE [dbo].[SubscriptionType]
END
GO

CREATE TABLE [dbo].[SubscriptionType] (
    [SubscriptionTypeID]     BIGINT           IDENTITY (1, 1) NOT NULL,
    [TypeCode]               VARCHAR(20)      NOT NULL,       -- 類型代碼 (BATH/GROOM/MIXED)
    [TypeName]               NVARCHAR(50)     NOT NULL,       -- 類型名稱
    [DefaultUsageLimit]      INT              NOT NULL,       -- 預設次數限制
    [DefaultPrice]           DECIMAL(10,2)    NOT NULL,       -- 預設價格
    [AvailableServiceTypes]  NVARCHAR(200)    NULL,           -- 可用的服務類型 (JSON格式)
    [Description]            NVARCHAR(500)    NULL,           -- 描述
    [IsActive]               BIT              DEFAULT (1) NOT NULL,  -- 是否啟用
    [SortOrder]              INT              DEFAULT (0) NOT NULL,  -- 排序
    [CreateUser]             NVARCHAR(50)     NOT NULL,
    [CreateTime]             DATETIME2        DEFAULT (GETDATE()) NOT NULL,
    [ModifyUser]             NVARCHAR(50)     NOT NULL,
    [ModifyTime]             DATETIME2        DEFAULT (GETDATE()) NOT NULL,
    
    -- 主鍵約束
    CONSTRAINT [PK_SubscriptionType] PRIMARY KEY CLUSTERED ([SubscriptionTypeID] ASC),
    
    -- 唯一約束
    CONSTRAINT [UK_SubscriptionType_TypeCode] UNIQUE ([TypeCode]),
    
    -- 檢查約束
    CONSTRAINT [CK_SubscriptionType_TypeCode] CHECK ([TypeCode] IN ('BATH', 'GROOM', 'MIXED')),
    CONSTRAINT [CK_SubscriptionType_DefaultUsageLimit] CHECK ([DefaultUsageLimit] > 0),
    CONSTRAINT [CK_SubscriptionType_DefaultPrice] CHECK ([DefaultPrice] >= 0),
    CONSTRAINT [CK_SubscriptionType_SortOrder] CHECK ([SortOrder] >= 0)
);

-- 建立索引以提升查詢效能
CREATE NONCLUSTERED INDEX [IX_SubscriptionType_TypeCode] ON [dbo].[SubscriptionType] ([TypeCode]);
CREATE NONCLUSTERED INDEX [IX_SubscriptionType_IsActive] ON [dbo].[SubscriptionType] ([IsActive]);
CREATE NONCLUSTERED INDEX [IX_SubscriptionType_SortOrder] ON [dbo].[SubscriptionType] ([SortOrder]);

GO

-- 表說明
EXECUTE sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'包月類型配置管理 - 存放不同包月類型的設定參數，支援動態配置和服務類型管理', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'SubscriptionType';

-- 欄位說明
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'包月類型唯一識別碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SubscriptionType', @level2type = N'COLUMN', @level2name = N'SubscriptionTypeID';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'類型代碼 (BATH洗澡/GROOM美容/MIXED混合)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SubscriptionType', @level2type = N'COLUMN', @level2name = N'TypeCode';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'類型名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SubscriptionType', @level2type = N'COLUMN', @level2name = N'TypeName';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預設次數限制', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SubscriptionType', @level2type = N'COLUMN', @level2name = N'DefaultUsageLimit';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預設價格', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SubscriptionType', @level2type = N'COLUMN', @level2name = N'DefaultPrice';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'可用的服務類型 (JSON格式儲存)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SubscriptionType', @level2type = N'COLUMN', @level2name = N'AvailableServiceTypes';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'包月類型描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SubscriptionType', @level2type = N'COLUMN', @level2name = N'Description';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否啟用 (1啟用/0禁用)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SubscriptionType', @level2type = N'COLUMN', @level2name = N'IsActive';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'排序順序', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SubscriptionType', @level2type = N'COLUMN', @level2name = N'SortOrder';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SubscriptionType', @level2type = N'COLUMN', @level2name = N'CreateUser';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SubscriptionType', @level2type = N'COLUMN', @level2name = N'CreateTime';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SubscriptionType', @level2type = N'COLUMN', @level2name = N'ModifyUser';
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SubscriptionType', @level2type = N'COLUMN', @level2name = N'ModifyTime';

GO