INSERT INTO SystemCode (CodeType, Code, Name, Description, CreateUser, CreateTime, ModifyUser)
VALUES 
('ReservationStatus', 'ReservationStatus', '預約狀態', '寵物美容服務的預約狀態分類','SYSTEM', GETDATE(),  'SYSTEM');

-- Reservation Status Types
INSERT INTO SystemCode (CodeType, Code, Name, Description, Sort, StartDate, CreateUser, ModifyUser)
VALUES 
('ReservationStatus', 'PENDING', '待確認', '預約待確認狀態', 1, GETDATE(), 'SYSTEM', 'SYSTEM'),
('ReservationStatus', 'CONFIRMED', '已確認', '預約已確認狀態', 2, GETDATE(), 'SYSTEM', 'SYSTEM'),
('ReservationStatus', 'IN_PROGRESS', '進行中', '服務進行中', 3, GETDATE(), 'SYSTEM', 'SYSTEM'),
('ReservationStatus', 'COMPLETED', '已完成', '服務已完成', 4, GETDATE(), 'SYSTEM', 'SYSTEM'),
('ReservationStatus', 'CANCELLED', '已取消', '預約已取消', 5, GETDATE(), 'SYSTEM', 'SYSTEM'),
('ReservationStatus', 'NO_SHOW', '未出現', '客戶未出現', 6, GETDATE(), 'SYSTEM', 'SYSTEM');