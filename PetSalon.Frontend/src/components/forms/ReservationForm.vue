<template>
  <Dialog
    :visible="visible"
    :header="isEdit ? '編輯預約' : '新增預約'"
    :style="{ width: '800px' }"
    :modal="true"
    @update:visible="$emit('close')"
  >
    <form @submit.prevent="handleSubmit" class="reservation-form">
      <!-- 基本資訊 -->
      <div class="form-section">
        <h4>基本資訊</h4>
        <div class="grid">
          <div class="col-12 md:col-6">
            <div class="field">
              <label for="petId" class="required">寵物</label>
              <Select
                id="petId"
                v-model="form.petId"
                :options="pets"
                option-label="petName"
                option-value="petId"
                placeholder="選擇寵物"
                :loading="loadingPets"
                @change="onPetChange"
                :class="{ 'p-invalid': errors.petId }"
              />
              <small v-if="errors.petId" class="p-error">{{ errors.petId }}</small>
            </div>
          </div>

          <div class="col-12 md:col-6">
            <div class="field">
              <label for="reservationDate" class="required">預約日期</label>
              <Calendar
                id="reservationDate"
                v-model="form.reservationDate"
                date-format="yy/mm/dd"
                :min-date="new Date()"
                placeholder="選擇日期"
                :class="{ 'p-invalid': errors.reservationDate }"
              />
              <small v-if="errors.reservationDate" class="p-error">{{ errors.reservationDate }}</small>
            </div>
          </div>

          <div class="col-12 md:col-6">
            <div class="field">
              <label for="reservationTime" class="required">預約時間</label>
              <Calendar
                id="reservationTime"
                v-model="form.reservationTime"
                time-only
                placeholder="選擇時間"
                :class="{ 'p-invalid': errors.reservationTime }"
              />
              <small v-if="errors.reservationTime" class="p-error">{{ errors.reservationTime }}</small>
            </div>
          </div>

          <div class="col-12 md:col-6">
            <div class="field">
              <label for="status">狀態</label>
              <Select
                id="status"
                v-model="form.status"
                :options="statusOptions"
                option-label="label"
                option-value="value"
                placeholder="選擇狀態"
              />
            </div>
          </div>
        </div>
      </div>

      <!-- 包月方案選擇 -->
      <div class="form-section" v-if="form.petId && availableSubscriptions.length > 0">
        <div class="field">
          <label for="subscriptionId">包月方案</label>
          <Select
            id="subscriptionId"
            v-model="form.subscriptionId"
            :options="availableSubscriptions"
            option-label="displayName"
            option-value="subscriptionId"
            placeholder="選擇包月方案（可選）"
            @change="onSubscriptionSelect"
          />
        </div>
      </div>

      <!-- 服務項目 -->
      <div class="form-section">
        <h4>服務項目</h4>
        <div class="field">
          <div v-if="loadingServices" class="service-loading">
            <ProgressSpinner style="width:30px;height:30px" strokeWidth="4" />
            <span>載入服務項目中...</span>
          </div>
          <div v-else class="service-checkboxes">
            <div
              v-for="service in services"
              :key="service.serviceId"
              class="service-checkbox-item"
            >
              <Checkbox
                :id="`service-${service.serviceId}`"
                v-model="form.serviceIds"
                :value="service.serviceId"
                @change="calculateCost"
              />
              <label
                :for="`service-${service.serviceId}`"
                class="service-label"
              >
                <div class="service-info">
                  <span class="service-name">{{ service.serviceName }}</span>
                  <div class="price-info">
                    <span v-if="service.price > 0" class="service-price">
                      NT$ {{ service.price.toLocaleString() }}
                    </span>
                    <span v-else class="free-price">免費</span>
                  </div>
                </div>
              </label>
            </div>
          </div>
        </div>

      </div>

      <!-- 費用資訊 -->
      <div class="form-section" v-if="costCalculation">
        <div class="cost-header">
          <h4>費用資訊</h4>
          <ProgressSpinner v-if="calculatingCost" style="width:20px;height:20px" strokeWidth="4" />
        </div>
        <div class="cost-summary">
          <div class="cost-row">
            <span>服務費用:</span>
            <span>NT$ {{ costCalculation.serviceTotal?.toLocaleString() || 0 }}</span>
          </div>
          <div class="cost-row">
            <span>附加費用:</span>
            <span>NT$ {{ costCalculation.addonTotal?.toLocaleString() || 0 }}</span>
          </div>
          <div class="cost-row" v-if="costCalculation.discount > 0">
            <span>優惠折扣:</span>
            <span class="discount">-NT$ {{ costCalculation.discount?.toLocaleString() || 0 }}</span>
          </div>
          <div class="cost-row total">
            <span>總計:</span>
            <span>NT$ {{ costCalculation.totalAmount?.toLocaleString() || 0 }}</span>
          </div>
          <div class="cost-row" v-if="costCalculation.estimatedDuration">
            <span>預估時長:</span>
            <span>{{ Math.ceil(costCalculation.estimatedDuration) }} 分鐘</span>
          </div>
          <div v-if="form.subscriptionId" class="subscription-note">
            <Tag icon="pi pi-info-circle" value="使用包月服務，費用將從包月方案扣除" severity="info" />
          </div>
          <div v-if="!form.subscriptionId && costCalculation.discount === 0 && form.serviceIds.length > 0" class="subscription-note">
            <Tag icon="pi pi-lightbulb" value="選擇包月方案可享有優惠價格" severity="warn" />
          </div>
        </div>
      </div>

      <!-- 備註 -->
      <div class="form-section">
        <div class="field">
          <div class="col-12">
            <label for="memo">備註</label>
          </div>
          <div class="col-12">
            <Textarea
              id="memo"
              v-model="form.memo"
              rows="4"
              auto-resize
              placeholder="預約備註..."
              style="min-height: 80px; width: 100%;"
            />
          </div>
        </div>
      </div>
    </form>

    <template #footer>
      <div class="dialog-footer">
        <Button
          label="取消"
          severity="secondary"
          @click="$emit('close')"
        />
        <Button
          label="儲存"
          @click="handleSubmit"
          :loading="submitting"
        />
      </div>
    </template>
  </Dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useToast } from 'primevue/usetoast'
