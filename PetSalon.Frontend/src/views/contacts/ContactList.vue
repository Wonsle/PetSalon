<template>
  <div class="contact-list-container">
    <!-- Header -->
    <div class="page-header">
      <div class="header-left">
        <h2>👥 聯絡人管理</h2>
        <span class="total-count">共 {{ total }} 位聯絡人</span>
      </div>
      <div class="header-right">
        <el-button type="primary" @click="openCreateDialog">
          <el-icon><Plus /></el-icon>
          新增聯絡人
        </el-button>
      </div>
    </div>

    <!-- Search and Filter -->
    <div class="search-section">
      <el-row :gutter="16">
        <el-col :span="8">
          <el-input
            v-model="searchForm.keyword"
            placeholder="搜尋姓名、暱稱或電話"
            clearable
            @input="handleSearch"
          >
            <template #prefix>
              <el-icon><Search /></el-icon>
            </template>
          </el-input>
        </el-col>
        <el-col :span="6">
          <el-input
            v-model="searchForm.name"
            placeholder="搜尋姓名"
            clearable
            @input="handleSearch"
          />
        </el-col>
        <el-col :span="6">
          <el-input
            v-model="searchForm.contactNumber"
            placeholder="搜尋電話"
            clearable
            @input="handleSearch"
          />
        </el-col>
        <el-col :span="4">
          <el-button @click="resetSearch">重置</el-button>
        </el-col>
      </el-row>
    </div>

    <!-- Contact Table -->
    <div class="table-container">
      <el-table
        :data="contacts"
        v-loading="loading"
        stripe
        @row-click="viewContact"
        style="width: 100%"
      >
        <el-table-column prop="name" label="姓名" width="150">
          <template #default="scope">
            <div class="contact-name">
              <el-avatar :size="32" class="name-avatar">
                {{ scope.row.name.charAt(0) }}
              </el-avatar>
              <div class="name-info">
                <div class="main-name">{{ scope.row.name }}</div>
                <div v-if="scope.row.nickName" class="nick-name">{{ scope.row.nickName }}</div>
              </div>
            </div>
          </template>
        </el-table-column>
        
        <el-table-column prop="contactNumber" label="聯絡電話" width="140" />
        
        <el-table-column prop="relatedPets" label="關聯寵物" min-width="200">
          <template #default="scope">
            <div v-if="scope.row.relatedPets && scope.row.relatedPets.length > 0" class="pet-list">
              <el-tag
                v-for="pet in scope.row.relatedPets"
                :key="pet.petRelationId"
                size="small"
                class="pet-tag"
              >
                {{ pet.petName }}
              </el-tag>
            </div>
            <span v-else class="text-gray">無關聯寵物</span>
          </template>
        </el-table-column>
        
        <el-table-column prop="createTime" label="建立時間" width="120">
          <template #default="scope">
            {{ formatDate(scope.row.createTime) }}
          </template>
        </el-table-column>
        
        <el-table-column prop="modifyTime" label="修改時間" width="120">
          <template #default="scope">
            {{ formatDate(scope.row.modifyTime) }}
          </template>
        </el-table-column>
        
        <el-table-column label="操作" width="160" fixed="right">
          <template #default="scope">
            <el-button
              type="primary"
              size="small"
              @click.stop="editContact(scope.row)"
            >
              編輯
            </el-button>
            <el-button
              type="danger"
              size="small"
              @click.stop="deleteContact(scope.row)"
            >
              刪除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>

    <!-- Empty State -->
    <div v-if="!loading && contacts.length === 0" class="empty-state">
      <el-empty description="尚無聯絡人資料">
        <el-button type="primary" @click="openCreateDialog">
          新增第一位聯絡人
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
        @size-change="loadContacts"
        @current-change="loadContacts"
      />
    </div>

    <!-- Create/Edit Dialog -->
    <ContactForm
      v-if="showDialog"
      :visible="showDialog"
      :contact="selectedContact"
      @close="closeDialog"
      @success="handleFormSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Search } from '@element-plus/icons-vue'
import type { Contact, ContactSearchParams } from '@/types/contact'
import { contactApi } from '@/api/contact'
import ContactForm from '@/components/forms/ContactForm.vue'

// Data
const contacts = ref<Contact[]>([])
const total = ref(0)
const currentPage = ref(1)
const pageSize = ref(20)
const loading = ref(false)
const showDialog = ref(false)
const selectedContact = ref<Contact | null>(null)

// Search form
const searchForm = reactive<ContactSearchParams>({
  keyword: '',
  name: '',
  contactNumber: ''
})

// Methods
const loadContacts = async () => {
  loading.value = true
  try {
    const params = {
      ...searchForm,
      page: currentPage.value,
      pageSize: pageSize.value
    }
    const response = await contactApi.getContacts(params)
    contacts.value = response.data
    total.value = response.total
  } catch (error) {
    ElMessage.error('載入聯絡人清單失敗')
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  currentPage.value = 1
  loadContacts()
}

const resetSearch = () => {
  searchForm.keyword = ''
  searchForm.name = ''
  searchForm.contactNumber = ''
  handleSearch()
}

const openCreateDialog = () => {
  selectedContact.value = null
  showDialog.value = true
}

const editContact = (contact: Contact) => {
  selectedContact.value = contact
  showDialog.value = true
}

const viewContact = (contact: Contact) => {
  // TODO: Implement contact detail view
  editContact(contact)
}

const deleteContact = async (contact: Contact) => {
  try {
    await ElMessageBox.confirm(
      `確定要刪除聯絡人「${contact.name}」嗎？`,
      '確認刪除',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await contactApi.deleteContact(contact.contactPersonId)
    ElMessage.success('刪除成功')
    loadContacts()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗')
    }
  }
}

const closeDialog = () => {
  showDialog.value = false
  selectedContact.value = null
}

const handleFormSuccess = () => {
  closeDialog()
  loadContacts()
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('zh-TW')
}


// Lifecycle
onMounted(() => {
  loadContacts()
})
</script>

<style scoped>
.contact-list-container {
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

.contact-name {
  display: flex;
  align-items: center;
  gap: 8px;
}

.name-avatar {
  background: #409eff;
  color: white;
  font-weight: 500;
}

.name-info {
  display: flex;
  flex-direction: column;
}

.main-name {
  font-weight: 500;
  color: #303133;
}

.nick-name {
  font-size: 12px;
  color: #909399;
}

.pet-list {
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
}

.pet-tag {
  margin: 2px 0;
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

:deep(.el-table__row) {
  cursor: pointer;
}

:deep(.el-table__row:hover) {
  background-color: #f5f7fa;
}
</style>