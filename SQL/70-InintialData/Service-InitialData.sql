-- Service Initial Data
-- 寵物美容基礎服務項目初始化數據

INSERT INTO Service (ServiceName, ServiceType, BasePrice, Duration, Description, IsActive, Sort, CreateUser, CreateTime, ModifyUser, ModifyTime)
VALUES
-- 基礎服務項目
('基礎洗澡', 'BATH', 300.00, 60, '基礎清潔洗澡服務，包含洗毛、吹乾等基本護理', 1, 1, 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE()),
('精緻美容', 'GROOM', 800.00, 120, '完整美容護理服務，包含洗澡、造型修剪、指甲護理等', 1, 2, 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE()),
('指甲修剪', 'NAIL', 100.00, 15, '專業指甲修剪服務，維護寵物腳部健康', 1, 3, 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE()),
('耳朵清潔', 'EAR', 80.00, 10, '專業耳朵清潔護理，預防耳部感染問題', 1, 4, 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE()),

-- 附加服務項目 (原 ServiceAddon 資料)
('造型加價', 'STYLING', 200.00, 0, '專業造型設計服務，提升寵物外觀美感', 1, 5, 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE()),
('貴賓腳', 'STYLING', 100.00, 0, '貴賓犬專用腳部造型修剪服務', 1, 6, 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE()),
('除蚤處理', 'TREATMENT', 150.00, 0, '專業除蚤清潔護理，確保寵物健康', 1, 7, 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE()),
('SPA護理', 'TREATMENT', 300.00, 0, '深層清潔與舒緩護理，讓寵物放鬆享受', 1, 8, 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE()),
('指甲彩繪', 'BEAUTY', 80.00, 0, '時尚指甲彩繪裝飾，增添寵物魅力', 1, 9, 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE()),
('香水', 'BEAUTY', 50.00, 0, '寵物專用香水噴灑，保持清香怡人', 1, 10, 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE());



SELECT * from [Service]