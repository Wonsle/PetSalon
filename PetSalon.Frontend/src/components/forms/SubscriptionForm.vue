<template>
  <Dialog
    :visible="visible"
    :header="isEdit ? '編輯包月方案' : '新增包月方案'"
    :style="{ width: '700px' }"
    :modal="true"
    @update:visible="$emit('close')"
  >
    <form @submit.prevent="handleSubmit">
      <!-- 選擇寵物 -->
      <div class="field">
        <label class="label">選擇寵物 *</label>
        <PetSelector
          v-model="form.petId"
          :show-selected-info="true"
          :show-price="true"
          :invalid="!!petError"
          :disabled="isEdit"
          @pet-selected="(pet: Pet | Pet[]) => handlePetSelected(pet as Pet)"
        />
        <small v-if="petError" class="p-error">{{ petError }}</small>
        <div v-if="isEdit" class="edit-note">
          編輯模式下無法變更寵物，如需變更請重新建立包月方案
        </div>
      </div>

      <!-- 包月價格顯示 -->
      <div v-if="subscriptionPrice" class="field">
        <div class="price-display">
          <span class="price-label">包月價格:</span>
          <span class="price-amount">NT$ {{ subscriptionPrice.toLocaleString() }}</span>
        </div>
      </div>

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
              :min-date="new Date()"
              @date-select="handleStartDateChange"
              :class="{ 'p-invalid': startDateError }"
            />
            <small v-if="startDateError" class="p-error">{{ startDateError }}</small>
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
              :min-date="startDateModel || new Date()"
              @date-select="handleEndDateChange"
              :class="{ 'p-invalid': endDateError }"
            />
            <small v-if="endDateError" class="p-error">{{ endDateError }}</small>
            <div class="auto-info">
              預設為開始日期後2個月，可自行調整
            </div>
          </div>
        </div>
      </div>

      <!-- 期間資訊 -->
      <div v-if="form.startDate && form.endDate" class="duration-info">
        <Message
          :severity="'info'"
          :closable="false"
        >
          方案期間: {{ getDurationDays() }} 天
        </Message>
      </div>

      <!-- 使用次數限制 -->
      <div class="field">
        <label for="totalTimes" class="label">使用次數限制 *</label>
        <InputNumber
          id="totalTimes"
          v-model="form.totalTimes"
          :min="1"
          :max="50"
          placeholder="請輸入使用次數"
          :class="{ 'p-invalid': totalTimesError }"
          :step="1"
          :allow-empty="false"
          @blur="handleTotalTimesBlur"
        />
        <small v-if="totalTimesError" class="p-error">{{ totalTimesError }}</small>
        <div class="auto-info">
          設定包月期間內可使用的總次數（1-50次）
        </div>
      </div>

      <!-- 備註 -->
      <div class="field">
        <label for="note" class="label">備註</label>
        <Textarea
          id="note"
          v-model="form.note"
          :rows="3"
          placeholder="請輸入方案說明或特殊條件"
        />
      </div>

      <!-- 方案摘要 -->
      <Card v-if="selectedPet" class="summary-card">
        <template #header>
          <span>包月方案摘要</span>
        </template>
        <template #content>
          <div class="summary-content">
            <div class="summary-row">
              <span>寵物名稱:</span>
              <span>{{ selectedPet.petName || selectedPet.name || `寵物 #${selectedPet.petId}` }}</span>
            </div>
            <div class="summary-row">
              <span>包月價格:</span>
              <span class="highlight">NT$ {{ (subscriptionPrice || 0).toLocaleString() }}</span>
            </div>
            <div v-if="form.startDate && form.endDate" class="summary-row">
              <span>服務期間:</span>
              <span>{{ formatDateRange() }}</span>
            </div>
          </div>
        </template>
      </Card>
    </form>

    <template #footer>
      <div class="dialog-footer">
        <Button label="取消" severity="secondary" @click="handleClose" />
        <Button
          :label="isEdit ? '更新' : '建立方案'"
          :loading="submitting"
          @click="handleSubmit"
        />
      </div>
    </template>
  </Dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch, computed, onMounted } from 'vue'
import { useToast } from 'primevue/usetoast'
import type { Subscription, SubscriptionCreateRequest, SubscriptionUpdateRequest } from '@/types/subscription'
import type { Pet } from '@/types/pet'
import { subscriptionApi } from '@/api/subscription'

// 後端 SubscriptionCreateDto 對應的類型
interface BackendSubscriptionCreateDto {
  petId: number
  startDate: string
  endDate: string
  subscriptionDate: string
  totalUsageLimit: number
  subscriptionPrice: number
  notes: string
}
import PetSelector from '@/components/common/PetSelector.vue'
import { calculateEndDate, generateSubscriptionName } from '@/composables/usePetSelector'
import { petServicePriceApi } from '@/api/petServicePrice'

interface Props {
  visible: boolean
  subscription?: Subscription | null
}

