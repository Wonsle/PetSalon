<template>
  <div class="pet-contacts-manager">
    <div class="contacts-header">
      <h3>聯絡人管理</h3>
      <el-button type="primary" size="small" @click="openAddContactDialog">
        <el-icon><Plus /></el-icon>
        新增聯絡人
      </el-button>
    </div>

    <!-- 現有聯絡人列表 -->
    <div class="contacts-list" v-loading="loading">
      <el-table :data="petContacts" border size="small">
        <el-table-column prop="contactPerson.name" label="姓名" width="120">
          <template #default="{ row }">
            {{ row.contactPerson?.name || '未知聯絡人' }}
          </template>
        </el-table-column>
        <el-table-column prop="contactPerson.contactNumber" label="電話" width="120">
          <template #default="{ row }">
            {{ row.contactPerson?.contactNumber || '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="relationshipType" label="關係" width="100">
          <template #default="{ row }">
            {{ getRelationshipName(row.relationshipType) || '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="contactPerson.email" label="Email" min-width="150">
          <template #default="{ row }">
            {{ row.contactPerson?.email || '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="sort" label="排序" width="80">
          <template #default="{ row }">
            <el-input-number
              v-model="row.sort"
              size="small"
              :min="1"
              :max="99"
              @change="updateSort(row)"
            />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="120">
          <template #default="{ row }">
            <el-button size="small" type="warning" @click="editContact(row)">
              編輯
            </el-button>
            <el-button size="small" type="danger" @click="removeContact(row)">
              移除
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 空狀態 -->
      <div v-if="petContacts.length === 0" class="empty-contacts">
        <el-empty description="尚未新增聯絡人" size="small">
          <el-button type="primary" @click="openAddContactDialog">
            新增第一個聯絡人
          </el-button>
        </el-empty>
      </div>
    </div>

    <!-- 新增/編輯聯絡人對話框 -->
    <el-dialog
      v-model="contactDialogVisible"
      :title="isEditingContact ? '編輯聯絡人關聯' : '新增聯絡人關聯'"
      width="500px"
    >
      <el-form ref="contactFormRef" :model="contactForm" :rules="contactRules" label-width="80px">
        <el-form-item label="聯絡人" prop="contactPersonId">
          <el-select
            v-model="contactForm.contactPersonId"
            placeholder="請選擇聯絡人"
            filterable
            remote
            :remote-method="searchContacts"
            :loading="contactSearchLoading"
            style="width: 100%"
          >
            <el-option
              v-for="contact in availableContacts"
              :key="contact.contactPersonId"
              :label="`${contact.name} (${contact.contactNumber})`"
              :value="contact.contactPersonId"
            >
              <span style="float: left">{{ contact.name }}</span>
              <span style="float: right; color: #8492a6; font-size: 13px">{{ contact.contactNumber }}</span>
            </el-option>
          </el-select>
        </el-form-item>

        <el-form-item label="關係" prop="relationshipType">
          <SystemCodeSelect
            v-model="contactForm.relationshipType"
            code-type="Relationship"
            placeholder="請選擇關係"
            clearable
          />
        </el-form-item>

        <el-form-item label="排序" prop="sort">
          <el-input-number
            v-model="contactForm.sort"
            :min="1"
            :max="99"
            placeholder="數字越小越優先"
          />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="contactDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="saveContact" :loading="saving">
          {{ isEditingContact ? '更新' : '新增' }}
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, watch } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import type { PetRelation, PetRelationCreateRequest, PetRelationUpdateRequest } from '@/types/petRelation'
import { petRelationApi } from '@/api/petRelation'
import { contactApi } from '@/api/contact'
import { commonApi, type SystemCode } from '@/api/common'
import { SystemCodeSelect } from '@/components/common'

interface Props {
  petId: number
}

type Emits = {
  contactsUpdated: []
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

// Refs
const contactFormRef = ref<FormInstance>()
const loading = ref(false)
const saving = ref(false)
const contactSearchLoading = ref(false)
const contactDialogVisible = ref(false)
const isEditingContact = ref(false)

// Data
const petContacts = ref<PetRelation[]>([])
const availableContacts = ref<any[]>([])
const editingContactId = ref<number | null>(null)
const relationshipCodes = ref<SystemCode[]>([])

// Form
const contactForm = reactive<PetRelationCreateRequest>({
  petId: props.petId,
  contactPersonId: 0,
  relationshipType: '',
  sort: 1
})

// Rules
const contactRules: FormRules = {
  contactPersonId: [
    { required: true, message: '請選擇聯絡人', trigger: 'change' }
  ],
  relationshipType: [
    { required: true, message: '請選擇關係', trigger: 'change' }
  ],
  sort: [
    { required: true, message: '請輸入排序', trigger: 'blur' }
  ]
}

// Methods
const loadRelationshipCodes = async () => {
  try {
    relationshipCodes.value = await commonApi.getSystemCodes('Relationship')
  } catch (error) {
    console.error('Load relationship codes error:', error)
    ElMessage.error('載入關係代碼失敗')
  }
}

const getRelationshipName = (code: string) => {
  const relationship = relationshipCodes.value.find(r => r.code === code)
  return relationship?.name || code
}

const loadPetContacts = async () => {
  if (!props.petId || isNaN(props.petId) || props.petId <= 0) return

  loading.value = true
  try {
    petContacts.value = await petRelationApi.getRelationsByPet(props.petId)
  } catch (error) {
    console.error('Load pet contacts error:', error)
    ElMessage.error('載入聯絡人資料失敗')
  } finally {
    loading.value = false
  }
}

const searchContacts = async (query: string) => {
  if (!query || query.length < 2) {
    availableContacts.value = []
    return
  }

  contactSearchLoading.value = true
  try {
    const contacts = await contactApi.searchContacts(query)
    availableContacts.value = contacts
  } catch (error) {
    console.error('Search contacts error:', error)
    ElMessage.error('搜尋聯絡人失敗')
  } finally {
    contactSearchLoading.value = false
  }
}

const openAddContactDialog = () => {
  if (!props.petId || isNaN(props.petId) || props.petId <= 0) {
    ElMessage.error('無效的寵物ID，無法新增聯絡人')
    return
  }
  
  isEditingContact.value = false
  editingContactId.value = null
  Object.assign(contactForm, {
    petId: props.petId,
    contactPersonId: 0,
    relationshipType: '',
    sort: petContacts.value.length + 1
  })
  contactDialogVisible.value = true
}

const editContact = (relation: PetRelation) => {
  isEditingContact.value = true
  editingContactId.value = relation.petRelationId
  Object.assign(contactForm, {
    petId: relation.petId,
    contactPersonId: relation.contactPersonId,
    relationshipType: relation.relationshipType || '',
    sort: relation.sort
  })
  contactDialogVisible.value = true
}

const saveContact = async () => {
  if (!contactFormRef.value) return

  try {
    const valid = await contactFormRef.value.validate()
    if (!valid) return

    saving.value = true

    if (isEditingContact.value && editingContactId.value) {
      const updateData: PetRelationUpdateRequest = {
        ...contactForm,
        petRelationId: editingContactId.value
      }
      await petRelationApi.updatePetRelation(updateData)
      ElMessage.success('更新成功')
    } else {
      await petRelationApi.createPetRelation(contactForm)
      ElMessage.success('新增成功')
    }

    contactDialogVisible.value = false
    await loadPetContacts()
    emit('contactsUpdated')
  } catch (error: any) {
    console.error('Save contact error:', error)
    ElMessage.error(error.response?.data?.message || '操作失敗')
  } finally {
    saving.value = false
  }
}

const updateSort = async (relation: PetRelation) => {
  try {
    const updateData: PetRelationUpdateRequest = {
      petRelationId: relation.petRelationId,
      petId: relation.petId,
      contactPersonId: relation.contactPersonId,
      relationshipType: relation.relationshipType,
      sort: relation.sort
    }
    await petRelationApi.updatePetRelation(updateData)
    ElMessage.success('排序已更新')
    emit('contactsUpdated')
  } catch (error: any) {
    console.error('Update sort error:', error)
    ElMessage.error('更新排序失敗')
  }
}

const removeContact = async (relation: PetRelation) => {
  try {
    await ElMessageBox.confirm(
      `確定要移除聯絡人「${relation.contactPerson?.name}」嗎？`,
      '確認移除',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )

    await petRelationApi.deletePetRelation(relation.petRelationId)
    ElMessage.success('移除成功')
    await loadPetContacts()
    emit('contactsUpdated')
  } catch (error: any) {
    if (error !== 'cancel') {
      console.error('Remove contact error:', error)
      ElMessage.error('移除失敗')
    }
  }
}

// Watch for petId changes
watch(() => props.petId, (newPetId) => {
  if (newPetId && !isNaN(newPetId) && newPetId > 0) {
    loadPetContacts()
  }
}, { immediate: true })

// Lifecycle
onMounted(() => {
  loadRelationshipCodes()
  loadPetContacts()
})
</script>

<style scoped>
.pet-contacts-manager {
  margin-top: 20px;
}

.contacts-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.contacts-header h3 {
  margin: 0;
  color: #303133;
  font-size: 16px;
}

.contacts-list {
  background: #f8f9fa;
  padding: 16px;
  border-radius: 6px;
  border: 1px solid #e4e7ed;
}

.empty-contacts {
  padding: 20px;
  text-align: center;
}
</style>
