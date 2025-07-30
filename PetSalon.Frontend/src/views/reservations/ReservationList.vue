<template>
  <div class="reservation-list">
    <div class="header">
      <h2>預約管理</h2>
      <Button
        label="新增預約"
        icon="pi pi-plus"
        @click="showCreateDialog = true"
      />
    </div>

    <!-- 篩選區域 -->
    <Card class="search-section">
      <template #content>
        <div class="search-grid">
          <div class="search-field">
            <label>搜尋關鍵字</label>
            <InputText
              v-model="searchParams.keyword"
              placeholder="寵物名稱或主人姓名"
              @input="debounceSearch"
            />
          </div>
          <div class="search-field">
            <label>預約狀態</label>
            <Select
              v-model="searchParams.status"
              :options="statusOptions"
              option-label="label"
              option-value="value"
              placeholder="請選擇狀態"
              @change="loadReservations"
            />
          </div>
          <div class="search-field">
            <label>日期範圍</label>
            <Calendar
              v-model="dateRange"
              selection-mode="range"
              date-format="yy/mm/dd"
              placeholder="選擇日期範圍"
              @date-select="handleDateRangeChange"
            />
          </div>
          <div class="search-field">
            <label>&nbsp;</label>
            <Button
              label="重置篩選"
              severity="secondary"
              icon="pi pi-refresh"
              @click="resetFilters"
            />
          </div>
        </div>
      </template>
    </Card>

    <!-- 資料表格 -->
    <div class="table-section">
      <DataTable
        :value="reservations"
        :loading="loading"
        :rows="searchParams.pageSize"
        :total-records="totalRecords"
        lazy
        paginator
        :rows-per-page-options="[10, 20, 50]"
        @page="onPageChange"
        responsive-layout="scroll"
        class="p-datatable-sm"
      >
        <template #empty>
          <div class="empty-state">
            <i class="pi pi-calendar" style="font-size: 3rem; color: #ccc;"></i>
            <h3>暫無預約記錄</h3>
            <p>目前沒有符合條件的預約記錄</p>
            <Button
              label="新增預約"
              icon="pi pi-plus"
              @click="showCreateDialog = true"
            />
          </div>
        </template>

        <Column field="petName" header="寵物資訊" style="min-width: 200px">
          <template #body="{ data }">
            <div class="pet-info">
              <div class="pet-name">{{ data.petName || data.name || `寵物 #${data.petId}` }}</div>
              <div class="owner-name">{{ data.ownerName }}</div>
              <div v-if="data.useSubscription" class="subscription-badge">
                <Tag icon="pi pi-star" value="包月" severity="success" size="small" />
              </div>
            </div>
          </template>
        </Column>

        <Column field="contactPhone" header="聯絡電話" style="min-width: 120px" />

        <Column field="reserveDate" header="預約日期" style="min-width: 120px">
          <template #body="{ data }">
            {{ formatDate(data.reserveDate) }}
          </template>
        </Column>

        <Column field="reserveTime" header="預約時間" style="min-width: 100px" />

        <Column field="serviceType" header="服務項目" style="min-width: 120px" />

        <Column field="designer" header="設計師" style="min-width: 100px">
          <template #body="{ data }">
            {{ data.designer || '未指定' }}
          </template>
        </Column>

        <Column field="status" header="狀態" style="min-width: 100px">
          <template #body="{ data }">
            <div class="status-container">
              <Tag
                :value="getStatusLabel(data.status)"
                :severity="getStatusSeverity(data.status)"
              />
              <div v-if="data.useSubscription" class="subscription-info">
                <small class="subscription-detail">
                  扣除 {{ data.subscriptionDeductionCount || 1 }} 次
                </small>
              </div>
            </div>
          </template>
        </Column>

        <Column field="subscriptionName" header="使用方案" style="min-width: 120px">
          <template #body="{ data }">
            {{ data.subscriptionName || '單次服務' }}
          </template>
        </Column>

        <Column header="操作" style="min-width: 150px">
          <template #body="{ data }">
            <div class="actions">
              <Button
                icon="pi pi-eye"
                size="small"
                severity="info"
                @click="viewReservation(data)"
                v-tooltip="'查看詳情'"
              />
              <Button
                icon="pi pi-pencil"
                size="small"
                severity="warning"
                @click="editReservation(data)"
                v-tooltip="'編輯'"
                :disabled="data.status === 'completed' || data.status === 'cancelled'"
              />
              <Button
                icon="pi pi-check"
                size="small"
                severity="success"
                @click="updateStatus(data, 'completed')"
                v-tooltip="'完成'"
                v-if="data.status === 'confirmed' || data.status === 'in_progress'"
              />
              <Button
                icon="pi pi-times"
                size="small"
                severity="danger"
                @click="cancelReservation(data)"
                v-tooltip="'取消'"
                v-if="data.status !== 'completed' && data.status !== 'cancelled'"
              />
            </div>
          </template>
        </Column>
      </DataTable>
    </div>

    <!-- 新增/編輯預約對話框 -->
    <ReservationForm
      :visible="showCreateDialog || showEditDialog"
      :reservation="selectedReservation"
      @close="closeDialog"
      @success="handleSuccess"
    />

    <!-- 查看詳情對話框 -->
    <Dialog
      :visible="showViewDialog"
      header="預約詳情"
      :style="{ width: '600px' }"
      :modal="true"
      @update:visible="showViewDialog = false"
    >
      <div v-if="selectedReservation" class="reservation-details">
        <div class="detail-row">
          <span class="label">預約編號:</span>
          <span class="value">{{ selectedReservation.id }}</span>
        </div>
        <div class="detail-row">
          <span class="label">寵物名稱:</span>
          <span class="value">{{ selectedReservation.petName || selectedReservation.name || `寵物 #${selectedReservation.petId}` }}</span>
        </div>
        <div class="detail-row">
          <span class="label">主人姓名:</span>
          <span class="value">{{ selectedReservation.ownerName }}</span>
        </div>
        <div class="detail-row">
          <span class="label">聯絡電話:</span>
          <span class="value">{{ selectedReservation.contactPhone }}</span>
        </div>
        <div class="detail-row">
          <span class="label">預約日期:</span>
          <span class="value">{{ formatDate(selectedReservation.reserveDate) }}</span>
        </div>
        <div class="detail-row">
          <span class="label">預約時間:</span>
          <span class="value">{{ selectedReservation.reserveTime }}</span>
        </div>
        <div class="detail-row">
          <span class="label">服務項目:</span>
          <span class="value">{{ selectedReservation.serviceType }}</span>
        </div>
        <div class="detail-row">
          <span class="label">指定設計師:</span>
          <span class="value">{{ selectedReservation.designer || '未指定' }}</span>
        </div>
        <div class="detail-row">
          <span class="label">使用方案:</span>
          <span class="value">{{ selectedReservation.subscriptionName || '單次服務' }}</span>
        </div>
        <div class="detail-row">
          <span class="label">預約狀態:</span>
          <Tag
            :value="getStatusLabel(selectedReservation.status)"
            :severity="getStatusSeverity(selectedReservation.status)"
          />
        </div>
        <div v-if="selectedReservation.note" class="detail-row">
          <span class="label">備註:</span>
          <span class="value">{{ selectedReservation.note }}</span>
        </div>
        <div class="detail-row">
          <span class="label">建立時間:</span>
          <span class="value">{{ formatDateTime(selectedReservation.createTime) }}</span>
        </div>
      </div>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import type { Reservation, ReservationSearchParams } from '@/types/reservation'