interface Emits {
  (e: 'close'): void
  (e: 'success'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

// Refs
const submitting = ref(false)
const toast = useToast()
const subscriptionPrice = ref<number | null>(null) // 訂閱價格

// Date models for Calendar components (PrimeVue expects Date objects)
const startDateModel = ref<Date | null>(null)
const endDateModel = ref<Date | null>(null)

// Data
const selectedPet = ref<Pet | null>(null)

// Computed
const isEdit = computed(() => !!props.subscription)

const generatedName = computed(() => {
  if (!selectedPet.value) return ''
  return generateSubscriptionName(selectedPet.value, 2)
})

// Form data
const form = reactive<SubscriptionCreateRequest>({
  name: '',
  petId: 0,
  totalTimes: 1, // 預設為1次使用限制
  totalAmount: 0,
  startDate: '',
  endDate: '',
  serviceContent: '',
  paidAmount: 0,
  note: ''
})

// Validation errors
const petError = ref('')
const startDateError = ref('')
const endDateError = ref('')
const totalTimesError = ref('')

// Methods
const loadSubscriptionPrice = async (petId: number) => {
  try {
    subscriptionPrice.value = await petServicePriceApi.getSubscriptionPrice(petId)
    if (subscriptionPrice.value) {
      form.totalAmount = subscriptionPrice.value
    }
  } catch (error) {
    console.error('載入訂閱價格失敗:', error)
    toast.add({
      severity: 'warn',
      summary: '警告',
      detail: '無法載入訂閱價格，請手動輸入',
      life: 3000
    })
  }
}

const handlePetSelected = async (pet: Pet) => {
  selectedPet.value = pet
  form.petId = pet.petId

  // Auto-generate subscription name
  form.name = generatedName.value

  // 載入訂閱價格並設定為總額
  await loadSubscriptionPrice(pet.petId)
}


const formatDateRange = () => {
  if (!form.startDate || !form.endDate) return ''
  const start = new Date(form.startDate).toLocaleDateString('zh-TW')
  const end = new Date(form.endDate).toLocaleDateString('zh-TW')
  return `${start} ~ ${end}`
}


const handleStartDateChange = () => {
  if (startDateModel.value) {
    form.startDate = startDateModel.value.toISOString().split('T')[0]

    // 如果結束日期還未設定，自動設定為2個月後
    if (!endDateModel.value) {
      const end = calculateEndDate(startDateModel.value, 2)
      endDateModel.value = end
      form.endDate = end.toISOString().split('T')[0]
    }
  }
}

const handleEndDateChange = () => {
  if (endDateModel.value) {
    form.endDate = endDateModel.value.toISOString().split('T')[0]
  }
}

const handleTotalTimesBlur = () => {
  // 確保 totalTimes 至少為 1
  if (!form.totalTimes || form.totalTimes < 1) {
    form.totalTimes = 1
  }
}

const getDurationDays = () => {
  if (!form.startDate || !form.endDate) return 0
  const start = new Date(form.startDate)
  const end = new Date(form.endDate)
  return Math.ceil((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24))
}

const validateForm = () => {
  let isValid = true

  // Reset errors
  petError.value = ''
  startDateError.value = ''
  endDateError.value = ''
  totalTimesError.value = ''

  if (!form.petId) {
    petError.value = '請選擇寵物'
    isValid = false
  }

  if (!subscriptionPrice.value || subscriptionPrice.value <= 0) {
    petError.value = '所選寵物未設定包月價格，請先設定訂閱服務價格'
    isValid = false
  }

  if (!form.startDate) {
    startDateError.value = '請選擇開始日期'
    isValid = false
  }

  if (!form.endDate) {
    endDateError.value = '請選擇結束日期'
    isValid = false
  }

  if (form.startDate && form.endDate && new Date(form.startDate) >= new Date(form.endDate)) {
    endDateError.value = '結束日期必須大於開始日期'
    isValid = false
  }

  if (!form.totalTimes || form.totalTimes < 1) {
    totalTimesError.value = '使用次數必須至少為1次'
    isValid = false
  }

  if (form.totalTimes > 50) {
    totalTimesError.value = '使用次數不能超過50次'
    isValid = false
  }

  return isValid
}

const handleSubmit = async () => {
  if (!validateForm()) return

  try {
    submitting.value = true

    // 將前端欄位對應到後端DTO格式
    const requestData: BackendSubscriptionCreateDto = {
      petId: form.petId,
      startDate: new Date(form.startDate).toISOString().split('T')[0],
      endDate: new Date(form.endDate).toISOString().split('T')[0],
      subscriptionDate: new Date().toISOString().split('T')[0],
      totalUsageLimit: form.totalTimes, // 直接使用 form.totalTimes
      subscriptionPrice: form.totalAmount,
      notes: form.note || '' // 後端期望 Notes 欄位
    }

    if (isEdit.value && props.subscription) {
      const updateData: SubscriptionUpdateRequest = {
        ...requestData,
        id: props.subscription.subscriptionId
      }
      await subscriptionApi.updateSubscription(updateData)
      toast.add({
        severity: 'success',
        summary: '成功',
        detail: '更新成功',
        life: 3000
      })
    } else {
      await subscriptionApi.createSubscription(requestData as any) // 暫時使用 any 類型轉換
      toast.add({
        severity: 'success',
        summary: '成功',
        detail: '建立成功',
        life: 3000
      })
    }

    emit('success')
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: error.response?.data?.message || '操作失敗',
      life: 3000
    })
  } finally {
    submitting.value = false
  }
}

