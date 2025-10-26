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
                :stepMinute="5"
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
        <small v-if="errors.serviceIds" class="p-error">{{ errors.serviceIds }}</small>
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
              <div class="checkbox-container">
                <Checkbox
                  :id="`service-${service.serviceId}`"
                  v-model="form.serviceIds"
                  :value="service.serviceId"
                  @change="onServiceToggle(service.serviceId)"
                />
              </div>
              <div class="service-content">
                <label
                  :for="`service-${service.serviceId}`"
                  class="service-label"
                >
                  <span class="service-name">{{ service.serviceName }}</span>
                </label>
                <div class="price-input-container">
                  <div class="price-input-wrapper">
                    <span class="price-currency">NT$</span>
                    <InputNumber
                      v-model="servicePrices[service.serviceId]"
                      :min="0"
                      :max="999999"
                      mode="decimal"
                      :use-grouping="false"
                      placeholder="請輸入金額"
                      class="price-input"
                      :class="{ 'p-invalid': errors.servicePrices?.[service.serviceId] }"
                      @input="onPriceChange(service.serviceId)"
                      @blur="calculateCost"
                      show-buttons
                      :button-layout="'horizontal'"
                      :step="50"
                    />
                  </div>
                  <small v-if="errors.servicePrices?.[service.serviceId]" class="p-error">
                    {{ errors.servicePrices[service.serviceId] }}
                  </small>
                  <small v-else-if="service.price > 0" class="default-price-hint">
                    預設: NT$ {{ service.price.toLocaleString() }}
                  </small>
                </div>
              </div>
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
import { petServicePriceApi } from '@/api/petServicePrice'
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
  servicePrices?: Record<number, string>
  [key: string]: string | Record<number, string> | undefined
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
  serviceType?: string
  description?: string
  estimatedDuration?: number
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

// 服務互斥規則：定義哪些服務類型是互斥的
// BATH (洗澡) 和 GROOM (美容) 互斥，因為美容已包含洗澡
const MUTUALLY_EXCLUSIVE_SERVICES: Record<string, string[]> = {
  'BATH': ['GROOM'],  // 洗澡 與 美容 互斥
  'GROOM': ['BATH']   // 美容 與 洗澡 互斥
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

const servicePrices = ref<Record<number, number>>({})

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

/**
 * 將時間無條件進位到最近的5分鐘單位
 */
const roundTimeToNext5Minutes = () => {
  const now = new Date()
  const minutes = now.getMinutes()
  const remainder = minutes % 5

  // 如果已經是5的倍數，直接使用；否則無條件進位
  const roundedMinutes = remainder === 0 ? minutes : minutes + (5 - remainder)

  // 處理分鐘數超過60的情況（跨小時）
  if (roundedMinutes >= 60) {
    now.setHours(now.getHours() + 1)
    now.setMinutes(0)
  } else {
    now.setMinutes(roundedMinutes)
  }

  // 清除秒和毫秒
  now.setSeconds(0)
  now.setMilliseconds(0)

  return now
}

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
    
    // 初始化服務價格
    services.value.forEach(service => {
      servicePrices.value[service.serviceId] = service.price
    })
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
      
      // 初始化備用服務價格
      services.value.forEach(service => {
        servicePrices.value[service.serviceId] = service.price
      })
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
    // 重置服務價格為預設值
    await loadServices()
    return
  }

  try {
    // 並行載入包月方案和寵物服務價格
    const [subscriptions, petPrices] = await Promise.all([
      subscriptionApi.getSubscriptionsByPet(form.value.petId),
      petServicePriceApi.getPetServicePrices(form.value.petId)
    ])

    console.log('Raw subscriptions:', subscriptions)
    console.log('Pet service prices:', petPrices)

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

    // 更新服務價格：使用寵物的自訂價格覆蓋預設價格
    petPrices.forEach(priceConfig => {
      if (priceConfig.customPrice !== null && priceConfig.customPrice !== undefined) {
        servicePrices.value[priceConfig.serviceId] = priceConfig.customPrice
        console.log(`Updated price for service ${priceConfig.serviceId}: ${priceConfig.customPrice}`)
      }
    })

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
  // 選擇包月時清除價格驗證錯誤
  if (form.value.subscriptionId) {
    if (errors.value.servicePrices) {
      delete errors.value.servicePrices
    }
    console.log('選擇包月方案，已清除價格驗證錯誤')
  }
  calculateCost()
}

