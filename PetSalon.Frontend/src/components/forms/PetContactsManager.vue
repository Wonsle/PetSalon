<template>
  <div class="pet-contacts-manager">
    <div class="contacts-header">
      <h3>聯絡人管理</h3>
      <Button
        label="新增聯絡人"
        icon="pi pi-plus"
        size="small"
        @click="openAddContactDialog"
      />
    </div>

    <!-- 現有聯絡人列表 -->
    <div class="contacts-list">
      <ProgressSpinner v-if="loading" />
      <DataTable v-else :value="petContacts" :size="'small'" :bordered="true">
        <Column field="contactPerson.name" header="姓名" style="width: 120px">
          <template #body="{ data }">
            {{ data.contactPerson?.name || '未知聯絡人' }}
          </template>
        </Column>
        <Column field="contactPerson.contactNumber" header="電話" style="width: 120px">
          <template #body="{ data }">
            {{ data.contactPerson?.contactNumber || '-' }}
          </template>
        </Column>
        <Column field="relationshipType" header="關係" style="width: 100px">
          <template #body="{ data }">
            {{ getRelationshipName(data.relationshipType) || '-' }}
          </template>
        </Column>
        <Column field="contactPerson.email" header="Email" style="min-width: 150px">
          <template #body="{ data }">
            {{ data.contactPerson?.email || '-' }}
          </template>
        </Column>
        <Column field="sort" header="排序" style="width: 80px">
          <template #body="{ data }">
            <InputNumber
              v-model="data.sort"
              :min="1"
              :max="99"
              @blur="updateSort(data)"
              showButtons
              buttonLayout="horizontal"
              size="small"
            />
          </template>
        </Column>
        <Column header="操作" style="width: 120px">
          <template #body="{ data }">
            <div class="action-buttons">
              <Button
                label="編輯"
                severity="warning"
                size="small"
                @click="editContact(data)"
              />
              <Button
                label="移除"
                severity="danger"
                size="small"
                @click="removeContact(data)"
              />
            </div>
          </template>
        </Column>
      </DataTable>

      <!-- 空狀態 -->
      <Card v-if="!loading && petContacts.length === 0" class="empty-contacts">
        <template #content>
          <div class="empty-content">
            <i class="pi pi-users" style="font-size: 3rem; color: #6c757d;"></i>
            <h4>尚未新增聯絡人</h4>
            <p>為這隻寵物新增聯絡人資訊</p>
            <Button
              label="新增第一個聯絡人"
              icon="pi pi-plus"
              @click="openAddContactDialog"
            />
          </div>
        </template>
      </Card>
    </div>

    <!-- 新增/編輯聯絡人對話框 -->
    <Dialog
      :visible="contactDialogVisible"
      :header="isEditingContact ? '編輯聯絡人關聯' : '新增聯絡人關聯'"
      :style="{ width: '500px' }"
      modal
      @update:visible="contactDialogVisible = $event"
    >
      <form @submit.prevent="saveContact" class="contact-form">
        <div class="form-field">
          <label class="form-label required">聯絡人</label>
          <Select
            v-model="contactForm.contactPersonId"
            :options="availableContacts"
            optionLabel="name"
            optionValue="contactPersonId"
            placeholder="請選擇聯絡人"
            filter
            :loading="contactSearchLoading"
            @filter="searchContacts"
            class="w-full"
            :invalid="!!errors.contactPersonId"
          >
            <template #option="{ option }">
              <div class="contact-option">
                <span class="contact-name">{{ option.name }}</span>
                <span class="contact-phone">{{ option.contactNumber }}</span>
              </div>
            </template>
          </Select>
          <small v-if="errors.contactPersonId" class="form-error">{{ errors.contactPersonId }}</small>
        </div>

        <div class="form-field">
          <label class="form-label required">關係</label>
          <SystemCodeSelect
            v-model="contactForm.relationshipType"
            code-type="Relationship"
            placeholder="請選擇關係"
            clearable
          />
          <small v-if="errors.relationshipType" class="form-error">{{ errors.relationshipType }}</small>
        </div>

        <div class="form-field">
          <label class="form-label required">排序</label>
          <InputNumber
            v-model="contactForm.sort"
            :min="1"
            :max="99"
            placeholder="數字越小越優先"
            showButtons
            class="w-full"
            :invalid="!!errors.sort"
          />
          <small v-if="errors.sort" class="form-error">{{ errors.sort }}</small>
        </div>
      </form>

      <template #footer>
        <div class="dialog-footer">
          <Button label="取消" severity="secondary" @click="contactDialogVisible = false" />
          <Button
            :label="isEditingContact ? '更新' : '新增'"
            :loading="saving"
            @click="saveContact"
          />
        </div>
      </template>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, watch } from 'vue'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
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
const toast = useToast()
const confirm = useConfirm()

