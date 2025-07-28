<template>
  <div class="subscription-list">
    <Card>
      <template #header>
        <div class="header">
          <h2>💳 包月管理</h2>
          <Button
            label="新增包月"
            icon="pi pi-plus"
            @click="handleCreate"
          />
        </div>
      </template>
      <template #content>
        <!-- 搜尋過濾器 -->
        <div class="filters-section">
          <div class="grid">
            <div class="col-12 md:col-3">
              <div class="field">
                <label for="petSearch" class="label">寵物搜尋</label>
                <InputText
                  id="petSearch"
                  v-model="filters.petName"
                  placeholder="搜尋寵物名稱"
                  @input="handleSearch"
                />
              </div>
            </div>
            <div class="col-12 md:col-3">
              <div class="field">
                <label for="statusFilter" class="label">狀態</label>
                <Select
                  id="statusFilter"
                  v-model="filters.status"
                  :options="statusOptions"
                  option-label="label"
                  option-value="value"
                  placeholder="全部狀態"
                  @change="handleSearch"
                />
              </div>
            </div>
            <div class="col-12 md:col-3">
              <div class="field">
                <label for="dateRange" class="label">日期範圍</label>
                <Calendar
                  id="dateRange"
                  v-model="dateRange"
                  selection-mode="range"
                  date-format="yy/mm/dd"
                  placeholder="選擇日期範圍"
                  @date-select="handleSearch"
                />
              </div>
            </div>
            <div class="col-12 md:col-3">
              <div class="field">
                <label class="label">&nbsp;</label>
                <div class="flex gap-2">
                  <Button
                    label="重置"
                    severity="secondary"
                    @click="resetFilters"
                  />
                  <Button
                    label="搜尋"
                    @click="handleSearch"
                  />
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- 資料表格 -->
        <div class="table-section">
          <DataTable
            :value="subscriptions"
            :loading="loading"
            paginator
            :rows="pageSize"
            :total-records="total"
            :lazy="true"
            @page="onPageChange"
            row-hover
            striped-rows
            responsive-layout="scroll"
            @row-click="(event) => viewSubscription(event.data)"
          >
            <Column field="petName" header="寵物名稱" :sortable="true">
              <template #body="slotProps">
                <div class="pet-info">
                  <span class="pet-name">{{ slotProps.data.petName || '未設定' }}</span>
                  <small class="pet-id">ID: {{ slotProps.data.petId }}</small>
                </div>
              </template>
            </Column>

            <Column field="name" header="方案名稱" :sortable="true">
              <template #body="slotProps">
                <span class="subscription-name">{{ slotProps.data.name || '未命名方案' }}</span>
              </template>
            </Column>

            <Column field="serviceContent" header="服務內容">
              <template #body="slotProps">
                <Tag
                  :value="slotProps.data.serviceContent || '未設定'"
                  severity="info"
                />
              </template>
            </Column>

            <Column field="totalTimes" header="服務次數" :sortable="true">
              <template #body="slotProps">
                <div class="usage-info">
                  <span class="usage-count">
                    {{ slotProps.data.usedCount || 0 }} / {{ slotProps.data.totalTimes || slotProps.data.totalUsageLimit || '∞' }}
                  </span>
                  <div class="usage-bar">
                    <ProgressBar
                      :value="getUsagePercentage(slotProps.data)"
                      :show-value="false"
                      style="height: 4px"
                    />
                  </div>
                </div>
              </template>
            </Column>

            <Column field="totalAmount" header="方案金額" :sortable="true">
              <template #body="slotProps">
                <div class="amount-info">
                  <span class="total-amount">NT$ {{ (slotProps.data.totalAmount || slotProps.data.subscriptionPrice || 0).toLocaleString() }}</span>
                  <small v-if="slotProps.data.paidAmount" class="paid-amount">
                    已付: NT$ {{ slotProps.data.paidAmount.toLocaleString() }}
                  </small>
                </div>
              </template>
            </Column>

            <Column field="startDate" header="期間" :sortable="true">
              <template #body="slotProps">
                <div class="date-range">
                  <div class="start-date">{{ formatDate(slotProps.data.startDate) }}</div>
                  <small class="to">至</small>
                  <div class="end-date">{{ formatDate(slotProps.data.endDate) }}</div>
                  <Tag
                    v-if="isExpiringSoon(slotProps.data)"
                    value="即將到期"
                    severity="warning"
                    class="mt-1"
                  />
                </div>
              </template>
            </Column>

            <Column field="status" header="狀態" :sortable="true">
              <template #body="slotProps">
                <Tag
                  :value="getStatusText(slotProps.data.status)"
                  :severity="getStatusSeverity(slotProps.data.status)"
                />
              </template>
            </Column>

            <Column header="操作" :frozen="true" align-frozen="right">
              <template #body="slotProps">
                <div class="actions">
                  <Button
                    icon="pi pi-eye"
                    severity="info"
                    text
                    rounded
                    @click="viewSubscription(slotProps.data)"
                    v-tooltip="'查看詳情'"
                  />
                  <Button
                    icon="pi pi-pencil"
                    severity="warning"
                    text
                    rounded
                    @click="editSubscription(slotProps.data)"
                    v-tooltip="'編輯'"
                  />
                  <Button
                    icon="pi pi-trash"
                    severity="danger"
                    text
                    rounded
                    @click="deleteSubscription(slotProps.data)"
                    v-tooltip="'刪除'"
                  />
                </div>
              </template>
            </Column>

            <template #empty>
              <div class="empty-state">
                <i class="pi pi-calendar-times" style="font-size: 3rem; color: var(--p-text-color-secondary);"></i>
                <h3>沒有找到包月方案</h3>
                <p>目前沒有符合條件的包月方案，您可以新增第一個方案。</p>
                <Button
                  label="新增第一個包月方案"
                  icon="pi pi-plus"
                  @click="handleCreate"
                />
              </div>
            </template>
          </DataTable>
        </div>
      </template>
    </Card>

    <!-- 新增/編輯對話框 -->
    <SubscriptionForm
      :visible="showForm"
      :subscription="selectedSubscription"
      @close="closeForm"
      @success="handleFormSuccess"
    />

    <!-- 刪除確認對話框 -->
    <ConfirmDialog />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import type { Subscription } from '@/types/subscription'
