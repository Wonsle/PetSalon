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

      <!-- 包月服務選擇 -->
      <div class="form-section">
        <h4>包月服務</h4>
        <div class="subscription-section">
          <!-- 未選擇寵物的提示 -->
          <div v-if="!form.petId" class="no-subscription-notice">
            <Message severity="warn" :closable="false">
              請先選擇寵物以查看可用的包月方案
            </Message>
          </div>

          <!-- 沒有可用包月方案的提示 -->
          <div v-else-if="availableSubscriptions.length === 0" class="no-subscription-notice">
            <Message severity="info" :closable="false">
              該寵物目前沒有可用的包月方案
            </Message>
          </div>

          <!-- 有可用包月方案時顯示選項 -->
          <div v-else-if="availableSubscriptions.length > 0">
            <div class="field">
              <div class="flex align-items-center">
                <Checkbox
                  id="useSubscription"
                  v-model="form.useSubscription"
                  binary
                  @change="onSubscriptionToggle"
                />
                <label for="useSubscription" class="ml-2">使用包月服務</label>
              </div>
            </div>

            <!-- 始終顯示可用的包月方案列表 -->
            <div class="available-subscriptions-info">
              <div class="field">
                <label>可用包月方案</label>
                <div class="subscription-list">
                  <div
                    v-for="sub in availableSubscriptions"
                    :key="sub.subscriptionId"
                    class="subscription-item"
                    :class="{ 'selected': form.subscriptionId === sub.subscriptionId }"
                    @click="selectSubscription(sub)"
                  >
                    <div class="subscription-header">
                      <strong>{{ sub.subscriptionType }}</strong>
                      <div class="subscription-tags">
                        <Tag v-if="sub.isExpiringSoon" value="即將到期" severity="warning" size="small" />
                        <Tag v-if="sub.remainingUsage <= 3" value="次數不足" severity="danger" size="small" />
                      </div>
                    </div>
                    <div class="usage-info">
                      <div class="usage-numbers">
                        <span class="used">已用: {{ sub.usedCount }}次</span>
                        <span class="reserved">預留: {{ sub.reservedCount }}次</span>
                        <span class="remaining">剩餘: {{ sub.remainingUsage }}次</span>
                      </div>
                      <ProgressBar
                        :value="getUsagePercentage(sub)"
                        :show-value="false"
                        style="height: 6px; margin: 4px 0;"
                      />
                    </div>
                    <div class="expiry-info">
                      有效期至: {{ formatDate(sub.endDate) }}
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div v-if="form.useSubscription" class="subscription-options">
              <div class="field">
              <label for="subscriptionId">選擇包月方案</label>
              <Select
                id="subscriptionId"
                v-model="form.subscriptionId"
                :options="availableSubscriptions"
                option-label="displayName"
                option-value="subscriptionId"
                placeholder="選擇包月方案"
                @change="onSubscriptionSelect"
              >
                <template #option="slotProps">
                  <div class="subscription-option">
                    <div class="subscription-name">{{ slotProps.option.subscriptionType }}</div>
                    <div class="subscription-usage">
                      <div class="usage-numbers">
                        <span class="used">已用: {{ slotProps.option.usedCount }}次</span>
                        <span class="reserved">預留: {{ slotProps.option.reservedCount }}次</span>
                        <span class="remaining">剩餘: {{ slotProps.option.remainingUsage }}次</span>
                      </div>
                      <div class="subscription-tags">
                        <Tag v-if="slotProps.option.isExpiringSoon" value="即將到期" severity="warning" size="small" />
                        <Tag v-if="slotProps.option.remainingUsage <= 3" value="次數不足" severity="danger" size="small" />
                      </div>
                    </div>
                  </div>
                </template>
              </Select>
            </div>

            <div v-if="selectedSubscriptionInfo" class="subscription-info-card">
              <Card>
                <template #content>
                  <div class="subscription-details">
                    <h5>{{ selectedSubscriptionInfo.subscriptionType }}包月方案</h5>
                    <div class="usage-info">
                      <div class="usage-stats">
                        <span>已使用: {{ selectedSubscriptionInfo.usedCount }}次</span>
                        <span>預留: {{ selectedSubscriptionInfo.reservedCount }}次</span>
                        <span>剩餘: {{ selectedSubscriptionInfo.remainingUsage }}次</span>
                      </div>
                      <ProgressBar
                        :value="getUsagePercentage(selectedSubscriptionInfo)"
                        :show-value="false"
                        style="height: 8px; margin: 8px 0;"
                      />
                    </div>
                    <div class="expiry-info">
                      有效期至: {{ formatDate(selectedSubscriptionInfo.endDate) }}
                    </div>
                  </div>
                </template>
              </Card>
            </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 服務項目 -->
      <div class="form-section">
        <h4>服務項目</h4>
        <div class="field">
          <label>選擇服務</label>
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
                  <span v-if="service.price > 0" class="service-price">
                    NT$ {{ service.price.toLocaleString() }}
                  </span>
                </div>
              </label>
            </div>
          </div>
        </div>

        <div class="field">
          <label>附加服務</label>
          <div v-if="loadingAddons" class="service-loading">
            <ProgressSpinner style="width:30px;height:30px" strokeWidth="4" />
            <span>載入附加服務中...</span>
          </div>
          <div v-else class="service-checkboxes">
            <div
              v-for="addon in addons"
              :key="addon.addonId"
              class="service-checkbox-item"
            >
              <Checkbox
                :id="`addon-${addon.addonId}`"
                v-model="form.addonIds"
                :value="addon.addonId"
                @change="calculateCost"
              />
              <label
                :for="`addon-${addon.addonId}`"
                class="service-label"
              >
                <div class="service-info">
                  <span class="service-name">{{ addon.addonName }}</span>
                  <span v-if="addon.price > 0" class="service-price">
                    NT$ {{ addon.price.toLocaleString() }}
                  </span>
                </div>
              </label>
            </div>
          </div>
        </div>
      </div>

      <!-- 費用資訊 -->
      <div class="form-section" v-if="costCalculation">
        <h4>費用資訊</h4>
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
          <div v-if="form.useSubscription" class="subscription-note">
            <Tag icon="pi pi-info-circle" value="使用包月服務，費用將從包月方案扣除" severity="info" />
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
  addonIds: number[]
  useSubscription: boolean
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
}

