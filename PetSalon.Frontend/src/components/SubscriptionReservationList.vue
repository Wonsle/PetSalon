<template>
  <Card class="reservation-list-card">
    <template #header>
      <div class="card-header">
        <h3>使用記錄</h3>
        <Tag
          :value="`共 ${reservations.length} 筆`"
          severity="info"
        />
      </div>
    </template>
    <template #content>
      <div v-if="loading" class="loading-section">
        <ProgressSpinner />
        <p>載入預約記錄中...</p>
      </div>

      <div v-else-if="reservations.length === 0" class="empty-section">
        <i class="pi pi-calendar-times" style="font-size: 2rem; color: var(--p-text-color-secondary);"></i>
        <p>此包月方案尚未使用</p>
      </div>

      <DataTable
        v-else
        :value="reservations"
        responsive-layout="scroll"
        class="p-datatable-sm"
      >
        <Column field="reserveDate" header="預約時間" style="min-width: 200px">
          <template #body="{ data }">
            <div class="datetime-cell">
              <div class="date">{{ formatDate(data.reserveDate) }}</div>
              <div class="time">{{ data.reserveTime }}</div>
            </div>
          </template>
        </Column>

        <Column field="serviceType" header="服務項目" style="min-width: 300px">
          <template #body="{ data }">
            <div class="services-tags">
              <Tag
                v-for="(service, index) in parseServices(data.serviceType)"
                :key="index"
                :value="service"
                severity="info"
                class="service-tag"
              />
            </div>
          </template>
        </Column>

        <Column field="status" header="狀態" style="min-width: 100px">
          <template #body="{ data }">
            <Tag
              :value="getStatusLabel(data.status)"
              :severity="getStatusSeverity(data.status)"
            />
          </template>
        </Column>

        <Column header="操作" style="min-width: 100px; text-align: center">
          <template #body="{ data }">
            <Button
              icon="pi pi-eye"
              size="small"
              severity="info"
              text
              @click="viewReservationDetail(data)"
              v-tooltip="'查看詳情'"
            />
            <Button
              icon="pi pi-pencil"
              size="small"
              severity="warning"
              text
              @click="editReservation(data)"
              v-tooltip="'編輯'"
            />
          </template>
        </Column>
      </DataTable>
    </template>
  </Card>

  <!-- 預約詳情 Dialog -->
  <ReservationDetailDialog
    v-model:visible="showDetailDialog"
    :reservation="selectedReservation"
  />

  <!-- 編輯預約 Dialog -->
  <ReservationForm
    :visible="showEditDialog"
    :reservation="selectedReservation"
    @close="closeDialog"
    @success="handleSuccess"
  />
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useToast } from 'primevue/usetoast'
import type { Reservation } from '@/types/reservation'
import { reservationApi } from '@/api/reservation'
import ReservationDetailDialog from '@/components/ReservationDetailDialog.vue'
import ReservationForm from '@/components/forms/ReservationForm.vue'

interface Props {
  subscriptionId: number
}

const props = defineProps<Props>()
const toast = useToast()

// State
const loading = ref(false)
const reservations = ref<Reservation[]>([])
const selectedReservation = ref<Reservation | null>(null)
const showDetailDialog = ref(false)
const showEditDialog = ref(false)

// Methods
const loadReservations = async () => {
  if (!props.subscriptionId || props.subscriptionId <= 0) {
    console.warn('⚠️ [SubscriptionReservationList] Invalid subscriptionId:', props.subscriptionId)
    return
  }

  console.log('🔍 [SubscriptionReservationList] Loading reservations for subscription:', props.subscriptionId)
  loading.value = true
  try {
    reservations.value = await reservationApi.getReservationsBySubscription(props.subscriptionId)
    console.log('✅ [SubscriptionReservationList] Loaded reservations:', reservations.value.length, 'records')
  } catch (error: any) {
    console.error('❌ [SubscriptionReservationList] 載入預約記錄失敗:', error)
    console.error('❌ [SubscriptionReservationList] Error details:', {
      message: error.message,
      response: error.response?.data,
      status: error.response?.status
    })
    toast.add({
      severity: 'error',
      summary: '載入失敗',
      detail: error.response?.data?.message || '載入預約記錄失敗',
      life: 3000
    })
  } finally {
    loading.value = false
  }
}

const formatDate = (dateStr: string) => {
  if (!dateStr) return '-'
  const date = new Date(dateStr)
  return date.toLocaleDateString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit'
  })
}

const parseServices = (serviceType: string): string[] => {
  if (!serviceType || serviceType === '未指定服務') return ['未指定服務']
  // 服務項目可能以逗號或其他分隔符分隔
  return serviceType.split(/[,，]/).map(s => s.trim()).filter(s => s)
}

const getStatusLabel = (status: string) => {
  const statusMap: Record<string, string> = {
    PENDING: '待確認',
    CONFIRMED: '已確認',
    IN_PROGRESS: '進行中',
    COMPLETED: '已完成',
    CANCELLED: '已取消',
    NO_SHOW: '未到場'
  }
  return statusMap[status] || status
}

const getStatusSeverity = (status: string) => {
  const severityMap: Record<string, string> = {
    PENDING: 'warning',
    CONFIRMED: 'info',
    IN_PROGRESS: 'success',
    COMPLETED: 'success',
    CANCELLED: 'danger',
    NO_SHOW: 'warning'
  }
  return severityMap[status] || 'secondary'
}

const viewReservationDetail = (reservation: Reservation) => {
  selectedReservation.value = reservation
  showDetailDialog.value = true
}

const editReservation = (reservation: Reservation) => {
  selectedReservation.value = reservation
  showEditDialog.value = true
}

const closeDialog = () => {
  showEditDialog.value = false
  selectedReservation.value = null
}

const handleSuccess = () => {
  closeDialog()
  loadReservations()
}

// Watch for subscriptionId changes
watch(() => props.subscriptionId, () => {
  loadReservations()
}, { immediate: true })

// Lifecycle
onMounted(() => {
  loadReservations()
})
</script>

<style scoped>
.reservation-list-card {
  margin-top: 2rem;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
}

.card-header h3 {
  margin: 0;
  color: var(--p-text-color);
  font-size: 1.25rem;
}

.loading-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 200px;
  color: var(--p-text-color-secondary);
}

.loading-section p {
  margin-top: 1rem;
}

.empty-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 200px;
  color: var(--p-text-color-secondary);
}

.empty-section p {
  margin-top: 1rem;
  font-size: 1rem;
}

.datetime-cell {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.datetime-cell .date {
  font-weight: 600;
  color: var(--p-text-color);
}

.datetime-cell .time {
  font-size: 0.875rem;
  color: var(--p-text-color-secondary);
}

.services-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.service-tag {
  font-size: 0.875rem;
}

:deep(.p-datatable) {
  border: 1px solid var(--p-surface-border);
  border-radius: var(--p-border-radius);
}

:deep(.p-datatable-header) {
  background: var(--p-surface-50);
  border-bottom: 1px solid var(--p-surface-border);
}

:deep(.p-datatable-tbody > tr:hover) {
  background: var(--p-surface-50);
}
</style>
