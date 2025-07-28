<template>
  <div class="contact-list">
    <div class="header">
      <h2>👥 聯絡人管理</h2>
      <Button
        label="新增聯絡人"
        icon="pi pi-plus"
        @click="handleCreate"
      />
    </div>

    <!-- 篩選區域 -->
    <div class="filters-section">
      <div class="grid">
        <div class="md:col-6">
          <div class="field">
            <label class="label">搜尋關鍵字</label>
            <InputText
              v-model="searchParams.keyword"
              placeholder="姓名或電話號碼"
              @input="debounceSearch"
            />
          </div>
        </div>
        <div class="md:col-6">
          <div class="field">
            <Button
              label="重置篩選"
              severity="secondary"
              icon="pi pi-refresh"
              @click="resetFilters"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- 資料表格 -->
    <div class="table-section">
      <DataTable
        :value="contacts"
        :loading="loading"
        :rows="searchParams.pageSize"
        :total-records="totalRecords"
        lazy
        paginator
        :rows-per-page-options="[10, 20, 50]"
        @page="onPageChange"
        responsive-layout="scroll"
        class="p-datatable-sm"
      >
        <template #empty>
          <div class="empty-state">
            <i class="pi pi-users" style="font-size: 3rem; color: #ccc;"></i>
            <h3>暫無聯絡人記錄</h3>
            <p>目前沒有符合條件的聯絡人記錄</p>
            <Button
              label="新增聯絡人"
              icon="pi pi-plus"
              @click="handleCreate"
            />
          </div>
        </template>

        <Column field="name" header="姓名" style="min-width: 150px">
          <template #body="{ data }">
            <div class="contact-info">
              <div class="contact-name">{{ data.name }}</div>
              <div v-if="data.nickName" class="contact-nickname">{{ data.nickName }}</div>
            </div>
          </template>
        </Column>

        <Column field="contactNumber" header="聯絡電話" style="min-width: 150px" />

        <Column field="relatedPets" header="關聯寵物" style="min-width: 200px">
          <template #body="{ data }">
            <div v-if="data.relatedPets && data.relatedPets.length > 0" class="pets-list">
              <Tag
                v-for="pet in data.relatedPets.slice(0, 3)"
                :key="pet.petRelationId"
                :value="pet.petName"
                severity="info"
                class="pet-tag"
              />
              <span v-if="data.relatedPets.length > 3" class="more-pets">
                +{{ data.relatedPets.length - 3 }}
              </span>
            </div>
            <span v-else class="no-pets">暫無關聯寵物</span>
          </template>
        </Column>

        <Column field="createTime" header="建立時間" style="min-width: 120px">
          <template #body="{ data }">
            {{ formatDateTime(data.createTime) }}
          </template>
        </Column>

        <Column header="操作" style="min-width: 200px">
          <template #body="{ data }">
            <div class="actions">
              <Button
                icon="pi pi-eye"
                size="small"
                severity="info"
                @click="viewContact(data)"
                v-tooltip="'查看詳情'"
              />
              <Button
                icon="pi pi-pencil"
                size="small"
                severity="warning"
                @click="editContact(data)"
                v-tooltip="'編輯'"
              />
              <Button
                icon="pi pi-link"
                size="small"
                severity="success"
                @click="managePets(data)"
                v-tooltip="'管理寵物關聯'"
              />
              <Button
                icon="pi pi-trash"
                size="small"
                severity="danger"
                @click="deleteContact(data)"
                v-tooltip="'刪除'"
              />
            </div>
          </template>
        </Column>
      </DataTable>
    </div>

    <!-- 新增/編輯聯絡人對話框 -->
    <ContactForm
      :visible="showDialog"
      :contact="selectedContact"
      @close="closeDialog"
      @success="handleSuccess"
    />

    <!-- 查看詳情對話框 -->
    <Dialog
      :visible="showViewDialog"
      header="聯絡人詳情"
      :style="{ width: '600px' }"
      :modal="true"
      @update:visible="showViewDialog = false"
    >
      <div v-if="selectedContact" class="contact-details">
        <div class="detail-row">
          <span class="label">姓名:</span>
          <span class="value">{{ selectedContact.name }}</span>
        </div>
        <div v-if="selectedContact.nickName" class="detail-row">
          <span class="label">暱稱:</span>
          <span class="value">{{ selectedContact.nickName }}</span>
        </div>
        <div class="detail-row">
          <span class="label">聯絡電話:</span>
          <span class="value">{{ selectedContact.contactNumber }}</span>
        </div>
        <div class="detail-row">
          <span class="label">建立時間:</span>
          <span class="value">{{ formatDateTime(selectedContact.createTime) }}</span>
        </div>
        <div class="detail-row">
          <span class="label">更新時間:</span>
          <span class="value">{{ formatDateTime(selectedContact.modifyTime) }}</span>
        </div>
        <div v-if="selectedContact.relatedPets && selectedContact.relatedPets.length > 0" class="detail-row">
          <span class="label">關聯寵物:</span>
          <div class="pets-detail">
            <div
              v-for="pet in selectedContact.relatedPets"
              :key="pet.petRelationId"
              class="pet-item"
            >
              <Tag :value="pet.petName" severity="info" />
              <span class="pet-breed">{{ pet.breed }}</span>
              <span class="pet-gender">({{ getGenderDisplay(pet.gender) }})</span>
            </div>
          </div>
        </div>
      </div>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import type { Contact, ContactSearchParams } from '@/types/contact'
import { contactApi } from '@/api/contact'
import ContactForm from '@/components/forms/ContactForm.vue'

