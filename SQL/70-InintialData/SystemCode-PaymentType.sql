-- Payment Types
INSERT INTO SystemCode (CodeType, Code, Name, Description, Sort, StartDate, CreateUser, ModifyUser)
VALUES 
-- Income Types
('IncomeType', 'GROOMING', '美容收入', '美容服務收入', 1, GETDATE(), 'SYSTEM', 'SYSTEM'),
('IncomeType', 'RETAIL', '零售收入', '商品零售收入', 2, GETDATE(), 'SYSTEM', 'SYSTEM'),
('IncomeType', 'ADDON', '加購收入', '額外服務收入', 3, GETDATE(), 'SYSTEM', 'SYSTEM'),
('IncomeType', 'SUBSCRIPTION', '包月收入', '包月套餐收入', 4, GETDATE(), 'SYSTEM', 'SYSTEM'),

-- Expense Types  
('ExpenseType', 'UTILITIES', '水電費', '水電費支出', 1, GETDATE(), 'SYSTEM', 'SYSTEM'),
('ExpenseType', 'PHONE', '電話費', '電話通訊費', 2, GETDATE(), 'SYSTEM', 'SYSTEM'),
('ExpenseType', 'RENT', '租金', '店面租金', 3, GETDATE(), 'SYSTEM', 'SYSTEM'),
('ExpenseType', 'SUPPLIES', '耗材', '美容用品耗材', 4, GETDATE(), 'SYSTEM', 'SYSTEM'),
('ExpenseType', 'EQUIPMENT', '設備', '設備購買維修', 5, GETDATE(), 'SYSTEM', 'SYSTEM'),
('ExpenseType', 'MARKETING', '行銷', '廣告行銷費用', 6, GETDATE(), 'SYSTEM', 'SYSTEM'),
('ExpenseType', 'OTHER', '其他', '其他雜項支出', 99, GETDATE(), 'SYSTEM', 'SYSTEM');