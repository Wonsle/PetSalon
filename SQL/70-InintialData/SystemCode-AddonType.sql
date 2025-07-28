-- Service Addon Types
INSERT INTO SystemCode (CodeType, Code, Name, Description, Sort, StartDate, CreateUser, ModifyUser)
VALUES 
('AddonType', 'STYLING', '造型加價', '特殊造型設計加價', 1, GETDATE(), 'SYSTEM', 'SYSTEM'),
('AddonType', 'POODLE_FOOT', '貴賓腳', '貴賓犬腳部造型', 2, GETDATE(), 'SYSTEM', 'SYSTEM'),
('AddonType', 'FLEA_TREATMENT', '除蚤處理', '除蚤藥浴處理', 3, GETDATE(), 'SYSTEM', 'SYSTEM'),
('AddonType', 'NAIL_PAINTING', '指甲彩繪', '寵物指甲彩繪', 4, GETDATE(), 'SYSTEM', 'SYSTEM'),
('AddonType', 'PERFUME', '香水', '寵物香水服務', 5, GETDATE(), 'SYSTEM', 'SYSTEM'),
('AddonType', 'TEETH_CLEANING', '潔牙', '牙齒清潔服務', 6, GETDATE(), 'SYSTEM', 'SYSTEM'),
('AddonType', 'SPA', 'SPA護理', '深層護理SPA', 7, GETDATE(), 'SYSTEM', 'SYSTEM'),
('AddonType', 'EMERGENCY', '緊急處理', '緊急處理費用', 10, GETDATE(), 'SYSTEM', 'SYSTEM');