const router = useRouter()
const toast = useToast()
const confirm = useConfirm()

// Refs
const loading = ref(false)
const contacts = ref<Contact[]>([])
const totalRecords = ref(0)
const selectedContact = ref<Contact | null>(null)
const showDialog = ref(false)
const showViewDialog = ref(false)

// 搜尋參數
const searchParams = reactive<ContactSearchParams>({
  keyword: '',
  page: 1,
  pageSize: 10
})

// 防抖搜尋
let searchTimeout: number
const debounceSearch = () => {
  clearTimeout(searchTimeout)
  searchTimeout = window.setTimeout(() => {
    searchParams.page = 1
    loadContacts()
  }, 500)
}

// 載入聯絡人列表
const loadContacts = async () => {
  loading.value = true
  try {
    const response = await contactApi.getContacts(searchParams)
    contacts.value = response.data
    totalRecords.value = response.total
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: '載入聯絡人列表失敗',
      life: 3000
    })
  } finally {
    loading.value = false
  }
}

// 分頁變更
const onPageChange = (event: any) => {
  searchParams.page = event.page + 1
  searchParams.pageSize = event.rows
  loadContacts()
}

// 重置篩選
const resetFilters = () => {
  Object.assign(searchParams, {
    keyword: '',
    page: 1,
    pageSize: 10
  })
  loadContacts()
}

// 新增聯絡人
const handleCreate = () => {
  selectedContact.value = null
  showDialog.value = true
}

// 查看聯絡人詳情
const viewContact = (contact: Contact) => {
  selectedContact.value = contact
  showViewDialog.value = true
}

// 編輯聯絡人
const editContact = (contact: Contact) => {
  selectedContact.value = contact
  showDialog.value = true
}

// 管理寵物關聯
const managePets = (contact: Contact) => {
  router.push(`/contacts/${contact.contactPersonId}`)
}

// 刪除聯絡人
const deleteContact = (contact: Contact) => {
  confirm.require({
    message: `確定要刪除聯絡人「${contact.name}」嗎？此操作無法撤銷。`,
    header: '確認刪除',
    icon: 'pi pi-exclamation-triangle',
    accept: async () => {
      try {
        await contactApi.deleteContact(contact.contactPersonId)
        toast.add({
          severity: 'success',
          summary: '成功',
          detail: '聯絡人已刪除',
          life: 3000
        })
        loadContacts()
      } catch (error: any) {
        toast.add({
          severity: 'error',
          summary: '錯誤',
          detail: '刪除聯絡人失敗',
          life: 3000
        })
      }
    }
  })
}

// 關閉對話框
const closeDialog = () => {
  showDialog.value = false
  selectedContact.value = null
}

// 處理成功操作
const handleSuccess = () => {
  closeDialog()
  loadContacts()
}

// 格式化日期時間
const formatDateTime = (dateStr: string) => {
  if (!dateStr) return '-'
  const date = new Date(dateStr)
  return date.toLocaleString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

// 獲取性別顯示
const getGenderDisplay = (gender: string) => {
  const genderMap: Record<string, string> = {
    'M': '公',
    'F': '母'
  }
  return genderMap[gender] || gender
}

// 初始化載入
onMounted(() => {
  loadContacts()
})
</script>

<style scoped>
.contact-list {
  padding: 20px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.header h2 {
  margin: 0;
  color: var(--p-text-color);
}

.filters-section {
  margin-bottom: 1.5rem;
  padding: 1rem;
  background: var(--p-surface-50);
  border-radius: var(--p-border-radius);
}

.field {
  margin-bottom: 1rem;
}

.label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: var(--p-text-color);
}

.grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1rem;
}

.md\:col-6 {
  grid-column: span 6;
}

@media (max-width: 768px) {
  .md\:col-6 {
    grid-column: span 12;
  }
}

.table-section {
  margin-top: 1rem;
}

.contact-info {
  display: flex;
  flex-direction: column;
}

.contact-name {
  font-weight: 600;
  color: var(--p-text-color);
}

.contact-nickname {
  color: var(--p-text-color-secondary);
  font-size: 0.875rem;
}

.pets-list {
  display: flex;
  flex-wrap: wrap;
  gap: 0.25rem;
  align-items: center;
}

.pet-tag {
  font-size: 0.75rem;
}

.more-pets {
  color: var(--p-text-color-secondary);
  font-size: 0.875rem;
  margin-left: 0.5rem;
}

.no-pets {
  color: var(--p-text-color-secondary);
  font-style: italic;
}

.actions {
  display: flex;
  gap: 0.5rem;
}

.empty-state {
  text-align: center;
  padding: 3rem 1rem;
  color: var(--p-text-color-secondary);
}

.empty-state h3 {
  margin: 1rem 0;
  color: var(--p-text-color);
}

.empty-state p {
  margin-bottom: 1.5rem;
}

.contact-details {
  padding: 1rem 0;
}

.detail-row {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 0.75rem 0;
  border-bottom: 1px solid var(--p-surface-200);
}

.detail-row:last-child {
  border-bottom: none;
}

.detail-row .label {
  font-weight: 600;
  color: var(--p-text-color);
  margin-bottom: 0;
  min-width: 120px;
}

.detail-row .value {
  color: var(--p-text-color-secondary);
  text-align: right;
  flex: 1;
}

.pets-detail {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  align-items: flex-end;
}

.pet-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.pet-breed,
.pet-gender {
  color: var(--p-text-color-secondary);
  font-size: 0.875rem;
}
</style>