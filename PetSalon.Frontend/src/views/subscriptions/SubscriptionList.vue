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
        <Card class="search-section">
          <template #content>
            <div class="search-grid">
              <div class="search-field">
                <label>寵物搜尋</label>
                <InputText
                  v-model="filters.petName"
                  placeholder="搜尋寵物名稱"
                  @input="handleSearch"
                />
              </div>
              <div class="search-field">
                <label>開始日期</label>
                <Calendar
                  v-model="startDateFilter"
                  date-format="yy/mm/dd"
                  placeholder="選擇開始日期"
                  @date-select="handleSearch"
                />
              </div>
              <div class="search-field">
                <label>結束日期</label>
                <Calendar
                  v-model="endDateFilter"
                  date-format="yy/mm/dd"
                  placeholder="選擇結束日期"
                  @date-select="handleSearch"
                />
              </div>
              <div class="search-field">
                <label>狀態篩選</label>
                <SelectButton
                  v-model="filters.expiryStatus"
                  :options="expiryStatusOptions"
                  option-label="label"
                  option-value="value"
                  @change="handleSearch"
                />
              </div>
              <div class="search-field">
                <label>&nbsp;</label>
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
          </template>
        </Card>

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
                  <span class="pet-name">{{ getPetDisplayName(slotProps.data.pet) }}</span>
                </div>
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
                  <div class="usage-stats">
                    <span class="usage-count">
                      已用: {{ slotProps.data.usedCount || 0 }}
                    </span>
                    <span class="reserved-count" v-if="slotProps.data.reservedCount > 0">
                      預留: {{ slotProps.data.reservedCount }}
                    </span>
                    <span class="total-count">
                      總計: {{ slotProps.data.totalUsageLimit || '∞' }}
                    </span>
                  </div>
                  <div class="usage-progress">
                    <ProgressBar
                      :value="getUsagePercentage(slotProps.data)"
                      :show-value="false"
                      style="height: 6px"
                    />
                    <small class="remaining-text">
                      剩餘: {{ getRemainingUsage(slotProps.data) }}次
                    </small>
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

            <Column field="isActive" header="狀態" :sortable="true">
              <template #body="slotProps">
                <div class="status-container">
                  <Tag
                    :value="getSubscriptionStatus(slotProps.data).label"
                    :severity="getSubscriptionStatus(slotProps.data).severity"
                    :icon="getSubscriptionStatus(slotProps.data).icon"
                  />
                  <small v-if="isExpiringSoon(slotProps.data)" class="expiry-warning">
                    {{ getDaysUntilExpiry(slotProps.data) }}天後到期
                  </small>
                  <small v-if="isUsageAlmostExhausted(slotProps.data)" class="usage-warning">
                    次數即將用完
                  </small>
                </div>
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
const startDateFilter = ref<Date | null>(null)
const endDateFilter = ref<Date | null>(null)

// 搜尋過濾器
const filters = reactive({
  petName: '',
  startDate: '',
  endDate: '',
  expiryStatus: ''
})

