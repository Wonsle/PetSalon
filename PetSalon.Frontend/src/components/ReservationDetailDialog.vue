<template>
  <Dialog
    :visible="visible"
    header="預約詳情"
    :style="{ width: '650px' }"
    :modal="true"
    @update:visible="handleClose"
  >
    <div v-if="reservation" class="reservation-details">
      <!-- 基本資訊區塊 -->
      <div class="section">
        <h4 class="section-title">基本資訊</h4>
        <div class="detail-row">
          <span class="label">預約編號:</span>
          <span class="value">{{ reservation.id }}</span>
        </div>
        <div class="detail-row">
          <span class="label">寵物名稱:</span>
          <span class="value">{{ reservation.petName || reservation.name || `寵物 #${reservation.petId}` }}</span>
        </div>
        <div class="detail-row">
          <span class="label">主人姓名:</span>
          <span class="value">{{ reservation.ownerName || '未指定' }}</span>
        </div>
        <div class="detail-row">
          <span class="label">聯絡電話:</span>
          <span class="value">{{ reservation.contactPhone || '未提供' }}</span>
        </div>
      </div>

      <!-- 預約資訊區塊 -->
      <div class="section">
        <h4 class="section-title">預約資訊</h4>
        <div class="detail-row">
          <span class="label">預約日期:</span>
          <span class="value">{{ formatDate(reservation.reserveDate) }}</span>
        </div>
        <div class="detail-row">
          <span class="label">預約時間:</span>
          <span class="value">{{ reservation.reserveTime }}</span>
        </div>
        <div class="detail-row">
          <span class="label">服務項目:</span>
          <div class="services-tags">
            <Tag
              v-for="(service, index) in parseServices(reservation.serviceType)"
              :key="index"
              :value="service"
              severity="info"
            />
          </div>
        </div>
        <div v-if="reservation.designer" class="detail-row">
          <span class="label">指定設計師:</span>
          <span class="value">{{ reservation.designer }}</span>
        </div>
      </div>

      <!-- 包月資訊區塊 -->
      <div v-if="reservation.useSubscription || reservation.subscriptionId" class="section">
        <h4 class="section-title">包月資訊</h4>
        <div class="detail-row">
          <span class="label">使用方案:</span>
          <span class="value">
            <Tag icon="pi pi-star" :value="reservation.subscriptionName || '包月方案'" severity="success" />
          </span>
        </div>
        <div v-if="reservation.subscriptionDeductionCount" class="detail-row">
          <span class="label">扣除次數:</span>
          <span class="value">{{ reservation.subscriptionDeductionCount }} 次</span>
        </div>
      </div>

      <!-- 狀態與備註區塊 -->
      <div class="section">
        <h4 class="section-title">其他資訊</h4>
        <div class="detail-row">
          <span class="label">預約狀態:</span>
          <span class="value">
            <Tag
              :value="getStatusLabel(reservation.status)"
              :severity="getStatusSeverity(reservation.status)"
            />
          </span>
        </div>
        <div v-if="reservation.serviceDurationMinutes" class="detail-row">
          <span class="label">服務時長:</span>
          <span class="value">{{ reservation.serviceDurationMinutes }} 分鐘</span>
        </div>
        <div v-if="reservation.totalAmount !== undefined" class="detail-row">
          <span class="label">總金額:</span>
          <span class="value amount">NT$ {{ reservation.totalAmount.toLocaleString() }}</span>
        </div>
        <div v-if="reservation.note || reservation.memo" class="detail-row">
          <span class="label">備註:</span>
          <span class="value">{{ reservation.note || reservation.memo }}</span>
        </div>
        <div class="detail-row">
          <span class="label">建立時間:</span>
          <span class="value">{{ formatDateTime(reservation.createTime) }}</span>
        </div>
      </div>
    </div>

    <template #footer>
      <div class="dialog-footer">
        <Button label="關閉" severity="secondary" @click="handleClose" />
      </div>
    </template>
  </Dialog>
</template>

<script setup lang="ts">
import type { Reservation } from '@/types/reservation'

interface Props {
  visible: boolean
  reservation: Reservation | null
}

interface Emits {
  (e: 'update:visible', value: boolean): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

// Methods
const handleClose = () => {
  emit('update:visible', false)
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

const parseServices = (serviceType: string): string[] => {
  if (!serviceType || serviceType === '未指定服務') return ['未指定服務']
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
</script>

<style scoped>
.reservation-details {
  padding: 0.5rem 0;
}

.section {
  margin-bottom: 1.5rem;
}

.section:last-child {
  margin-bottom: 0;
}

.section-title {
  margin: 0 0 1rem 0;
  padding-bottom: 0.5rem;
  border-bottom: 2px solid var(--p-primary-color);
  color: var(--p-primary-color);
  font-size: 1rem;
  font-weight: 600;
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

.label {
  font-weight: 600;
  color: var(--p-text-color);
  min-width: 120px;
  flex-shrink: 0;
}

.value {
  color: var(--p-text-color-secondary);
  text-align: right;
  flex: 1;
}

.value.amount {
  font-weight: 600;
  color: var(--p-primary-color);
  font-size: 1.1rem;
}

.services-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  justify-content: flex-end;
  flex: 1;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
}

@media (max-width: 768px) {
  .detail-row {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.5rem;
  }

  .label {
    min-width: auto;
  }

  .value {
    text-align: left;
  }

  .services-tags {
    justify-content: flex-start;
  }
}
</style>
