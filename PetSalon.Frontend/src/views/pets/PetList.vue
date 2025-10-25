<template>
  <div class="pet-list-container">
    <!-- Header -->
    <div class="page-header">
      <div class="header-left">
        <h2>🐾 寵物管理</h2>
        <span class="total-count">共 {{ total }} 隻寵物</span>
      </div>
      <div class="header-right">
        <Button
          label="新增寵物"
          icon="pi pi-plus"
          @click="openCreateDialog"
        />
      </div>
    </div>

    <!-- Search and Filter -->
    <Card class="search-section">
      <template #content>
        <div class="search-grid">
          <div class="search-field">
            <label>搜尋</label>
            <InputText
              v-model="searchForm.keyword"
              placeholder="搜尋寵物名稱或主人姓名"
              @input="handleSearch"
              ref="keywordInputRef"
            />
          </div>
          <div class="search-field">
            <label>品種</label>
            <SystemCodeSelect
              v-model="searchForm.breed"
              code-type="Breed"
              placeholder="選擇品種"
              clearable
              @update:model-value="handleSearch"
            />
          </div>
          <div class="search-field">
            <label>性別</label>
            <SystemCodeSelect
              v-model="searchForm.gender"
              code-type="Gender"
              placeholder="選擇性別"
              clearable
              @update:model-value="handleSearch"
            />
          </div>
          <div class="search-field">
            <label>&nbsp;</label>
            <Button
              label="重置"
              severity="secondary"
              @click="resetSearch"
            />
          </div>
        </div>
      </template>
    </Card>

    <!-- Multi-select actions bar -->
    <Card v-if="selectedPets.length > 0" class="multi-select-bar">
      <template #content>
        <div class="multi-select-content">
          <div class="multi-select-info">
            <i class="pi pi-check-circle"></i>
            <span>已選擇 {{ selectedPets.length }} 隻寵物</span>
          </div>
          <div class="multi-select-actions">
            <Button
              label="移除主人"
              icon="pi pi-user-minus"
              severity="warning"
              @click="removeOwnersFromSelected"
            />
            <Button
              label="批量刪除"
              icon="pi pi-trash"
              severity="danger"
              @click="deleteSelectedPets"
            />
            <Button
              label="取消選擇"
              icon="pi pi-times"
              severity="secondary"
              @click="clearSelection"
            />
          </div>
        </div>
      </template>
    </Card>

    <!-- Pet Cards Grid -->
    <div class="pet-grid" v-if="loading">
      <ProgressSpinner />
    </div>
    <div v-else class="pet-grid">
      <Card
        v-for="(pet, index) in pets"
        :key="pet.id || index"
        class="pet-card"
        :class="{ 'selected': isSelected(pet.id) }"
        @click="viewPet(pet)"
      >
        <template #content>
          <div class="pet-card-content">
            <!-- Selection checkbox -->
            <div class="pet-selection">
              <Checkbox
                :value="pet.id"
                v-model="selectedPets"
                @click.stop
              />
            </div>
            
            <div class="pet-avatar" @click.stop>
              <Image
                v-if="pet.photoUrl"
                :src="pet.photoUrl"
                :alt="pet.name || '寵物照片'"
                class="pet-photo"
                preview
              />
              <div v-else class="pet-photo-placeholder">
                🐾
              </div>
            </div>

            <div class="pet-info">
              <h3 class="pet-name">{{ pet.name || '未命名' }}</h3>
              <div class="pet-details">
                <p><strong>品種:</strong> {{ pet.breedName || '未知' }}</p>
                <p><strong>性別:</strong> {{ getGenderDisplay(pet.gender) }}</p>
                <p class="owner-info"><strong>主人:</strong> {{ pet.ownerName || '無主人資訊' }}</p>
              </div>
            </div>

            <div class="pet-actions">
              <Button
                label="編輯"
                size="small"
                @click.stop="editPet(pet)"
              />
              <Button
                label="刪除"
                severity="danger"
                size="small"
                @click.stop="deletePet(pet)"
              />
            </div>
          </div>
        </template>
      </Card>
    </div>

    <!-- Empty State -->
    <Card v-if="!loading && pets.length === 0" class="empty-state">
      <template #content>
        <div class="empty-content">
          <i class="pi pi-inbox" style="font-size: 4rem; color: #6c757d;"></i>
          <h3>尚無寵物資料</h3>
          <p>開始新增您的第一隻寵物吧！</p>
          <Button
            label="新增第一隻寵物"
            icon="pi pi-plus"
            @click="openCreateDialog"
          />
        </div>
      </template>
    </Card>

    <!-- Pagination -->
    <div class="pagination-wrapper" v-if="total > pageSize">
      <Paginator
        v-model:first="paginatorFirst"
        :rows="pageSize"
        :totalRecords="total"
        :rowsPerPageOptions="[12, 24, 48]"
        @page="onPageChange"
        template="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink RowsPerPageDropdown"
      />
    </div>

    <!-- Create/Edit Dialog -->
    <PetForm
      v-if="showDialog"
      :visible="showDialog"
      :pet="selectedPet"
      @close="closeDialog"
      @success="handleFormSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive, computed } from 'vue'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import type { PetSearchParams } from '@/types/pet'