import { reservationApi } from '@/api/reservation'
import ReservationForm from '@/components/forms/ReservationForm.vue'

// Refs
const loading = ref(false)
const reservations = ref<Reservation[]>([])
const totalRecords = ref(0)
const selectedReservation = ref<Reservation | null>(null)
const showCreateDialog = ref(false)
const showEditDialog = ref(false)
const showViewDialog = ref(false)
const dateRange = ref<Date[] | null>(null)

const toast = useToast()
const confirm = useConfirm()

// 搜尋參數
const searchParams = reactive<ReservationSearchParams>({
  keyword: '',
  status: '',
  startDate: '',
  endDate: '',
  page: 1,
  pageSize: 10
})

// 狀態選項
const statusOptions = [
  { label: '全部狀態', value: '' },
  { label: '待確認', value: 'pending' },
  { label: '已確認', value: 'confirmed' },
  { label: '進行中', value: 'in_progress' },
  { label: '已完成', value: 'completed' },
  { label: '已取消', value: 'cancelled' },
  { label: '未到場', value: 'no_show' }
]

// 防抖搜尋
let searchTimeout: number
const debounceSearch = () => {
  clearTimeout(searchTimeout)
  searchTimeout = window.setTimeout(() => {
    searchParams.page = 1
    loadReservations()
  }, 500)
}

// 載入預約列表
const loadReservations = async () => {
  loading.value = true
  try {
    const response = await reservationApi.getReservations(searchParams)
    reservations.value = response.data
    totalRecords.value = response.total
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: '載入預約列表失敗',
      life: 3000
    })
  } finally {
    loading.value = false
  }
}

