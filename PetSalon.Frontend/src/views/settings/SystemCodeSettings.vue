<template>
  <div class="system-code-settings">
    <Card>
      <template #header>
        <div class="header">
          <h2>🔧 系統代碼維護</h2>
          <Button
            label="新增代碼"
            icon="pi pi-plus"
            @click="showCreateDialog = true"
          />
        </div>
      </template>
      <template #content>
        <!-- Filter and Search -->
        <div class="filter-section">
          <div class="p-fluid">
            <div class="p-grid p-align-center">
              <div class="p-col-12 p-md-3">
                <label for="type-select">代碼類型</label>
                <Select
                  id="type-select"
                  v-model="searchForm.type"
                  :options="typeOptions"
                    optionLabel="label"
                    optionValue="value"
                  placeholder="全部類型"
                  showClear
                  @change="handleSearch"
                />
              </div>
              <div class="p-col-12 p-md-3">
                <label for="keyword-input">關鍵字</label>
                <InputText
                  id="keyword-input"
                  v-model="searchForm.keyword"
                  placeholder="搜尋代碼或名稱"
                  @input="handleSearch"
                />
              </div>
              <div class="p-col-12 p-md-3">
                <label for="status-select">狀態</label>
                <Select
                  id="status-select"
                  v-model="searchForm.isActive"
                  :options="statusOptions"
                    optionLabel="label"
                    optionValue="value"
                  placeholder="全部狀態"
                  showClear
                  @change="handleSearch"
                />
              </div>
            </div>
          </div>
        </div>

        <!-- Data Table -->
        <DataTable
          :value="filteredSystemCodes"
          :loading="loading"
          stripedRows
          paginator
          :rows="10"
          :rowsPerPageOptions="[10, 25, 50]"
          class="p-mt-4"
        >
          <Column field="type" header="類型" style="min-width: 120px">
            <template #body="{ data }">
              <Tag :severity="getTypeTagType(data.type)">{{ getTypeName(data.type) }}</Tag>
            </template>
          </Column>
          <Column field="code" header="代碼" style="min-width: 150px" />
          <Column field="name" header="名稱" style="min-width: 150px" />
          <Column field="value" header="值" style="min-width: 150px" />
          <Column field="sort" header="排序" style="min-width: 80px; text-align: center" />
          <Column field="isActive" header="狀態" style="min-width: 80px; text-align: center">
            <template #body="{ data }">
              <Tag :severity="data.isActive ? 'success' : 'danger'">
                {{ data.isActive ? '啟用' : '停用' }}
              </Tag>
            </template>
          </Column>
          <Column header="操作" style="min-width: 200px; text-align: center">
            <template #body="{ data }">
              <Button
                icon="pi pi-pencil"
                label="編輯"
                size="small"
                @click="editSystemCode(data)"
                class="p-mr-2"
              />
              <Button
                icon="pi pi-trash"
                label="刪除"
                size="small"
                severity="danger"
                @click="deleteSystemCode(data)"
                :disabled="data.id <= 50"
              />
            </template>
          </Column>
        </DataTable>
      </template>
    </Card>

    <!-- Create/Edit Dialog -->
    <Dialog
      v-model:visible="showCreateDialog"
      :header="editingCode ? '編輯系統代碼' : '新增系統代碼'"
      :style="{ width: '600px' }"
      @hide="resetForm"
      modal
    >
      <div class="p-fluid">
        <div class="field">
          <label for="form-type">代碼類型 *</label>
          <Select
            id="form-type"
            v-model="formData.type"
            :options="typeValueOptions"
            placeholder="請選擇類型"
            :disabled="!!editingCode"
            :class="{ 'p-invalid': formErrors.type }"
          />
          <small v-if="formErrors.type" class="p-error">{{ formErrors.type }}</small>
        </div>

        <div class="field">
          <label for="form-code">代碼 *</label>
          <InputText
            id="form-code"
            v-model="formData.code"
            placeholder="請輸入代碼 (英文大寫)"
            :disabled="!!editingCode"
            @input="handleCodeInput"
            :class="{ 'p-invalid': formErrors.code }"
          />
          <small v-if="formErrors.code" class="p-error">{{ formErrors.code }}</small>
        </div>

        <div class="field">
          <label for="form-name">名稱 *</label>
          <InputText
            id="form-name"
            v-model="formData.name"
            placeholder="請輸入名稱"
            :class="{ 'p-invalid': formErrors.name }"
          />
          <small v-if="formErrors.name" class="p-error">{{ formErrors.name }}</small>
        </div>

        <div class="field">
          <label for="form-value">值 *</label>
          <InputText
            id="form-value"
            v-model="formData.value"
            placeholder="請輸入值"
            :class="{ 'p-invalid': formErrors.value }"
          />
          <small v-if="formErrors.value" class="p-error">{{ formErrors.value }}</small>
        </div>

        <div class="field">
          <label for="form-sort">排序 *</label>
          <InputNumber
            id="form-sort"
            v-model="formData.sort"
            :min="1"
            :max="999"
            :class="{ 'p-invalid': formErrors.sort }"
          />
          <small v-if="formErrors.sort" class="p-error">{{ formErrors.sort }}</small>
        </div>

        <div class="field-checkbox">
          <ToggleSwitch
            id="form-active"
            v-model="formData.isActive"
          />
          <label for="form-active">啟用狀態</label>
        </div>
      </div>

      <template #footer>
        <Button label="取消" @click="showCreateDialog = false" />
        <Button
          :label="editingCode ? '更新' : '新增'"
          :loading="submitLoading"
          @click="handleSubmit"
        />
      </template>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import { commonApi, type SystemCode } from '@/api/common'
