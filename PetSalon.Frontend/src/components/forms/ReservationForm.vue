<template>
  <Dialog
    :visible="visible"
    :header="isEdit ? '編輯預約' : '新增預約'"
    :style="{ width: '700px' }"
    :modal="true"
    @update:visible="$emit('close')"
  >
    <form @submit.prevent="handleSubmit">
      <!-- 寵物選擇 -->
      <div class="field">
        <label class="label">寵物 *</label>
        <Select
          v-model="form.petId"
          :options="pets"
          option-label="displayName"
          option-value="id"
          filter
          :loading="petLoading"
          @change="(event) => handlePetChange(event.value)"
        />
        <small v-if="errors.petId" class="p-error">{{ errors.petId }}</small>
      </div>
      <!-- 方案選擇 -->
      <div class="field">
        <label class="label">使用方案</label>
        <Select
          v-model="form.subscriptionId"
          :options="availableSubscriptions"
          option-label="name"
          option-value="id"
          placeholder="請選擇方案 (可選)"
          :disabled="!availableSubscriptions.length"
        />
        <div v-if="!form.subscriptionId" class="form-tip">未選擇方案將以單次服務計費</div>
      </div>
      <!-- 預約日期與時間 -->
      <div class="grid">
        <div class="col-6">
          <label class="label">預約日期 *</label>
          <Calendar
            v-model="dateModel"
            date-format="yy/mm/dd"
            :min-date="new Date()"
            @date-select="checkTimeAvailability"
          />
          <small v-if="errors.reserveDate" class="p-error">{{ errors.reserveDate }}</small>
        </div>
        <div class="col-6">
          <label class="label">預約時間 *</label>
          <InputText
            v-model="form.reserveTime"
            placeholder="請輸入時間(如14:00)"
            @blur="checkTimeAvailability"
          />
          <small v-if="errors.reserveTime" class="p-error">{{ errors.reserveTime }}</small>
        </div>
      </div>
      <!-- 服務項目、設計師 -->
      <div class="grid">
        <div class="col-6">
          <label class="label">服務項目 *</label>
          <Select
            v-model="form.serviceType"
            :options="serviceOptions"
            option-label="label"
            option-value="value"
          />
          <small v-if="errors.serviceType" class="p-error">{{ errors.serviceType }}</small>
        </div>
        <div class="col-6">
          <label class="label">指定設計師</label>
          <Select
            v-model="form.designer"
            :options="designerOptions"
            option-label="label"
            option-value="value"
          />
        </div>
      </div>
      <!-- 備註 -->
      <div class="field">
        <label class="label">備註</label>
        <Textarea v-model="form.note" :rows="3" placeholder="請輸入特殊需求或備註" />
      </div>
      <!-- 寵物資訊卡片 -->
      <Card v-if="selectedPet" class="pet-info-card">
        <template #header>
          <span>寵物資訊</span>
        </template>
        <template #content>
          <div class="pet-details">
            <p><strong>寵物名稱:</strong> {{ selectedPet.name }}</p>
            <p><strong>品種:</strong> {{ selectedPet.breedName }}</p>
            <p><strong>年齡:</strong> {{ getPetAge(selectedPet.birthDay) }} 歲</p>
            <p><strong>主人:</strong> {{ selectedPet.ownerName }}</p>
            <p><strong>聯絡電話:</strong> {{ selectedPet.contactPhone }}</p>
          </div>
        </template>
      </Card>
      <!-- 時段可用性提示 -->
      <Message v-if="availabilityMessage" :severity="isTimeAvailable ? 'success' : 'warn'" :closable="false">
        {{ availabilityMessage }}
      </Message>
      <!-- 按鈕 -->
      <div class="dialog-footer">
        <Button label="取消" severity="secondary" @click="handleClose" />
        <Button :label="isEdit ? '更新' : '預約'" :loading="submitting" :disabled="!isTimeAvailable && !!availabilityMessage" type="submit" />
      </div>
    </form>
  </Dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch, computed, onMounted } from 'vue'
import { useToast } from 'primevue/usetoast'
import type { Reservation, ReservationCreateRequest, ReservationUpdateRequest } from '@/types/reservation'
import type { Pet } from '@/types/pet'
import { reservationApi } from '@/api/reservation'
import { petApi } from '@/api/pet'

interface Props {
  visible: boolean
  reservation?: Reservation | null
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
const availabilityMessage = ref('')
const isTimeAvailable = ref(true)
const toast = useToast()

// Data
const pets = ref<Pet[]>([])
const selectedPet = ref<Pet | null>(null)
const availableSubscriptions = ref<any[]>([])

// 選項
const serviceOptions = [
  { label: '基礎洗澡', value: '基礎洗澡' },
  { label: '精緻美容', value: '精緻美容' },
  { label: '造型設計', value: '造型設計' },
  { label: '指甲修剪', value: '指甲修剪' },
  { label: '耳朵清潔', value: '耳朵清潔' },
  { label: '全套護理', value: '全套護理' }
]
const designerOptions = [
  { label: '不指定', value: '' },
  { label: '王美美', value: '王美美' },
  { label: '李小花', value: '李小花' },
  { label: '張設計', value: '張設計' },
  { label: '陳專業', value: '陳專業' }
]

// 日期模型
const dateModel = ref<Date | null>(null)

// Computed
const isEdit = computed(() => !!props.reservation)

// 表單資料
const form = reactive<ReservationCreateRequest>({
  petId: 0,
  subscriptionId: undefined,
  reserveDate: '',
  reserveTime: '',
  serviceType: '',
  designer: '',
  note: ''
})

// 驗證錯誤
const errors = reactive({
  petId: '',
  reserveDate: '',
  reserveTime: '',
  serviceType: ''
})

function getPetAge(birthDay?: string) {
  if (!birthDay) return '-'
  const birth = new Date(birthDay)
  const now = new Date()
  let age = now.getFullYear() - birth.getFullYear()
  const m = now.getMonth() - birth.getMonth()
  if (m < 0 || (m === 0 && now.getDate() < birth.getDate())) {
    age--
  }
  return age
}

// Methods
const loadInitialPets = async () => {
  petLoading.value = true
  try {
    const response = await petApi.getPets({ pageSize: 50 })
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
      detail: '載入寵物列表失敗',
      life: 3000
    })
  } finally {
    petLoading.value = false
  }
}

