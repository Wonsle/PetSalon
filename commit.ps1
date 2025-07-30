# Git commit script
Write-Host "正在準備提交變更..."

# Stage all changes
git add .

# Check status
Write-Host "檢查變更狀態:"
git status --short

# Commit with detailed message
$commitMessage = @"
feat: 重構包月管理系統，移除Status欄位並改善前端組件

主要變更：
- 移除 Subscription 表的 Status 欄位，改由日期計算狀態
- 新增 PetSelector 通用組件和 usePetSelector composable
- 重構 ReservationForm 和 SubscriptionForm 使用新的 PetSelector
- 更新 SubscriptionManager、SubscriptionList 移除狀態相關邏輯
- 修正 Entity Models 和 DTOs 以匹配新的資料庫結構
- 更新 SubscriptionController 和 SubscriptionService 移除狀態管理
- 新增資料庫遷移腳本以移除 Status 相關約束
- 改善程式碼產生器註冊和循環參考處理

技術細節：
- 前端組件現在根據 endDate 計算是否過期
- 後端服務不再依賴 Status 欄位進行邏輯判斷
- 新增 SubscriptionTypeID 欄位以支援導航屬性
- 所有相關的索引、約束和說明都已更新
"@

Write-Host "執行提交:"
git commit -m $commitMessage

Write-Host "提交完成！"
