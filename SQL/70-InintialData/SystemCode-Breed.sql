DECLARE @CodeType VARCHAR(100) = 'Breed'
DELETE FROM SystemCode WHERE CodeType = @CodeType

INSERT INTO SystemCode VALUES(@CodeType, '001', '貴賓', '2023-01-01', NULL, 1, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '002', '馬爾濟斯', '2023-01-01', NULL, 2, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '003', '柯基', '2023-01-01', NULL, 3, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '004',  '法鬥', '2023-01-01', NULL, 4, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '005',  '吉娃娃', '2023-01-01', NULL, 5, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '006',  '臘腸', '2023-01-01', NULL, 6, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '007',  '博美', '2023-01-01', NULL, 7, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '008',  '狐狸', '2023-01-01', NULL, 8, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '009',  '比熊', '2023-01-01', NULL, 8, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '010',  'Mix', '2023-01-01', NULL, 9, '米克斯', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');