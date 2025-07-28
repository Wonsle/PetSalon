<template>
  <Dialog
    :visible="visible"
    :header="isEdit ? '編輯包月方案' : '新增包月方案'"
    :style="{ width: '700px' }"
    :modal="true"
    @update:visible="$emit('close')"
  >
    <form @submit.prevent="handleSubmit">
      <!-- 方案名稱 -->
      <div class="field">
        <label for="name" class="label">方案名稱 *</label>
        <InputText
          id="name"
          v-model="form.name"
          placeholder="請輸入方案名稱"
          :class="{ 'p-invalid': nameError }"
        />
        <small v-if="nameError" class="p-error">{{ nameError }}</small>
      </div>

      <!-- 選擇寵物 -->
      <div class="field">
        <label for="pet" class="label">選擇寵物 *</label>
        <Select
          id="pet"
          v-model="form.petId"
          :options="pets"
          option-label="displayName"
          option-value="petId"
          placeholder="請選擇寵物"
          filter
          :loading="petLoading"
          @change="(event) => handlePetChange(event.value)"
          :class="{ 'p-invalid': petError }"
        />
        <small v-if="petError" class="p-error">{{ petError }}</small>
      </div>

      <!-- 選中的寵物資訊 -->
      <Card v-if="selectedPet" class="pet-info-card">
        <template #header>
          <span>寵物資訊</span>
        </template>
        <template #content>
          <div class="pet-details">
            <div class="pet-summary">
              <div class="pet-avatar">
                <img
                  v-if="selectedPet.photoUrl"
                  :src="selectedPet.photoUrl"
                  :alt="selectedPet.petName"
                  class="pet-photo"
                />
                <div v-else class="pet-photo-placeholder">
                  🐾
                </div>
              </div>
              <div class="pet-info">
                <p><strong>寵物:</strong> {{ selectedPet.petName }}</p>
                <p><strong>品種:</strong> {{ selectedPet.breed }}</p>
                <p><strong>主人:</strong> {{ selectedPet.ownerName || '未設定' }}</p>
                <p><strong>電話:</strong> {{ selectedPet.contactPhone || '未設定' }}</p>
              </div>
            </div>
          </div>
        </template>
      </Card>

      <!-- 服務內容 -->
      <div class="field">
        <label for="service" class="label">服務內容 *</label>
        <Select
          id="service"
          v-model="form.serviceContent"
          :options="serviceOptions"
          option-label="label"
          option-value="value"
          placeholder="請選擇服務內容"
          :class="{ 'p-invalid': serviceError }"
        />
        <small v-if="serviceError" class="p-error">{{ serviceError }}</small>
      </div>

      <!-- 服務次數和單次價格 -->
      <div class="grid">
        <div class="col-6">
          <div class="field">
            <label for="times" class="label">服務次數 *</label>
            <InputNumber
              id="times"
              v-model="form.totalTimes"
              :min="1"
              :max="100"
              show-buttons
              @update:model-value="calculateAmount"
              :class="{ 'p-invalid': timesError }"
            />
            <small v-if="timesError" class="p-error">{{ timesError }}</small>
          </div>
        </div>
        <div class="col-6">
          <div class="field">
            <label for="unitPrice" class="label">單次價格</label>
            <InputNumber
              id="unitPrice"
              v-model="unitPrice"
              :min="0"
              mode="currency"
              currency="TWD"
              locale="zh-TW"
              @update:model-value="calculateAmount"
            />
          </div>
        </div>
      </div>

      <!-- 方案總額和已付金額 -->
      <div class="grid">
        <div class="col-6">
          <div class="field">
            <label for="totalAmount" class="label">方案總額 *</label>
            <InputNumber
              id="totalAmount"
              v-model="form.totalAmount"
              :min="0"
              mode="currency"
              currency="TWD"
              locale="zh-TW"
              :class="{ 'p-invalid': amountError }"
            />
            <div class="price-tip">
              建議價格: NT$ {{ suggestedPrice.toLocaleString() }}
            </div>
            <small v-if="amountError" class="p-error">{{ amountError }}</small>
          </div>
        </div>
        <div class="col-6">
          <div class="field">
            <label for="paidAmount" class="label">已付金額</label>
            <InputNumber
              id="paidAmount"
              v-model="form.paidAmount"
              :min="0"
              :max="form.totalAmount"
              mode="currency"
              currency="TWD"
              locale="zh-TW"
            />
          </div>
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
              :min-date="startDateModel || undefined"
              :class="{ 'p-invalid': endDateError }"
            />
            <small v-if="endDateError" class="p-error">{{ endDateError }}</small>
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
      <Card v-if="form.totalTimes && form.totalAmount" class="summary-card">
        <template #header>
          <span>方案摘要</span>
        </template>
        <template #content>
          <div class="summary-content">
            <div class="summary-row">
              <span>服務次數:</span>
              <span>{{ form.totalTimes }} 次</span>
            </div>
            <div class="summary-row">
              <span>單次價格:</span>
              <span>NT$ {{ unitPrice.toLocaleString() }}</span>
            </div>
            <div class="summary-row">
              <span>方案總額:</span>
              <span class="highlight">NT$ {{ form.totalAmount.toLocaleString() }}</span>
            </div>
            <div class="summary-row">
              <span>已付金額:</span>
              <span>NT$ {{ form.paidAmount.toLocaleString() }}</span>
            </div>
            <div class="summary-row">
              <span>未付金額:</span>
              <span class="unpaid">NT$ {{ (form.totalAmount - form.paidAmount).toLocaleString() }}</span>
            </div>
            <div v-if="discountAmount > 0" class="summary-row">
              <span>優惠金額:</span>
              <span class="discount">-NT$ {{ discountAmount.toLocaleString() }}</span>
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
import { petApi } from '@/api/pet'

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
const petLoading = ref(false)
const unitPrice = ref(500) // Default unit price
const toast = useToast()