import { petApi } from '@/api/pet'
import { commonApi } from '@/api/common'
import PetForm from '@/components/forms/PetForm.vue'
import { SystemCodeSelect } from '@/components/common'
import { getResourceUrl } from '@/utils/resource'

const toast = useToast()
const confirm = useConfirm()

// 畫面顯示用寵物型別
interface PetViewModel {
  id: number
  name: string
  breedName: string
  gender: string
  birthDay?: string
  ownerName?: string
  photoUrl?: string
  [key: string]: any
}
const pets = ref<PetViewModel[]>([])
const genders = ref<any[]>([])
const total = ref(0)
const currentPage = ref(1)
const pageSize = ref(12)
const loading = ref(false)
const showDialog = ref(false)
const selectedPet = ref<PetViewModel | null>(null)
const keywordInputRef = ref()
const selectedPets = ref<number[]>([])

// Pagination
const paginatorFirst = computed({
  get: () => (currentPage.value - 1) * pageSize.value,
  set: (value: number) => {
    currentPage.value = Math.floor(value / pageSize.value) + 1
  }
})

// Search form
const searchForm = reactive<PetSearchParams>({
  keyword: '',
  breed: undefined,
  gender: undefined
})
// 保持搜尋欄 focus
const onInputFocus = () => {
  if (keywordInputRef.value) {
    keywordInputRef.value.focus()
  }
}

// Methods
const loadPets = async () => {
  loading.value = true
  try {
    const params = {
      ...searchForm,
      page: currentPage.value,
      pageSize: pageSize.value
    }
    const response = await petApi.getPets(params)

    pets.value = response.data.map((item: any) => {
      return {
        id: item.petId || item.id,
        name: item.petName || item.name,
        breed: item.breed, // 保持 code 原值，供編輯時使用
        breedName: item.breedName || item.breed, // 優先使用 breedName 中文名稱
        gender: item.gender,
        birthDay: item.birthDay,
        ownerName: item.ownersDisplay || '無主人資訊',
        photoUrl: getResourceUrl(item.photoUrl || item.photo), // 轉換為完整 URL
        ...item
      }
    })
    total.value = response.total || response.data.length
  } catch (error) {
    console.error('載入寵物清單失敗:', error)
    toast.add({
      severity: 'error',
      summary: '載入失敗',
      detail: '載入寵物清單失敗',
      life: 3000
    })
  } finally {
    loading.value = false
  }
}


const loadGenders = async () => {
  try {
    const response = await commonApi.getSystemCodes('Gender')
    genders.value = response
  } catch (error) {
    console.error('載入性別清單失敗:', error)
  }
}

const getGenderDisplay = (genderCode: string) => {
  if (!genderCode) return '未知'

  // 如果已經載入了性別系統代碼，就使用系統代碼
  if (genders.value.length > 0) {
    const gender = genders.value.find(g => g.code === genderCode || g.id === genderCode)
    return gender?.name || genderCode
  }

  // 如果還沒載入系統代碼，使用預設轉換
  return genderCode === 'M' ? '公' : genderCode === 'F' ? '母' : genderCode
}

const handleSearch = () => {
  currentPage.value = 1
  loadPets()
}

const resetSearch = () => {
  searchForm.keyword = ''
  searchForm.breed = undefined
  searchForm.gender = undefined
  handleSearch()
}

const openCreateDialog = () => {
  selectedPet.value = null
  showDialog.value = true
}

const editPet = (pet: PetViewModel) => {
  selectedPet.value = pet
  showDialog.value = true
}

const viewPet = (pet: PetViewModel) => {
  // 暫時使用編輯功能作為詳細檢視，待詳細檢視頁面完成後更新
  editPet(pet)
}

