<template>
  <el-dialog
    :model-value="visible"
    :title="`${userRole?.userDisplayName} 的權限詳情`"
    width="700px"
    :before-close="handleClose"
    @update:model-value="$emit('close')"
  >
    <div v-if="userRole" class="user-permissions-content">
      <!-- User Info -->
      <div class="user-info-section">
        <el-card>
          <template #header>
            <span>用戶資訊</span>
          </template>
          <div class="user-info">
            <div class="user-summary">
              <el-avatar :size="50" class="user-avatar">
                {{ userRole.userDisplayName.charAt(0) }}
              </el-avatar>
              <div class="user-details">
                <h3>{{ userRole.userDisplayName }}</h3>
                <p>帳號: {{ userRole.userName }}</p>
                <p>角色: <el-tag :type="getRoleType(userRole.roleCode)">{{ userRole.roleName }}</el-tag></p>
                <p>分配時間: {{ formatDateTime(userRole.assignedTime) }}</p>
                <p v-if="userRole.expiryDate">
                  到期時間: 
                  <el-tag :type="getExpiryType(userRole.expiryDate)">
                    {{ formatDateTime(userRole.expiryDate) }}
                  </el-tag>
                </p>
              </div>
            </div>
          </div>
        </el-card>
      </div>

      <!-- Permissions List -->
      <div class="permissions-section">
        <el-card>
          <template #header>
            <div class="permissions-header">
              <span>權限清單</span>
              <el-tag type="info" size="small">
                共 {{ permissions.length }} 項權限
              </el-tag>
            </div>
          </template>
          
          <div v-loading="permissionsLoading" class="permissions-content">
            <div v-if="groupedPermissions.length === 0 && !permissionsLoading" class="no-permissions">
              <el-empty description="此角色暫無權限" />
            </div>
            
            <div v-else class="permission-groups">
              <div
                v-for="group in groupedPermissions"
                :key="group.module"
                class="permission-group"
              >
                <div class="group-header">
                  <h4>{{ getModuleName(group.module) }}</h4>
                  <el-tag size="small" type="primary">
                    {{ group.permissions.length }} 項
                  </el-tag>
                </div>
                
                <div class="group-permissions">
                  <div
                    v-for="permission in group.permissions"
                    :key="permission.id"
                    class="permission-item"
                  >
                    <div class="permission-info">
                      <div class="permission-name">
                        <el-icon class="permission-icon">
                          <Check />
                        </el-icon>
                        {{ permission.name }}
                      </div>
                      <div class="permission-description">
                        {{ permission.description }}
                      </div>
                      <div class="permission-meta">
                        <el-tag size="small" type="info">
                          {{ permission.action }}
                        </el-tag>
                        <span class="permission-code">{{ permission.code }}</span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </el-card>
      </div>

      <!-- Permission Summary -->
      <div class="summary-section">
        <el-card>
          <template #header>
            <span>權限摘要</span>
          </template>
          <div class="summary-stats">
            <el-row :gutter="16">
              <el-col :span="6">
                <div class="stat-item">
                  <div class="stat-icon create">
                    <el-icon><Plus /></el-icon>
                  </div>
                  <div class="stat-info">
                    <div class="stat-value">{{ getPermissionCountByAction('create') }}</div>
                    <div class="stat-label">新增權限</div>
                  </div>
                </div>
              </el-col>
              <el-col :span="6">
                <div class="stat-item">
                  <div class="stat-icon view">
                    <el-icon><View /></el-icon>
                  </div>
                  <div class="stat-info">
                    <div class="stat-value">{{ getPermissionCountByAction('view') }}</div>
                    <div class="stat-label">查看權限</div>
                  </div>
                </div>
              </el-col>
              <el-col :span="6">
                <div class="stat-item">
                  <div class="stat-icon update">
                    <el-icon><Edit /></el-icon>
                  </div>
                  <div class="stat-info">
                    <div class="stat-value">{{ getPermissionCountByAction('update') }}</div>
                    <div class="stat-label">編輯權限</div>
                  </div>
                </div>
              </el-col>
              <el-col :span="6">
                <div class="stat-item">
                  <div class="stat-icon delete">
                    <el-icon><Delete /></el-icon>
                  </div>
                  <div class="stat-info">
                    <div class="stat-value">{{ getPermissionCountByAction('delete') }}</div>
                    <div class="stat-label">刪除權限</div>
                  </div>
                </div>
              </el-col>
            </el-row>
          </div>
        </el-card>
      </div>
    </div>

    <template #footer>
      <div class="dialog-footer">
        <el-button @click="handleClose">關閉</el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { Check, Plus, View, Edit, Delete } from '@element-plus/icons-vue'
