<template>
  <div class="code-type-management">
    <!-- 頁面標題 -->
    <div class="page-header">
      <h1 class="page-title">
        <i class="pi pi-cog"></i>
        代碼類別維護
      </h1>
      <p class="page-subtitle">管理系統中使用的代碼類別定義</p>
    </div>

    <!-- 工具列 -->
    <div class="toolbar">
      <div class="toolbar-left">
        <Button
          label="新增代碼類別"
          icon="pi pi-plus"
          @click="openCreateDialog"
          class="p-button-success"
        />
        <Button
          label="重新整理"
          icon="pi pi-refresh"
          @click="loadCodeTypes"
          class="p-button-outlined"
          :loading="loading"
        />
      </div>
      <div class="toolbar-right">
        <span class="p-input-icon-left search-box">
          <i class="pi pi-search"></i>
          <InputText
            v-model="searchKeyword"
            placeholder="搜尋代碼類別..."
            @input="handleSearch"
            class="search-input"
          />
        </span>
      </div>
    </div>

    <!-- 資料表格 -->
    <Card class="data-card">
      <template #content>
        <DataTable
          :value="codeTypes"
          :loading="loading"
          :rows="pageSize"
          :totalRecords="totalRecords"
          :lazy="false"
          paginator
          :rowsPerPageOptions="[10, 25, 50]"
          responsiveLayout="scroll"
          stripedRows
          class="data-table"
          @page="onPageChange"
        >
          <template #empty>
            <div class="empty-state">
              <i class="pi pi-inbox empty-icon"></i>
              <p>沒有找到代碼類別資料</p>
            </div>
          </template>

          <Column field="id" header="ID" :sortable="true" style="width: 80px">
            <template #body="{ data }">
              <span class="id-badge">{{ data.id }}</span>
            </template>
          </Column>

          <Column field="codeType" header="代碼類型" :sortable="true" style="min-width: 150px">
            <template #body="{ data }">
              <span class="code-type-text">{{ data.codeType }}</span>
            </template>
          </Column>

          <Column field="name" header="類型名稱" :sortable="true" style="min-width: 200px">
            <template #body="{ data }">
              <span class="name-text">{{ data.name }}</span>
            </template>
          </Column>

          <Column field="description" header="描述說明" style="min-width: 250px">
            <template #body="{ data }">
              <span class="description-text">{{ data.description || '-' }}</span>
            </template>
          </Column>

          <Column field="createTime" header="建立時間" :sortable="true" style="min-width: 180px">
            <template #body="{ data }">
              <span class="time-text">{{ formatDateTime(data.createTime) }}</span>
            </template>
          </Column>

          <Column field="modifyTime" header="修改時間" :sortable="true" style="min-width: 180px">
            <template #body="{ data }">
              <span class="time-text">{{ formatDateTime(data.modifyTime) }}</span>
            </template>
          </Column>

          <Column header="操作" style="width: 120px" :frozen="true" alignFrozen="right">
            <template #body="{ data }">
              <div class="action-buttons">
                <Button
                  icon="pi pi-pencil"
                  class="p-button-text p-button-rounded p-button-warning"
                  @click="openEditDialog(data)"
                  v-tooltip.top="'編輯'"
                />
                <Button
                  icon="pi pi-trash"
                  class="p-button-text p-button-rounded p-button-danger"
                  @click="confirmDelete(data)"
                  v-tooltip.top="'刪除'"
                />
              </div>
            </template>
          </Column>
        </DataTable>
      </template>
    </Card>

    <!-- 新增/編輯對話框 -->
    <Dialog
      v-model:visible="dialogVisible"
      :header="dialogMode === 'create' ? '新增代碼類別' : '編輯代碼類別'"
      :style="{ width: '600px' }"
      :modal="true"
      :closable="true"
      @hide="resetDialog"
    >
      <form @submit.prevent="handleSubmit" class="dialog-form">
        <div class="form-group">
          <label for="codeType" class="form-label required">
            代碼類型代碼
          </label>
          <InputText
            id="codeType"
            v-model="formData.codeType"
            :class="{ 'p-invalid': formErrors.codeType }"
            placeholder="例如: GENDER"
            :disabled="dialogMode === 'edit'"
            class="form-input"
          />
          <small v-if="formErrors.codeType" class="p-error">
            {{ formErrors.codeType }}
          </small>
        </div>

        <div class="form-group">
          <label for="name" class="form-label required">
            類型名稱
          </label>
          <InputText
            id="name"
            v-model="formData.name"
            :class="{ 'p-invalid': formErrors.name }"
            placeholder="例如: 性別"
            class="form-input"
          />
          <small v-if="formErrors.name" class="p-error">
            {{ formErrors.name }}
          </small>
        </div>

        <div class="form-group">
          <label for="description" class="form-label">
            描述說明
          </label>
          <Textarea
            id="description"
            v-model="formData.description"
            rows="3"
            placeholder="描述此代碼類別的用途..."
            class="form-input"
          />
        </div>

        <div class="dialog-footer">
          <Button
            label="取消"
            icon="pi pi-times"
            @click="dialogVisible = false"
            class="p-button-text"
          />
          <Button
            label="確定"
            icon="pi pi-check"
            type="submit"
            :loading="submitting"
            class="p-button-primary"
          />
        </div>
      </form>
    </Dialog>

    <!-- 刪除確認對話框 -->
    <ConfirmDialog>
      <template #message="slotProps">
        <div class="confirm-content">
          <i class="pi pi-exclamation-triangle confirm-icon"></i>
          <span>{{ slotProps.message }}</span>
        </div>
      </template>
    </ConfirmDialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { useConfirm } from 'primevue/useconfirm'