import { useAuthStore } from '@/stores/auth'

const toast = useToast()
const confirm = useConfirm()
const authStore = useAuthStore()

// Reactive data
const loading = ref(false)
const submitLoading = ref(false)
const showCreateDialog = ref(false)
const editingCode = ref<SystemCode | null>(null)
const systemCodes = ref<SystemCode[]>([])

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

// Form errors
const formErrors = reactive({
  type: '',
  code: '',
  name: '',
  value: '',
  sort: ''
})

// Options
const typeOptions = [
  { label: '品種', value: 'Breed' },
  { label: '性別', value: 'Gender' },
  { label: '關係', value: 'Relationship' },
  { label: '服務類型', value: 'ServiceType' },
  { label: '預約狀態', value: 'ReservationStatus' },
  { label: '付款類型', value: 'PaymentType' },
  { label: '加購項目', value: 'AddonType' },
  { label: '包月狀態', value: 'SubscriptionStatus' }
]

const typeValueOptions = typeOptions.slice(1) // Remove "全部類型" for form

const statusOptions = [
  { label: '全部狀態', value: null },
  { label: '啟用', value: true },
  { label: '停用', value: false }
]

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
    'Breed': 'info',
    'Gender': 'success',
    'Relationship': 'warning',
    'ServiceType': 'primary',
    'ReservationStatus': 'info',
    'PaymentType': 'success',
    'AddonType': 'warning',
    'SubscriptionStatus': 'primary'
  }
  return tagMap[type] || 'secondary'
}

const loadSystemCodes = async () => {
  try {
    loading.value = true
    const data = await commonApi.getAllSystemCodes()
    systemCodes.value = data
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: '載入失敗',
      detail: '載入系統代碼失敗',
      life: 3000
    })
    console.error('Failed to load system codes:', error)
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  // Filter is handled by computed property
}

const handleCodeInput = (event: Event) => {
  const target = event.target as HTMLInputElement
  formData.code = target.value.toUpperCase()
}

