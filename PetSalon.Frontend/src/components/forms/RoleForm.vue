<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? '編輯角色' : '新增角色'"
    width="800px"
    :before-close="handleClose"
    @update:model-value="$emit('close')"
  >
    <el-form
      ref="formRef"
      :model="form"
      :rules="rules"
      label-width="100px"
      @submit.prevent="handleSubmit"
    >
      <el-row :gutter="16">
        <el-col :span="12">
          <el-form-item label="角色名稱" prop="name">
            <el-input v-model="form.name" placeholder="請輸入角色名稱" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="角色代碼" prop="code">
            <el-input 
              v-model="form.code" 
              placeholder="請輸入角色代碼(英文)"
              :disabled="isEdit && selectedRole?.isSystem"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="16">
        <el-col :span="12">
          <el-form-item label="角色等級" prop="level">
            <el-select v-model="form.level" placeholder="請選擇角色等級">
              <el-option label="超級管理員 (1)" :value="1" />
              <el-option label="管理員 (2)" :value="2" />
              <el-option label="經理 (3)" :value="3" />
              <el-option label="員工 (4)" :value="4" />
              <el-option label="訪客 (5)" :value="5" />
            </el-select>
            <div class="form-tip">
              等級越低權限越高，1為最高等級
            </div>
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item label="角色描述">
        <el-input
          v-model="form.description"
          type="textarea"
          :rows="3"
          placeholder="請輸入角色描述"
        />
      </el-form-item>

      <!-- Permission Selection -->
      <el-form-item label="角色權限" prop="permissionIds">
        <div class="permission-selection">
          <div class="permission-header">
            <el-checkbox
              v-model="selectAll"
              :indeterminate="isIndeterminate"
              @change="handleSelectAll"
            >
              全選
            </el-checkbox>
            <span class="selected-count">
              已選擇 {{ form.permissionIds.length }} / {{ totalPermissions }} 項權限
            </span>
          </div>

          <div class="permission-groups">
            <div
              v-for="group in permissionGroups"
              :key="group.module"
              class="permission-group"
            >
              <div class="group-header">
                <el-checkbox
                  :model-value="isGroupSelected(group)"
                  :indeterminate="isGroupIndeterminate(group)"
                  @change="(val) => handleGroupSelect(group, val)"
                >
                  <strong>{{ getModuleName(group.module) }}</strong>
                </el-checkbox>
                <span class="group-count">
                  ({{ getSelectedCountInGroup(group) }} / {{ group.permissions.length }})
                </span>
              </div>
              
              <div class="group-permissions">
                <el-checkbox-group
                  v-model="form.permissionIds"
                  class="permission-checkboxes"
                >
                  <el-checkbox
                    v-for="permission in group.permissions"
                    :key="permission.id"
                    :value="permission.id"
                    class="permission-checkbox"
                  >
                    <div class="permission-info">
                      <span class="permission-name">{{ permission.name }}</span>
                      <span class="permission-desc">{{ permission.description }}</span>
                    </div>
                  </el-checkbox>
                </el-checkbox-group>
              </div>
            </div>
          </div>
        </div>
      </el-form-item>

      <!-- Selected Permissions Summary -->
      <div v-if="form.permissionIds.length > 0" class="selected-summary">
        <el-card>
          <template #header>
            <span>已選擇的權限摘要</span>
          </template>
          <div class="summary-content">
            <el-tag
              v-for="permissionId in form.permissionIds"
              :key="permissionId"
              size="small"
              closable
              @close="removePermission(permissionId)"
              class="summary-tag"
            >
              {{ getPermissionName(permissionId) }}
            </el-tag>
          </div>
        </el-card>
      </div>
    </el-form>

    <template #footer>
      <div class="dialog-footer">
        <el-button @click="handleClose">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="handleSubmit">
          {{ isEdit ? '更新' : '建立' }}
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch, computed, onMounted } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import type { Role, RoleCreateRequest, RoleUpdateRequest, Permission, PermissionGroup } from '@/types/permission'
import { permissionApi } from '@/api/permission'

interface Props {
  visible: boolean
  role?: Role | null
}

