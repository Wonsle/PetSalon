INSERT INTO CodeType (CodeType, Name, Description, CreateUser, CreateTime, ModifyUser)
VALUES
('ServiceType', '寵物美容服務類型', '寵物美容服務的類型分類','SYSTEM', GETDATE(),  'SYSTEM');

-- Service Types for Pet Grooming
INSERT INTO SystemCode (CodeType, Code, Name, Description, Sort, StartDate, CreateUser, ModifyUser)
VALUES
-- Basic Services
('ServiceType', 'BATH', '洗澡', '基礎洗澡服務', 1, GETDATE(), 'SYSTEM', 'SYSTEM'),
('ServiceType', 'GROOM', '美容', '完整美容服務', 2, GETDATE(), 'SYSTEM', 'SYSTEM'),
('ServiceType', 'NAIL', '剪指甲', '指甲修剪服務', 3, GETDATE(), 'SYSTEM', 'SYSTEM'),
('ServiceType', 'EAR', '清耳朵', '耳朵清潔服務', 4, GETDATE(), 'SYSTEM', 'SYSTEM'),

-- Special Services
('ServiceType', 'SPECIAL_STYLE', '特殊造型', '特殊造型設計', 10, GETDATE(), 'SYSTEM', 'SYSTEM'),
('ServiceType', 'POODLE_FOOT', '貴賓腳', '貴賓犬腳部造型', 11, GETDATE(), 'SYSTEM', 'SYSTEM');
