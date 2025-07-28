<template>
  <div class="reservation-list">
    <Card>
      <template #header>
        <div class="header">
          <h2>📅 預約管理</h2>
          <Button
            label="新增預約"
            icon="pi pi-plus"
            @click="handleCreate"
          />
        </div>
      </template>
      <template #content>
        <div class="coming-soon">
          <i class="pi pi-exclamation-triangle" style="font-size: 3rem; color: var(--p-orange-500);"></i>
          <h3>功能開發中</h3>
          <p>預約管理功能正在從Element Plus轉換為PrimeVue中，敬請期待！</p>
          <p>目前可用功能：儀表板、寵物管理</p>
        </div>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { useToast } from 'primevue/usetoast'

const toast = useToast()

const handleCreate = () => {
  toast.add({
    severity: 'info',
    summary: '功能開發中',
    detail: '預約管理功能正在開發中',
    life: 3000
  })
}

// 功能開發中，暫不需要其他邏輯
</script>

<style scoped>
.reservation-list-container {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
  padding-bottom: 16px;
  border-bottom: 1px solid #e4e7ed;
}

.header-left h2 {
  margin: 0;
  color: #303133;
  font-size: 24px;
}

.total-count {
  color: #909399;
  font-size: 14px;
  margin-left: 12px;
}

.header-right {
  display: flex;
  gap: 12px;
}

.search-section {
  margin-bottom: 24px;
  padding: 16px;
  background: #f8f9fa;
  border-radius: 8px;
}

.quick-tabs {
  margin-bottom: 24px;
}

.table-container {
  margin-bottom: 24px;
}

.pet-info {
  display: flex;
  flex-direction: column;
}

.pet-name {
  font-weight: 500;
  color: #303133;
}

.owner-name {
  font-size: 12px;
  color: #909399;
}

.text-gray {
  color: #909399;
}

.empty-state {
  text-align: center;
  padding: 60px 20px;
}

.pagination-wrapper {
  display: flex;
  justify-content: center;
  padding: 20px 0;
}

:deep(.el-table__row) {
  cursor: pointer;
}

:deep(.el-table__row:hover) {
  background-color: #f5f7fa;
}
</style>