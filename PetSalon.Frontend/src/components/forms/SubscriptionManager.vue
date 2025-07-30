<template>
  <div class="subscription-manager">
    <div class="header">
      <h3>包月方案管理</h3>
      <Button
        label="新增包月方案"
        icon="pi pi-plus"
        size="small"
        @click="showAddDialog = true"
      />
    </div>

    <div v-if="loading" class="loading-section">
      <ProgressSpinner />
    </div>

    <div v-else-if="subscriptions.length === 0" class="empty-section">
      <div class="empty-content">
        <i class="pi pi-calendar-times" style="font-size: 2rem; color: var(--p-text-color-secondary);"></i>
        <p>尚無包月方案</p>
        <Button
          label="新增第一個方案"
          icon="pi pi-plus"
          @click="showAddDialog = true"
        />
      </div>
    </div>

    <div v-else class="subscription-list">
      <Card
        v-for="subscription in subscriptions"
        :key="subscription.subscriptionId"
        class="subscription-card"
      >
        <template #content>
          <div class="subscription-info">
            <div class="subscription-header">
              <h4 class="subscription-name">包月方案</h4>
              <Tag
                :value="new Date(subscription.endDate) > new Date() ? '有效' : '已過期'"
                :severity="new Date(subscription.endDate) > new Date() ? 'success' : 'danger'"
              />
            </div>

            <div class="subscription-details">
              <div class="detail-row">
                <span class="label">服務內容:</span>
                <span class="value">{{ subscription.serviceContent || '基礎服務' }}</span>
              </div>

              <div class="detail-row">
                <span class="label">使用次數:</span>
                <div class="usage-info">
                  <span class="usage-text">
                    {{ subscription.usedCount || 0 }} / {{ subscription.totalTimes || subscription.totalUsageLimit || '∞' }}
                  </span>
                  <ProgressBar
                    :value="getUsagePercentage(subscription)"
                    :show-value="false"
                    class="usage-bar"
                  />
                </div>
              </div>

              <div class="detail-row">
                <span class="label">方案金額:</span>
                <span class="value amount">NT$ {{ (subscription.totalAmount || subscription.subscriptionPrice || 0).toLocaleString() }}</span>
              </div>

              <div class="detail-row">
                <span class="label">使用期間:</span>
                <span class="value">
                  {{ formatDate(subscription.startDate) }} ~ {{ formatDate(subscription.endDate) }}
                </span>
              </div>

              <div v-if="subscription.notes" class="detail-row">
                <span class="label">備註:</span>
                <span class="value">{{ subscription.notes }}</span>
              </div>
            </div>

            <div class="subscription-actions">
              <Button
                icon="pi pi-pencil"
                size="small"
                severity="warning"
                text
                @click="editSubscription(subscription)"
                v-tooltip="'編輯'"
              />
              <Button
                icon="pi pi-trash"
                size="small"
                severity="danger"
                text
                @click="deleteSubscription(subscription)"
                v-tooltip="'刪除'"
              />
            </div>
          </div>
        </template>
      </Card>
    </div>

    <!-- 新增/編輯包月方案對話框 -->
    <Dialog
      v-model:visible="showAddDialog"
      :header="editingSubscription ? '編輯包月方案' : '新增包月方案'"
      :style="{ width: '600px' }"
      :modal="true"
    >
      <form @submit.prevent="handleSubmit">
        <!-- 開始日期和結束日期 -->
        <div class="grid">
          <div class="col-6">
            <div class="field">
              <label for="startDate" class="label">開始日期 *</label>
              <Calendar
                id="startDate"
                v-model="startDateModel"
                date-format="yy/mm/dd"
                placeholder="請選擇開始日期"
                :class="{ 'p-invalid': errors.startDate }"
              />
              <small v-if="errors.startDate" class="p-error">{{ errors.startDate }}</small>
            </div>
          </div>
          <div class="col-6">
            <div class="field">
              <label for="endDate" class="label">結束日期 *</label>
              <Calendar
                id="endDate"
                v-model="endDateModel"
                date-format="yy/mm/dd"
                placeholder="請選擇結束日期"
                :min-date="startDateModel || undefined"
                :class="{ 'p-invalid': errors.endDate }"
              />
              <small v-if="errors.endDate" class="p-error">{{ errors.endDate }}</small>
            </div>
          </div>
        </div>

        <!-- 使用次數限制和方案價格 -->
        <div class="grid">
          <div class="col-6">
            <div class="field">
              <label for="totalUsageLimit" class="label">使用次數限制 *</label>
              <InputNumber
                id="totalUsageLimit"
                v-model="form.totalUsageLimit"
                :min="0"
                :max="999"
                show-buttons
                :class="{ 'p-invalid': errors.totalUsageLimit }"
              />
              <div class="form-tip">設為 0 表示期間內不限使用次數</div>
              <small v-if="errors.totalUsageLimit" class="p-error">{{ errors.totalUsageLimit }}</small>
            </div>
          </div>
          <div class="col-6">
            <div class="field">
              <label for="subscriptionPrice" class="label">方案價格 *</label>
              <InputNumber
                id="subscriptionPrice"
                v-model="form.subscriptionPrice"
                :min="0"
                mode="currency"
                currency="TWD"
                locale="zh-TW"
                :class="{ 'p-invalid': errors.subscriptionPrice }"
              />
              <small v-if="errors.subscriptionPrice" class="p-error">{{ errors.subscriptionPrice }}</small>
            </div>
          </div>
        </div>


        <!-- 備註 -->
        <div class="field">
          <label for="notes" class="label">備註</label>
          <Textarea
            id="notes"
            v-model="form.notes"
            :rows="3"
            placeholder="請輸入備註說明"
          />
        </div>
      </form>

      <template #footer>
        <div class="dialog-footer">
          <Button label="取消" severity="secondary" @click="closeDialog" />
          <Button
            :label="editingSubscription ? '更新' : '新增'"
            :loading="submitting"
            @click="handleSubmit"
          />
        </div>
      </template>
    </Dialog>

    <!-- 刪除確認對話框 -->
    <ConfirmDialog />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, nextTick, computed } from 'vue'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import { subscriptionApi } from '@/api/subscription'
