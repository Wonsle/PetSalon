DECLARE @CodeType VARCHAR(100) = 'Gender'
DELETE FROM SystemCode WHERE CodeType = @CodeType

INSERT INTO SystemCode VALUES(@CodeType, '1', '男', '2023-01-01', NULL, 1, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00')
INSERT INTO SystemCode VALUES(@CodeType, '2', '女', '2023-01-01', NULL, 2, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00')