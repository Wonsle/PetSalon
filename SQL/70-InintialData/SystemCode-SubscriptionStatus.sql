INSERT INTO CodeType (CodeType, Name, Description, CreateUser, CreateTime, ModifyUser)
VALUES 
('SubscriptionStatus', '包月方案狀態', '寵物美容服務的包月方案狀態分類','SYSTEM', GETDATE(),  'SYSTEM');

-- Subscription Status Types
INSERT INTO SystemCode (CodeType, Code, Name, Description, Sort, StartDate, CreateUser, ModifyUser)
VALUES 
('SubscriptionStatus', 'ACTIVE', '啟用', '包月方案啟用中', 1, GETDATE(), 'SYSTEM', 'SYSTEM'),
('SubscriptionStatus', 'INACTIVE', '停用', '包月方案暫停', 2, GETDATE(), 'SYSTEM', 'SYSTEM'),
('SubscriptionStatus', 'EXPIRED', '過期', '包月方案已過期', 3, GETDATE(), 'SYSTEM', 'SYSTEM'),
('SubscriptionStatus', 'CANCELLED', '取消', '包月方案已取消', 4, GETDATE(), 'SYSTEM', 'SYSTEM');