interface Emits {
  (e: 'close'): void
  (e: 'success'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

// Refs
const formRef = ref<FormInstance>()
const submitting = ref(false)
const selectAll = ref(false)

// Data
const permissionGroups = ref<PermissionGroup[]>([])
const allPermissions = ref<Permission[]>([])
const selectedRole = ref<Role | null>(null)

// Computed
const isEdit = computed(() => !!props.role)

const totalPermissions = computed(() => allPermissions.value.length)

const isIndeterminate = computed(() => {
  const selected = form.permissionIds.length
  return selected > 0 && selected < totalPermissions.value
})

// Form data
const form = reactive<RoleCreateRequest>({
  name: '',
  code: '',
  description: '',
  level: 4,
  permissionIds: []
})

// Form rules
const rules: FormRules = {
  name: [
    { required: true, message: '請輸入角色名稱', trigger: 'blur' },
    { min: 2, max: 50, message: '角色名稱長度應為 2-50 個字符', trigger: 'blur' }
  ],
  code: [
    { required: true, message: '請輸入角色代碼', trigger: 'blur' },
    { min: 2, max: 20, message: '角色代碼長度應為 2-20 個字符', trigger: 'blur' },
    { pattern: /^[a-zA-Z][a-zA-Z0-9_]*$/, message: '角色代碼只能包含英文字母、數字和下劃線，且以字母開頭', trigger: 'blur' }
  ],
  level: [
    { required: true, message: '請選擇角色等級', trigger: 'change' }
  ],
  permissionIds: [
    { type: 'array', min: 1, message: '請至少選擇一項權限', trigger: 'change' }
  ]
}

// Methods
const loadPermissions = async () => {
  try {
    const groups = await permissionApi.getPermissionGroups()
    permissionGroups.value = groups
    
    // Flatten all permissions
    allPermissions.value = groups.reduce((acc, group) => {
      return acc.concat(group.permissions)
    }, [] as Permission[])
  } catch (error) {
    ElMessage.error('載入權限清單失敗')
  }
}

const getModuleName = (module: string) => {
  const moduleNames: Record<string, string> = {
    'pet': '寵物管理',
    'contact': '聯絡人管理',
    'reservation': '預約管理',
    'subscription': '包月管理',
    'financial': '財務管理',
    'role': '角色管理',
    'user': '使用者管理',
    'system': '系統管理'
  }
  return moduleNames[module] || module
}

const isGroupSelected = (group: PermissionGroup) => {
  return group.permissions.every(p => form.permissionIds.includes(p.id))
}

const isGroupIndeterminate = (group: PermissionGroup) => {
  const selectedInGroup = group.permissions.filter(p => form.permissionIds.includes(p.id))
  return selectedInGroup.length > 0 && selectedInGroup.length < group.permissions.length
}

const getSelectedCountInGroup = (group: PermissionGroup) => {
  return group.permissions.filter(p => form.permissionIds.includes(p.id)).length
}

const handleSelectAll = (val: boolean) => {
  if (val) {
    form.permissionIds = allPermissions.value.map(p => p.id)
  } else {
    form.permissionIds = []
  }
}

const handleGroupSelect = (group: PermissionGroup, val: boolean) => {
  const groupPermissionIds = group.permissions.map(p => p.id)
  
  if (val) {
    // Add all permissions in this group
    groupPermissionIds.forEach(id => {
      if (!form.permissionIds.includes(id)) {
        form.permissionIds.push(id)
      }
    })
  } else {
    // Remove all permissions in this group
    form.permissionIds = form.permissionIds.filter(id => !groupPermissionIds.includes(id))
  }
}

const removePermission = (permissionId: number) => {
  const index = form.permissionIds.indexOf(permissionId)
  if (index > -1) {
    form.permissionIds.splice(index, 1)
  }
}

const getPermissionName = (permissionId: number) => {
  const permission = allPermissions.value.find(p => p.id === permissionId)
  return permission ? permission.name : `權限 ${permissionId}`
}

const handleSubmit = async () => {
  if (!formRef.value) return
  
  try {
    const valid = await formRef.value.validate()
    if (!valid) return
    
    submitting.value = true
    
    if (isEdit.value && props.role) {
      const updateData: RoleUpdateRequest = {
        ...form,
        id: props.role.id
      }
      await permissionApi.updateRole(updateData)
      ElMessage.success('更新成功')
    } else {
      await permissionApi.createRole(form)
      ElMessage.success('建立成功')
    }
    
    emit('success')
  } catch (error: any) {
    ElMessage.error(error.response?.data?.message || '操作失敗')
  } finally {
    submitting.value = false
  }
}

const handleClose = () => {
  emit('close')
}

const resetForm = () => {
  if (formRef.value) {
    formRef.value.resetFields()
  }
  form.permissionIds = []
  selectAll.value = false
}

// Watch for role changes
watch(() => props.role, (newRole) => {
  selectedRole.value = newRole
  if (newRole) {
    Object.assign(form, {
      name: newRole.name,
      code: newRole.code,
      description: newRole.description,
      level: newRole.level,
      permissionIds: newRole.permissions.map(p => p.id)
    })
  } else {
    resetForm()
  }
}, { immediate: true })

// Watch for dialog visibility
watch(() => props.visible, (visible) => {
  if (visible) {
    loadPermissions()
  } else {
    resetForm()
  }
})

// Watch permission selection for select all checkbox
watch(() => form.permissionIds.length, (newLength) => {
  selectAll.value = newLength === totalPermissions.value
})

// Lifecycle
onMounted(() => {
  loadPermissions()
})
</script>

<style scoped>
.form-tip {
  font-size: 12px;
  color: #909399;
  margin-top: 4px;
}

.permission-selection {
  width: 100%;
  border: 1px solid #e4e7ed;
  border-radius: 6px;
  padding: 16px;
}

.permission-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
  padding-bottom: 12px;
  border-bottom: 1px solid #f0f0f0;
}

.selected-count {
  font-size: 14px;
  color: #606266;
}

.permission-groups {
  max-height: 400px;
  overflow-y: auto;
}

.permission-group {
  margin-bottom: 20px;
}

.group-header {
  display: flex;
  align-items: center;
  margin-bottom: 12px;
  padding: 8px 12px;
  background: #f8f9fa;
  border-radius: 4px;
}

.group-count {
  margin-left: 8px;
  font-size: 12px;
  color: #909399;
}

.group-permissions {
  margin-left: 24px;
}

.permission-checkboxes {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.permission-checkbox {
  width: 100%;
  margin-right: 0;
}

.permission-checkbox :deep(.el-checkbox__label) {
  width: 100%;
}

.permission-info {
  display: flex;
  flex-direction: column;
  width: 100%;
}

.permission-name {
  font-weight: 500;
  color: #303133;
}

.permission-desc {
  font-size: 12px;
  color: #909399;
  margin-top: 2px;
}

.selected-summary {
  margin-top: 16px;
}

.summary-content {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.summary-tag {
  max-width: 200px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style>