// Refs
const loading = ref(false)
const saving = ref(false)
const contactSearchLoading = ref(false)
const contactDialogVisible = ref(false)
const isEditingContact = ref(false)

// Form validation errors
const errors = ref<Record<string, string>>({})

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

// Form validation
const validateForm = () => {
  errors.value = {}

  if (!contactForm.contactPersonId) {
    errors.value.contactPersonId = '請選擇聯絡人'
  }

  if (!contactForm.relationshipType) {
    errors.value.relationshipType = '請選擇關係'
  }

  if (!contactForm.sort) {
    errors.value.sort = '請輸入排序'
  }

  return Object.keys(errors.value).length === 0
}

// Methods
const loadRelationshipCodes = async () => {
  try {
    relationshipCodes.value = await commonApi.getSystemCodes('Relationship')
  } catch (error) {
    console.error('Load relationship codes error:', error)
    toast.add({
      severity: 'error',
      summary: '載入失敗',
      detail: '載入關係代碼失敗',
      life: 3000
    })
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
    toast.add({
      severity: 'error',
      summary: '載入失敗',
      detail: '載入聯絡人資料失敗',
      life: 3000
    })
  } finally {
    loading.value = false
  }
}

const searchContacts = async (event: any) => {
  const query = event.value || event
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
    toast.add({
      severity: 'error',
      summary: '搜尋失敗',
      detail: '搜尋聯絡人失敗',
      life: 3000
    })
  } finally {
    contactSearchLoading.value = false
  }
}

const openAddContactDialog = () => {
  if (!props.petId || isNaN(props.petId) || props.petId <= 0) {
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: '無效的寵物ID，無法新增聯絡人',
      life: 3000
    })
    return
  }

  isEditingContact.value = false
  editingContactId.value = null
  errors.value = {}
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
  errors.value = {}
  Object.assign(contactForm, {
    petId: relation.petId,
    contactPersonId: relation.contactPersonId,
    relationshipType: relation.relationshipType || '',
    sort: relation.sort
  })
  contactDialogVisible.value = true
}

const saveContact = async () => {
  if (!validateForm()) return

  try {
    saving.value = true

    if (isEditingContact.value && editingContactId.value) {
      const updateData: PetRelationUpdateRequest = {
        ...contactForm,
        petRelationId: editingContactId.value
      }
      await petRelationApi.updatePetRelation(updateData)
      toast.add({
        severity: 'success',
        summary: '更新成功',
        detail: '聯絡人關聯已更新',
        life: 3000
      })
    } else {
      await petRelationApi.createPetRelation(contactForm)
      toast.add({
        severity: 'success',
        summary: '新增成功',
        detail: '聯絡人關聯已新增',
        life: 3000
      })
    }

    contactDialogVisible.value = false
    await loadPetContacts()
    emit('contactsUpdated')
  } catch (error: any) {
    console.error('Save contact error:', error)
    toast.add({
      severity: 'error',
      summary: '操作失敗',
      detail: error.response?.data?.message || '操作失敗',
      life: 3000
    })
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
    toast.add({
      severity: 'success',
      summary: '更新成功',
      detail: '排序已更新',
      life: 3000
    })
    emit('contactsUpdated')
  } catch (error: any) {
    console.error('Update sort error:', error)
    toast.add({
      severity: 'error',
      summary: '更新失敗',
      detail: '更新排序失敗',
      life: 3000
    })
  }
}

