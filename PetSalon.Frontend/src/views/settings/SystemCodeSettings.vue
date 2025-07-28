<template>
  <div class="system-code-settings">
    <div class="header">
      <h1>系統代碼維護</h1>
      <div class="header-actions">
        <el-button type="primary" @click="showCreateDialog = true">
          <el-icon><Plus /></el-icon>
          新增代碼
        </el-button>
      </div>
    </div>

    <!-- Filter and Search -->
    <el-card class="filter-card">
      <el-form :model="searchForm" label-width="100px" :inline="true">
        <el-form-item label="代碼類型">
          <el-select v-model="searchForm.type" placeholder="全部類型" clearable @change="handleSearch">
            <el-option label="全部類型" value="" />
            <el-option label="品種" value="Breed" />
            <el-option label="性別" value="Gender" />
            <el-option label="關係" value="Relationship" />
            <el-option label="服務類型" value="ServiceType" />
            <el-option label="預約狀態" value="ReservationStatus" />
            <el-option label="付款類型" value="PaymentType" />
            <el-option label="加購項目" value="AddonType" />
            <el-option label="包月狀態" value="SubscriptionStatus" />
          </el-select>
        </el-form-item>
        <el-form-item label="關鍵字">
          <el-input 
            v-model="searchForm.keyword" 
            placeholder="搜尋代碼或名稱"
            @input="handleSearch"
            clearable
          />
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="searchForm.isActive" placeholder="全部狀態" clearable @change="handleSearch">
            <el-option label="全部狀態" :value="null" />
            <el-option label="啟用" :value="true" />
            <el-option label="停用" :value="false" />
          </el-select>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- Data Table -->
    <el-card class="table-card">
      <el-table :data="filteredSystemCodes" v-loading="loading" stripe>
        <el-table-column prop="type" label="類型" width="120">
          <template #default="{ row }">
            <el-tag :type="getTypeTagType(row.type)">{{ getTypeName(row.type) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="code" label="代碼" width="150" />
        <el-table-column prop="name" label="名稱" width="150" />
        <el-table-column prop="value" label="值" width="150" />
        <el-table-column prop="sort" label="排序" width="80" align="center" />
        <el-table-column prop="isActive" label="狀態" width="80" align="center">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'danger'">
              {{ row.isActive ? '啟用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" align="center">
          <template #default="{ row }">
            <el-button size="small" @click="editSystemCode(row)">
              <el-icon><Edit /></el-icon>
              編輯
            </el-button>
            <el-button 
              size="small" 
              type="danger" 
              @click="deleteSystemCode(row)"
              :disabled="row.id <= 50"
            >
              <el-icon><Delete /></el-icon>
              刪除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- Create/Edit Dialog -->
    <el-dialog 
      v-model="showCreateDialog" 
      :title="editingCode ? '編輯系統代碼' : '新增系統代碼'"
      width="600px"
      @close="resetForm"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="100px"
      >
        <el-form-item label="代碼類型" prop="type">
          <el-select v-model="formData.type" placeholder="請選擇類型" :disabled="!!editingCode">
            <el-option label="品種" value="Breed" />
            <el-option label="性別" value="Gender" />
            <el-option label="關係" value="Relationship" />
            <el-option label="服務類型" value="ServiceType" />
            <el-option label="預約狀態" value="ReservationStatus" />
            <el-option label="付款類型" value="PaymentType" />
            <el-option label="加購項目" value="AddonType" />
            <el-option label="包月狀態" value="SubscriptionStatus" />
          </el-select>
        </el-form-item>
        
        <el-form-item label="代碼" prop="code">
          <el-input 
            v-model="formData.code" 
            placeholder="請輸入代碼 (英文大寫)"
            :disabled="!!editingCode"
            @input="formData.code = $event.toUpperCase()"
          />
        </el-form-item>
        
        <el-form-item label="名稱" prop="name">
          <el-input v-model="formData.name" placeholder="請輸入名稱" />
        </el-form-item>
        
        <el-form-item label="值" prop="value">
          <el-input v-model="formData.value" placeholder="請輸入值" />
        </el-form-item>
        
        <el-form-item label="排序" prop="sort">
          <el-input-number v-model="formData.sort" :min="1" :max="999" />
        </el-form-item>
        
        <el-form-item label="狀態" prop="isActive">
          <el-switch v-model="formData.isActive" active-text="啟用" inactive-text="停用" />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="showCreateDialog = false">取消</el-button>
          <el-button type="primary" :loading="submitLoading" @click="handleSubmit">
            {{ editingCode ? '更新' : '新增' }}
          </el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import { commonApi, type SystemCode } from '@/api/common'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()

// Reactive data
const loading = ref(false)
const submitLoading = ref(false)
const showCreateDialog = ref(false)
const editingCode = ref<SystemCode | null>(null)
const systemCodes = ref<SystemCode[]>([])

// Form reference
const formRef = ref<FormInstance>()

// Search form
const searchForm = reactive({
  type: '',
  keyword: '',
  isActive: null as boolean | null
})

// Form data
const formData = reactive({
  type: '',
  code: '',
  name: '',
  value: '',
  sort: 1,
  isActive: true
})

// Form validation rules
const formRules: FormRules = {
  type: [
    { required: true, message: '請選擇代碼類型', trigger: 'change' }
  ],
  code: [
    { required: true, message: '請輸入代碼', trigger: 'blur' },
    { pattern: /^[A-Z_]+$/, message: '代碼只能包含大寫字母和下劃線', trigger: 'blur' }
  ],
  name: [
    { required: true, message: '請輸入名稱', trigger: 'blur' },
    { max: 50, message: '名稱長度不能超過 50 個字符', trigger: 'blur' }
  ],
  value: [
    { required: true, message: '請輸入值', trigger: 'blur' },
    { max: 100, message: '值長度不能超過 100 個字符', trigger: 'blur' }
  ],
  sort: [
    { required: true, message: '請輸入排序', trigger: 'blur' },
    { type: 'number', min: 1, max: 999, message: '排序應在 1-999 之間', trigger: 'blur' }
  ]
}

// Computed
const filteredSystemCodes = computed(() => {
  let result = systemCodes.value

  if (searchForm.type) {
    result = result.filter(item => item.type === searchForm.type)
  }

  if (searchForm.keyword) {
    const keyword = searchForm.keyword.toLowerCase()
    result = result.filter(item => 
      item.code.toLowerCase().includes(keyword) ||
      item.name.toLowerCase().includes(keyword) ||
      item.value.toLowerCase().includes(keyword)
    )
  }

  if (searchForm.isActive !== null) {
    result = result.filter(item => item.isActive === searchForm.isActive)
  }

  return result.sort((a, b) => {
    if (a.type !== b.type) {
      return a.type.localeCompare(b.type)
    }
    return a.sort - b.sort
  })
})

// Methods
const getTypeName = (type: string) => {
  const typeMap: Record<string, string> = {
    'Breed': '品種',
    'Gender': '性別', 
    'Relationship': '關係',
    'ServiceType': '服務類型',
    'ReservationStatus': '預約狀態',
    'PaymentType': '付款類型',
    'AddonType': '加購項目',
    'SubscriptionStatus': '包月狀態'
  }
  return typeMap[type] || type
}

const getTypeTagType = (type: string) => {
  const tagMap: Record<string, string> = {
    'Breed': 'primary',
    'Gender': 'success',
    'Relationship': 'warning',
    'ServiceType': 'info',
    'ReservationStatus': 'primary',
    'PaymentType': 'success',
    'AddonType': 'warning',
    'SubscriptionStatus': 'info'
  }
  return tagMap[type] || ''
}

const loadSystemCodes = async () => {
  try {
    loading.value = true
    const data = await commonApi.getAllSystemCodes()
    systemCodes.value = data
  } catch (error) {
    ElMessage.error('載入系統代碼失敗')
    console.error('Failed to load system codes:', error)
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  // Filter is handled by computed property
}

const editSystemCode = (code: SystemCode) => {
  editingCode.value = code
  Object.assign(formData, {
    type: code.type,
    code: code.code,
    name: code.name,
    value: code.value,
    sort: code.sort,
    isActive: code.isActive
  })
  showCreateDialog.value = true
}

const deleteSystemCode = async (code: SystemCode) => {
  try {
    await ElMessageBox.confirm(
      `確定要刪除系統代碼「${code.name}」嗎？`,
      '確認刪除',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )

    await commonApi.deleteSystemCode(code.id)
    
    // Remove from local array after successful deletion
    const index = systemCodes.value.findIndex(item => item.id === code.id)
    if (index > -1) {
      systemCodes.value.splice(index, 1)
    }
    
    ElMessage.success('刪除成功')
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗')
      console.error('Failed to delete system code:', error)
    }
  }
}

const handleSubmit = async () => {
  if (!formRef.value) return

  try {
    const valid = await formRef.value.validate()
    if (!valid) return

    submitLoading.value = true

    if (editingCode.value) {
      // Update existing code
      const updatedCode = {
        ...editingCode.value,
        ...formData,
        updateTime: new Date().toISOString(),
        updateUser: authStore.currentUser?.name || 'System'
      }
      
      await commonApi.updateSystemCode(updatedCode)
      
      // Update local array after successful update
      const index = systemCodes.value.findIndex(item => item.id === editingCode.value!.id)
      if (index > -1) {
        systemCodes.value[index] = updatedCode
      }
      
      ElMessage.success('更新成功')
    } else {
      // Create new code
      const newCodeData = {
        ...formData,
        createTime: new Date().toISOString(),
        createUser: authStore.currentUser?.name || 'System',
        updateTime: new Date().toISOString(),
        updateUser: authStore.currentUser?.name || 'System'
      }
      
      const result = await commonApi.createSystemCode(newCodeData)
      
      // Add to local array after successful creation
      systemCodes.value.push(result)
      
      ElMessage.success('新增成功')
    }

    showCreateDialog.value = false
    resetForm()
  } catch (error) {
    ElMessage.error(editingCode.value ? '更新失敗' : '新增失敗')
    console.error('Failed to submit system code:', error)
  } finally {
    submitLoading.value = false
  }
}

const resetForm = () => {
  editingCode.value = null
  Object.assign(formData, {
    type: '',
    code: '',
    name: '',
    value: '',
    sort: 1,
    isActive: true
  })
  if (formRef.value) {
    formRef.value.resetFields()
  }
}

// Initialize
onMounted(() => {
  loadSystemCodes()
})
</script>

<style scoped>
.system-code-settings {
  padding: 20px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.header h1 {
  margin: 0;
  color: #333;
}

.header-actions {
  display: flex;
  gap: 12px;
}

.filter-card, .table-card {
  margin-bottom: 20px;
}

.dialog-footer {
  text-align: right;
}

:deep(.el-form-item__label) {
  font-weight: 500;
  color: #333;
}

:deep(.el-table th) {
  background-color: #f8f9fa;
}
</style>