import type { UserRole, Permission, PermissionGroup } from '@/types/permission'
import { permissionApi } from '@/api/permission'

interface Props {
  visible: boolean
  userRole?: UserRole | null
}

interface Emits {
  (e: 'close'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

// Data
const permissions = ref<Permission[]>([])
const permissionsLoading = ref(false)

// Computed
const groupedPermissions = computed(() => {
  const groups: PermissionGroup[] = []
  const moduleMap = new Map<string, Permission[]>()
  
  permissions.value.forEach(permission => {
    const module = permission.module
    if (!moduleMap.has(module)) {
      moduleMap.set(module, [])
    }
    moduleMap.get(module)!.push(permission)
  })
  
  moduleMap.forEach((perms, module) => {
    groups.push({
      module,
      permissions: perms.sort((a, b) => a.name.localeCompare(b.name))
    })
  })
  
  return groups.sort((a, b) => a.module.localeCompare(b.module))
})

// Methods
const loadUserPermissions = async (userId: number) => {
  if (!userId) return
  
  permissionsLoading.value = true
  try {
    permissions.value = await permissionApi.getUserPermissions(userId)
  } catch (error) {
    console.error('載入用戶權限失敗:', error)
    permissions.value = []
  } finally {
    permissionsLoading.value = false
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

const getPermissionCountByAction = (action: string) => {
  return permissions.value.filter(p => p.action === action).length
}

const formatDateTime = (dateString: string) => {
  return new Date(dateString).toLocaleString('zh-TW')
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

const getExpiryType = (expiryDate: string) => {
  const expiry = new Date(expiryDate)
  const now = new Date()
  const diffDays = Math.ceil((expiry.getTime() - now.getTime()) / (1000 * 60 * 60 * 24))
  
  if (diffDays < 0) return 'danger'
  if (diffDays <= 7) return 'warning'
  return 'success'
}

const handleClose = () => {
  emit('close')
}

// Watch for userRole changes
watch(() => props.userRole, (newUserRole) => {
  if (newUserRole) {
    loadUserPermissions(newUserRole.userId)
  }
}, { immediate: true })
</script>

<style scoped>
.user-permissions-content {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.user-info-section {
  margin-bottom: 0;
}

.user-summary {
  display: flex;
  align-items: center;
  gap: 16px;
}

.user-avatar {
  background: #409eff;
  color: white;
  font-weight: 500;
}

.user-details h3 {
  margin: 0 0 8px 0;
  color: #303133;
}

.user-details p {
  margin: 4px 0;
  color: #606266;
}

.permissions-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.permissions-content {
  max-height: 400px;
  overflow-y: auto;
}

.no-permissions {
  text-align: center;
  padding: 40px 20px;
}

.permission-groups {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.permission-group {
  border: 1px solid #e4e7ed;
  border-radius: 6px;
  overflow: hidden;
}

.group-header {
  background: #f8f9fa;
  padding: 12px 16px;
  border-bottom: 1px solid #e4e7ed;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.group-header h4 {
  margin: 0;
  color: #303133;
  font-size: 16px;
}

.group-permissions {
  padding: 0;
}

.permission-item {
  padding: 12px 16px;
  border-bottom: 1px solid #f0f0f0;
}

.permission-item:last-child {
  border-bottom: none;
}

.permission-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.permission-name {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 500;
  color: #303133;
}

.permission-icon {
  color: #67c23a;
}

.permission-description {
  font-size: 14px;
  color: #606266;
}

.permission-meta {
  display: flex;
  align-items: center;
  gap: 12px;
}

.permission-code {
  font-size: 12px;
  color: #909399;
  background: #f5f7fa;
  padding: 2px 6px;
  border-radius: 3px;
}

.summary-stats {
  padding: 0;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 16px;
  border: 1px solid #e4e7ed;
  border-radius: 6px;
  background: #fafafa;
}

.stat-icon {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 18px;
}

.stat-icon.create {
  background: #67c23a;
}

.stat-icon.view {
  background: #409eff;
}

.stat-icon.update {
  background: #e6a23c;
}

.stat-icon.delete {
  background: #f56c6c;
}

.stat-info {
  flex: 1;
}

.stat-value {
  font-size: 24px;
  font-weight: 600;
  color: #303133;
  line-height: 1;
}

.stat-label {
  font-size: 12px;
  color: #909399;
  margin-top: 4px;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
}
</style>