const searchPets = async (query: string) => {
  if (!query) {
    await loadInitialPets()
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
    availableSubscriptions.value = []
    return
  }
  try {
    // Load pet details
    const pet = await petApi.getPet(petId)
    selectedPet.value = {
      ...pet,
      id: pet.petId,
      name: pet.petName,
      breedName: pet.breed,
      ownerName: pet.primaryContact?.name || '未設定',
      contactPhone: pet.primaryContact?.phone || '未設定'
    }
    // Load available subscriptions for this pet
    // TODO: Implement subscription API
    availableSubscriptions.value = []
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: '載入寵物資訊失敗',
      life: 3000
    })
  }
}

const checkTimeAvailability = async () => {
  if (!dateModel.value || !form.reserveTime) {
    availabilityMessage.value = ''
    isTimeAvailable.value = true
    return
  }
  try {
    const dateStr = dateModel.value.toISOString().split('T')[0]
    const response = await reservationApi.checkAvailability(dateStr, form.reserveTime)
    if (response.available) {
      availabilityMessage.value = '此時段可以預約'
      isTimeAvailable.value = true
    } else {
      availabilityMessage.value = `此時段已有 ${response.conflicts.length} 個預約，建議選擇其他時間`
      isTimeAvailable.value = false
    }
  } catch (error) {
    availabilityMessage.value = ''
    isTimeAvailable.value = true
  }
}

const validateForm = () => {
  let isValid = true
  errors.petId = ''
  errors.reserveDate = ''
  errors.reserveTime = ''
  errors.serviceType = ''
  if (!form.petId) {
    errors.petId = '請選擇寵物'
    isValid = false
  }
  if (!dateModel.value) {
    errors.reserveDate = '請選擇預約日期'
    isValid = false
  }
  if (!form.reserveTime) {
    errors.reserveTime = '請輸入預約時間'
    isValid = false
  }
  if (!form.serviceType) {
    errors.serviceType = '請選擇服務項目'
    isValid = false
  }
  return isValid
}

const handleSubmit = async () => {
  if (!validateForm()) return
  submitting.value = true
  try {
    const requestData = {
      ...form,
      reserveDate: dateModel.value ? dateModel.value.toISOString().split('T')[0] : ''
    }
    if (isEdit.value && props.reservation) {
      const updateData: ReservationUpdateRequest = {
        ...requestData,
        id: props.reservation.id
      }
      await reservationApi.updateReservation(updateData)
      toast.add({
        severity: 'success',
        summary: '成功',
        detail: '更新成功',
        life: 3000
      })
    } else {
      await reservationApi.createReservation(requestData)
      toast.add({
        severity: 'success',
        summary: '成功',
        detail: '預約成功',
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
  selectedPet.value = null
  availableSubscriptions.value = []
  pets.value = []
  availabilityMessage.value = ''
  isTimeAvailable.value = true
  dateModel.value = null
  Object.assign(form, {
    petId: 0,
    subscriptionId: undefined,
    reserveDate: '',
    reserveTime: '',
    serviceType: '',
    designer: '',
    note: ''
  })
  Object.keys(errors).forEach(key => {
    errors[key as keyof typeof errors] = ''
  })
}

// Watch for reservation changes
watch(() => props.reservation, async (newReservation) => {
  if (newReservation) {
    Object.assign(form, {
      petId: newReservation.petId,
      subscriptionId: newReservation.subscriptionId,
      reserveDate: newReservation.reserveDate,
      reserveTime: newReservation.reserveTime,
      serviceType: newReservation.serviceType,
      designer: newReservation.designer,
      note: newReservation.note
    })
    dateModel.value = newReservation.reserveDate ? new Date(newReservation.reserveDate) : null
    // Load pet info
    if (newReservation.petId) {
      try {
        const pet = await petApi.getPet(newReservation.petId)
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

// Watch for dialog visibility
watch(() => props.visible, (visible) => {
  if (visible && pets.value.length === 0) {
    loadInitialPets()
  } else if (!visible) {
    resetForm()
  }
})

// Load initial pets on mount
onMounted(() => {
  loadInitialPets()
})
</script>

<style scoped>
.form-tip {
  font-size: 12px;
  color: #909399;
  margin-top: 4px;
}

.pet-info-card {
  margin-top: 16px;
}

.pet-details p {
  margin: 8px 0;
  color: #606266;
}

.pet-details strong {
  color: #303133;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.p-error {
  color: var(--p-red-500);
  font-size: 0.875rem;
}
</style>