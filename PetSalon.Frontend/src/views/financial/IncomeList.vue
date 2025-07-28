<template>
  <div class="income-list">
    <Card>
      <template #header>
        <div class="header">
          <h2> 財務管理</h2>
          <Button 
            label="新增收入記錄" 
            icon="pi pi-plus"
            @click="handleCreate"
          />
        </div>
      </template>
      <template #content>
        <div class="coming-soon">
          <i class="pi pi-exclamation-triangle" style="font-size: 3rem; color: var(--p-orange-500);"></i>
          <h3>功能開發中</h3>
          <p>財務管理功能正在開發中，敬請期待！</p>
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
    detail: '財務管理功能正在開發中',
    life: 3000
  })
}
</script>

<style scoped>
.income-list {
  padding: 20px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header h2 {
  margin: 0;
  color: var(--p-text-color);
}

.coming-soon {
  text-align: center;
  padding: 60px 20px;
  color: var(--p-text-muted-color);
}

.coming-soon h3 {
  margin: 16px 0;
  color: var(--p-text-color);
}

.coming-soon p {
  margin: 8px 0;
}
</style>