const handleClose = () => {
  emit('close')
}

const resetForm = () => {
  Object.assign(form, {
    name: '',
    petId: 0,
    totalTimes: 1,
    totalAmount: 0,
    startDate: '',
    endDate: '',
    serviceContent: '',
    paidAmount: 0,
    note: ''
  })
  selectedPet.value = null
  startDateModel.value = null
  endDateModel.value = null

  // Reset validation errors
  petError.value = ''
  startDateError.value = ''
  endDateError.value = ''
  totalTimesError.value = ''
}

// Watch for pet changes to update name and amount
watch(() => selectedPet.value, async () => {
  if (selectedPet.value) {
    form.name = generatedName.value
    await loadSubscriptionPrice(selectedPet.value.petId)
  }
})

// Watch for subscription changes
watch(() => props.subscription, async (newSubscription) => {
  if (newSubscription) {
    Object.assign(form, {
      name: newSubscription.name || generateSubscriptionName({ petName: '寵物' } as Pet, 2),
      petId: newSubscription.petId,
      totalTimes: newSubscription.totalTimes || 1,
      totalAmount: newSubscription.totalAmount || 0,
      startDate: newSubscription.startDate,
      endDate: newSubscription.endDate,
      note: newSubscription.note || newSubscription.notes || ''
    })

    // Set date models
    startDateModel.value = new Date(newSubscription.startDate)
    endDateModel.value = new Date(newSubscription.endDate)

    // Load pet info if needed
    if (newSubscription.petId && !selectedPet.value) {
      // This would be handled by PetSelector component
    }
  } else {
    resetForm()
  }
}, { immediate: true })

// Watch date models to sync with form
watch(() => startDateModel.value, (date) => {
  if (date) {
    form.startDate = date.toISOString().split('T')[0]
  }
})

watch(() => endDateModel.value, (date) => {
  if (date) {
    form.endDate = date.toISOString().split('T')[0]
  }
})

// Watch for dialog visibility
watch(() => props.visible, (visible) => {
  if (!visible) {
    resetForm()
  } else {
    // Set default dates if creating new subscription
    if (!isEdit.value) {
      const today = new Date()
      startDateModel.value = today
      form.startDate = today.toISOString().split('T')[0]

      const endDate = calculateEndDate(today, 2)
      endDateModel.value = endDate
      form.endDate = endDate.toISOString().split('T')[0]
    }
  }
})
</script>

<style scoped>
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

.duration-info {
  margin: 16px 0;
}

.pet-summary {
  display: flex;
  align-items: center;
  gap: 16px;
}

.pet-avatar {
  flex-shrink: 0;
}

.pet-photo {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  object-fit: cover;
  border: 2px solid var(--p-surface-border);
}

.pet-photo-placeholder {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  background: var(--p-surface-100);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
  border: 2px solid var(--p-surface-border);
}

.pet-info p {
  margin: 4px 0;
  color: var(--p-text-color-secondary);
}

.pet-info strong {
  color: var(--p-text-color);
}

.price-info {
  font-size: 0.875rem;
  color: var(--p-blue-600);
  margin-top: 0.5rem;
  font-weight: 500;
}

.calculation-info {
  font-size: 0.875rem;
  color: var(--p-text-color-secondary);
  margin-top: 0.5rem;
}

.auto-info {
  font-size: 0.875rem;
  color: var(--p-text-color-secondary);
  margin-top: 0.5rem;
}

.calculated-field {
  background-color: var(--p-surface-100);
}

.duration-info {
  margin: 16px 0;
}

.summary-card {
  margin-top: 16px;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
  padding: 4px 0;
}

.summary-row:last-child {
  margin-bottom: 0;
  padding-top: 8px;
  border-top: 1px solid var(--p-surface-border);
}

.highlight {
  font-weight: 600;
  color: var(--p-primary-color);
}

.unpaid {
  color: var(--p-red-500);
  font-weight: 500;
}

.discount {
  color: var(--p-green-500);
  font-weight: 500;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.p-invalid {
  border-color: var(--p-red-500);
}

.p-error {
  color: var(--p-red-500);
  font-size: 0.875rem;
}

.price-display {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem;
  background: var(--p-blue-50);
  border: 1px solid var(--p-blue-200);
  border-radius: var(--p-border-radius);
  margin-bottom: 0.5rem;
}

.price-label {
  font-weight: 600;
  color: var(--p-blue-700);
}

.price-amount {
  font-weight: 700;
  font-size: 1.1rem;
  color: var(--p-blue-800);
}

.edit-note {
  font-size: 0.875rem;
  color: var(--p-orange-600);
  margin-top: 0.5rem;
  font-style: italic;
}
</style>