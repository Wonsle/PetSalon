import { ref, reactive, computed } from 'vue'
import { useToast } from 'primevue/usetoast'
import type { Pet } from '@/types/pet'
import { petApi } from '@/api/pet'

interface UsePetSelectorOptions {
  contactPersonId?: number
  filter?: (pet: Pet) => boolean
  initialLoad?: boolean
}

interface PetForSelector extends Pet {
  displayName: string
  ownerName?: string
  contactPhone?: string
}

export function usePetSelector(options: UsePetSelectorOptions = {}) {
  const toast = useToast()
  
  // 響應式狀態
  const loading = ref(false)
  const pets = ref<PetForSelector[]>([])
  const searchQuery = ref('')
  const selectedPets = ref<Pet[]>([])
  
  // 錯誤狀態
  const error = reactive({
    hasError: false,
    message: ''
  })
  
  // 計算屬性
  const filteredPets = computed(() => {
    let result = pets.value
    
    // 應用自定義過濾器
    if (options.filter) {
      result = result.filter(options.filter)
    }
    
    // 按聯絡人過濾
    if (options.contactPersonId) {
      result = result.filter(pet => 
        pet.primaryContact?.contactPersonId === options.contactPersonId
      )
    }
    
    return result
  })
  
  const hasResults = computed(() => pets.value.length > 0)
  const isEmpty = computed(() => pets.value.length === 0 && !loading.value)
  
  // 工具函數
  const clearError = () => {
    error.hasError = false
    error.message = ''
  }
  
  const setError = (message: string) => {
    error.hasError = true
    error.message = message
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: message,
      life: 3000
    })
  }
  
  const formatPetForSelector = (pet: Pet): PetForSelector => {
    const ownerName = pet.primaryContact?.name || '未設定'
    const contactPhone = pet.primaryContact?.phone || ''
    
    // 確保寵物名稱顯示正確，處理可能的 undefined 或空字串情況
    const petName = pet.petName || pet.name || `寵物 #${pet.petId}`
    
    return {
      ...pet,
      petName, // 確保 petName 字段有值
      displayName: `${petName} (${ownerName})`,
      ownerName,
      contactPhone
    }
  }
  
  // 主要方法
  const searchPets = async (query: string = '') => {
    if (loading.value) return
    
    loading.value = true
    clearError()
    searchQuery.value = query
    
    try {
      let response
      
      if (options.contactPersonId) {
        // 如果指定了聯絡人ID，獲取該聯絡人的寵物
        response = await petApi.getPets({ ownerId: options.contactPersonId, pageSize: 50 })
        response = response.data || response
      } else if (query && query.trim().length >= 2) {
        // 搜尋寵物
        response = await petApi.getPets({ 
          keyword: query.trim(), 
          pageSize: 50,
          page: 1
        })
        response = response.data || response
      } else if (query.trim().length === 0) {
        // 載入預設寵物列表
        response = await petApi.getPets({ 
          pageSize: 20,
          page: 1
        })
        response = response.data || response
      } else {
        // 查詢字串太短，清空結果
        pets.value = []
        return
      }
      
      // 格式化寵物資料
      const petsArray = Array.isArray(response) ? response : [response]
      pets.value = petsArray.map(formatPetForSelector)
      
    } catch (err: any) {
      console.error('搜尋寵物失敗:', err)
      setError(err.response?.data?.message || '搜尋寵物失敗，請稍後再試')
      pets.value = []
    } finally {
      loading.value = false
    }
  }
  
  const getPetById = async (petId: number): Promise<Pet | null> => {
    if (!petId) return null
    
    try {
      // 先檢查現有列表中是否有該寵物
      const existingPet = pets.value.find(p => p.petId === petId)
      if (existingPet) {
        return existingPet
      }
      
      // 如果沒有，從API獲取
      const pet = await petApi.getPet(petId)
      const formattedPet = formatPetForSelector(pet)
      
      // 添加到列表中（如果還沒有的話）
      if (!pets.value.find(p => p.petId === petId)) {
        pets.value.unshift(formattedPet)
      }
      
      return formattedPet
    } catch (err: any) {
      console.error('獲取寵物資訊失敗:', err)
      setError('無法載入寵物資訊')
      return null
    }
  }
  
  const refreshPets = async () => {
    await searchPets(searchQuery.value)
  }
  
  const selectPet = (pet: Pet) => {
    if (!selectedPets.value.find(p => p.petId === pet.petId)) {
      selectedPets.value.push(pet)
    }
  }
  
  const unselectPet = (petId: number) => {
    const index = selectedPets.value.findIndex(p => p.petId === petId)
    if (index > -1) {
      selectedPets.value.splice(index, 1)
    }
  }
  
  const clearSelection = () => {
    selectedPets.value = []
  }
  
  const isSelected = (petId: number): boolean => {
    return selectedPets.value.some(p => p.petId === petId)
  }
  
  // 預載入寵物列表
  const preloadPets = async (petIds: number[]) => {
    if (petIds.length === 0) return
    
    loading.value = true
    try {
      const loadPromises = petIds.map(id => getPetById(id))
      await Promise.all(loadPromises)
    } catch (err) {
      console.error('預載入寵物失敗:', err)
    } finally {
      loading.value = false
    }
  }
  
  // 統計信息
  const stats = computed(() => ({
    total: pets.value.length,
    selected: selectedPets.value.length
  }))
  
  // 初始化
  if (options.initialLoad !== false) {
    searchPets('')
  }
  
  return {
    // 響應式狀態
    loading,
    pets: filteredPets,
    searchQuery,
    selectedPets,
    error,
    
    // 計算屬性
    hasResults,
    isEmpty,
    stats,
    
    // 方法
    searchPets,
    getPetById,
    refreshPets,
    selectPet,
    unselectPet,
    clearSelection,
    isSelected,
    preloadPets,
    clearError
  }
}

// 輔助函數
export const calculateEndDate = (startDate: Date, months: number = 2): Date => {
  const endDate = new Date(startDate)
  endDate.setMonth(endDate.getMonth() + months)
  return endDate
}

export const calculateSubscriptionAmount = async (petId: number): Promise<number> => {
  // 注意：訂閱價格現在從 PetServicePrice 或 Service 表取得
  // 優先使用 PetServicePrice，其次使用 Service 預設值
  try {
    const { petServicePriceApi } = await import('@/api/petServicePrice')
    const price = await petServicePriceApi.getSubscriptionPrice(petId)
    return price ?? 0
  } catch (error) {
    console.error('取得訂閱價格失敗:', error)
    return 0
  }
}

export const generateSubscriptionName = (pet: Pet, months: number = 2): string => {
  const petName = pet.petName || pet.name || `寵物 #${pet.petId}`
  return `${petName} - ${months}個月包月`
}