import { subscriptionApi } from '@/api/subscription'
import SubscriptionForm from '@/components/forms/SubscriptionForm.vue'
import dayjs from 'dayjs'

// Composables
const toast = useToast()
const confirm = useConfirm()

// Reactive state
const loading = ref(false)
const subscriptions = ref<Subscription[]>([])
const total = ref(0)
const currentPage = ref(1)
const pageSize = ref(10)
const showForm = ref(false)
const selectedSubscription = ref<Subscription | null>(null)
const dateRange = ref<Date[] | null>(null)

// 搜尋過濾器
const filters = reactive({
  petName: '',
  status: '',
  startDate: '',
  endDate: ''
})

// 狀態選項
const statusOptions = [
  { label: '全部狀態', value: '' },
  { label: '啟用中', value: 'ACTIVE' },
  { label: '暫停', value: 'SUSPENDED' },
  { label: '已完成', value: 'COMPLETED' },
  { label: '已取消', value: 'CANCELLED' }
]

// Computed
const isExpiringSoon = (subscription: Subscription) => {
  if (!subscription.endDate) return false
  const endDate = dayjs(subscription.endDate)
  const today = dayjs()
  const daysLeft = endDate.diff(today, 'day')
  return daysLeft <= 7 && daysLeft >= 0
}

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
    const response = await subscriptionApi.getSubscriptions()
    subscriptions.value = response.map(sub => ({
      ...sub,
      // 補充缺少的屬性
      name: sub.name || `${sub.petName || '未知寵物'} - 包月方案`,
      serviceContent: sub.serviceContent || '基礎服務',
      totalTimes: sub.totalTimes || sub.totalUsageLimit || 0,
      totalAmount: sub.totalAmount || sub.subscriptionPrice || 0,
      paidAmount: sub.paidAmount || sub.subscriptionPrice || 0
    }))
    total.value = subscriptions.value.length
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

