<template>
  <el-dialog
    :model-value="visible"
    title="分配用戶角色"
    width="600px"
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
      <el-form-item label="選擇用戶" prop="userId">
        <el-select
          v-model="form.userId"
          placeholder="請選擇用戶"
          filterable
          remote
          :remote-method="searchUsers"
          :loading="userLoading"
          style="width: 100%"
          @change="handleUserChange"
        >
          <el-option
            v-for="user in users"
            :key="user.id"
            :label="`${user.name} (${user.userName})`"
            :value="user.id"
          />
        </el-select>
      </el-form-item>

      <!-- Selected User Info -->
      <div v-if="selectedUser" class="user-info-card">
        <el-card>
          <template #header>
            <span>用戶資訊</span>
          </template>
          <div class="user-details">
            <div class="user-summary">
              <el-avatar :size="50" class="user-avatar">
                {{ selectedUser.name.charAt(0) }}
              </el-avatar>
              <div class="user-info">
                <p><strong>姓名:</strong> {{ selectedUser.name }}</p>
                <p><strong>帳號:</strong> {{ selectedUser.userName }}</p>
                <p><strong>信箱:</strong> {{ selectedUser.email || '未設定' }}</p>
              </div>
            </div>
            
            <!-- Current Roles -->
            <div v-if="currentRoles.length > 0" class="current-roles">
              <p><strong>目前角色:</strong></p>
              <div class="role-tags">
                <el-tag
                  v-for="role in currentRoles"
                  :key="role.id"
                  :type="getRoleType(role.code)"
                  size="small"
                  class="role-tag"
                >
                  {{ role.name }}
                </el-tag>
              </div>
            </div>
          </div>
        </el-card>
      </div>

      <el-form-item label="分配角色" prop="roleIds">
        <div class="role-selection">
          <el-checkbox-group v-model="form.roleIds" class="role-checkboxes">
            <el-checkbox
              v-for="role in availableRoles"
              :key="role.id"
              :value="role.id"
              :disabled="role.level < currentUserLevel"
              class="role-checkbox"
            >
              <div class="role-option">
                <div class="role-header">
                  <span class="role-name">{{ role.name }}</span>
                  <el-tag :type="getRoleType(role.code)" size="small">
                    等級 {{ role.level }}
                  </el-tag>
                </div>
                <div class="role-description">{{ role.description }}</div>
                <div v-if="role.level < currentUserLevel" class="role-warning">
                  <el-text type="warning" size="small">
                    權限等級高於您的等級，無法分配
                  </el-text>
                </div>
              </div>
            </el-checkbox>
          </el-checkbox-group>
        </div>
      </el-form-item>

      <el-form-item label="到期時間">
        <el-date-picker
          v-model="form.expiryDate"
          type="datetime"
          placeholder="選擇到期時間（可選）"
          style="width: 100%"
          :disabled-date="disabledDate"
        />
        <div class="form-tip">
          不設定到期時間則為永久有效
        </div>
      </el-form-item>

      <!-- Role Conflicts Warning -->
      <div v-if="hasRoleConflicts" class="role-conflicts">
        <el-alert
          title="角色衝突警告"
          type="warning"
          :closable="false"
          show-icon
        >
          <template #default>
            <p>選擇的角色中存在權限重疊，建議只分配一個主要角色：</p>
            <ul>
              <li v-for="conflict in roleConflicts" :key="conflict">
                {{ conflict }}
              </li>
            </ul>
          </template>
        </el-alert>
      </div>

      <!-- Assignment Summary -->
      <div v-if="form.roleIds.length > 0" class="assignment-summary">
        <el-card>
          <template #header>
            <span>分配摘要</span>
          </template>
          <div class="summary-content">
            <p><strong>將為用戶分配以下角色：</strong></p>
            <div class="selected-roles">
              <el-tag
                v-for="roleId in form.roleIds"
                :key="roleId"
                :type="getRoleType(getRoleCode(roleId))"
                size="small"
                class="summary-tag"
              >
                {{ getRoleName(roleId) }}
              </el-tag>
            </div>
            <p v-if="form.expiryDate">
              <strong>到期時間:</strong> {{ formatDate(form.expiryDate) }}
            </p>
          </div>
        </el-card>
      </div>
    </el-form>

    <template #footer>
      <div class="dialog-footer">
        <el-button @click="handleClose">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="handleSubmit">
          分配角色
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import type { Role, UserRoleAssignRequest } from '@/types/permission'
import { permissionApi } from '@/api/permission'
import { useAuthStore } from '@/stores/auth'

interface User {
  id: number
  userName: string
  name: string
  email?: string
}

interface Props {
  visible: boolean
}

