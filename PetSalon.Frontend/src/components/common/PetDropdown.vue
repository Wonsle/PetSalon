<template>
  <div class="pet-dropdown">
    <Dropdown
      v-model="selectedPet"
      :options="pets"
      option-label="petName"
      option-value="petId"
      :placeholder="placeholder"
      :disabled="disabled"
      :show-clear="showClear"
      :filter="filter"
      filter-placeholder="搜尋寵物名稱"
      :loading="loading"
      class="w-full"
      @change="handleChange"
    >
      <!-- 自定義選中值的顯示模板 -->
      <template #value="slotProps">
        <div v-if="slotProps.value" class="pet-dropdown-value">
          <div class="pet-thumbnail">
            <img
              v-if="getSelectedPet(slotProps.value)?.photoUrl"
              :src="getSelectedPet(slotProps.value)?.photoUrl"
              alt="寵物照片"
            />
            <span v-else class="photo-placeholder">🐾</span>
          </div>
          <div class="pet-info">
            <span class="pet-name">{{ getSelectedPet(slotProps.value)?.petName }}</span>
            <span class="pet-details">
              {{ getPetDetails(getSelectedPet(slotProps.value)) }}
            </span>
          </div>
        </div>
        <span v-else class="placeholder-text">{{ placeholder }}</span>
      </template>

      <!-- 自定義選項的顯示模板 -->
      <template #option="slotProps">
        <div
          class="pet-dropdown-option"
          v-tooltip.right="getContactsTooltip(slotProps.option)"
        >
          <div class="pet-thumbnail">
            <img
              v-if="slotProps.option.photoUrl"
              :src="slotProps.option.photoUrl"
              alt="寵物照片"
            />
            <span v-else class="photo-placeholder">🐾</span>
          </div>
          <div class="pet-info">
            <span class="pet-name">{{ slotProps.option.petName }}</span>
            <span class="pet-details">
              {{ getPetDetails(slotProps.option) }}
            </span>
          </div>
        </div>
      </template>
    </Dropdown>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { petApi } from '@/api/pet'
import { commonApi } from '@/api/common'
import type { Pet, PetWithOwners, PetOwnerInfo } from '@/types/pet'
import type { SystemCode } from '@/api/common'
import { useToast } from 'primevue/usetoast'

interface Props {
  modelValue?: number | null
  placeholder?: string
  disabled?: boolean
  showClear?: boolean
  filter?: boolean
}

interface Emits {
  (e: 'update:modelValue', value: number | null): void
  (e: 'change', pet: Pet | null): void
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: '請選擇寵物',
  disabled: false,
  showClear: true,
  filter: true
})

const emit = defineEmits<Emits>()
const toast = useToast()

// 狀態
const pets = ref<PetWithOwners[]>([])
const loading = ref(false)
const coatColors = ref<SystemCode[]>([])

// 毛色代碼對應名稱的映射表
const coatColorMap = computed(() => {
  const map = new Map<string, string>()
  coatColors.value.forEach(color => {
    map.set(color.code, color.name)
  })
  return map
})

// 雙向綁定的選中值
const selectedPet = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value ?? null)
})

/**
 * 載入毛色 SystemCode
 */
const loadCoatColors = async () => {
  try {
    coatColors.value = await commonApi.getSystemCodes('CoatColor')
  } catch (error) {
    console.error('載入毛色列表失敗:', error)
  }
}

/**
 * 載入寵物列表
 */
const loadPets = async () => {
  loading.value = true
  try {
    const response = await petApi.getPets({ pageSize: 1000 })
    pets.value = response.data as PetWithOwners[]
  } catch (error) {
    console.error('載入寵物列表失敗:', error)
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: '載入寵物列表失敗',
      life: 3000
    })
  } finally {
    loading.value = false
  }
}

/**
 * 根據 petId 獲取寵物資料
 */
const getSelectedPet = (petId: number): Pet | undefined => {
  return pets.value.find(pet => pet.petId === petId)
}

/**
 * 格式化寵物詳細資訊（品種、毛色）
 */
const getPetDetails = (pet?: Pet | null): string => {
  if (!pet) return ''

  const parts: string[] = []

  if (pet.breedName) {
    parts.push(pet.breedName)
  }

  if (pet.coatColor) {
    // 使用毛色名稱而不是代碼
    const coatColorName = coatColorMap.value.get(pet.coatColor) || pet.coatColor
    parts.push(coatColorName)
  }

  return parts.length > 0 ? `（${parts.join('、')}）` : ''
}

/**
 * 產生聯絡人資訊的 Tooltip 內容
 */
const getContactsTooltip = (pet: PetWithOwners): string => {
  const petWithOwners = pet as PetWithOwners

  if (!petWithOwners.owners || petWithOwners.owners.length === 0) {
    return '無聯絡人資訊'
  }

  // 依照 sort 排序聯絡人
  const sortedOwners = [...petWithOwners.owners].sort((a, b) => {
    const sortA = a.sort ?? 999
    const sortB = b.sort ?? 999
    return sortA - sortB
  })

  const lines = ['聯絡人資訊：']
  sortedOwners.forEach((owner: PetOwnerInfo) => {
    const namePart = owner.nickName
      ? `${owner.name}（${owner.nickName}）`
      : owner.name

    const line = `• ${namePart} - ${owner.relationshipTypeName} - ${owner.contactNumber}`
    lines.push(line)
  })

  return lines.join('\n')
}

/**
 * 處理選擇變更事件
 */
const handleChange = (event: any) => {
  const selectedPetData = event.value
    ? pets.value.find(pet => pet.petId === event.value)
    : null

  emit('change', selectedPetData || null)
}

// 組件掛載時載入寵物列表和毛色 SystemCode
onMounted(async () => {
  await loadCoatColors()
  await loadPets()
})

// 監聽 modelValue 變化，確保數據同步
watch(() => props.modelValue, (newValue) => {
  if (newValue && pets.value.length > 0) {
    const pet = getSelectedPet(newValue)
    if (!pet) {
      // 如果找不到對應的寵物，重新載入列表
      loadPets()
    }
  }
})

// 暴露方法供父組件使用
defineExpose({
  loadPets,
  getSelectedPet
})
</script>

<style scoped>
.pet-dropdown {
  width: 100%;
}

.pet-dropdown-value,
.pet-dropdown-option {
  display: flex;
  align-items: center;
  gap: 12px;
}

.pet-thumbnail {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  overflow: hidden;
  flex-shrink: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #f0f0f0;
}

.pet-thumbnail img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.photo-placeholder {
  font-size: 20px;
  line-height: 1;
}

.pet-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
  flex: 1;
  min-width: 0;
}

.pet-name {
  font-weight: 600;
  font-size: 14px;
  color: #333;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.pet-details {
  font-size: 12px;
  color: #666;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.placeholder-text {
  color: #999;
}

/* Dropdown 選項懸停效果 */
.pet-dropdown-option {
  padding: 8px 0;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .pet-thumbnail {
    width: 36px;
    height: 36px;
  }

  .pet-name {
    font-size: 13px;
  }

  .pet-details {
    font-size: 11px;
  }
}
</style>
