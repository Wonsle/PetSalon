-- 新增訂閱服務類型到 SystemCode
-- 此腳本新增 SUBSCRIPTION 服務類型，用於包月訂閱服務

INSERT INTO SystemCode (CodeType, Code, Name, Description, Sort, StartDate, CreateUser, ModifyUser)
VALUES
-- Subscription Services
('ServiceType', 'SUBSCRIPTION', '包月訂閱', '包月訂閱服務方案', 20, GETDATE(), 'SYSTEM', 'SYSTEM');