import dayjs from 'dayjs'
import { petApi } from '@/api/pet'
import { subscriptionApi } from '@/api/subscription'
import { reservationApi } from '@/api/reservation'
import { commonApi } from '@/api/common'
import { serviceApi } from '@/api/service'
import type { PetServicePrice } from '@/types/service'
import type { CostCalculationRequest, DurationCalculationRequest, ModernReservationRequest } from '@/types/reservation'

// Props & Emits
interface Props {
  visible: boolean
  reservation?: any
}

const props = withDefaults(defineProps<Props>(), {
  visible: false,
  reservation: null
})

const emit = defineEmits<{
  close: []
  success: []
}>()

// 定義類型介面
interface ReservationForm {
  petId: number | null
  reservationDate: Date | null
  reservationTime: Date | null
  serviceIds: number[]
  subscriptionId: number | null
  status: string
  memo: string
}

interface ValidationErrors {
  petId?: string
  reservationDate?: string
  reservationTime?: string
  serviceIds?: string
  [key: string]: string | undefined
}

interface Pet {
  petId: number
  petName: string
  [key: string]: any
}

interface Service {
  serviceId: number
  serviceName: string
  price: number
  [key: string]: any
}

interface Addon {
  addonId: number
  addonName: string
  price: number
  isCustomPrice?: boolean
  estimatedDuration?: number
  [key: string]: any
}

interface Subscription {
  subscriptionId: number
  subscriptionType: string
  totalUsageLimit: number
  usedCount: number
  reservedCount: number
  remainingUsage: number
  endDate: string
  displayName: string
  isExpiringSoon: boolean
  [key: string]: any
}

interface CostCalculation {
  serviceTotal: number
  addonTotal: number
  discount: number
  totalAmount: number
  estimatedDuration?: number // 預估總時長（分鐘）
}