// Composables
const toast = useToast()

// State
const form = ref<ReservationForm>({
  petId: null,
  reservationDate: null,
  reservationTime: null,
  serviceIds: [],
  addonIds: [],
  useSubscription: false,
  subscriptionId: null,
  status: 'PENDING',
  memo: ''
})

const errors = ref<ValidationErrors>({})
const submitting = ref(false)
const loadingPets = ref(false)
const loadingServices = ref(false)
const loadingAddons = ref(false)

const pets = ref<Pet[]>([])
const services = ref<Service[]>([])
const addons = ref<Addon[]>([])
const availableSubscriptions = ref<Subscription[]>([])
const costCalculation = ref<CostCalculation | null>(null)

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

const selectedSubscriptionInfo = computed(() => {
  if (!form.value.subscriptionId) return null
  return availableSubscriptions.value.find(sub => sub.subscriptionId === form.value.subscriptionId)
})

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
    // 載入系統服務類型代碼
    const response = await commonApi.getSystemCodes('ServiceType')
    services.value = response?.map((item: any) => ({
      serviceId: item.id,
      serviceName: item.name,
      serviceType: item.code,
      price: 0 // 預設價格，實際應從服務價格表取得
    })) || []
  } catch (error) {
    console.error('載入服務項目失敗:', error)
    toast.add({
      severity: 'error',
      summary: '載入失敗',
      detail: '無法載入服務項目',
      life: 3000
    })
  } finally {
    loadingServices.value = false
  }
}

const loadAddons = async () => {
  loadingAddons.value = true
  try {
    // 載入附加服務類型代碼
    const response = await commonApi.getSystemCodes('AddonType')
    addons.value = response?.map((item: any) => ({
      addonId: item.id,
      addonName: item.name,
      addonType: item.code,
      price: 0 // 預設價格，實際應從附加服務價格表取得
    })) || []
  } catch (error) {
    console.error('載入附加服務失敗:', error)
    toast.add({
      severity: 'error',
      summary: '載入失敗',
      detail: '無法載入附加服務',
      life: 3000
    })
  } finally {
    loadingAddons.value = false
  }
}

const onPetChange = async () => {
  if (!form.value.petId) {
    availableSubscriptions.value = []
    return
  }

  try {
    // 載入該寵物的可用包月方案
    const subscriptions = await subscriptionApi.getSubscriptionsByPet(form.value.petId)
    console.log('載入的包月方案原始數據:', subscriptions)

    // 過濾出有效的包月方案並計算必要字段
    availableSubscriptions.value = subscriptions
      .map(sub => {
        // 計算剩餘次數（總次數 - 已使用 - 預留）
        const remainingUsage = Math.max(0, (sub.totalUsageLimit || 0) - (sub.usedCount || 0) - (sub.reservedCount || 0))

        const processed = {
          ...sub,
          remainingUsage,
          displayName: `${sub.subscriptionType} (剩餘: ${remainingUsage}次)`,
          isExpiringSoon: isExpiringSoon(sub.endDate)
        }
        console.log('處理後的包月方案:', processed)
        return processed
      })
      .filter(sub => {
        const now = new Date()
        const endDate = new Date(sub.endDate)
        const isValid = sub.remainingUsage > 0 && endDate > now
        console.log(`包月方案 ${sub.subscriptionId} 有效性檢查:`, {
          remainingUsage: sub.remainingUsage,
          endDate: sub.endDate,
          now: now,
          isValid
        })
        return isValid
      })

    console.log('最終可用的包月方案:', availableSubscriptions.value)

  } catch (error) {
    console.error('載入包月方案失敗:', error)
    toast.add({
      severity: 'warn',
      summary: '載入包月方案失敗',
      detail: '無法載入該寵物的包月方案',
      life: 3000
    })
    availableSubscriptions.value = []
  }
}

