INSERT INTO CodeType (CodeType, Name, Description, CreateUser, CreateTime, ModifyUser)
VALUES
('AddonType', '附加服務類型', '寵物美容附加服務的類型分類','SYSTEM', GETDATE(),  'SYSTEM');

-- Addon Types for Pet Grooming Services
INSERT INTO SystemCode (CodeType, Code, Name, Description, Sort, StartDate, CreateUser, ModifyUser)
VALUES
-- Styling Services
('AddonType', 'STYLING', '造型服務', '美容造型相關的附加服務', 1, GETDATE(), 'SYSTEM', 'SYSTEM'),

-- Treatment Services  
('AddonType', 'TREATMENT', '護理服務', '健康護理相關的附加服務', 2, GETDATE(), 'SYSTEM', 'SYSTEM'),

-- Beauty Services
('AddonType', 'BEAUTY', '美容服務', '美容裝飾相關的附加服務', 3, GETDATE(), 'SYSTEM', 'SYSTEM'),

-- Health Services
('AddonType', 'HEALTH', '健康服務', '健康保健相關的附加服務', 4, GETDATE(), 'SYSTEM', 'SYSTEM');