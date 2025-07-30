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
      <div class="form-section" v-if="availableSubscriptions.length > 0">
        <h4>包月服務</h4>
        <div class="subscription-section">
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
                      剩餘: {{ slotProps.option.remainingUsage }}次
                      <Tag v-if="slotProps.option.isExpiringSoon" value="即將到期" severity="warning" size="small" />
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

      <!-- 服務項目 -->
      <div class="form-section">
        <h4>服務項目</h4>
        <div class="field">
          <label for="serviceIds">選擇服務</label>
          <MultiSelect
            id="serviceIds"
            v-model="form.serviceIds"
            :options="services"
            option-label="serviceName"
            option-value="serviceId"
            placeholder="選擇服務項目"
            :loading="loadingServices"
            @change="calculateCost"
          />
        </div>

        <div class="field">
          <label for="addonIds">附加服務</label>
          <MultiSelect
            id="addonIds"
            v-model="form.addonIds"
            :options="addons"
            option-label="addonName"
            option-value="addonId"
            placeholder="選擇附加服務"
            :loading="loadingAddons"
            @change="calculateCost"
          />
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
          <label for="memo">備註</label>
          <Textarea
            id="memo"
            v-model="form.memo"
            rows="3"
            placeholder="預約備註..."
          />
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
  usedCount: number
  reservedCount: number
  remainingUsage: number
  endDate: string
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
    // TODO: 調用 API 載入寵物列表
    pets.value = []
  } catch (error) {
    console.error('載入寵物列表失敗:', error)
  } finally {
    loadingPets.value = false
  }
}

const loadServices = async () => {
  loadingServices.value = true
  try {
    // TODO: 調用 API 載入服務項目
    services.value = []
  } catch (error) {
    console.error('載入服務項目失敗:', error)
  } finally {
    loadingServices.value = false
  }
}

const loadAddons = async () => {
  loadingAddons.value = true
  try {
    // TODO: 調用 API 載入附加服務
    addons.value = []
  } catch (error) {
    console.error('載入附加服務失敗:', error)
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
    // TODO: 調用 API 載入該寵物的可用包月方案
    availableSubscriptions.value = []
  } catch (error) {
    console.error('載入包月方案失敗:', error)
  }
}

const onSubscriptionToggle = () => {
  if (!form.value.useSubscription) {
    form.value.subscriptionId = null
    calculateCost()
  }
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
    // TODO: 調用 API 計算費用
    costCalculation.value = {
      serviceTotal: 0,
      addonTotal: 0,
      discount: 0,
      totalAmount: 0
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
    // TODO: 調用 API 儲存預約

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
      detail: error.message || '儲存預約失敗',
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
}
</style>
