-- =============================================
-- CodeType 資料表初始化腳本
-- 用於系統代碼維護功能，確保所有代碼類型都有完整定義
-- =============================================

-- 確保所有需要的代碼類型都存在
MERGE INTO CodeType AS target
USING (VALUES
    ('Breed', '品種', '寵物的品種分類'),
    ('Gender', '性別', '寵物的性別分類'),
    ('Relationship', '關係', '客戶與寵物的關係'),
    ('ServiceType', '服務類型', '提供的服務類型'),
    ('ReservationStatus', '預約狀態', '預約的狀態分類'),
    ('PaymentType', '付款類型', '付款方式分類'),
    ('PaymentMethod', '付款方式', '付款記錄的付款方式分類'),
    ('SubscriptionStatus', '包月狀態', '包月服務的狀態'),
    ('CoatColor', '毛色', '寵物的毛色分類'),
    ('IncomeType', '收入類型', '寵物美容服務的收入類型分類'),
    ('RefundType', '退費類型', '退費記錄的退費類型分類')
) AS source (CodeType, Name, Description)
ON target.CodeType1 = source.CodeType
WHEN NOT MATCHED THEN
    INSERT (CodeType1, Name, Description, CreateUser, CreateTime, ModifyUser, ModifyTime)
    VALUES (source.CodeType, source.Name, source.Description, 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE())
WHEN MATCHED THEN
    UPDATE SET 
        Name = source.Name,
        Description = source.Description,
        ModifyUser = 'SYSTEM',
        ModifyTime = GETDATE();

GO

-- 顯示所有代碼類型
SELECT 
    Id,
    CodeType1 AS CodeType,
    Name,
    Description,
    CreateUser,
    CreateTime,
    ModifyUser,
    ModifyTime
FROM CodeType
ORDER BY CodeType1;

GO
