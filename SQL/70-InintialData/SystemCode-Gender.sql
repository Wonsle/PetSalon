DECLARE @CodeType VARCHAR(100) = 'Gender'
INSERT INTO CodeType (CodeType, Name, Description, CreateUser, CreateTime, ModifyUser)
VALUES 
(@CodeType, '性別', '寵物的性別分類','System', GETDATE(),  'System');

DELETE FROM SystemCode WHERE CodeType = @CodeType

INSERT INTO SystemCode VALUES(@CodeType, 'M', '公', '2023-01-01', NULL, 1, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00')
INSERT INTO SystemCode VALUES(@CodeType, 'F', '母', '2023-01-01', NULL, 2, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00')