const onServiceToggle = (serviceId: number) => {
  const service = services.value.find(s => s.serviceId === serviceId)

  // 檢查是否選中了服務
  if (service && form.value.serviceIds.includes(serviceId)) {
    // 檢查此服務是否有互斥規則
    const exclusiveTypes = MUTUALLY_EXCLUSIVE_SERVICES[service.serviceType]

    if (exclusiveTypes && exclusiveTypes.length > 0) {
      // 找出當前已選服務中，與此服務互斥的項目
      const conflictingServices = services.value.filter(s =>
        form.value.serviceIds.includes(s.serviceId) &&
        exclusiveTypes.includes(s.serviceType) &&
        s.serviceId !== serviceId
      )

      // 移除互斥的服務
      if (conflictingServices.length > 0) {
        conflictingServices.forEach(conflictService => {
          // 從選擇的服務列表中移除
          const index = form.value.serviceIds.indexOf(conflictService.serviceId)
          if (index > -1) {
            form.value.serviceIds.splice(index, 1)
          }

          // 清除互斥服務的價格
          delete servicePrices.value[conflictService.serviceId]
        })

        // 顯示 Toast 提示訊息
        const conflictNames = conflictingServices.map(s => s.serviceName).join('、')
        toast.add({
          severity: 'info',
          summary: '服務互斥提醒',
          detail: `已選擇${service.serviceName}服務，已自動取消${conflictNames}（${service.serviceName}已包含相關服務）`,
          life: 4000
        })
      }
    }

    // 如果選中服務，設定預設價格
    if (servicePrices.value[serviceId] === undefined || servicePrices.value[serviceId] === 0) {
      servicePrices.value[serviceId] = service.price
    }
  }

  calculateCost()
}

const onPriceChange = (serviceId: number) => {
  // 價格變更時清除該服務的錯誤
  if (errors.value.servicePrices && errors.value.servicePrices[serviceId]) {
    delete errors.value.servicePrices[serviceId]
    // 如果沒有其他錯誤，清除整個servicePrices錯誤對象
    if (Object.keys(errors.value.servicePrices).length === 0) {
      delete errors.value.servicePrices
    }
  }

  // 即時計算成本，提供更好的用戶體驗
  setTimeout(() => {
    calculateCost()
  }, 300) // 防抖延遲300ms
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
    // 如果 API 失敗，使用自訂價格計算
    const selectedServices = services.value.filter(s => form.value.serviceIds.includes(s.serviceId))
    const serviceTotal = selectedServices.reduce((sum, service) => {
      const customPrice = servicePrices.value[service.serviceId]
      return sum + (customPrice !== undefined ? customPrice : service.price || 0)
    }, 0)
    
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
  const newErrors: ValidationErrors = {}

  // 基本驗證
  if (!form.value.petId) newErrors.petId = '請選擇寵物'
  if (!form.value.reservationDate) newErrors.reservationDate = '請選擇預約日期'
  if (!form.value.reservationTime) newErrors.reservationTime = '請選擇預約時間'
  if (form.value.serviceIds.length === 0) newErrors.serviceIds = '請至少選擇一項服務'

  // 服務價格驗證（當沒有包月時）
  if (!form.value.subscriptionId && form.value.serviceIds.length > 0) {
    const servicePriceErrors: Record<number, string> = {}

    form.value.serviceIds.forEach(serviceId => {
      const price = servicePrices.value[serviceId]
      const service = services.value.find(s => s.serviceId === serviceId)

      if (price === undefined || price === null || price <= 0) {
        servicePriceErrors[serviceId] = `${service?.serviceName || '此服務'} 的金額必須大於 0`
      }
    })

    if (Object.keys(servicePriceErrors).length > 0) {
      newErrors.servicePrices = servicePriceErrors
    }
  }

  errors.value = newErrors
  return Object.keys(newErrors).length === 0
}

