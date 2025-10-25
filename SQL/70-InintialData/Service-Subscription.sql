-- 新增訂閱服務項目到 Service 表
-- 此腳本新增包月訂閱服務，作為預設的訂閱價格來源

INSERT INTO [Service] (ServiceName, ServiceType, BasePrice, Duration, Description, IsActive, Sort, CreateUser, CreateTime, ModifyUser, ModifyTime)
VALUES
-- 包月訂閱服務（2個月為單位）
('包月訂閱方案', 'SUBSCRIPTION', 1800.00, 0, '2個月包月訂閱服務，可享受優惠價格', 1, 100, 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE());

-- 註解：
-- BasePrice: 預設包月價格為 1800 元（2個月）
-- Duration: 訂閱服務不計算時長，設為 0
-- 實際價格可透過 PetServicePrice 為每隻寵物設定客製化價格