import { useToast } from 'primevue/usetoast'
import { codeTypeApi } from '@/api/codeType'
import type { CodeType, CreateOrUpdateCodeTypeDto } from '@/types/codeType'

// PrimeVue 組件
import Button from 'primevue/button'
import Card from 'primevue/card'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import ConfirmDialog from 'primevue/confirmdialog'

// 組合式函數
const confirm = useConfirm()
const toast = useToast()

// 響應式資料
const loading = ref(false)
const submitting = ref(false)
const codeTypes = ref<CodeType[]>([])
const searchKeyword = ref('')
const totalRecords = ref(0)
const currentPage = ref(1)
const pageSize = ref(10)

// 對話框狀態
const dialogVisible = ref(false)
const dialogMode = ref<'create' | 'edit'>('create')
const editingItem = ref<CodeType | null>(null)

// 表單資料
const formData = reactive<CreateOrUpdateCodeTypeDto>({
  codeType: '',
  name: '',
  description: ''
})

// 表單驗證錯誤
const formErrors = reactive({
  codeType: '',
  name: ''
})

// 計算屬性
const filteredCodeTypes = computed(() => {
  if (!searchKeyword.value.trim()) {
    return codeTypes.value
  }

  const keyword = searchKeyword.value.toLowerCase()
  return codeTypes.value.filter(item =>
    item.codeType.toLowerCase().includes(keyword) ||
    item.name.toLowerCase().includes(keyword) ||
    (item.description && item.description.toLowerCase().includes(keyword))
  )
})

// 生命週期
onMounted(() => {
  loadCodeTypes()
})

// 方法
async function loadCodeTypes() {
  try {
    loading.value = true
    const data = await codeTypeApi.getAllCodeTypes()
    codeTypes.value = data
    totalRecords.value = data.length
  } catch (error: any) {
    console.error('載入代碼類別失敗:', error)
    toast.add({
      severity: 'error',
      summary: '載入失敗',
      detail: error.response?.data?.message || '載入代碼類別資料失敗',
      life: 3000
    })
  } finally {
    loading.value = false
  }
}

function handleSearch() {
  // 前端過濾，無需 API 呼叫
}

function openCreateDialog() {
  dialogMode.value = 'create'
  resetForm()
  dialogVisible.value = true
}

function openEditDialog(item: CodeType) {
  dialogMode.value = 'edit'
  editingItem.value = item
  formData.codeType = item.codeType
  formData.name = item.name
  formData.description = item.description || ''
  clearFormErrors()
  dialogVisible.value = true
}

function resetDialog() {
  resetForm()
  editingItem.value = null
}

function resetForm() {
  formData.codeType = ''
  formData.name = ''
  formData.description = ''
  clearFormErrors()
}

function clearFormErrors() {
  formErrors.codeType = ''
  formErrors.name = ''
}

function validateForm(): boolean {
  clearFormErrors()
  let isValid = true

  if (!formData.codeType.trim()) {
    formErrors.codeType = '代碼類型代碼為必填欄位'
    isValid = false
  }

  if (!formData.name.trim()) {
    formErrors.name = '類型名稱為必填欄位'
    isValid = false
  }

  return isValid
}

async function handleSubmit() {
  if (!validateForm()) {
    return
  }

  try {
    submitting.value = true

    if (dialogMode.value === 'create') {
      // 檢查代碼是否已存在
      const exists = await codeTypeApi.checkCodeTypeExists(formData.codeType)
      if (exists) {
        formErrors.codeType = '此代碼類型代碼已存在'
        return
      }

      await codeTypeApi.createCodeType(formData)
      toast.add({
        severity: 'success',
        summary: '建立成功',
        detail: '代碼類別已建立',
        life: 3000
      })
    } else if (editingItem.value) {
      await codeTypeApi.updateCodeType(editingItem.value.id, formData)
      toast.add({
        severity: 'success',
        summary: '更新成功',
        detail: '代碼類別已更新',
        life: 3000
      })
    }

    dialogVisible.value = false
    await loadCodeTypes()
  } catch (error: any) {
    console.error('提交失敗:', error)
    const errorMessage = error.response?.data?.message || '操作失敗'
    toast.add({
      severity: 'error',
      summary: '操作失敗',
      detail: errorMessage,
      life: 3000
    })
  } finally {
    submitting.value = false
  }
}

