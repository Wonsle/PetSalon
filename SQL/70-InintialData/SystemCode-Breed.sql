DECLARE @CodeType VARCHAR(100) = 'Breed'
DELETE FROM SystemCode WHERE CodeType = @CodeType

INSERT INTO SystemCode VALUES(@CodeType, 'PD', '貴賓狗', '2023-01-01', NULL, 1, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00')
INSERT INTO SystemCode VALUES(@CodeType, 'ML', '馬爾濟斯', '2023-01-01', NULL, 2, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00')