const removeContact = async (relation: PetRelation) => {
  confirm.require({
    message: `確定要移除聯絡人「${relation.contactPerson?.name}」嗎？`,
    header: '確認移除',
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
        await petRelationApi.deletePetRelation(relation.petRelationId)
        toast.add({
          severity: 'success',
          summary: '移除成功',
          detail: '聯絡人關聯已移除',
          life: 3000
        })
        await loadPetContacts()
        emit('contactsUpdated')
      } catch (error: any) {
        console.error('Remove contact error:', error)
        toast.add({
          severity: 'error',
          summary: '移除失敗',
          detail: '移除聯絡人關聯失敗',
          life: 3000
        })
      }
    },
    reject: () => {
      // 使用者取消移除，不需要做任何事
    }
  })
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
  margin-top: 1.5rem;
}

.contacts-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.contacts-header h3 {
  margin: 0;
  color: var(--p-text-color);
  font-size: 1rem;
  font-weight: 600;
}

.contacts-list {
  background: var(--p-content-background);
  padding: 1rem;
  border-radius: var(--p-border-radius);
  border: 1px solid var(--p-content-border-color);
}

.contacts-list :deep(.p-datatable) {
  border: 1px solid var(--p-content-border-color);
}

.action-buttons {
  display: flex;
  gap: 0.5rem;
}

.empty-contacts {
  margin: 1rem 0;
}

.empty-content {
  text-align: center;
  padding: 2rem 1rem;
}

.empty-content h4 {
  color: var(--p-text-color);
  margin: 1rem 0 0.5rem 0;
}

.empty-content p {
  color: var(--p-text-muted-color);
  margin-bottom: 1.5rem;
}

.contact-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-label {
  font-weight: 500;
  color: var(--p-text-color);
  font-size: 0.875rem;
}

.form-label.required::after {
  content: ' *';
  color: var(--p-red-500);
}

.form-error {
  font-size: 0.75rem;
  color: var(--p-red-500);
  margin-top: 0.25rem;
}

.w-full {
  width: 100%;
}

.contact-option {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}

.contact-name {
  font-weight: 500;
}

.contact-phone {
  color: var(--p-text-muted-color);
  font-size: 0.875rem;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  padding-top: 1rem;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .contacts-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }

  .action-buttons {
    flex-direction: column;
    width: 100%;
  }

  .action-buttons .p-button {
    width: 100%;
  }

  .dialog-footer {
    flex-direction: column-reverse;
  }

  .dialog-footer .p-button {
    width: 100%;
  }

  .contact-form {
    gap: 1rem;
  }
}

@media (max-width: 480px) {
  .pet-contacts-manager {
    margin-top: 1rem;
  }

  .contacts-list {
    padding: 0.5rem;
  }

  .empty-content {
    padding: 1.5rem 0.5rem;
  }

  .form-field {
    gap: 0.25rem;
  }
}

/* PrimeVue 元件覆寫 */
.contact-form :deep(.p-inputtext),
.contact-form :deep(.p-inputnumber-input),
.contact-form :deep(.p-select) {
  width: 100%;
}

.contact-form :deep(.p-inputnumber) {
  width: 100%;
}

/* 確保選擇框在小螢幕上正確顯示 */
@media (max-width: 576px) {
  .contact-form :deep(.p-select-overlay) {
    max-width: calc(100vw - 2rem);
  }
}

/* 調整 DataTable 在小螢幕上的顯示 */
@media (max-width: 768px) {
  .contacts-list :deep(.p-datatable-table) {
    font-size: 0.875rem;
  }

  .contacts-list :deep(.p-datatable-tbody > tr > td) {
    padding: 0.5rem 0.25rem;
  }
}
</style>
