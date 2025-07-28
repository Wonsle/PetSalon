<template>
  <div class="pet-list-container">
    <!-- Header -->
    <div class="page-header">
      <div class="header-left">
        <h2>🐾 寵物管理</h2>
        <span class="total-count">共 {{ total }} 隻寵物</span>
      </div>
      <div class="header-right">
        <el-button type="primary" @click="openCreateDialog">
          <el-icon><Plus /></el-icon>
          新增寵物
        </el-button>
      </div>
    </div>

    <!-- Search and Filter -->
    <div class="search-section">
      <el-row :gutter="16">
        <el-col :span="6">
          <el-input
            v-model="searchForm.keyword"
            placeholder="搜尋寵物名稱或主人姓名"
            clearable
            @input="handleSearch"
            autofocus
            @focus="onInputFocus"
            ref="keywordInputRef"
          >
            <template #prefix>
              <el-icon><Search /></el-icon>
            </template>
          </el-input>
        </el-col>
        <el-col :span="4">
          <el-select
            v-model="searchForm.breed"
            placeholder="品種"
            clearable
            @change="handleSearch"
          >
            <el-option
              v-for="breed in breeds"
              :key="breed.id"
              :label="breed.name"
              :value="breed.id"
            />
          </el-select>
        </el-col>
        <el-col :span="4">
          <SystemCodeSelect
            v-model="searchForm.gender"
            code-type="Gender"
            placeholder="性別"
            clearable
            @update:model-value="handleSearch"
          />
        </el-col>
        <el-col :span="4">
          <el-button @click="resetSearch">重置</el-button>
        </el-col>
      </el-row>
    </div>

    <!-- Pet Cards Grid -->
    <div class="pet-grid" v-loading="loading">

      <div
        v-for="(pet, index) in pets"
        :key="pet.id || index"
        class="pet-card"
        @click="viewPet(pet)"
      >
        <div class="pet-avatar">
          <img
            v-if="pet.photoUrl"
            :src="pet.photoUrl"
            :alt="pet.name || '寵物照片'"
            class="pet-photo"
          />
          <div v-else class="pet-photo-placeholder">
            🐾
          </div>
        </div>

        <div class="pet-info">
          <h3 class="pet-name">{{ pet.name || '未命名' }}</h3>
          <div class="pet-details">
            <p><strong>品種:</strong> {{ pet.breedName || '未知' }}</p>
            <p><strong>年齡:</strong> {{ pet.age || 0 }} 歲</p>
            <p><strong>性別:</strong> {{ getGenderDisplay(pet.gender) }}</p>
            <p><strong>主人:</strong> {{ pet.ownerName || '未知' }}</p>
          </div>
        </div>

        <div class="pet-actions">
          <el-button
            type="primary"
            size="small"
            @click.stop="editPet(pet)"
          >
            編輯
          </el-button>
          <el-button
            type="danger"
            size="small"
            @click.stop="deletePet(pet)"
          >
            刪除
          </el-button>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-if="!loading && pets.length === 0" class="empty-state">
      <el-empty description="尚無寵物資料">
        <el-button type="primary" @click="openCreateDialog">
          新增第一隻寵物
        </el-button>
      </el-empty>
    </div>

    <!-- Pagination -->
    <div class="pagination-wrapper" v-if="total > pageSize">
      <el-pagination
        v-model:current-page="currentPage"
        v-model:page-size="pageSize"
        :page-sizes="[12, 24, 48]"
        :total="total"
        layout="total, sizes, prev, pager, next"
        @size-change="loadPets"
        @current-change="loadPets"
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
import { ref, onMounted, reactive } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Search } from '@element-plus/icons-vue'
import type { PetSearchParams } from '@/types/pet'
import { petApi } from '@/api/pet'
import { commonApi } from '@/api/common'
import PetForm from '@/components/forms/PetForm.vue'
import { SystemCodeSelect } from '@/components/common'

