-- PaymentMethod (付款方式) SystemCode 資料初始化
-- 支援PaymentRecord強化功能的付款方式分類

-- 新增 PaymentMethod CodeType (如果不存在)
IF NOT EXISTS (SELECT 1 FROM CodeType WHERE CodeType = 'PaymentMethod')
BEGIN
    INSERT INTO CodeType (CodeType, Name, Description, CreateUser, CreateTime, ModifyUser, ModifyTime)
    VALUES ('PaymentMethod', '付款方式', '付款記錄的付款方式分類', 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE());
END

-- PaymentMethod 系統代碼資料
INSERT INTO SystemCode (CodeType, Code, Name, Description, Sort, StartDate, CreateUser, ModifyUser, CreateTime, ModifyTime)
VALUES 
-- 付款方式
('PaymentMethod', 'CASH', '現金', '現金付款', 1, GETDATE(), 'SYSTEM', 'SYSTEM', GETDATE(), GETDATE()),
('PaymentMethod', 'CARD', '刷卡', '信用卡或金融卡付款', 2, GETDATE(), 'SYSTEM', 'SYSTEM', GETDATE(), GETDATE()),
('PaymentMethod', 'TRANSFER', '轉帳', '銀行轉帳付款', 3, GETDATE(), 'SYSTEM', 'SYSTEM', GETDATE(), GETDATE()),
('PaymentMethod', 'MOBILE_PAY', '行動支付', '手機支付 (Line Pay, Apple Pay等)', 4, GETDATE(), 'SYSTEM', 'SYSTEM', GETDATE(), GETDATE()),
('PaymentMethod', 'OTHER', '其他', '其他付款方式', 99, GETDATE(), 'SYSTEM', 'SYSTEM', GETDATE(), GETDATE());

-- 新增 PaymentTransactionType CodeType (付款交易類型)
IF NOT EXISTS (SELECT 1 FROM CodeType WHERE CodeType = 'PaymentTransactionType')
BEGIN
    INSERT INTO CodeType (CodeType, Name, Description, CreateUser, CreateTime, ModifyUser, ModifyTime)
    VALUES ('PaymentTransactionType', '付款交易類型', '付款記錄的交易類型分類', 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE());
END

-- PaymentTransactionType 系統代碼資料
INSERT INTO SystemCode (CodeType, Code, Name, Description, Sort, StartDate, CreateUser, ModifyUser, CreateTime, ModifyTime)
VALUES 
-- 付款交易類型
('PaymentTransactionType', 'SUBSCRIPTION_PURCHASE', '包月購買', '購買包月服務', 1, GETDATE(), 'SYSTEM', 'SYSTEM', GETDATE(), GETDATE()),
('PaymentTransactionType', 'SERVICE_PAYMENT', '服務付款', '一般服務付款', 2, GETDATE(), 'SYSTEM', 'SYSTEM', GETDATE(), GETDATE()),
('PaymentTransactionType', 'FULL_REFUND', '全額退費', '全額退款', 3, GETDATE(), 'SYSTEM', 'SYSTEM', GETDATE(), GETDATE()),
('PaymentTransactionType', 'PARTIAL_REFUND', '部分退費', '部分退款', 4, GETDATE(), 'SYSTEM', 'SYSTEM', GETDATE(), GETDATE()),
('PaymentTransactionType', 'CANCELLATION_REFUND', '取消退費', '取消服務退款', 5, GETDATE(), 'SYSTEM', 'SYSTEM', GETDATE(), GETDATE()),
('PaymentTransactionType', 'ADJUSTMENT', '調整', '價格調整', 6, GETDATE(), 'SYSTEM', 'SYSTEM', GETDATE(), GETDATE());

-- 新增 RefundType CodeType (退費類型)
IF NOT EXISTS (SELECT 1 FROM CodeType WHERE CodeType = 'RefundType')
BEGIN
    INSERT INTO CodeType (CodeType, Name, Description, CreateUser, CreateTime, ModifyUser, ModifyTime)
    VALUES ('RefundType', '退費類型', '退費記錄的退費類型分類', 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE());
END

-- RefundType 系統代碼資料
INSERT INTO SystemCode (CodeType, Code, Name, Description, Sort, StartDate, CreateUser, ModifyUser, CreateTime, ModifyTime)
VALUES 
-- 退費類型
('RefundType', 'FULL_REFUND', '全額退費', '100% 金額退還', 1, GETDATE(), 'SYSTEM', 'SYSTEM', GETDATE(), GETDATE()),
('RefundType', 'PARTIAL_REFUND', '部分退費', '部分金額退還', 2, GETDATE(), 'SYSTEM', 'SYSTEM', GETDATE(), GETDATE()),
('RefundType', 'CANCELLATION_REFUND', '取消退費', '因取消服務而退費', 3, GETDATE(), 'SYSTEM', 'SYSTEM', GETDATE(), GETDATE());

PRINT 'PaymentMethod、PaymentTransactionType、RefundType SystemCode 資料初始化完成';