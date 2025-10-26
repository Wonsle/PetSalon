-- Service Initial Data
-- 寵物美容基礎服務項目初始化數據
DELETE FROM [Service]
INSERT INTO [Service] (ServiceName, ServiceType, BasePrice, Duration, Description, IsActive, Sort, CreateUser, CreateTime, ModifyUser, ModifyTime)
VALUES
-- 基礎服務項目
('基礎洗澡', 'BATH', 0.00, 60, '基礎清潔洗澡服務，包含洗毛、吹乾等基本護理', 1, 1, 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE()),
('精緻美容', 'GROOM', 0.00, 120, '完整美容護理服務，包含洗澡、造型修剪、指甲護理等', 1, 2, 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE()),
('腳柱', 'FOOTPOSTS', 200.00, 20, '腳柱', 1, 3, 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE()),
('指甲修剪', 'NAIL', 50.00, 15, '專業指甲修剪服務，維護寵物腳部健康', 1, 4, 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE()),
('耳朵清潔', 'EAR', 50.00, 10, '專業耳朵清潔護理，預防耳部感染問題', 1, 5, 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE()),
('刷牙', 'BRUSHTEETH', 50.00, 10, '專業耳朵清潔護理，預防耳部感染問題', 1, 6, 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE())