const deletePet = async (pet: PetViewModel) => {
  confirm.require({
    message: `確定要刪除寵物「${pet.name}」嗎？`,
    header: '確認刪除',
    icon: 'pi pi-exclamation-triangle',
    rejectProps: {
      label: '取消',
      severity: 'secondary',
      outlined: true
    },
    acceptProps: {
      label: '確定',
      severity: 'danger'
    },
    accept: async () => {
      try {
        await petApi.deletePet(pet.id)
        toast.add({
          severity: 'success',
          summary: '刪除成功',
          detail: '寵物已成功刪除',
          life: 3000
        })
        loadPets()
      } catch (error: any) {
        toast.add({
          severity: 'error',
          summary: '刪除失敗',
          detail: '刪除寵物時發生錯誤',
          life: 3000
        })
      }
    },
    reject: () => {
      // 使用者取消刪除，不需要做任何事
    }
  })
}

const closeDialog = () => {
  showDialog.value = false
  selectedPet.value = null
}

const onPageChange = (event: any) => {
  currentPage.value = event.page + 1
  pageSize.value = event.rows
  loadPets()
}

const handleFormSuccess = () => {
  closeDialog()
  loadPets()
}

// Multi-select methods
const isSelected = (petId: number) => {
  return selectedPets.value.includes(petId)
}

const clearSelection = () => {
  selectedPets.value = []
}

const deleteSelectedPets = async () => {
  if (selectedPets.value.length === 0) return
  
  const selectedPetNames = pets.value
    .filter(pet => selectedPets.value.includes(pet.id))
    .map(pet => pet.name)
    .join('、')
  
  confirm.require({
    message: `確定要刪除所選的 ${selectedPets.value.length} 隻寵物（${selectedPetNames}）嗎？`,
    header: '確認批量刪除',
    icon: 'pi pi-exclamation-triangle',
    rejectProps: {
      label: '取消',
      severity: 'secondary',
      outlined: true
    },
    acceptProps: {
      label: '確定',
      severity: 'danger'
    },
    accept: async () => {
      try {
        const deletePromises = selectedPets.value.map(petId => petApi.deletePet(petId))
        await Promise.all(deletePromises)
        
        toast.add({
          severity: 'success',
          summary: '批量刪除成功',
          detail: `已成功刪除 ${selectedPets.value.length} 隻寵物`,
          life: 3000
        })
        
        clearSelection()
        loadPets()
      } catch (error: any) {
        toast.add({
          severity: 'error',
          summary: '批量刪除失敗',
          detail: '刪除寵物時發生錯誤',
          life: 3000
        })
      }
    },
    reject: () => {
      // 使用者取消刪除，不需要做任何事
    }
  })
}

const removeOwnersFromSelected = async () => {
  if (selectedPets.value.length === 0) return
  
  const selectedPetNames = pets.value
    .filter(pet => selectedPets.value.includes(pet.id))
    .map(pet => pet.name)
    .join('、')
  
  confirm.require({
    message: `確定要移除所選 ${selectedPets.value.length} 隻寵物（${selectedPetNames}）的主人關聯嗎？`,
    header: '確認移除主人',
    icon: 'pi pi-exclamation-triangle',
    rejectProps: {
      label: '取消',
      severity: 'secondary',
      outlined: true
    },
    acceptProps: {
      label: '確定',
      severity: 'warning'
    },
    accept: async () => {
      try {
        // 這裡需要調用後端 API 來移除主人關聯
        // 暫時使用模擬的實作
        const updatePromises = selectedPets.value.map(petId => {
          const pet = pets.value.find(p => p.id === petId)
          if (pet) {
            return petApi.updatePet({
              petId: pet.id,
              petName: pet.name,
              breed: pet.breed,
              gender: pet.gender
            })
          }
          return Promise.resolve()
        })
        
        await Promise.all(updatePromises)
        
        toast.add({
          severity: 'success',
          summary: '移除主人成功',
          detail: `已成功移除 ${selectedPets.value.length} 隻寵物的主人關聯`,
          life: 3000
        })
        
        clearSelection()
        loadPets()
      } catch (error: any) {
        toast.add({
          severity: 'error',
          summary: '移除主人失敗',
          detail: '移除主人關聯時發生錯誤',
          life: 3000
        })
      }
    },
    reject: () => {
      // 使用者取消操作，不需要做任何事
    }
  })
}