const getStatusText = (status: string) => {
  const statusMap: Record<string, string> = {
    'ACTIVE': '啟用中',
    'SUSPENDED': '暫停',
    'COMPLETED': '已完成',
    'CANCELLED': '已取消'
  }
  return statusMap[status] || status
}

const getStatusSeverity = (status: string) => {
  const severityMap: Record<string, string> = {
    'ACTIVE': 'success',
    'SUSPENDED': 'warning',
    'COMPLETED': 'info',
    'CANCELLED': 'danger'
  }
  return severityMap[status] || 'info'
}

const handleCreate = () => {
  selectedSubscription.value = null
  showForm.value = true
}

const viewSubscription = (subscription: Subscription) => {
  selectedSubscription.value = subscription
  showForm.value = true
}

const editSubscription = (subscription: Subscription) => {
  selectedSubscription.value = subscription
  showForm.value = true
}

const deleteSubscription = (subscription: Subscription) => {
  confirm.require({
    message: `確定要刪除「${subscription.name || '此包月方案'}」嗎？此操作無法復原。`,
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

const closeForm = () => {
  showForm.value = false
  selectedSubscription.value = null
}

const handleFormSuccess = () => {
  closeForm()
  loadSubscriptions()
}

const handleSearch = () => {
  // 將日期範圍轉換為過濾器
  if (dateRange.value && dateRange.value.length === 2) {
    filters.startDate = dayjs(dateRange.value[0]).format('YYYY-MM-DD')
    filters.endDate = dayjs(dateRange.value[1]).format('YYYY-MM-DD')
  } else {
    filters.startDate = ''
    filters.endDate = ''
  }

  currentPage.value = 1
  loadSubscriptions()
}

const resetFilters = () => {
  Object.assign(filters, {
    petName: '',
    status: '',
    startDate: '',
    endDate: ''
  })
  dateRange.value = null
  currentPage.value = 1
  loadSubscriptions()
}

const onPageChange = (event: any) => {
  currentPage.value = event.page + 1
  loadSubscriptions()
}

// Lifecycle
onMounted(() => {
  loadSubscriptions()
})
</script>

<style scoped>
.subscription-list {
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

.filters-section {
  margin-bottom: 1.5rem;
  padding: 1rem;
  background: var(--p-surface-50);
  border-radius: var(--p-border-radius);
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
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1rem;
}

.col-12 {
  grid-column: span 12;
}

.md\:col-3 {
  grid-column: span 3;
}

@media (max-width: 768px) {
  .md\:col-3 {
    grid-column: span 12;
  }
}

.table-section {
  margin-top: 1rem;
}

.pet-info {
  display: flex;
  flex-direction: column;
}

.pet-name {
  font-weight: 600;
  color: var(--p-text-color);
}

.pet-id {
  color: var(--p-text-color-secondary);
  font-size: 0.875rem;
}

.subscription-name {
  font-weight: 500;
  color: var(--p-text-color);
}

.usage-info {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.usage-count {
  font-weight: 500;
  color: var(--p-text-color);
}

.usage-bar {
  width: 100%;
}

.amount-info {
  display: flex;
  flex-direction: column;
}

.total-amount {
  font-weight: 600;
  color: var(--p-text-color);
}

.paid-amount {
  color: var(--p-text-color-secondary);
  font-size: 0.875rem;
}

.date-range {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.start-date,
.end-date {
  font-size: 0.875rem;
  color: var(--p-text-color);
}

.to {
  color: var(--p-text-color-secondary);
  font-size: 0.75rem;
}

.actions {
  display: flex;
  gap: 0.5rem;
}

.empty-state {
  text-align: center;
  padding: 3rem 1rem;
  color: var(--p-text-color-secondary);
}

.empty-state h3 {
  margin: 1rem 0;
  color: var(--p-text-color);
}

.empty-state p {
  margin-bottom: 1.5rem;
}

.flex {
  display: flex;
}

.gap-2 {
  gap: 0.5rem;
}

.mt-1 {
  margin-top: 0.25rem;
}
</style>