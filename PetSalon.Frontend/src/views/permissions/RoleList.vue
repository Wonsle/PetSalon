<template>
  <div class="role-list-container">
    <!-- Header -->
    <div class="page-header">
      <div class="header-left">
        <h2>🔐 角色管理</h2>
        <span class="total-count">共 {{ total }} 個角色</span>
      </div>
      <div class="header-right">
        <el-button type="primary" @click="openCreateDialog" v-permission="'role:create'">
          <el-icon><Plus /></el-icon>
          新增角色
        </el-button>
      </div>
    </div>

    <!-- Search and Filter -->
    <div class="search-section">
      <el-row :gutter="16">
        <el-col :span="6">
          <el-input
            v-model="searchForm.keyword"
            placeholder="搜尋角色名稱或代碼"
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
            v-model="searchForm.level"
            placeholder="角色等級"
            clearable
            @change="handleSearch"
          >
            <el-option label="超級管理員 (1)" :value="1" />
            <el-option label="管理員 (2)" :value="2" />
            <el-option label="經理 (3)" :value="3" />
            <el-option label="員工 (4)" :value="4" />
            <el-option label="訪客 (5)" :value="5" />
          </el-select>
        </el-col>
        <el-col :span="4">
          <el-select
            v-model="searchForm.isActive"
            placeholder="狀態"
            clearable
            @change="handleSearch"
          >
            <el-option label="啟用" :value="true" />
            <el-option label="停用" :value="false" />
          </el-select>
        </el-col>
        <el-col :span="4">
          <el-button @click="resetSearch">重置</el-button>
        </el-col>
      </el-row>
    </div>

    <!-- Role Cards -->
    <div class="role-grid" v-loading="loading">
      <div
        v-for="role in roles"
        :key="role.id"
        class="role-card"
        @click="viewRole(role)"
      >
        <div class="card-header">
          <div class="role-info">
            <div class="role-title">
              <h3>{{ role.name }}</h3>
              <el-tag :type="getRoleLevelType(role.level)" size="small">
                等級 {{ role.level }}
              </el-tag>
            </div>
            <div class="role-code">{{ role.code }}</div>
          </div>
          <div class="role-status">
            <el-switch
              v-model="role.isActive"
              :disabled="role.isSystem"
              @change="toggleRoleStatus(role)"
            />
          </div>
        </div>

        <div class="card-body">
          <p class="role-description">{{ role.description || '無描述' }}</p>
          
          <div class="role-stats">
            <div class="stat-item">
              <span class="stat-label">權限數量:</span>
              <span class="stat-value">{{ role.permissions.length }} 項</span>
            </div>
            <div class="stat-item">
              <span class="stat-label">系統角色:</span>
              <span class="stat-value">
                <el-tag :type="role.isSystem ? 'warning' : 'info'" size="small">
                  {{ role.isSystem ? '是' : '否' }}
                </el-tag>
              </span>
            </div>
          </div>

          <!-- Permission Preview -->
          <div class="permission-preview">
            <span class="preview-label">主要權限:</span>
            <div class="permission-tags">
              <el-tag
                v-for="permission in role.permissions.slice(0, 3)"
                :key="permission.id"
                size="small"
                class="permission-tag"
              >
                {{ permission.name }}
              </el-tag>
              <el-tag
                v-if="role.permissions.length > 3"
                size="small"
                type="info"
              >
                +{{ role.permissions.length - 3 }} 更多
              </el-tag>
            </div>
          </div>
        </div>

        <div class="card-footer">
          <div class="create-time">
            建立時間: {{ formatDate(role.createTime) }}
          </div>
          <div class="role-actions">
            <el-button
              type="primary"
              size="small"
              @click.stop="editRole(role)"
              v-permission="'role:update'"
            >
              編輯
            </el-button>
            <el-button
              type="danger"
              size="small"
              :disabled="role.isSystem"
              @click.stop="deleteRole(role)"
              v-permission="'role:delete'"
            >
              刪除
            </el-button>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-if="!loading && roles.length === 0" class="empty-state">
      <el-empty description="尚無角色資料">
        <el-button type="primary" @click="openCreateDialog" v-permission="'role:create'">
          新增第一個角色
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
        @size-change="loadRoles"
        @current-change="loadRoles"
      />
    </div>

    <!-- Create/Edit Dialog -->
    <RoleForm
      v-if="showDialog"
      :visible="showDialog"
      :role="selectedRole"
      @close="closeDialog"
      @success="handleFormSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Search } from '@element-plus/icons-vue'