function confirmDelete(item: CodeType) {
  confirm.require({
    message: `確定要刪除代碼類別「${item.name}」嗎？此操作無法復原。`,
    header: '刪除確認',
    icon: 'pi pi-exclamation-triangle',
    acceptClass: 'p-button-danger',
    acceptLabel: '確定刪除',
    rejectLabel: '取消',
    accept: () => deleteCodeType(item.id)
  })
}

async function deleteCodeType(id: number) {
  try {
    await codeTypeApi.deleteCodeType(id)
    toast.add({
      severity: 'success',
      summary: '刪除成功',
      detail: '代碼類別已刪除',
      life: 3000
    })
    await loadCodeTypes()
  } catch (error: any) {
    console.error('刪除失敗:', error)
    const errorMessage = error.response?.data?.message || '刪除失敗'
    toast.add({
      severity: 'error',
      summary: '刪除失敗',
      detail: errorMessage,
      life: 3000
    })
  }
}

function onPageChange(event: any) {
  currentPage.value = event.page + 1
  pageSize.value = event.rows
}

function formatDateTime(dateTimeString?: string): string {
  if (!dateTimeString) return '-'

  try {
    const date = new Date(dateTimeString)
    return date.toLocaleString('zh-TW', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    })
  } catch {
    return '-'
  }
}
</script>

<style scoped>
.code-type-management {
  padding: 1.5rem;
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: 2rem;

  .page-title {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    font-size: 2rem;
    font-weight: 600;
    color: var(--text-color);
    margin: 0 0 0.5rem 0;

    .pi {
      color: var(--primary-color);
    }
  }

  .page-subtitle {
    color: var(--text-color-secondary);
    margin: 0;
    font-size: 1rem;
  }
}

.toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  gap: 1rem;
  flex-wrap: wrap;

  .toolbar-left {
    display: flex;
    gap: 0.75rem;
    flex-wrap: wrap;
  }

  .toolbar-right {
    .search-box {
      position: relative;

      .search-input {
        width: 300px;
        padding-left: 2.5rem;
      }

      .pi-search {
        position: absolute;
        left: 0.75rem;
        top: 50%;
        transform: translateY(-50%);
        color: var(--text-color-secondary);
      }
    }
  }
}

.data-card {
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
  border-radius: 8px;

  :deep(.p-card-content) {
    padding: 0;
  }
}

.data-table {
  :deep(.p-datatable-header) {
    background: var(--surface-50);
    border-bottom: 1px solid var(--surface-200);
  }

  :deep(.p-datatable-thead > tr > th) {
    background: var(--surface-50);
    color: var(--text-color);
    font-weight: 600;
    border-bottom: 1px solid var(--surface-200);
  }

  :deep(.p-datatable-tbody > tr:hover) {
    background: var(--surface-50);
  }
}

.empty-state {
  text-align: center;
  padding: 3rem 1rem;
  color: var(--text-color-secondary);

  .empty-icon {
    font-size: 3rem;
    margin-bottom: 1rem;
    color: var(--surface-400);
  }

  p {
    margin: 0;
    font-size: 1.1rem;
  }
}

.id-badge {
  display: inline-block;
  padding: 0.25rem 0.5rem;
  background: var(--surface-100);
  border-radius: 4px;
  font-weight: 500;
  font-size: 0.875rem;
}

.code-type-text {
  font-family: 'Courier New', monospace;
  font-weight: 600;
  color: var(--primary-color);
}

.name-text {
  font-weight: 500;
}

.description-text {
  color: var(--text-color-secondary);
}

.time-text {
  font-size: 0.875rem;
  color: var(--text-color-secondary);
}

.action-buttons {
  display: flex;
  gap: 0.25rem;
  justify-content: center;
}

.dialog-form {
  .form-group {
    margin-bottom: 1.5rem;

    .form-label {
      display: block;
      margin-bottom: 0.5rem;
      font-weight: 500;
      color: var(--text-color);

      &.required::after {
        content: ' *';
        color: var(--red-500);
      }
    }

    .form-input {
      width: 100%;
    }

    .p-error {
      display: block;
      margin-top: 0.25rem;
    }
  }

  .dialog-footer {
    display: flex;
    justify-content: flex-end;
    gap: 0.75rem;
    margin-top: 2rem;
    padding-top: 1.5rem;
    border-top: 1px solid var(--surface-200);
  }
}

.confirm-content {
  display: flex;
  align-items: center;
  gap: 0.75rem;

  .confirm-icon {
    font-size: 1.5rem;
    color: var(--orange-500);
  }
}

/* 響應式設計 */
@media (max-width: 768px) {
  .code-type-management {
    padding: 1rem;
  }

  .toolbar {
    flex-direction: column;
    align-items: stretch;

    .toolbar-left,
    .toolbar-right {
      width: 100%;
    }

    .search-box .search-input {
      width: 100%;
    }
  }

  .data-table {
    :deep(.p-datatable-wrapper) {
      overflow-x: auto;
    }
  }
}
</style>