interface Emits {
  (e: 'close'): void
  (e: 'success'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const authStore = useAuthStore()

// Refs
const formRef = ref<FormInstance>()
const submitting = ref(false)
const userLoading = ref(false)

// Data
const users = ref<User[]>([])
const availableRoles = ref<Role[]>([])
const selectedUser = ref<User | null>(null)
const currentRoles = ref<Role[]>([])

// Computed
const currentUserLevel = computed(() => {
  // 獲取當前用戶的最低等級（最高權限）
  const userRoles = authStore.currentUser?.roles || []
  if (userRoles.includes('Admin')) return 1
  if (userRoles.includes('Manager')) return 2
  if (userRoles.includes('Designer')) return 3
  return 4
})

const hasRoleConflicts = computed(() => {
  return roleConflicts.value.length > 0
})

const roleConflicts = computed(() => {
  const conflicts: string[] = []
  const selectedRoles = availableRoles.value.filter(role => 
    form.roleIds.includes(role.id)
  )
  
  // 檢查是否同時選擇了不同等級的角色
  const levels = selectedRoles.map(role => role.level)
  const uniqueLevels = [...new Set(levels)]
  
  if (uniqueLevels.length > 1) {
    conflicts.push('選擇了不同等級的角色，可能導致權限混亂')
  }
  
  return conflicts
})

// Form data
const form = reactive<UserRoleAssignRequest>({
  userId: 0,
  roleIds: [],
  expiryDate: undefined
})

// Form rules
const rules: FormRules = {
  userId: [
    { required: true, message: '請選擇用戶', trigger: 'change' }
  ],
  roleIds: [
    { type: 'array', min: 1, message: '請至少選擇一個角色', trigger: 'change' }
  ]
}

// Methods
const loadAvailableRoles = async () => {
  try {
    const response = await permissionApi.getRoles({ isActive: true })
    availableRoles.value = response.data
  } catch (error) {
    ElMessage.error('載入角色清單失敗')
  }
}

const searchUsers = async (query: string) => {
  if (!query) {
    users.value = []
    return
  }
  
  userLoading.value = true
  try {
    // TODO: 實現用戶搜索 API
    // const response = await userApi.searchUsers({ keyword: query, limit: 20 })
    // users.value = response.data
    
    // 模擬數據
    users.value = [
      { id: 1, userName: 'admin', name: '系統管理員', email: 'admin@example.com' },
      { id: 2, userName: 'manager', name: '店長', email: 'manager@example.com' },
      { id: 3, userName: 'stylist', name: '設計師', email: 'stylist@example.com' }
    ].filter(user => 
      user.name.includes(query) || user.userName.includes(query)
    )
  } catch (error) {
    ElMessage.error('搜尋用戶失敗')
  } finally {
    userLoading.value = false
  }
}

const handleUserChange = async (userId: number) => {
  const user = users.value.find(u => u.id === userId)
  selectedUser.value = user || null
  
  if (userId) {
    try {
      // 載入用戶當前角色
      const userRoleResponse = await permissionApi.getUserRoles({ 
        keyword: user?.userName, 
        isActive: true 
      })
      currentRoles.value = userRoleResponse.data.map(ur => ({
        id: ur.roleId,
        name: ur.roleName,
        code: ur.roleCode,
        description: '',
        level: 0,
        permissions: [],
        isActive: true,
        isSystem: false,
        createTime: '',
        updateTime: ''
      }))
    } catch (error) {
      console.error('載入用戶角色失敗:', error)
    }
  } else {
    currentRoles.value = []
  }
}

const disabledDate = (date: Date) => {
  // 不能選擇過去的時間
  return date < new Date()
}

const getRoleType = (roleCode: string) => {
  switch (roleCode) {
    case 'admin':
      return 'danger'
    case 'manager':
      return 'warning'
    case 'designer':
      return 'primary'
    default:
      return 'info'
  }
}

const getRoleName = (roleId: number) => {
  const role = availableRoles.value.find(r => r.id === roleId)
  return role ? role.name : `角色 ${roleId}`
}

const getRoleCode = (roleId: number) => {
  const role = availableRoles.value.find(r => r.id === roleId)
  return role ? role.code : ''
}

const formatDate = (date: string | Date) => {
  return new Date(date).toLocaleString('zh-TW')
}

const handleSubmit = async () => {
  if (!formRef.value) return
  
  try {
    const valid = await formRef.value.validate()
    if (!valid) return
    
    submitting.value = true
    
    const requestData = {
      ...form,
      expiryDate: form.expiryDate ? new Date(form.expiryDate).toISOString() : undefined
    }
    
    await permissionApi.assignUserRoles(requestData)
    ElMessage.success('角色分配成功')
    emit('success')
  } catch (error: any) {
    ElMessage.error(error.response?.data?.message || '分配失敗')
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
  selectedUser.value = null
  currentRoles.value = []
  users.value = []
}

// Lifecycle
onMounted(() => {
  loadAvailableRoles()
})
</script>

<style scoped>
.user-info-card {
  margin: 16px 0;
}

.user-summary {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-bottom: 16px;
}

.user-avatar {
  background: #409eff;
  color: white;
  font-weight: 500;
}

.user-info p {
  margin: 4px 0;
  color: #606266;
}

.user-info strong {
  color: #303133;
}

.current-roles {
  margin-top: 16px;
  padding-top: 16px;
  border-top: 1px solid #f0f0f0;
}

.role-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin-top: 8px;
}

.role-tag {
  margin-right: 0;
}

.role-selection {
  width: 100%;
  border: 1px solid #e4e7ed;
  border-radius: 6px;
  padding: 16px;
  max-height: 300px;
  overflow-y: auto;
}

.role-checkboxes {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.role-checkbox {
  width: 100%;
  margin-right: 0;
}

.role-checkbox :deep(.el-checkbox__label) {
  width: 100%;
}

.role-option {
  width: 100%;
}

.role-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 4px;
}

.role-name {
  font-weight: 500;
  color: #303133;
}

.role-description {
  font-size: 12px;
  color: #909399;
  margin-bottom: 4px;
}

.role-warning {
  margin-top: 4px;
}

.form-tip {
  font-size: 12px;
  color: #909399;
  margin-top: 4px;
}

.role-conflicts {
  margin: 16px 0;
}

.assignment-summary {
  margin-top: 16px;
}

.summary-content p {
  margin: 8px 0;
}

.selected-roles {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin: 8px 0;
}

.summary-tag {
  margin-right: 0;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style>