// Composables
const toast = useToast()

// State
const form = ref<ReservationForm>({
  petId: null,
  reservationDate: null,
  reservationTime: null,
  serviceIds: [],
  subscriptionId: null,
  status: 'PENDING',
  memo: ''
})

const errors = ref<ValidationErrors>({})
const submitting = ref(false)
const loadingPets = ref(false)
const loadingServices = ref(false)
const calculatingCost = ref(false)

const pets = ref<Pet[]>([])
const services = ref<Service[]>([])
const availableSubscriptions = ref<Subscription[]>([])
const costCalculation = ref<CostCalculation | null>(null)
const petServicePrices = ref<PetServicePrice[]>([])

// Computed
const isEdit = computed(() => !!props.reservation?.id)

const statusOptions = [
  { label: '待確認', value: 'PENDING' },
  { label: '已確認', value: 'CONFIRMED' },
  { label: '進行中', value: 'IN_PROGRESS' },
  { label: '已完成', value: 'COMPLETED' },
  { label: '已取消', value: 'CANCELLED' },
  { label: '未到場', value: 'NO_SHOW' }
]


// Methods
const loadPets = async () => {
  loadingPets.value = true
  try {
    const response = await petApi.getPets({ page: 1, pageSize: 100 })
    pets.value = response.data || []
  } catch (error) {
    console.error('載入寵物列表失敗:', error)
    toast.add({
      severity: 'error',
      summary: '載入失敗',
      detail: '無法載入寵物列表',
      life: 3000
    })
  } finally {
    loadingPets.value = false
  }
}

const loadServices = async () => {
  loadingServices.value = true
  try {
    // 使用真實的 Service API
    const servicesData = await serviceApi.getActiveServices()
    services.value = servicesData.map(service => ({
      serviceId: service.serviceId,
      serviceName: service.serviceName,
      serviceType: service.serviceType,
      price: service.basePrice,
      description: service.description,
      estimatedDuration: service.estimatedDuration || 0
    }))
  } catch (error) {
    console.error('載入服務項目失敗:', error)
    // 如果 API 失敗，嘗試使用 SystemCode 作為備用
    try {
      const response = await commonApi.getSystemCodes('ServiceType')
      services.value = response?.map((item: any) => ({
        serviceId: item.id,
        serviceName: item.name,
        serviceType: item.code,
        price: 0,
        description: item.value
      })) || []
    } catch (fallbackError) {
      console.error('備用載入也失敗:', fallbackError)
      toast.add({
        severity: 'error',
        summary: '載入失敗',
        detail: '無法載入服務項目',
        life: 3000
      })
    }
  } finally {
    loadingServices.value = false
  }
}


const onPetChange = async () => {
  console.log('onPetChange called, petId:', form.value.petId)

  if (!form.value.petId) {
    availableSubscriptions.value = []
    petServicePrices.value = []
    // 重置服務價格
    await loadServices() // 重新載入預設服務價格
    return
  }

  try {
    // 載入包月方案 (附加服務價格功能已移除)
    const subscriptions = await subscriptionApi.getSubscriptionsByPet(form.value.petId)

    console.log('Raw subscriptions:', subscriptions)

    // 過濾出有效的包月方案並計算必要字段
    availableSubscriptions.value = subscriptions
      .map(sub => {
        // 計算剩餘次數（總次數 - 已使用 - 預留）
        const remainingUsage = Math.max(0, (sub.totalUsageLimit || 0) - (sub.usedCount || 0) - (sub.reservedCount || 0))

        const processed = {
          ...sub,
          remainingUsage,
          displayName: `${sub.subscriptionType} (剩餘: ${remainingUsage}次) ${formatDate(sub.startDate)} ~ ${formatDate(sub.endDate)}`,
          isExpiringSoon: isExpiringSoon(sub.endDate)
        }
        console.log('Processed subscription:', processed)
        return processed
      })
      .filter(sub => {
        const now = new Date()
        const endDate = new Date(sub.endDate)
        const isValid = sub.remainingUsage > 0 && endDate > now
        console.log(`Subscription ${sub.subscriptionId} validity:`, { remainingUsage: sub.remainingUsage, endDate, now, isValid })
        return isValid
      })

    console.log('Final available subscriptions:', availableSubscriptions.value)


    // 服務價格功能已移除，保持預設價格

    // 重新計算成本
    await calculateCost()

  } catch (error) {
    console.error('載入寵物相關資料失敗:', error)
    toast.add({
      severity: 'warn',
      summary: '載入失敗',
      detail: '無法載入該寵物的相關資料',
      life: 3000
    })
    availableSubscriptions.value = []
    petServicePrices.value = []
  }
}