const onSubscriptionToggle = () => {
  if (!form.value.useSubscription) {
    form.value.subscriptionId = null
    calculateCost()
  }
}

const selectSubscription = (subscription: Subscription) => {
  form.value.useSubscription = true
  form.value.subscriptionId = subscription.subscriptionId
  calculateCost()
}

const onSubscriptionSelect = () => {
  calculateCost()
}

const calculateCost = async () => {
  if (!form.value.petId || (!form.value.serviceIds.length && !form.value.addonIds.length)) {
    costCalculation.value = null
    return
  }

  try {
    // 計算服務費用
    const selectedServices = services.value.filter(s => form.value.serviceIds.includes(s.serviceId))
    const serviceTotal = selectedServices.reduce((sum, service) => sum + (service.price || 0), 0)

    // 計算附加服務費用
    const selectedAddons = addons.value.filter(a => form.value.addonIds.includes(a.addonId))
    const addonTotal = selectedAddons.reduce((sum, addon) => sum + (addon.price || 0), 0)

    // 包月折扣計算
    let discount = 0
    if (form.value.useSubscription && selectedSubscriptionInfo.value) {
      // 如果使用包月，服務費用可能有折扣
      discount = serviceTotal * 0.1 // 假設10%折扣，實際應根據包月方案設定
    }

    const totalAmount = serviceTotal + addonTotal - discount

    costCalculation.value = {
      serviceTotal,
      addonTotal,
      discount,
      totalAmount: Math.max(0, totalAmount)
    }
  } catch (error) {
    console.error('計算費用失敗:', error)
  }
}

const getUsagePercentage = (subscription: any) => {
  if (!subscription.totalUsageLimit || subscription.totalUsageLimit === 0) return 0
  const used = (subscription.usedCount || 0) + (subscription.reservedCount || 0)
  return Math.min((used / subscription.totalUsageLimit) * 100, 100)
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
    const reservationData = {
      petId: form.value.petId!,
      reservationDate: form.value.reservationDate!,
      reservationTime: form.value.reservationTime!,
      serviceIds: form.value.serviceIds,
      addonIds: form.value.addonIds,
      useSubscription: form.value.useSubscription,
      subscriptionId: form.value.subscriptionId,
      status: form.value.status,
      memo: form.value.memo || ''
    }

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
    toast.add({
      severity: 'error',
      summary: '儲存失敗',
      detail: error.response?.data?.message || error.message || '儲存預約失敗',
      life: 3000
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
      addonIds: [],
      useSubscription: false,
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
  loadAddons()
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

.subscription-section {
  background: var(--p-surface-50);
  padding: 1rem;
  border-radius: var(--p-border-radius);
  border: 1px solid var(--p-surface-border);
}

.subscription-options {
  margin-top: 1rem;
}

.subscription-option {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}

.subscription-name {
  font-weight: 600;
}

.subscription-usage {
  font-size: 0.875rem;
  color: var(--p-text-color-secondary);
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.usage-numbers {
  display: flex;
  gap: 0.75rem;
}

.usage-numbers .used {
  color: var(--p-orange-600);
}

.usage-numbers .reserved {
  color: var(--p-blue-600);
}

.usage-numbers .remaining {
  color: var(--p-green-600);
  font-weight: 600;
}

.subscription-tags {
  display: flex;
  gap: 0.25rem;
}

.no-subscription-notice {
  margin-bottom: 1rem;
}

.available-subscriptions-info {
  margin: 1rem 0;
}

.subscription-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.subscription-item {
  padding: 1rem;
  border: 1px solid var(--p-surface-border);
  border-radius: var(--p-border-radius);
  background: var(--p-surface-0);
  transition: all 0.2s ease;
  cursor: pointer;
}

.subscription-item:hover {
  border-color: var(--p-primary-color);
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.subscription-item.selected {
  border-color: var(--p-primary-color);
  background: var(--p-primary-50);
}

.subscription-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.subscription-header strong {
  font-size: 1.1rem;
  color: var(--p-text-color);
}

.expiry-info {
  font-size: 0.875rem;
  color: var(--p-text-color-secondary);
  margin-top: 0.5rem;
}

.subscription-info-card {
  margin-top: 1rem;
}

.subscription-details h5 {
  margin: 0 0 1rem 0;
  color: var(--p-text-color);
}

.usage-stats {
  display: flex;
  gap: 1rem;
  margin-bottom: 0.5rem;
}

.usage-stats span {
  font-size: 0.875rem;
  color: var(--p-text-color-secondary);
}

.expiry-info {
  margin-top: 1rem;
  font-size: 0.875rem;
  color: var(--p-text-color-secondary);
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

.service-price {
  font-size: 0.875rem;
  color: var(--p-primary-color);
  font-weight: 600;
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