// Lifecycle
onMounted(async () => {
  // 先載入性別資料，然後載入寵物資料
  await loadGenders()
  await loadPets()

  setTimeout(() => {
    if (keywordInputRef.value) keywordInputRef.value.focus()
  }, 300)
})
</script>

<style scoped>
.pet-list-container {
  padding: 1.5rem;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid var(--p-content-border-color);
}

.header-left h2 {
  margin: 0;
  color: var(--p-text-color);
  font-size: 1.5rem;
  font-weight: 600;
}

.total-count {
  color: var(--p-text-muted-color);
  font-size: 0.875rem;
  margin-left: 0.75rem;
}

.search-section {
  margin-bottom: 2rem;
}

.search-grid {
  display: grid;
  grid-template-columns: 2fr 1fr 1fr auto;
  gap: 1rem;
  align-items: end;
}

.search-field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.search-field label {
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--p-text-color);
}

.pet-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
  min-height: 200px;
}

.pet-grid:has(.p-progress-spinner) {
  display: flex;
  justify-content: center;
  align-items: center;
}

.pet-card {
  cursor: pointer;
  transition: all 0.3s ease;
  border: 1px solid var(--p-content-border-color);
}

.pet-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
}

.pet-card-content {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.pet-avatar {
  text-align: center;
  margin-bottom: 1rem;
}

.pet-photo {
  display: inline-block;
  width: 80px;
  height: 80px;
  border-radius: 50%;
  overflow: hidden;
  border: 3px solid var(--p-content-border-color);
}

.pet-photo :deep(img) {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.pet-photo-placeholder {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background: var(--p-content-background);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2rem;
  margin: 0 auto;
  border: 3px solid var(--p-content-border-color);
  color: var(--p-text-muted-color);
}

.pet-info {
  text-align: center;
  margin-bottom: 1rem;
  flex: 1;
}

.pet-name {
  margin: 0 0 0.75rem 0;
  color: var(--p-text-color);
  font-size: 1.125rem;
  font-weight: 600;
}

.pet-details p {
  margin: 0.25rem 0;
  color: var(--p-text-muted-color);
  font-size: 0.875rem;
}

.pet-details strong {
  color: var(--p-primary-color);
  font-weight: 500;
}

.owner-info {
  background: var(--p-surface-100);
  padding: 0.5rem;
  border-radius: 0.25rem;
  margin-top: 0.5rem;
  font-size: 0.875rem;
}

.owner-info strong {
  color: var(--p-primary-600);
}

.pet-actions {
  display: flex;
  justify-content: center;
  gap: 0.5rem;
  margin-top: auto;
}

.empty-state {
  margin: 2rem 0;
}

.empty-content {
  text-align: center;
  padding: 3rem 1rem;
}

.empty-content h3 {
  color: var(--p-text-color);
  margin: 1rem 0 0.5rem 0;
}

.empty-content p {
  color: var(--p-text-muted-color);
  margin-bottom: 1.5rem;
}

.pagination-wrapper {
  display: flex;
  justify-content: center;
  padding: 1.5rem 0;
}

/* Multi-select styles */
.multi-select-bar {
  margin-bottom: 1.5rem;
  background: var(--p-primary-50);
  border: 1px solid var(--p-primary-200);
}

.multi-select-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
}

.multi-select-info {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--p-primary-700);
  font-weight: 500;
}

.multi-select-info i {
  color: var(--p-primary-600);
}

.multi-select-actions {
  display: flex;
  gap: 0.5rem;
}

.pet-card.selected {
  border-color: var(--p-primary-500);
  box-shadow: 0 0 0 2px var(--p-primary-200);
}

.pet-selection {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  z-index: 1;
}

.pet-card {
  position: relative;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .pet-list-container {
    padding: 1rem;
  }

  .search-grid {
    grid-template-columns: 1fr;
    gap: 1rem;
  }

  .pet-grid {
    grid-template-columns: 1fr;
    gap: 1rem;
  }

  .page-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }

  .header-left {
    width: 100%;
  }

  .header-right {
    width: 100%;
  }
}

@media (max-width: 480px) {
  .pet-list-container {
    padding: 0.5rem;
  }

  .page-header {
    margin-bottom: 1rem;
    padding-bottom: 0.5rem;
  }

  .search-section {
    margin-bottom: 1rem;
  }

  .empty-content {
    padding: 2rem 0.5rem;
  }
}
</style>