// 到期狀態選項
const expiryStatusOptions = [
  { label: '全部', value: '' },
  { label: '有效', value: 'active' },
  { label: '已過期', value: 'expired' },
  { label: '即將到期', value: 'expiring' }
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
    let filteredData = response.map(sub => {
      // 取得正確的寵物名稱
      const petName = sub.petName || `寵物 #${sub.petId}`

      return {
        ...sub,
        // 補充缺少的屬性
        name: sub.name || `${petName} - 包月方案`,
        serviceContent: sub.serviceContent || '基礎服務',
        totalTimes: sub.totalTimes || sub.totalUsageLimit || 0,
        totalAmount: sub.totalAmount || sub.subscriptionPrice || 0,
        paidAmount: sub.paidAmount || sub.subscriptionPrice || 0
      } as Subscription
    })

    // 套用篩選器
    filteredData = applyFilters(filteredData)

    subscriptions.value = filteredData
    total.value = filteredData.length
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


const getRemainingUsage = (subscription: any) => {
  if (!subscription.totalUsageLimit || subscription.totalUsageLimit === 0) return '無限'
  const remaining = subscription.totalUsageLimit - (subscription.usedCount || 0) - (subscription.reservedCount || 0)
  return Math.max(0, remaining)
}

const getDaysUntilExpiry = (subscription: any) => {
  if (!subscription.endDate) return 0
  return dayjs(subscription.endDate).diff(dayjs(), 'day')
}

const isUsageAlmostExhausted = (subscription: any) => {
  if (!subscription.totalUsageLimit || subscription.totalUsageLimit === 0) return false
  const remaining = getRemainingUsage(subscription)
  const remainingNum = typeof remaining === 'string' ? 0 : remaining
  return remainingNum <= 2 && remainingNum > 0
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
    message: `確定要刪除「${getPetDisplayName(subscription)}」的包月方案嗎？此操作無法復原。`,
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
  // 將獨立日期欄位轉換為過濾器
  if (startDateFilter.value) {
    filters.startDate = dayjs(startDateFilter.value).format('YYYY-MM-DD')
  } else {
    filters.startDate = ''
  }

  if (endDateFilter.value) {
    filters.endDate = dayjs(endDateFilter.value).format('YYYY-MM-DD')
  } else {
    filters.endDate = ''
  }

  currentPage.value = 1
  loadSubscriptions()
}

const resetFilters = () => {
  Object.assign(filters, {
    petName: '',
    startDate: '',
    endDate: '',
    expiryStatus: ''
  })
  startDateFilter.value = null
  endDateFilter.value = null
  currentPage.value = 1
  loadSubscriptions()
}

// 套用篩選器
const applyFilters = (data: Subscription[]) => {
  return data.filter(subscription => {
    // 寵物名稱篩選
    if (filters.petName && !getPetDisplayName(subscription).toLowerCase().includes(filters.petName.toLowerCase())) {
      return false
    }

    // 日期範圍篩選（基於包月期間）
    if (filters.startDate && filters.endDate) {
      const subStart = dayjs(subscription.startDate)
      const subEnd = dayjs(subscription.endDate)
      const filterStart = dayjs(filters.startDate)
      const filterEnd = dayjs(filters.endDate)

      // 檢查包月期間是否與篩選範圍有交集
      if (subEnd.isBefore(filterStart) || subStart.isAfter(filterEnd)) {
        return false
      }
    }

    // 到期狀態篩選
    if (filters.expiryStatus) {
      const now = dayjs()
      const endDate = dayjs(subscription.endDate)
      const isExpired = endDate.isBefore(now)
      const isExpiringSoon = !isExpired && endDate.diff(now, 'day') <= 7

      switch (filters.expiryStatus) {
        case 'active':
          return !isExpired && !isExpiringSoon
        case 'expired':
          return isExpired
        case 'expiring':
          return isExpiringSoon
        default:
          return true
      }
    }

    return true
  })
}

// 取得寵物顯示名稱
const getPetDisplayName = (subscription: Subscription) => {
  return subscription.petName || subscription.name || `寵物 #${subscription.petId}`
}

// 取得包月狀態
const getSubscriptionStatus = (subscription: Subscription) => {
  const now = dayjs()
  const endDate = dayjs(subscription.endDate)
  const isExpired = endDate.isBefore(now)
  const isExpiringSoon = !isExpired && endDate.diff(now, 'day') <= 7

  if (isExpired) {
    return {
      label: '已過期',
      severity: 'danger',
      icon: 'pi pi-times'
    }
  } else if (isExpiringSoon) {
    return {
      label: '即將到期',
      severity: 'warning',
      icon: 'pi pi-exclamation-triangle'
    }
  } else {
    return {
      label: '有效',
      severity: 'success',
      icon: 'pi pi-check'
    }
  }
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

.search-section {
  margin-bottom: 1.5rem;
}

.search-grid {
  display: grid;
  grid-template-columns: 2fr 1fr 1fr 3fr auto;
  gap: 1rem;
  align-items: end;
}

.search-field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.search-field label {
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--p-text-color);
}

@media (max-width: 768px) {
  .search-grid {
    grid-template-columns: 1fr;
    gap: 1rem;
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

/* SelectButton 自定義樣式 */
.search-field :deep(.p-selectbutton) {
  display: flex;
  flex-wrap: wrap;
  gap: 0.25rem;
}

.search-field :deep(.p-selectbutton .p-button) {
  flex: 1;
  min-width: 0;
  font-size: 0.8rem;
  padding: 0.4rem 0.6rem;
}

@media (max-width: 768px) {
  .search-field :deep(.p-selectbutton .p-button) {
    font-size: 0.75rem;
    padding: 0.3rem 0.5rem;
  }
}
</style>