const validateForm = () => {
  // Reset errors
  Object.keys(formErrors).forEach(key => {
    formErrors[key as keyof typeof formErrors] = ''
  })

  let isValid = true

  if (!formData.type) {
    formErrors.type = '請選擇代碼類型'
    isValid = false
  }

  if (!formData.code) {
    formErrors.code = '請輸入代碼'
    isValid = false
  } else if (!/^[A-Z_]+$/.test(formData.code)) {
    formErrors.code = '代碼只能包含大寫字母和下劃線'
    isValid = false
  }

  if (!formData.name) {
    formErrors.name = '請輸入名稱'
    isValid = false
  } else if (formData.name.length > 50) {
    formErrors.name = '名稱長度不能超過 50 個字符'
    isValid = false
  }

  if (!formData.value) {
    formErrors.value = '請輸入值'
    isValid = false
  } else if (formData.value.length > 100) {
    formErrors.value = '值長度不能超過 100 個字符'
    isValid = false
  }

  if (!formData.sort || formData.sort < 1 || formData.sort > 999) {
    formErrors.sort = '排序應在 1-999 之間'
    isValid = false
  }

  return isValid
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

const deleteSystemCode = (code: SystemCode) => {
  confirm.require({
    message: `確定要刪除系統代碼「${code.name}」嗎？`,
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
        await commonApi.deleteSystemCode(code.id)

        // Remove from local array after successful deletion
        const index = systemCodes.value.findIndex(item => item.id === code.id)
        if (index > -1) {
          systemCodes.value.splice(index, 1)
        }

        toast.add({
          severity: 'success',
          summary: '刪除成功',
          detail: '系統代碼已成功刪除',
          life: 3000
        })
      } catch (error) {
        toast.add({
          severity: 'error',
          summary: '刪除失敗',
          detail: '刪除系統代碼失敗',
          life: 3000
        })
        console.error('Failed to delete system code:', error)
      }
    }
  })
}

const handleSubmit = async () => {
  if (!validateForm()) return

  try {
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

      toast.add({
        severity: 'success',
        summary: '更新成功',
        detail: '系統代碼已成功更新',
        life: 3000
      })
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

      toast.add({
        severity: 'success',
        summary: '新增成功',
        detail: '系統代碼已成功新增',
        life: 3000
      })
    }

    showCreateDialog.value = false
    resetForm()
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: editingCode.value ? '更新失敗' : '新增失敗',
      detail: editingCode.value ? '更新系統代碼失敗' : '新增系統代碼失敗',
      life: 3000
    })
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
  Object.keys(formErrors).forEach(key => {
    formErrors[key as keyof typeof formErrors] = ''
  })
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
}

.header h2 {
  margin: 0;
  color: var(--p-text-color);
}

.filter-section {
  margin-bottom: 20px;
  padding: 20px;
  background: var(--p-surface-100);
  border-radius: 8px;
}

.filter-section label {
  display: block;
  margin-bottom: 4px;
  font-weight: 500;
  color: var(--p-text-color);
}

.field {
  margin-bottom: 20px;
}

.field label {
  display: block;
  margin-bottom: 4px;
  font-weight: 500;
  color: var(--p-text-color);
}

.field-checkbox {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 20px;
}

.field-checkbox label {
  margin-bottom: 0;
}

.p-grid {
  display: flex;
  flex-wrap: wrap;
  margin: -0.5rem;
}

.p-col-12 {
  flex: 0 0 100%;
  padding: 0.5rem;
}

.p-md-3 {
  flex: 0 0 25%;
}

@media (max-width: 768px) {
  .p-md-3 {
    flex: 0 0 100%;
  }
}

.p-align-center {
  align-items: center;
}

.p-fluid .p-inputtext,
.p-fluid .p-select,
.p-fluid .p-inputnumber {
  width: 100%;
}

.p-mt-4 {
  margin-top: 1rem;
}

.p-mr-2 {
  margin-right: 0.5rem;
}
</style>