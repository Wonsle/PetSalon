DECLARE @CodeType VARCHAR(100) = 'CoatColor'
insert into CodeType (CodeType, Name, Description, CreateUser, CreateTime, ModifyUser)
VALUES 
(@CodeType, '毛色', '寵物的毛色分類','System', GETDATE(),  'System');


DELETE FROM SystemCode WHERE CodeType = @CodeType

-- 毛色按常見度排序，貴賓狗40%、馬爾濟斯30%、其他30%分布考量
INSERT INTO SystemCode VALUES(@CodeType, '001', '白色', '2023-01-01', NULL, 1, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '002', '奶油色', '2023-01-01', NULL, 2, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '003', '杏色', '2023-01-01', NULL, 3, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '004', '香檳色', '2023-01-01', NULL, 4, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '005', '黑色', '2023-01-01', NULL, 5, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '006', '巧克力色', '2023-01-01', NULL, 6, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '007', '棕色', '2023-01-01', NULL, 7, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '008', '紅棕色', '2023-01-01', NULL, 8, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '009', '灰色', '2023-01-01', NULL, 9, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '010', '銀色', '2023-01-01', NULL, 10, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '011', '金色', '2023-01-01', NULL, 11, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '012', '小麥色', '2023-01-01', NULL, 12, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '013', '咖啡色', '2023-01-01', NULL, 13, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '014', '黑白', '2023-01-01', NULL, 14, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '015', '棕白', '2023-01-01', NULL, 15, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '016', '黑棕', '2023-01-01', NULL, 16, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '017', '三色', '2023-01-01', NULL, 17, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '018', '藍色', '2023-01-01', NULL, 18, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '019', '肉桂色', '2023-01-01', NULL, 19, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');
INSERT INTO SystemCode VALUES(@CodeType, '020', '斑點', '2023-01-01', NULL, 20, '', 'System', '2023-01-01 00:00:00', 'System', '2023-01-01 00:00:00');