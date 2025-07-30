<template>
  <div class="pet-selector">
    <Select
      :model-value="modelValue"
      :options="pets"
      :option-label="petDisplayLabel"
      :option-value="petValueField"
      :placeholder="placeholder"
      :filter="enableFilter"
      :loading="loading"
      :disabled="disabled"
      :invalid="invalid"
      :multiple="multiple"
      @update:model-value="handleUpdate"
      @change="handleChange"
      @filter="handleFilter"
      class="w-full"
    >
      <template #option="{ option }">
        <div class="pet-option">
          <div class="pet-avatar">
            <img
              v-if="option.photoUrl"
              :src="option.photoUrl"
              :alt="option.petName"
              class="pet-photo"
            />
            <div v-else class="pet-photo-placeholder">
              🐾
            </div>
          </div>
          <div class="pet-info">
            <div class="pet-name">{{ option.petName || option.name || `寵物 #${option.petId}` }}</div>
            <div class="pet-details">
              <span class="breed">{{ option.breed }}</span>
              <span v-if="option.ownerName" class="owner">• {{ option.ownerName }}</span>
              <span v-if="option.contactPhone" class="phone">• {{ option.contactPhone }}</span>
            </div>
          </div>
          <div v-if="showPrice && option.subscriptionPrice" class="pet-price">
            NT$ {{ option.subscriptionPrice.toLocaleString() }}
          </div>
        </div>
      </template>

      <template #value="{ value, placeholder }">
        <div v-if="value && !multiple" class="selected-pet">
          <div class="pet-avatar-small">
            <img
              v-if="selectedPet?.photoUrl"
              :src="selectedPet.photoUrl"
              :alt="selectedPet.petName"
              class="pet-photo-small"
            />
            <div v-else class="pet-photo-placeholder-small">
              🐾
            </div>
          </div>
          <span>{{ selectedPet?.petName || selectedPet?.name || `寵物 #${selectedPet?.petId || value}` }}</span>
        </div>
        <span v-else-if="!value && !multiple">{{ placeholder }}</span>
        <span v-else-if="multiple && value?.length">
          已選擇 {{ value.length }} 隻寵物
        </span>
        <span v-else>{{ placeholder }}</span>
      </template>
    </Select>

    <!-- 選中寵物資訊卡 -->
    <Card v-if="showSelectedInfo && selectedPet && !multiple" class="selected-pet-card">
      <template #content>
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
          <div class="pet-details">
            <p><strong>品種:</strong> {{ selectedPet.breed }}</p>
          </div>
        </div>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { Pet } from '@/types/pet'
import { usePetSelector } from '@/composables/usePetSelector'

interface Props {
  modelValue?: number | number[] | null
  multiple?: boolean
  placeholder?: string
  contactPersonId?: number
  filter?: (pet: Pet) => boolean
  disabled?: boolean
  invalid?: boolean
  enableFilter?: boolean
  showSelectedInfo?: boolean
  showPrice?: boolean
}

interface Emits {
  (e: 'update:modelValue', value: number | number[] | null): void
  (e: 'pet-selected', pet: Pet | Pet[]): void
  (e: 'change', event: any): void
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: '請選擇寵物',
  enableFilter: true,
  showSelectedInfo: false,
  showPrice: false,
  multiple: false
})

const emit = defineEmits<Emits>()

// 使用 composable
const {
  pets,
  loading,
  searchPets,
  getPetById
} = usePetSelector({
  contactPersonId: props.contactPersonId,
  filter: props.filter
})

// 計算屬性
const petDisplayLabel = computed(() => 'displayName')
const petValueField = computed(() => 'petId')

const selectedPet = computed(() => {
  if (!props.modelValue || props.multiple) return null
  return pets.value.find(pet => pet.petId === props.modelValue) || null
})

// 方法
const handleUpdate = (value: number | number[] | null) => {
  emit('update:modelValue', value)
}

const handleChange = (event: any) => {
  const value = event.value
  let selectedPets: Pet | Pet[] | null = null

  if (props.multiple && Array.isArray(value)) {
    selectedPets = pets.value.filter(pet => value.includes(pet.petId))
  } else if (!props.multiple && value) {
    selectedPets = pets.value.find(pet => pet.petId === value) || null
  }

  if (selectedPets) {
    emit('pet-selected', selectedPets)
  }

  emit('change', event)
}

const handleFilter = (event: any) => {
  const query = event.value
  searchPets(query)
}

// 初始載入
searchPets('')
</script>

<style scoped>
.pet-selector {
  width: 100%;
}

.pet-option {
  display: flex;
  align-items: center;
  padding: 0.5rem 0;
  gap: 0.75rem;
}

.pet-avatar {
  flex-shrink: 0;
}

.pet-photo {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  object-fit: cover;
  border: 2px solid var(--p-surface-border);
}

.pet-photo-placeholder {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: var(--p-surface-100);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
  border: 2px solid var(--p-surface-border);
}

.pet-info {
  flex: 1;
  min-width: 0;
}

.pet-name {
  font-weight: 600;
  color: var(--p-text-color);
  margin-bottom: 0.25rem;
}

.pet-details {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  font-size: 0.875rem;
  color: var(--p-text-color-secondary);
}

.breed {
  color: var(--p-blue-600);
}

.owner {
  color: var(--p-green-600);
}

.phone {
  color: var(--p-orange-600);
}

.pet-price {
  flex-shrink: 0;
  font-weight: 600;
  color: var(--p-primary-color);
  font-size: 0.875rem;
}

.selected-pet {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.pet-avatar-small {
  flex-shrink: 0;
}

.pet-photo-small {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  object-fit: cover;
  border: 1px solid var(--p-surface-border);
}

.pet-photo-placeholder-small {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  background: var(--p-surface-100);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  border: 1px solid var(--p-surface-border);
}

.selected-pet-card {
  margin-top: 1rem;
}

.pet-summary {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.pet-summary .pet-details h4 {
  margin: 0 0 0.5rem 0;
  color: var(--p-text-color);
}

.pet-summary .pet-details p {
  margin: 0.25rem 0;
  color: var(--p-text-color-secondary);
}

.pet-summary .pet-details strong {
  color: var(--p-text-color);
}

/* 響應式設計 */
@media (max-width: 768px) {
  .pet-option {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.5rem;
  }

  .pet-details {
    flex-direction: column;
    gap: 0.25rem;
  }

  .pet-price {
    align-self: flex-end;
  }

  .pet-summary {
    flex-direction: column;
    align-items: flex-start;
  }
}
</style>