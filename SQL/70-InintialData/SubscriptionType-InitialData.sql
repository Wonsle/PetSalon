/*
包月類型配置表 - 初始資料
建立預設的包月類型配置
*/

-- 確保表格存在
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SubscriptionType')
BEGIN
    PRINT '錯誤: SubscriptionType 表格不存在，請先執行建表腳本'
    RETURN
END

-- 清空現有資料（如果需要重建）
-- DELETE FROM [dbo].[SubscriptionType]

-- 插入預設包月類型配置
INSERT INTO [dbo].[SubscriptionType] 
(
    [TypeCode], 
    [TypeName], 
    [DefaultUsageLimit], 
    [DefaultPrice], 
    [AvailableServiceTypes], 
    [Description], 
    [IsActive], 
    [SortOrder], 
    [CreateUser], 
    [ModifyUser]
) 
VALUES 
(
    'BATH', 
    N'洗澡包月', 
    4, 
    800.00, 
    N'["BATH", "NAIL_TRIM", "EAR_CLEAN", "BASIC_GROOM"]', 
    N'每月4次洗澡服務，包含基本清潔項目', 
    1, 
    1, 
    'SYSTEM', 
    'SYSTEM'
),
(
    'GROOM', 
    N'美容包月', 
    2, 
    1200.00, 
    N'["FULL_GROOM", "STYLING", "NAIL_TRIM", "EAR_CLEAN", "TEETH_CLEAN"]', 
    N'每月2次完整美容服務，包含造型設計', 
    1, 
    2, 
    'SYSTEM', 
    'SYSTEM'
),
(
    'MIXED', 
    N'混合包月', 
    6, 
    1500.00, 
    N'["BATH", "GROOM", "NAIL_TRIM", "EAR_CLEAN", "STYLING", "SPA"]', 
    N'每月6次混合服務，可彈性搭配洗澡和美容', 
    1, 
    3, 
    'SYSTEM', 
    'SYSTEM'
);

PRINT '包月類型配置初始資料建立完成'

-- 顯示插入的資料
SELECT 
    SubscriptionTypeID,
    TypeCode,
    TypeName,
    DefaultUsageLimit,
    DefaultPrice,
    Description,
    IsActive,
    SortOrder
FROM [dbo].[SubscriptionType]
ORDER BY SortOrder;

GO