const onSubscriptionSelect = () => {
  calculateCost()
}

const calculateCost = async () => {
  if (!form.value.petId || !form.value.serviceIds.length) {
    costCalculation.value = null
    return
  }

  calculatingCost.value = true
  try {
    // 使用真實的成本計算 API
    const costRequest: CostCalculationRequest = {
      petId: form.value.petId,
      serviceIds: form.value.serviceIds,
      useSubscription: !!form.value.subscriptionId,
      subscriptionId: form.value.subscriptionId || undefined
    }

    const [costResult, durationResult] = await Promise.all([
      reservationApi.calculateCost(costRequest),
      form.value.serviceIds.length > 0 
        ? reservationApi.calculateDuration(form.value.petId, {
            serviceIds: form.value.serviceIds
          })
        : Promise.resolve({ totalDuration: 0 })
    ])

    costCalculation.value = {
      serviceTotal: costResult.serviceTotal,
      addonTotal: costResult.addonTotal,
      discount: costResult.discount,
      totalAmount: costResult.totalAmount,
      estimatedDuration: durationResult.totalDuration
    }

    console.log('Cost calculation result:', costCalculation.value)
  } catch (error) {
    console.error('計算費用失敗:', error)
    // 如果 API 失敗，使用預設計算
    const selectedServices = services.value.filter(s => form.value.serviceIds.includes(s.serviceId))
    const serviceTotal = selectedServices.reduce((sum, service) => sum + (service.price || 0), 0)
    
    const addonTotal = 0
    
    let discount = 0
    if (form.value.subscriptionId) {
      discount = serviceTotal * 0.1
    }
    
    costCalculation.value = {
      serviceTotal,
      addonTotal,
      discount,
      totalAmount: Math.max(0, serviceTotal + addonTotal - discount),
      estimatedDuration: 60 // 預設 60 分鐘
    }
  } finally {
    calculatingCost.value = false
  }
}


const formatDate = (dateStr: string) => {
  return dayjs(dateStr).format('YYYY/MM/DD')
}

const isExpiringSoon = (endDate: string, days: number = 7) => {
  const end = dayjs(endDate)
  const now = dayjs()
  return end.diff(now, 'day') <= days
}

const validateForm = () => {
  const newErrors: any = {}

  if (!form.value.petId) newErrors.petId = '請選擇寵物'
  if (!form.value.reservationDate) newErrors.reservationDate = '請選擇預約日期'
  if (!form.value.reservationTime) newErrors.reservationTime = '請選擇預約時間'

  errors.value = newErrors
  return Object.keys(newErrors).length === 0
}

const handleSubmit = async () => {
  if (!validateForm()) return

  submitting.value = true
  try {
    const reservationData: ModernReservationRequest = {
      petId: form.value.petId!,
      reservationDate: form.value.reservationDate!,
      reservationTime: form.value.reservationTime!,
      serviceIds: form.value.serviceIds,
      useSubscription: !!form.value.subscriptionId,
      subscriptionId: form.value.subscriptionId || undefined,
      status: form.value.status,
      memo: form.value.memo || ''
    }

    console.log('Submitting reservation data:', reservationData)

    if (isEdit.value) {
      await reservationApi.updateReservation({
        id: props.reservation.id,
        ...reservationData
      })
    } else {
      await reservationApi.createReservation(reservationData)
    }

    toast.add({
      severity: 'success',
      summary: '儲存成功',
      detail: `預約${isEdit.value ? '更新' : '建立'}成功`,
      life: 3000
    })

    emit('success')
  } catch (error: any) {
    console.error('Save reservation error:', error)
    const errorMessage = error.response?.data?.message || 
                        error.response?.data?.title || 
                        error.message || 
                        '儲存預約失敗'
    
    toast.add({
      severity: 'error',
      summary: '儲存失敗',
      detail: errorMessage,
      life: 5000
    })
  } finally {
    submitting.value = false
  }
}