import type { Role, RoleSearchParams } from '@/types/permission'
import { permissionApi } from '@/api/permission'
import RoleForm from '@/components/forms/RoleForm.vue'

// Data
const roles = ref<Role[]>([])
const total = ref(0)
const currentPage = ref(1)
const pageSize = ref(12)
const loading = ref(false)
const showDialog = ref(false)
const selectedRole = ref<Role | null>(null)

// Search form
const searchForm = reactive<RoleSearchParams>({
  keyword: '',
  level: undefined,
  isActive: undefined
})

// Methods
const loadRoles = async () => {
  loading.value = true
  try {
    const params = {
      ...searchForm,
      page: currentPage.value,
      pageSize: pageSize.value
    }
    const response = await permissionApi.getRoles(params)
    roles.value = response.data
    total.value = response.total
  } catch (error) {
    ElMessage.error('載入角色清單失敗')
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  currentPage.value = 1
  loadRoles()
}

const resetSearch = () => {
  searchForm.keyword = ''
  searchForm.level = undefined
  searchForm.isActive = undefined
  handleSearch()
}

const openCreateDialog = () => {
  selectedRole.value = null
  showDialog.value = true
}

const editRole = (role: Role) => {
  selectedRole.value = role
  showDialog.value = true
}

const viewRole = (role: Role) => {
  // TODO: Implement role detail view or edit
  editRole(role)
}

const toggleRoleStatus = async (role: Role) => {
  try {
    await permissionApi.toggleRoleStatus(role.id, role.isActive)
    ElMessage.success(role.isActive ? '角色已啟用' : '角色已停用')
  } catch (error) {
    // Revert the switch if API call fails
    role.isActive = !role.isActive
    ElMessage.error('狀態變更失敗')
  }
}

const deleteRole = async (role: Role) => {
  try {
    await ElMessageBox.confirm(
      `確定要刪除角色「${role.name}」嗎？此操作無法復原。`,
      '確認刪除',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await permissionApi.deleteRole(role.id)
    ElMessage.success('刪除成功')
    loadRoles()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗')
    }
  }
}

const closeDialog = () => {
  showDialog.value = false
  selectedRole.value = null
}

const handleFormSuccess = () => {
  closeDialog()
  loadRoles()
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('zh-TW')
}

const getRoleLevelType = (level: number) => {
  switch (level) {
    case 1:
      return 'danger'
    case 2:
      return 'warning'
    case 3:
      return 'primary'
    case 4:
      return 'success'
    default:
      return 'info'
  }
}

// Lifecycle
onMounted(() => {
  loadRoles()
})
</script>

<style scoped>
.role-list-container {
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

.role-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
  gap: 20px;
  margin-bottom: 24px;
}

.role-card {
  border: 1px solid #e4e7ed;
  border-radius: 12px;
  background: white;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.role-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
  border-color: #409eff;
}

.card-header {
  padding: 16px;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  border-bottom: 1px solid #f0f0f0;
}

.role-info {
  flex: 1;
}

.role-title {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 8px;
}

.role-title h3 {
  margin: 0;
  color: #303133;
  font-size: 18px;
  font-weight: 600;
}

.role-code {
  font-size: 12px;
  color: #909399;
  background: #f5f7fa;
  padding: 2px 8px;
  border-radius: 4px;
  display: inline-block;
}

.card-body {
  padding: 16px;
}

.role-description {
  margin: 0 0 16px 0;
  color: #606266;
  font-size: 14px;
  line-height: 1.5;
}

.role-stats {
  display: flex;
  justify-content: space-between;
  margin-bottom: 16px;
}

.stat-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.stat-label {
  font-size: 12px;
  color: #909399;
}

.stat-value {
  font-size: 14px;
  color: #303133;
  font-weight: 500;
}

.permission-preview {
  margin-top: 16px;
}

.preview-label {
  font-size: 12px;
  color: #909399;
  display: block;
  margin-bottom: 8px;
}

.permission-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
}

.permission-tag {
  max-width: 120px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.card-footer {
  padding: 12px 16px;
  border-top: 1px solid #f0f0f0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #fafafa;
  border-radius: 0 0 12px 12px;
}

.create-time {
  font-size: 12px;
  color: #909399;
}

.role-actions {
  display: flex;
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
  .role-grid {
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