INSERT INTO CodeType (CodeType, Name, Description, CreateUser, CreateTime, ModifyUser)
VALUES 
('Relationship', '聯絡人關係類型', '寵物聯絡人與寵物的關係分類','SYSTEM', GETDATE(),  'SYSTEM');

-- Contact Person Relationship Types
INSERT INTO SystemCode (CodeType, Code, Name, Description, Sort, StartDate, CreateUser, ModifyUser)
VALUES 
('Relationship', 'OWNER', '飼主', '寵物主要飼主', 1, GETDATE(), 'SYSTEM', 'SYSTEM'),
('Relationship', 'FATHER', '爸爸', '寵物爸爸', 2, GETDATE(), 'SYSTEM', 'SYSTEM'),
('Relationship', 'MOTHER', '媽媽', '寵物媽媽', 3, GETDATE(), 'SYSTEM', 'SYSTEM'),
('Relationship', 'BROTHER', '哥哥', '寵物哥哥', 4, GETDATE(), 'SYSTEM', 'SYSTEM'),
('Relationship', 'SISTER', '姐姐', '寵物姐姐', 5, GETDATE(), 'SYSTEM', 'SYSTEM'),
('Relationship', 'FAMILY', '家人', '其他家庭成員', 6, GETDATE(), 'SYSTEM', 'SYSTEM'),
('Relationship', 'FRIEND', '朋友', '朋友代為照顧', 7, GETDATE(), 'SYSTEM', 'SYSTEM'),
('Relationship', 'CAREGIVER', '照顧者', '專業照顧者', 8, GETDATE(), 'SYSTEM', 'SYSTEM');