// 分頁變更
const onPageChange = (event: any) => {
  searchParams.page = event.page + 1
  searchParams.pageSize = event.rows
  loadReservations()
}

// 處理日期範圍變更
const handleDateRangeChange = () => {
  if (dateRange.value && dateRange.value.length === 2) {
    searchParams.startDate = dateRange.value[0].toISOString().split('T')[0]
    searchParams.endDate = dateRange.value[1].toISOString().split('T')[0]
  } else {
    searchParams.startDate = ''
    searchParams.endDate = ''
  }
  searchParams.page = 1
  loadReservations()
}

// 重置篩選
const resetFilters = () => {
  Object.assign(searchParams, {
    keyword: '',
    status: '',
    startDate: '',
    endDate: '',
    page: 1,
    pageSize: 10
  })
  dateRange.value = null
  loadReservations()
}

// 查看預約詳情
const viewReservation = (reservation: Reservation) => {
  selectedReservation.value = reservation
  showViewDialog.value = true
}

// 編輯預約
const editReservation = (reservation: Reservation) => {
  selectedReservation.value = reservation
  showEditDialog.value = true
}

// 更新預約狀態
const updateStatus = async (reservation: Reservation, status: string) => {
  const statusLabel = getStatusLabel(status)
  confirm.require({
    message: `確定要將預約狀態改為「${statusLabel}」嗎？`,
    header: '確認操作',
    icon: 'pi pi-exclamation-triangle',
    accept: async () => {
      try {
        await reservationApi.updateReservationStatus(reservation.id, status)
        toast.add({
          severity: 'success',
          summary: '成功',
          detail: `預約狀態已更新為「${statusLabel}」`,
          life: 3000
        })
        loadReservations()
      } catch (error: any) {
        toast.add({
          severity: 'error',
          summary: '錯誤',
          detail: '更新狀態失敗',
          life: 3000
        })
      }
    }
  })
}

// 取消預約
const cancelReservation = (reservation: Reservation) => {
  confirm.require({
    message: '確定要取消這個預約嗎？此操作無法撤銷。',
    header: '確認取消',
    icon: 'pi pi-exclamation-triangle',
    accept: async () => {
      try {
        await reservationApi.updateReservationStatus(reservation.id, 'cancelled')
        toast.add({
          severity: 'success',
          summary: '成功',
          detail: '預約已取消',
          life: 3000
        })
        loadReservations()
      } catch (error: any) {
        toast.add({
          severity: 'error',
          summary: '錯誤',
          detail: '取消預約失敗',
          life: 3000
        })
      }
    }
  })
}

// 關閉對話框
const closeDialog = () => {
  showCreateDialog.value = false
  showEditDialog.value = false
  selectedReservation.value = null
}

// 處理成功操作
const handleSuccess = () => {
  closeDialog()
  loadReservations()
}

// 格式化日期
const formatDate = (dateStr: string) => {
  if (!dateStr) return '-'
  const date = new Date(dateStr)
  return date.toLocaleDateString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit'
  })
}

// 格式化日期時間
const formatDateTime = (dateStr: string) => {
  if (!dateStr) return '-'
  const date = new Date(dateStr)
  return date.toLocaleString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

// 獲取狀態標籤
const getStatusLabel = (status: string) => {
  const statusMap: Record<string, string> = {
    pending: '待確認',
    confirmed: '已確認',
    in_progress: '進行中',
    completed: '已完成',
    cancelled: '已取消',
    no_show: '未到場'
  }
  return statusMap[status] || status
}

// 獲取狀態嚴重程度
const getStatusSeverity = (status: string) => {
  const severityMap: Record<string, string> = {
    pending: 'warning',
    confirmed: 'info',
    in_progress: 'success',
    completed: 'success',
    cancelled: 'danger',
    no_show: 'secondary'
  }
  return severityMap[status] || 'info'
}

// 初始化載入
onMounted(() => {
  loadReservations()
})
</script>

<style scoped>
.reservation-list {
  padding: 20px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
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
  grid-template-columns: 2fr 1fr 1fr auto;
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

.owner-name {
  color: var(--p-text-color-secondary);
  font-size: 0.875rem;
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

.reservation-details {
  padding: 1rem 0;
}

.detail-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 0;
  border-bottom: 1px solid var(--p-surface-200);
}

.detail-row:last-child {
  border-bottom: none;
}

.detail-row .label {
  font-weight: 600;
  color: var(--p-text-color);
  margin-bottom: 0;
  min-width: 120px;
}

.detail-row .value {
  color: var(--p-text-color-secondary);
  text-align: right;
  flex: 1;
}

.flex {
  display: flex;
}

.gap-2 {
  gap: 0.5rem;
}
</style>