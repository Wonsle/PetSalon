<template>
  <div class="subscription-edit">
    <Card>
      <template #header>
        <div class="header">
          <h2>✏️ 編輯包月方案</h2>
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

        <div v-if="loading" class="loading-section">
          <div class="loading-content">
            <ProgressSpinner />
            <p>載入包月方案資料中...</p>
          </div>
        </div>

        <div v-else-if="subscription" class="form-section">
          <SubscriptionForm
            :visible="true"
            :subscription="subscription"
            @close="$router.push('/subscriptions')"
            @success="handleSuccess"
          />
        </div>

        <div v-else class="error-section">
          <Message severity="error" :closable="false">
            <i class="pi pi-exclamation-triangle"></i>
            找不到指定的包月方案，可能已被刪除或不存在。
          </Message>
          <div class="error-actions">
            <Button
              label="返回列表"
              icon="pi pi-arrow-left"
              @click="$router.push('/subscriptions')"
            />
          </div>
        </div>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import type { Subscription } from '@/types/subscription'
import { subscriptionApi } from '@/api/subscription'
import SubscriptionForm from '@/components/forms/SubscriptionForm.vue'

const route = useRoute()
const router = useRouter()
const toast = useToast()

// State
const loading = ref(true)
const subscription = ref<Subscription | null>(null)
const subscriptionId = ref<number>(0)

// 麵包屑導航
const breadcrumbItems = ref([
  { label: '首頁', to: '/' },
  { label: '包月管理', to: '/subscriptions' },
  { label: '編輯包月方案' }
])

// Methods
const loadSubscription = async () => {
  loading.value = true
  try {
    const id = Number(route.params.id)
    if (!id || isNaN(id)) {
      throw new Error('無效的包月方案ID')
    }

    subscriptionId.value = id
    subscription.value = await subscriptionApi.getSubscription(id)

    // 更新麵包屑標題
    if (subscription.value?.name) {
      breadcrumbItems.value[2].label = `編輯 - ${subscription.value.name}`
    }
  } catch (error: any) {
    console.error('載入包月方案失敗:', error)
    toast.add({
      severity: 'error',
      summary: '載入失敗',
      detail: error.response?.data?.message || '載入包月方案資料失敗',
      life: 3000
    })
    subscription.value = null
  } finally {
    loading.value = false
  }
}

const handleSuccess = () => {
  toast.add({
    severity: 'success',
    summary: '成功',
    detail: '包月方案已成功更新',
    life: 3000
  })
  router.push('/subscriptions')
}

// Lifecycle
onMounted(() => {
  loadSubscription()
})
</script>

<style scoped>
.subscription-edit {
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

.loading-section {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 300px;
}

.loading-content {
  text-align: center;
  color: var(--p-text-color-secondary);
}

.loading-content p {
  margin-top: 1rem;
  font-size: 1rem;
}

.form-section {
  margin-top: 1rem;
}

.error-section {
  margin-top: 1rem;
  text-align: center;
}

.error-actions {
  margin-top: 1.5rem;
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