import type { Subscription, SubscriptionCreateRequest, SubscriptionUpdateRequest } from '@/types/subscription'
import dayjs from 'dayjs'

interface Props {
  petId: number
}

const props = defineProps<Props>()

// Composables
const toast = useToast()
const confirm = useConfirm()

// State
const loading = ref(false)
const submitting = ref(false)
const subscriptions = ref<Subscription[]>([])
const showAddDialog = ref(false)
const editingSubscription = ref<Subscription | null>(null)

// Date models
const startDateModel = ref<Date | null>(null)
const endDateModel = ref<Date | null>(null)

// Form data
const form = reactive<SubscriptionCreateRequest>({
  name: '基礎包月方案',
  petId: props.petId,
  serviceContent: '基礎服務',
  totalTimes: 5,
  totalAmount: 0,
  paidAmount: 0,
  startDate: '',
  endDate: '',
  subscriptionDate: '',
  totalUsageLimit: 5,
  subscriptionPrice: 0,
  notes: ''
})

// Form errors
const errors = reactive({
  startDate: '',
  endDate: '',
  totalUsageLimit: '',
  subscriptionPrice: ''
})


// Computed
const getUsagePercentage = (subscription: Subscription) => {
  const used = subscription.usedCount || 0
  const total = subscription.totalTimes || subscription.totalUsageLimit || 0
  if (total === 0) return 0
  return Math.min((used / total) * 100, 100)
}

// Methods
const loadSubscriptions = async () => {
  loading.value = true
  try {
    subscriptions.value = await subscriptionApi.getSubscriptionsByPet(props.petId)
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: '載入失敗',
      detail: error.response?.data?.message || '載入包月方案失敗',
      life: 3000
    })
  } finally {
    loading.value = false
  }
}

const formatDate = (dateStr: string) => {
  if (!dateStr) return ''
  return dayjs(dateStr).format('YYYY/MM/DD')
}


const editSubscription = (subscription: Subscription) => {
  editingSubscription.value = subscription
  Object.assign(form, {
    petId: props.petId,
    startDate: subscription.startDate,
    endDate: subscription.endDate,
    subscriptionDate: subscription.subscriptionDate,
    totalUsageLimit: subscription.totalUsageLimit,
    subscriptionPrice: subscription.subscriptionPrice,
    notes: subscription.notes || ''
  })

  startDateModel.value = new Date(subscription.startDate)
  endDateModel.value = new Date(subscription.endDate)
  showAddDialog.value = true
}

const deleteSubscription = (subscription: Subscription) => {
  confirm.require({
    message: `確定要刪除此包月方案嗎？此操作無法復原。`,
    header: '確認刪除',
    icon: 'pi pi-exclamation-triangle',
    rejectClass: 'p-button-secondary p-button-outlined',
    rejectLabel: '取消',
    acceptLabel: '刪除',
    accept: async () => {
      try {
        await subscriptionApi.deleteSubscription(subscription.subscriptionId)
        toast.add({
          severity: 'success',
          summary: '刪除成功',
          detail: '包月方案已成功刪除',
          life: 3000
        })
        await loadSubscriptions()
      } catch (error: any) {
        toast.add({
          severity: 'error',
          summary: '刪除失敗',
          detail: error.response?.data?.message || '刪除包月方案失敗',
          life: 3000
        })
      }
    }
  })
}

