<template>
  <div class="user-role-list-container">
    <!-- Header -->
    <div class="page-header">
      <div class="header-left">
        <h2>👤 用戶角色管理</h2>
        <span class="total-count">共 {{ total }} 筆分配記錄</span>
      </div>
      <div class="header-right">
        <el-button type="primary" @click="openAssignDialog" v-permission="'user-role:assign'">
          <el-icon><Plus /></el-icon>
          分配角色
        </el-button>
      </div>
    </div>

    <!-- Search and Filter -->
    <div class="search-section">
      <el-row :gutter="16">
        <el-col :span="6">
          <el-input
            v-model="searchForm.keyword"
            placeholder="搜尋用戶名稱"
            clearable
            @input="handleSearch"
          >
            <template #prefix>
              <el-icon><Search /></el-icon>
            </template>
          </el-input>
        </el-col>
        <el-col :span="4">
          <el-select
            v-model="searchForm.roleId"
            placeholder="篩選角色"
            clearable
            @change="handleSearch"
          >
            <el-option
              v-for="role in availableRoles"
              :key="role.id"
              :label="role.name"
              :value="role.id"
            />
          </el-select>
        </el-col>
        <el-col :span="4">
          <el-select
            v-model="searchForm.isActive"
            placeholder="狀態"
            clearable
            @change="handleSearch"
          >
            <el-option label="有效" :value="true" />
            <el-option label="無效" :value="false" />
          </el-select>
        </el-col>
        <el-col :span="4">
          <el-button @click="resetSearch">重置</el-button>
        </el-col>
      </el-row>
    </div>

    <!-- User Role Table -->
    <div class="table-container">
      <el-table
        :data="userRoles"
        v-loading="loading"
        stripe
        style="width: 100%"
      >
        <el-table-column prop="userName" label="用戶名稱" width="150">
          <template #default="scope">
            <div class="user-info">
              <el-avatar :size="32" class="user-avatar">
                {{ scope.row.userDisplayName.charAt(0) }}
              </el-avatar>
              <div class="user-details">
                <div class="user-name">{{ scope.row.userDisplayName }}</div>
                <div class="user-account">{{ scope.row.userName }}</div>
              </div>
            </div>
          </template>
        </el-table-column>
        
        <el-table-column prop="roleName" label="角色" width="150">
          <template #default="scope">
            <el-tag :type="getRoleType(scope.row.roleCode)" size="small">
              {{ scope.row.roleName }}
            </el-tag>
          </template>
        </el-table-column>
        
        <el-table-column prop="assignedBy" label="分配人" width="120" />
        
        <el-table-column prop="assignedTime" label="分配時間" width="180">
          <template #default="scope">
            {{ formatDateTime(scope.row.assignedTime) }}
          </template>
        </el-table-column>
        
        <el-table-column prop="expiryDate" label="到期時間" width="180">
          <template #default="scope">
            <span v-if="scope.row.expiryDate">
              <el-tag
                :type="getExpiryType(scope.row.expiryDate)"
                size="small"
              >
                {{ formatDateTime(scope.row.expiryDate) }}
              </el-tag>
            </span>
            <span v-else class="text-gray">永久有效</span>
          </template>
        </el-table-column>
        
        <el-table-column prop="isActive" label="狀態" width="100" align="center">
          <template #default="scope">
            <el-tag
              :type="scope.row.isActive ? 'success' : 'danger'"
              size="small"
            >
              {{ scope.row.isActive ? '有效' : '無效' }}
            </el-tag>
          </template>
        </el-table-column>
        
        <el-table-column label="操作" width="160" fixed="right">
          <template #default="scope">
            <el-button
              type="warning"
              size="small"
              @click="revokeRole(scope.row)"
              v-permission="'user-role:revoke'"
            >
              撤銷
            </el-button>
            <el-button
              type="primary"
              size="small"
              @click="viewPermissions(scope.row)"
            >
              查看權限
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>

    <!-- Empty State -->
    <div v-if="!loading && userRoles.length === 0" class="empty-state">
      <el-empty description="尚無用戶角色分配記錄">
        <el-button type="primary" @click="openAssignDialog" v-permission="'user-role:assign'">
          分配第一個角色
        </el-button>
      </el-empty>
    </div>

    <!-- Pagination -->
    <div class="pagination-wrapper" v-if="total > pageSize">
      <el-pagination
        v-model:current-page="currentPage"
        v-model:page-size="pageSize"
        :page-sizes="[20, 50, 100]"
        :total="total"
        layout="total, sizes, prev, pager, next"
        @size-change="loadUserRoles"
        @current-change="loadUserRoles"
      />
    </div>

    <!-- Role Assignment Dialog -->
    <UserRoleAssignForm
      v-if="showAssignDialog"
      :visible="showAssignDialog"
      @close="closeAssignDialog"
      @success="handleAssignSuccess"
    />

    <!-- User Permissions Dialog -->
    <UserPermissionsDialog
      v-if="showPermissionsDialog"
      :visible="showPermissionsDialog"
      :user-role="selectedUserRole"
      @close="closePermissionsDialog"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Search } from '@element-plus/icons-vue'