// Watchers
watch(() => props.visible, (visible) => {
  if (visible && props.reservation) {
    Object.assign(form.value, props.reservation)
  } else if (visible) {
    // 重置表單
    Object.assign(form.value, {
      petId: null,
      reservationDate: null,
      reservationTime: null,
      serviceIds: [],
          subscriptionId: null,
      status: 'PENDING',
      memo: ''
    })
  }
  errors.value = {}
})

// Lifecycle
onMounted(() => {
  loadPets()
  loadServices()
})
</script>

<style scoped>
.reservation-form {
  padding: 1rem 0;
}

.form-section {
  margin-bottom: 2rem;
}

.form-section h4 {
  margin: 0 0 1rem 0;
  color: var(--p-text-color);
  border-bottom: 1px solid var(--p-surface-border);
  padding-bottom: 0.5rem;
}

.field {
  margin-bottom: 1rem;
}

.required::after {
  content: ' *';
  color: var(--p-red-500);
}


.cost-summary {
  background: var(--p-surface-50);
  padding: 1rem;
  border-radius: var(--p-border-radius);
  border: 1px solid var(--p-surface-border);
}

.cost-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 0.5rem;
}

.cost-row.total {
  border-top: 1px solid var(--p-surface-border);
  padding-top: 0.5rem;
  font-weight: 600;
  font-size: 1.1rem;
}

.discount {
  color: var(--p-red-500);
}

.cost-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.cost-header h4 {
  margin: 0;
}

.subscription-note {
  margin-top: 1rem;
}

.service-loading {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 1rem;
  color: var(--p-text-color-secondary);
}

.service-checkboxes {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 0.75rem;
  margin-top: 0.5rem;
}

.service-checkbox-item {
  display: flex;
  align-items: flex-start;
  gap: 0.5rem;
  padding: 0.75rem;
  border: 1px solid var(--p-surface-border);
  border-radius: var(--p-border-radius);
  background: var(--p-surface-0);
  transition: all 0.2s ease;
  cursor: pointer;
}

.service-checkbox-item:hover {
  background: var(--p-surface-50);
  border-color: var(--p-primary-color);
}

.service-checkbox-item:has(:checked) {
  background: var(--p-primary-50);
  border-color: var(--p-primary-color);
}

.service-label {
  flex: 1;
  cursor: pointer;
  margin: 0;
}

.service-info {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.service-name {
  font-weight: 500;
  color: var(--p-text-color);
}

.price-info {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 0.125rem;
}

.service-price {
  font-size: 0.875rem;
  color: var(--p-primary-color);
  font-weight: 600;
}

.service-price.custom-price {
  color: var(--p-orange-500);
}

.free-price {
  font-size: 0.875rem;
  color: var(--p-green-600);
  font-weight: 600;
}

.custom-label {
  font-size: 0.75rem;
  color: var(--p-text-color-secondary);
  font-weight: 400;
}

.dialog-footer {
  display: flex;
  gap: 0.5rem;
  justify-content: flex-end;
}

.grid {
  display: grid;
  grid-template-columns: repeat(12, 1fr);
  gap: 1rem;
}

.col-12 {
  grid-column: span 12;
}

.md\:col-6 {
  grid-column: span 6;
}

@media (max-width: 768px) {
  .md\:col-6 {
    grid-column: span 12;
  }

  .service-checkboxes {
    grid-template-columns: 1fr;
  }
}
</style>