const validateForm = () => {
  let isValid = true

  // Reset errors
  Object.keys(errors).forEach(key => {
    errors[key as keyof typeof errors] = ''
  })

  if (!startDateModel.value) {
    errors.startDate = '請選擇開始日期'
    isValid = false
  }

  if (!endDateModel.value) {
    errors.endDate = '請選擇結束日期'
    isValid = false
  }

  if (form.totalUsageLimit !== undefined && form.totalUsageLimit < 0) {
    errors.totalUsageLimit = '使用次數限制不能為負數'
    isValid = false
  }

  if (!form.subscriptionPrice || form.subscriptionPrice <= 0) {
    errors.subscriptionPrice = '請輸入有效的方案價格'
    isValid = false
  }

  return isValid
}

const handleSubmit = async () => {
  if (!validateForm()) return

  // Sync date models to form
  if (startDateModel.value) {
    form.startDate = startDateModel.value.toISOString().split('T')[0]
  }
  if (endDateModel.value) {
    form.endDate = endDateModel.value.toISOString().split('T')[0]
  }
  form.subscriptionDate = new Date().toISOString().split('T')[0]

  submitting.value = true
  try {
    if (editingSubscription.value) {
      const updateData: SubscriptionUpdateRequest = {
        id: editingSubscription.value.subscriptionId,
        subscriptionId: editingSubscription.value.subscriptionId,
        startDate: form.startDate,
        endDate: form.endDate,
        totalUsageLimit: form.totalUsageLimit,
        subscriptionPrice: form.subscriptionPrice,
        notes: form.notes
      }
      await subscriptionApi.updateSubscription(updateData)
      toast.add({
        severity: 'success',
        summary: '更新成功',
        detail: '包月方案已成功更新',
        life: 3000
      })
    } else {
      await subscriptionApi.createSubscription(form)
      toast.add({
        severity: 'success',
        summary: '新增成功',
        detail: '包月方案已成功建立',
        life: 3000
      })
    }

    closeDialog()
    await loadSubscriptions()
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: '操作失敗',
      detail: error.response?.data?.message || '操作失敗',
      life: 3000
    })
  } finally {
    submitting.value = false
  }
}

const closeDialog = () => {
  showAddDialog.value = false
  editingSubscription.value = null

  // Reset form
  Object.assign(form, {
    name: '基礎包月方案',
    petId: props.petId,
    serviceContent: '基礎服務',
    totalTimes: 5,
    totalAmount: 0,
    paidAmount: 0,
    startDate: '',
    endDate: '',
    subscriptionDate: '',
    totalUsageLimit: 5,
    subscriptionPrice: 0,
    notes: ''
  })

  startDateModel.value = null
  endDateModel.value = null

  // Reset errors
  Object.keys(errors).forEach(key => {
    errors[key as keyof typeof errors] = ''
  })
}

// Lifecycle
onMounted(() => {
  loadSubscriptions()
})
</script>

<style scoped>
.subscription-manager {
  margin: 1rem 0;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.header h3 {
  margin: 0;
  color: var(--p-text-color);
}

.loading-section {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 200px;
}

.empty-section {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 200px;
}

.empty-content {
  text-align: center;
  color: var(--p-text-color-secondary);
}

.empty-content p {
  margin: 1rem 0;
}

.subscription-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.subscription-card {
  border: 1px solid var(--p-surface-border);
}

.subscription-info {
  position: relative;
}

.subscription-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.subscription-name {
  margin: 0;
  font-size: 1.1rem;
  color: var(--p-text-color);
}

.subscription-details {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.detail-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.label {
  font-weight: 500;
  color: var(--p-text-color-secondary);
  min-width: 80px;
}

.value {
  color: var(--p-text-color);
}

.value.amount {
  font-weight: 600;
  color: var(--p-primary-color);
}

.usage-info {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 0.25rem;
}

.usage-text {
  font-size: 0.9rem;
  color: var(--p-text-color);
}

.usage-bar {
  width: 100px;
  height: 4px;
}

.subscription-actions {
  position: absolute;
  top: 0;
  right: 0;
  display: flex;
  gap: 0.5rem;
}

.field {
  margin-bottom: 1rem;
}

.label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: var(--p-text-color);
}

.grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.col-6 {
  /* Grid item styling handled by parent grid */
}

.form-tip {
  font-size: 0.875rem;
  color: var(--p-text-color-secondary);
  margin-top: 0.25rem;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
}

.p-invalid {
  border-color: var(--p-red-500);
}

.p-error {
  color: var(--p-red-500);
  font-size: 0.875rem;
}
</style>