import type { UserRole, Role, UserRoleSearchParams } from '@/types/permission'
import { permissionApi } from '@/api/permission'
import UserRoleAssignForm from '@/components/forms/UserRoleAssignForm.vue'
import UserPermissionsDialog from '@/components/dialogs/UserPermissionsDialog.vue'

// Data
const userRoles = ref<UserRole[]>([])
const availableRoles = ref<Role[]>([])
const total = ref(0)
const currentPage = ref(1)
const pageSize = ref(20)
const loading = ref(false)
const showAssignDialog = ref(false)
const showPermissionsDialog = ref(false)
const selectedUserRole = ref<UserRole | null>(null)

// Search form
const searchForm = reactive<UserRoleSearchParams>({
  keyword: '',
  roleId: undefined,
  isActive: undefined
})

// Methods
const loadUserRoles = async () => {
  loading.value = true
  try {
    const params = {
      ...searchForm,
      page: currentPage.value,
      pageSize: pageSize.value
    }
    const response = await permissionApi.getUserRoles(params)
    userRoles.value = response.data
    total.value = response.total
  } catch (error) {
    ElMessage.error('載入用戶角色清單失敗')
  } finally {
    loading.value = false
  }
}

const loadAvailableRoles = async () => {
  try {
    const response = await permissionApi.getRoles({ isActive: true })
    availableRoles.value = response.data
  } catch (error) {
    console.error('載入角色清單失敗:', error)
  }
}

const handleSearch = () => {
  currentPage.value = 1
  loadUserRoles()
}

const resetSearch = () => {
  searchForm.keyword = ''
  searchForm.roleId = undefined
  searchForm.isActive = undefined
  handleSearch()
}

const openAssignDialog = () => {
  showAssignDialog.value = true
}

const closeAssignDialog = () => {
  showAssignDialog.value = false
}

const handleAssignSuccess = () => {
  closeAssignDialog()
  loadUserRoles()
}

const revokeRole = async (userRole: UserRole) => {
  try {
    await ElMessageBox.confirm(
      `確定要撤銷用戶「${userRole.userDisplayName}」的「${userRole.roleName}」角色嗎？`,
      '確認撤銷',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await permissionApi.revokeUserRole(userRole.userId, userRole.roleId)
    ElMessage.success('撤銷成功')
    loadUserRoles()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('撤銷失敗')
    }
  }
}

const viewPermissions = (userRole: UserRole) => {
  selectedUserRole.value = userRole
  showPermissionsDialog.value = true
}

const closePermissionsDialog = () => {
  showPermissionsDialog.value = false
  selectedUserRole.value = null
}

const formatDateTime = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleString('zh-TW')
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
  if (diffDays <= 30) return 'primary'
  return 'success'
}

// Lifecycle
onMounted(() => {
  loadUserRoles()
  loadAvailableRoles()
})
</script>

<style scoped>
.user-role-list-container {
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

.table-container {
  margin-bottom: 24px;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 12px;
}

.user-avatar {
  background: #409eff;
  color: white;
  font-weight: 500;
}

.user-details {
  flex: 1;
}

.user-name {
  font-weight: 500;
  color: #303133;
}

.user-account {
  font-size: 12px;
  color: #909399;
}

.text-gray {
  color: #909399;
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
</style>