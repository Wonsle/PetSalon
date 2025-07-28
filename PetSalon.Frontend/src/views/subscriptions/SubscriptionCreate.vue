<template>
  <div class="subscription-create">
    <Card>
      <template #header>
        <div class="header">
          <h2>🆕 新增包月方案</h2>
          <Button
            label="返回列表"
            icon="pi pi-arrow-left"
            severity="secondary"
            @click="$router.push('/subscriptions')"
          />
        </div>
      </template>
      <template #content>
        <div class="breadcrumb-section">
          <Breadcrumb :model="breadcrumbItems" />
        </div>

        <div class="form-section">
          <SubscriptionForm
            :visible="true"
            @close="$router.push('/subscriptions')"
            @success="handleSuccess"
          />
        </div>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import SubscriptionForm from '@/components/forms/SubscriptionForm.vue'

const router = useRouter()
const toast = useToast()

// 麵包屑導航
const breadcrumbItems = ref([
  { label: '首頁', to: '/' },
  { label: '包月管理', to: '/subscriptions' },
  { label: '新增包月方案' }
])

const handleSuccess = () => {
  toast.add({
    severity: 'success',
    summary: '成功',
    detail: '包月方案已成功建立',
    life: 3000
  })
  router.push('/subscriptions')
}
</script>

<style scoped>
.subscription-create {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
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

.breadcrumb-section {
  margin-bottom: 1.5rem;
}

.form-section {
  margin-top: 1rem;
}

/* 調整表單在頁面中的顯示 */
.form-section :deep(.p-dialog) {
  position: static;
  width: 100%;
  max-width: none;
  margin: 0;
  box-shadow: none;
  border: 1px solid var(--p-surface-border);
}

.form-section :deep(.p-dialog-content) {
  padding: 1.5rem;
}

.form-section :deep(.p-dialog-header) {
  background: var(--p-surface-50);
  border-bottom: 1px solid var(--p-surface-border);
}
</style>