// Date models for Calendar components (PrimeVue expects Date objects)
const startDateModel = ref<Date | null>(null)
const endDateModel = ref<Date | null>(null)

// Data
const pets = ref<Pet[]>([])
const selectedPet = ref<Pet | null>(null)

// 服務選項
const serviceOptions = [
  { label: '基礎洗澡套餐', value: '基礎洗澡套餐' },
  { label: '精緻美容套餐', value: '精緻美容套餐' },
  { label: '全套護理套餐', value: '全套護理套餐' },
  { label: '造型設計套餐', value: '造型設計套餐' },
  { label: '自訂服務套餐', value: '自訂服務套餐' }
]

// Computed
const isEdit = computed(() => !!props.subscription)

const suggestedPrice = computed(() => {
  const basePrice = unitPrice.value * form.totalTimes
  // Apply bulk discount for more services
  if (form.totalTimes >= 10) {
    return Math.round(basePrice * 0.85) // 15% discount
  } else if (form.totalTimes >= 5) {
    return Math.round(basePrice * 0.9) // 10% discount
  }
  return basePrice
})

const discountAmount = computed(() => {
  const basePrice = unitPrice.value * form.totalTimes
  return Math.max(0, basePrice - form.totalAmount)
})

// Form data
const form = reactive<SubscriptionCreateRequest>({
  name: '',
  petId: 0,
  serviceContent: '',
  totalTimes: 5,
  totalAmount: 0,
  paidAmount: 0,
  startDate: '',
  endDate: '',
  note: ''
})

// Validation errors
const nameError = ref('')
const petError = ref('')
const serviceError = ref('')
const timesError = ref('')
const amountError = ref('')
const startDateError = ref('')
const endDateError = ref('')

// Methods
const searchPets = async (query: string) => {
  if (!query) {
    pets.value = []
    return
  }

  petLoading.value = true
  try {
    const response = await petApi.getPets({ keyword: query, pageSize: 20 })
    pets.value = response.data.map(pet => ({
      ...pet,
      id: pet.petId,
      name: pet.petName,
      breedName: pet.breed,
      ownerName: pet.primaryContact?.name || '未設定',
      contactPhone: pet.primaryContact?.phone || '未設定',
      displayName: `${pet.petName} (${pet.primaryContact?.name || '未設定'})`
    }))
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: '搜尋寵物失敗',
      life: 3000
    })
  } finally {
    petLoading.value = false
  }
}

const handlePetChange = async (petId: number) => {
  if (!petId) {
    selectedPet.value = null
    return
  }

  try {
    const pet = await petApi.getPet(petId)
    selectedPet.value = {
      ...pet,
      id: pet.petId,
      name: pet.petName,
      breedName: pet.breed,
      ownerName: pet.primaryContact?.name || '未設定',
      contactPhone: pet.primaryContact?.phone || '未設定'
    }

    // Auto-generate subscription name
    if (!form.name && selectedPet.value) {
      form.name = `${selectedPet.value.petName} - ${form.serviceContent || '包月方案'}`
    }
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: '載入寵物資訊失敗',
      life: 3000
    })
  }
}