const handleSubmit = async () => {
  if (!validateForm()) {
    console.error('表單驗證失敗:', errors.value)
    console.error('表單數據:', {
      petId: form.value.petId,
      reservationDate: form.value.reservationDate,
      reservationTime: form.value.reservationTime,
      serviceIds: form.value.serviceIds,
      subscriptionId: form.value.subscriptionId,
      useSubscription: !!form.value.subscriptionId
    })
    console.error('服務價格:', servicePrices.value)
    console.error('已選擇的服務:', services.value.filter(s => form.value.serviceIds.includes(s.serviceId)))
    return
  }

  submitting.value = true
  try {
    // Convert Date objects to proper formats for backend API
    const reservationData = {
      petId: form.value.petId!,
      reservationDate: dayjs(form.value.reservationDate).format('YYYY-MM-DD'),
      reservationTime: dayjs(form.value.reservationTime).format('HH:mm:ss'),
      serviceIds: form.value.serviceIds,
      addonIds: [], // Backend requires this field even if empty
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
      reservationDate: new Date(),              // 當天日期
      reservationTime: roundTimeToNext5Minutes(), // 當前時間（進位到5分鐘）
      serviceIds: [],
      subscriptionId: null,
      status: 'PENDING',
      memo: ''
    })
    
    // 重置服務價格為預設值
    services.value.forEach(service => {
      servicePrices.value[service.serviceId] = service.price
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
  gap: 1rem;
  padding: 1.25rem;
  border: 1px solid var(--p-surface-border);
  border-radius: var(--p-border-radius);
  background: var(--p-surface-0);
  transition: all 0.2s ease;
}

.checkbox-container {
  display: flex;
  align-items: center;
  flex-shrink: 0;
  margin-top: 0.125rem;
  padding-right: 0.5rem;
}

/* 增強Checkbox視覺效果 */
.checkbox-container :deep(.p-checkbox) {
  transform: scale(1.3);
}

.checkbox-container :deep(.p-checkbox-box) {
  border-width: 2px;
  transition: all 0.2s ease;
}

.checkbox-container :deep(.p-checkbox-box:hover) {
  box-shadow: 0 0 0 2px var(--p-primary-50);
}

.checkbox-container :deep(.p-checkbox-box.p-highlight) {
  box-shadow: 0 0 0 3px var(--p-primary-100);
}

.service-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.service-checkbox-item:hover {
  background: var(--p-surface-50);
  border-color: var(--p-primary-color);
}

.service-checkbox-item:has(:checked) {
  background: linear-gradient(135deg, var(--p-primary-50) 0%, var(--p-primary-100) 100%);
  border: 2px solid var(--p-primary-color);
  box-shadow: 0 2px 8px rgba(0, 123, 255, 0.15);
}

.service-checkbox-item:has(:checked) .service-name {
  color: var(--p-primary-color);
  font-weight: 600;
}

.service-label {
  cursor: pointer;
  margin: 0;
}

.service-name {
  font-weight: 500;
  color: var(--p-text-color);
  font-size: 0.95rem;
}

.price-input-container {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.price-input-wrapper {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.price-currency {
  font-size: 0.875rem;
  color: var(--p-text-color-secondary);
  font-weight: 500;
}

.price-input {
  width: 140px;
  font-weight: 500;
}

.price-input :deep(.p-inputnumber-input) {
  text-align: center;
  font-weight: 500;
}

.price-input :deep(.p-inputnumber-button) {
  width: 2rem;
}

.price-input :deep(.p-inputnumber-button-up),
.price-input :deep(.p-inputnumber-button-down) {
  background: var(--p-primary-50);
  border-color: var(--p-primary-200);
}

.price-input :deep(.p-inputnumber-button-up):hover,
.price-input :deep(.p-inputnumber-button-down):hover {
  background: var(--p-primary-100);
  border-color: var(--p-primary-300);
}

.default-price-hint {
  color: var(--p-text-color-secondary);
  font-size: 0.75rem;
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