// 畫面顯示用寵物型別
interface PetViewModel {
  id: number
  name: string
  breedName: string
  gender: string
  birthDay?: string
  age?: number
  ownerName?: string
  photoUrl?: string
  [key: string]: any
}
const pets = ref<PetViewModel[]>([])
const breeds = ref<any[]>([])
const genders = ref<any[]>([])
const total = ref(0)
const currentPage = ref(1)
const pageSize = ref(12)
const loading = ref(false)
const showDialog = ref(false)
const selectedPet = ref<PetViewModel | null>(null)
const keywordInputRef = ref()

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
      // 根據品種ID找到對應的中文名稱
      const breedInfo = breeds.value.find(b => b.id == item.breed || b.code == item.breed)
      const breedName = breedInfo?.name || item.breedName || item.breed || '未知品種'

      return {
        id: item.petId || item.id,
        name: item.petName || item.name,
        breedName: breedName,
        gender: item.gender,
        birthDay: item.birthDay,
        age: item.age || (item.birthDay ? new Date().getFullYear() - new Date(item.birthDay).getFullYear() : undefined),
        ownerName: item.ownerName || item.contactName || item.primaryContact?.name || '未知主人',
        photoUrl: item.photoUrl || item.photo || '',
        ...item
      }
    })
    total.value = response.total || response.data.length
  } catch (error) {
    console.error('載入寵物清單失敗:', error)
    ElMessage.error('載入寵物清單失敗')
  } finally {
    loading.value = false
  }
}

const loadBreeds = async () => {
  try {
    const response = await commonApi.getBreeds()
    breeds.value = response
  } catch (error) {
    console.error('載入品種清單失敗:', error)
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
  try {
    await ElMessageBox.confirm(
      `確定要刪除寵物「${pet.name}」嗎？`,
      '確認刪除',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )

    await petApi.deletePet(pet.id)
    ElMessage.success('刪除成功')
    loadPets()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗')
    }
  }
}

const closeDialog = () => {
  showDialog.value = false
  selectedPet.value = null
}

const handleFormSuccess = () => {
  closeDialog()
  loadPets()
}

// Lifecycle
onMounted(async () => {
  // 先載入性別資料，然後載入寵物和品種資料
  await loadGenders()
  await Promise.all([loadPets(), loadBreeds()])
  
  setTimeout(() => {
    if (keywordInputRef.value) keywordInputRef.value.focus()
  }, 300)
})
</script>

<style scoped>
.pet-list-container {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
  padding-bottom: 16px;
  border-bottom: 1px solid #e4e7ed;
}

.header-left h2 {
  margin: 0;
  color: #303133;
  font-size: 24px;
}

.total-count {
  color: #909399;
  font-size: 14px;
  margin-left: 12px;
}

.search-section {
  margin-bottom: 24px;
  padding: 16px;
  background: #f8f9fa;
  border-radius: 8px;
}

.pet-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 20px;
  margin-bottom: 24px;
}

.pet-card {
  border: 1px solid #e4e7ed;
  border-radius: 12px;
  padding: 20px;
  background: white;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.pet-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
  border-color: #409eff;
}

.pet-avatar {
  text-align: center;
  margin-bottom: 16px;
}

.pet-photo {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  object-fit: cover;
  border: 3px solid #e4e7ed;
}

.pet-photo-placeholder {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background: #f5f7fa;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 32px;
  margin: 0 auto;
  border: 3px solid #e4e7ed;
}

.pet-info {
  text-align: center;
  margin-bottom: 16px;
}

.pet-name {
  margin: 0 0 12px 0;
  color: #303133;
  font-size: 18px;
  font-weight: 600;
}

.pet-details p {
  margin: 4px 0;
  color: #606266;
  font-size: 14px;
}

.pet-details strong {
  color: #409eff;
}

.pet-actions {
  display: flex;
  justify-content: center;
  gap: 8px;
}

.empty-state {
  text-align: center;
  padding: 60px 20px;
}

.pagination-wrapper {
  display: flex;
  justify-content: center;
  padding: 20px 0;
}

@media (max-width: 768px) {
  .pet-grid {
    grid-template-columns: 1fr;
  }

  .page-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 16px;
  }

  .search-section .el-row {
    flex-direction: column;
    gap: 12px;
  }
}
</style>