const calculateAmount = () => {
  if (form.totalTimes && unitPrice.value) {
    form.totalAmount = suggestedPrice.value
    // Auto set paid amount to total amount for convenience
    if (form.paidAmount === 0) {
      form.paidAmount = form.totalAmount
    }
  }
}

const handleStartDateChange = () => {
  if (startDateModel.value) {
    form.startDate = startDateModel.value.toISOString().split('T')[0]

    if (!endDateModel.value) {
      // Auto set end date to 3 months later
      const end = new Date(startDateModel.value)
      end.setMonth(end.getMonth() + 3)
      endDateModel.value = end
      form.endDate = end.toISOString().split('T')[0]
    }
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
  nameError.value = ''
  petError.value = ''
  serviceError.value = ''
  timesError.value = ''
  amountError.value = ''
  startDateError.value = ''
  endDateError.value = ''

  if (!form.name) {
    nameError.value = '請輸入方案名稱'
    isValid = false
  }

  if (!form.petId) {
    petError.value = '請選擇寵物'
    isValid = false
  }

  if (!form.serviceContent) {
    serviceError.value = '請選擇服務內容'
    isValid = false
  }

  if (!form.totalTimes || form.totalTimes < 1) {
    timesError.value = '請輸入有效的服務次數'
    isValid = false
  }

  if (!form.totalAmount || form.totalAmount < 1) {
    amountError.value = '請輸入有效的方案總額'
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

  return isValid
}

const handleSubmit = async () => {
  if (!validateForm()) return

  try {
    submitting.value = true

    const requestData = {
      ...form,
      startDate: new Date(form.startDate).toISOString().split('T')[0],
      endDate: new Date(form.endDate).toISOString().split('T')[0]
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
      await subscriptionApi.createSubscription(requestData)
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
    serviceContent: '',
    totalTimes: 5,
    totalAmount: 0,
    paidAmount: 0,
    startDate: '',
    endDate: '',
    note: ''
  })
  selectedPet.value = null
  pets.value = []
  unitPrice.value = 500
}

// Watch for service content changes to update name
watch(() => [form.serviceContent, selectedPet.value], () => {
  if (selectedPet.value && form.serviceContent && !isEdit.value) {
    form.name = `${selectedPet.value.petName} - ${form.serviceContent}`
  }
})

// Watch for subscription changes
watch(() => props.subscription, async (newSubscription) => {
  if (newSubscription) {
    Object.assign(form, {
      name: newSubscription.name || '',
      petId: newSubscription.petId,
      serviceContent: newSubscription.serviceContent || '',
      totalTimes: newSubscription.totalTimes || 1,
      totalAmount: newSubscription.totalAmount || 0,
      paidAmount: newSubscription.paidAmount || 0,
      startDate: newSubscription.startDate,
      endDate: newSubscription.endDate,
      note: newSubscription.note || newSubscription.notes || ''
    })

    // Set date models
    startDateModel.value = new Date(newSubscription.startDate)
    endDateModel.value = new Date(newSubscription.endDate)

    // Calculate unit price from existing data
    if (newSubscription.totalTimes && newSubscription.totalTimes > 0) {
      unitPrice.value = Math.round((newSubscription.totalAmount || 0) / newSubscription.totalTimes)
    }

    // Load pet info
    if (newSubscription.petId) {
      try {
        const pet = await petApi.getPet(newSubscription.petId)
        selectedPet.value = {
          ...pet,
          id: pet.petId,
          name: pet.petName,
          breedName: pet.breed,
          ownerName: pet.primaryContact?.name || '未設定',
          contactPhone: pet.primaryContact?.phone || '未設定'
        }
        pets.value = [selectedPet.value]
      } catch (error) {
        console.error('載入寵物資訊失敗:', error)
      }
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

      const endDate = new Date(today)
      endDate.setMonth(endDate.getMonth() + 3)
      endDateModel.value = endDate
      form.endDate = endDate.toISOString().split('T')[0]
    }

    // Load initial pets
    searchPets('')
  }
})

// Auto calculate amount when times or unit price changes
watch(() => [form.totalTimes, unitPrice.value], () => {
  calculateAmount()
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

.col-6 {
  /* Grid item styling handled by parent grid */
}

.pet-info-card {
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

.price-tip {
  font-size: 12px;
  color: var(--p-text-color-secondary);
  margin-top: 4px;
}

.duration-info {
  margin: 16px 0;
}

.summary-card {
  margin-top: 16px;
}

.summary-content {
  space-y: 8px;
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
</style>