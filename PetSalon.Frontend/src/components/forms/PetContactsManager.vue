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
      <DataTable v-else :value="displayContacts" :size="'small'" :bordered="true">
        <Column field="contactPerson.name" header="姓名" style="width: 120px">
          <template #body="{ data }">
            {{ data.contactPerson?.name || data.name || '未知聯絡人' }}
          </template>
        </Column>
        <Column field="contactPerson.contactNumber" header="電話" style="width: 120px">
          <template #body="{ data }">
            {{ data.contactPerson?.contactNumber || data.contactNumber || '-' }}
          </template>
        </Column>
        <Column field="relationshipType" header="關係" style="width: 100px">
          <template #body="{ data }">
            {{ getRelationshipName(data.relationshipType) || '-' }}
          </template>
        </Column>
        <Column header="操作" style="width: 80px">
          <template #body="{ data, index }">
            <Button
              label="移除"
              severity="danger"
              size="small"
              @click="removeContact(data, index)"
            />
          </template>
        </Column>
      </DataTable>

      <!-- 空狀態 -->
      <Card v-if="!loading && displayContacts.length === 0" class="empty-contacts">
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

    <!-- 新增/編輯聯絡人對話框 - 統一表單設計 -->
    <Dialog
      :visible="contactDialogVisible"
      header="新增聯絡人"
      :style="{ width: '600px' }"
      modal
      @update:visible="contactDialogVisible = $event"
    >
      <form @submit.prevent="handleSaveContact" class="contact-form">
        <!-- 狀態指示器 -->
        <Message v-if="foundContact" severity="success" :closable="false" class="status-message">
          <div class="found-contact-message">
            <i class="pi pi-check-circle"></i>
            <div class="message-content">
              <strong>已找到現有聯絡人：{{ foundContact.name }}</strong>
              <small>您可以直接使用或編輯資訊後更新</small>
            </div>
          </div>
        </Message>

        <Message v-else-if="contactForm.name && contactForm.contactNumber" severity="info" :closable="false" class="status-message">
          <div class="new-contact-message">
            <i class="pi pi-info-circle"></i>
            <span>將新增聯絡人</span>
          </div>
        </Message>

        <!-- 電話號碼欄位 - 支援反向查詢 -->
        <div class="form-field">
          <label class="form-label required">電話號碼</label>
          <InputText
            v-model="contactForm.contactNumber"
            placeholder="請輸入電話號碼（輸入3碼以上自動查詢）"
            @input="handlePhoneInput"
            :invalid="!!formErrors.contactNumber"
          />
          <small v-if="formErrors.contactNumber" class="form-error">
            {{ formErrors.contactNumber }}
          </small>
          <small v-if="phoneSearchLoading" class="form-hint">
            <i class="pi pi-spin pi-spinner"></i> 查詢中...
          </small>
        </div>

        <!-- 姓名欄位 - AutoComplete -->
        <div class="form-field">
          <label class="form-label required">聯絡人姓名</label>
          <AutoComplete
            v-model="contactForm.name"
            :suggestions="nameSuggestions"
            @complete="searchContactsByName"
            @item-select="handleNameSelect"
            optionLabel="name"
            placeholder="請輸入姓名"
            :invalid="!!formErrors.name"
            class="w-full"
          >
            <template #option="slotProps">
              <div class="contact-option">
                <span class="contact-name">{{ slotProps.option.name }}</span>
                <span class="contact-phone">{{ slotProps.option.contactNumber }}</span>
              </div>
            </template>
          </AutoComplete>
          <small v-if="formErrors.name" class="form-error">
            {{ formErrors.name }}
          </small>
        </div>

        <!-- 暱稱欄位 -->
        <div class="form-field">
          <label class="form-label">暱稱</label>
          <InputText
            v-model="contactForm.nickName"
            placeholder="請輸入暱稱（選填）"
          />
        </div>

        <!-- 關係欄位 -->
        <div class="form-field">
          <label class="form-label required">關係</label>
          <SystemCodeSelect
            v-model="contactForm.relationshipType"
            code-type="Relationship"
            placeholder="請選擇關係"
            clearable
          />
          <small v-if="formErrors.relationshipType" class="form-error">
            {{ formErrors.relationshipType }}
          </small>
        </div>

        <!-- 排序欄位 -->
        <div class="form-field">
          <label class="form-label required">排序</label>
          <InputNumber
            v-model="contactForm.sort"
            :min="1"
            :max="99"
            placeholder="數字越小越優先"
            showButtons
            class="w-full"
            :invalid="!!formErrors.sort"
          />
          <small v-if="formErrors.sort" class="form-error">
            {{ formErrors.sort }}
          </small>
        </div>
      </form>

      <template #footer>
        <div class="dialog-footer">
          <Button label="取消" severity="secondary" @click="contactDialogVisible = false" />
          <Button
            :label="foundContact && hasContactInfoChanged() ? '更新並新增關聯' : foundContact ? '新增關聯' : '新增聯絡人'"
            :loading="saving"
            @click="handleSaveContact"
          />
        </div>
      </template>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, watch, computed } from 'vue'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import AutoComplete from 'primevue/autocomplete'
import type { PetRelation } from '@/types/petRelation'
import { petRelationApi } from '@/api/petRelation'
import { contactApi } from '@/api/contact'
import { commonApi, type SystemCode } from '@/api/common'
import { SystemCodeSelect } from '@/components/common'

// 臨時聯絡人資料介面
export interface TempContactData {
  isExisting: boolean
  contactPersonId?: number
  name: string
  contactNumber: string
  nickName?: string
  relationshipType: string
  sort: number
}

interface Props {
  petId?: number
  isNewPet?: boolean
  tempContacts?: TempContactData[]
}

type Emits = {
  contactsUpdated: []
  tempContactAdded: [contact: TempContactData]
  tempContactRemoved: [index: number]
}

const props = withDefaults(defineProps<Props>(), {
  isNewPet: false,
  tempContacts: () => []
})
const emit = defineEmits<Emits>()
const toast = useToast()
const confirm = useConfirm()

// Refs
const loading = ref(false)
const saving = ref(false)
const contactSearchLoading = ref(false)
const phoneSearchLoading = ref(false)
const contactDialogVisible = ref(false)

// 電話查詢防抖
const phoneSearchDebounce = ref<ReturnType<typeof setTimeout> | null>(null)

// 找到的現有聯絡人
const foundContact = ref<any | null>(null)

// 姓名搜尋建議列表
const nameSuggestions = ref<any[]>([])

// Form validation errors
const formErrors = ref<Record<string, string>>({})

// Data
const petContacts = ref<PetRelation[]>([])
const relationshipCodes = ref<SystemCode[]>([])

// 顯示的聯絡人列表（根據模式決定）
const displayContacts = computed(() => {
  if (props.isNewPet) {
    return props.tempContacts || []
  }
  return petContacts.value
})

// 統一表單結構
const contactForm = reactive({
  contactPersonId: 0,        // 現有聯絡人 ID（找到時設定）
  name: '',
  contactNumber: '',
  nickName: '',
  relationshipType: '',
  sort: 1
})

// 記錄原始找到的聯絡人資訊（用於比對是否有修改）
const originalContactInfo = ref({
  name: '',
  contactNumber: '',
  nickName: ''
})

// Form validation
const validateContactForm = () => {
  formErrors.value = {}

  if (!contactForm.name?.trim()) {
    formErrors.value.name = '請輸入聯絡人姓名'
  }

  if (!contactForm.contactNumber?.trim()) {
    formErrors.value.contactNumber = '請輸入電話號碼'
  }

  if (!contactForm.relationshipType) {
    formErrors.value.relationshipType = '請選擇關係'
  }

  return Object.keys(formErrors.value).length === 0
}

// 檢查聯絡人資訊是否有變更
const hasContactInfoChanged = () => {
  if (!foundContact.value) return false

  return (
    contactForm.name !== originalContactInfo.value.name ||
    contactForm.contactNumber !== originalContactInfo.value.contactNumber ||
    contactForm.nickName !== (originalContactInfo.value.nickName || '')
  )
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

// 電話號碼輸入處理（防抖）
const handlePhoneInput = () => {
  // 清除之前的計時器
  if (phoneSearchDebounce.value) {
    clearTimeout(phoneSearchDebounce.value)
  }

  // 設定新的防抖計時器（500ms）
  phoneSearchDebounce.value = setTimeout(() => {
    searchContactByPhone()
  }, 500)
}

// 根據電話號碼反向查詢聯絡人
const searchContactByPhone = async () => {
  const phone = contactForm.contactNumber.trim()

  // 少於 3 碼不查詢
  if (phone.length < 3) {
    foundContact.value = null
    return
  }

  phoneSearchLoading.value = true
  try {
    const contacts = await contactApi.searchContacts(phone)

    if (contacts.length > 0) {
      // 找到匹配的聯絡人
      const contact = contacts[0]
      foundContact.value = contact

      // 自動填入表單
      contactForm.contactPersonId = contact.contactPersonId
      contactForm.name = contact.name
      contactForm.nickName = contact.nickName || ''

      // 記錄原始資訊（用於比對是否修改）
      originalContactInfo.value = {
        name: contact.name,
        contactNumber: contact.contactNumber,
        nickName: contact.nickName || ''
      }

      toast.add({
        severity: 'success',
        summary: '已找到聯絡人',
        detail: `找到現有聯絡人：${contact.name}`,
        life: 3000
      })
    } else {
      // 沒找到匹配的聯絡人
      foundContact.value = null
      contactForm.contactPersonId = 0
    }
  } catch (error) {
    console.error('Search contact by phone error:', error)
    toast.add({
      severity: 'error',
      summary: '查詢失敗',
      detail: '查詢聯絡人失敗',
      life: 3000
    })
  } finally {
    phoneSearchLoading.value = false
  }
}

// 根據姓名搜尋聯絡人（AutoComplete）
const searchContactsByName = async (event: any) => {
  const query = event.query || ''

  if (!query || query.length < 2) {
    nameSuggestions.value = []
    return
  }

  contactSearchLoading.value = true
  try {
    const contacts = await contactApi.searchContacts(query)
    nameSuggestions.value = contacts
  } catch (error) {
    console.error('Search contacts by name error:', error)
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

// 選擇姓名搜尋結果
const handleNameSelect = (event: any) => {
  let contact = event.value

  // 如果 event.value 是字符串，從 suggestions 中找到對應的聯絡人對象
  if (typeof contact === 'string') {
    contact = nameSuggestions.value.find(c => c.name === contact)
  }

  if (contact && typeof contact === 'object') {
    foundContact.value = contact

    // 自動填入表單（確保 name 是字符串）
    contactForm.contactPersonId = contact.contactPersonId
    contactForm.name = contact.name
    contactForm.contactNumber = contact.contactNumber
    contactForm.nickName = contact.nickName || ''

    // 記錄原始資訊（用於比對是否修改）
    originalContactInfo.value = {
      name: contact.name,
      contactNumber: contact.contactNumber,
      nickName: contact.nickName || ''
    }

    toast.add({
      severity: 'success',
      summary: '已選擇聯絡人',
      detail: `選擇現有聯絡人：${contact.name}`,
      life: 3000
    })
  }
}

const openAddContactDialog = () => {
  if (!props.isNewPet && (!props.petId || isNaN(props.petId) || props.petId <= 0)) {
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: '無效的寵物ID，無法新增聯絡人',
      life: 3000
    })
    return
  }

  // 重置表單
  formErrors.value = {}
  foundContact.value = null
  Object.assign(contactForm, {
    contactPersonId: 0,
    name: '',
    contactNumber: '',
    nickName: '',
    relationshipType: '',
    sort: displayContacts.value.length + 1
  })
  Object.assign(originalContactInfo.value, {
    name: '',
    contactNumber: '',
    nickName: ''
  })

  contactDialogVisible.value = true
}

// 統一處理保存聯絡人（支援 3 種情境）
const handleSaveContact = async () => {
  if (!validateContactForm()) return

  try {
    saving.value = true

    let finalContactPersonId = contactForm.contactPersonId

    // 情境 1: 找到現有聯絡人且資訊被修改 → 更新聯絡人
    if (foundContact.value && hasContactInfoChanged()) {
      await contactApi.updateContact({
        contactPersonId: foundContact.value.contactPersonId,
        name: contactForm.name,
        contactNumber: contactForm.contactNumber,
        nickName: contactForm.nickName
      })

      toast.add({
        severity: 'success',
        summary: '更新成功',
        detail: '聯絡人資訊已更新',
        life: 3000
      })

      finalContactPersonId = foundContact.value.contactPersonId
    }
    // 情境 2: 找到現有聯絡人且資訊未改 → 直接使用現有聯絡人
    else if (foundContact.value) {
      finalContactPersonId = foundContact.value.contactPersonId
    }
    // 情境 3: 未找到現有聯絡人 → 新增聯絡人
    else {
      if (!props.isNewPet) {
        // 編輯模式：立即建立新聯絡人
        finalContactPersonId = await contactApi.createContact({
          name: contactForm.name,
          contactNumber: contactForm.contactNumber,
          nickName: contactForm.nickName
        })

        toast.add({
          severity: 'success',
          summary: '新增成功',
          detail: '聯絡人已新增',
          life: 3000
        })
      }
    }

    // 根據模式處理關聯
    if (props.isNewPet) {
      // 新增模式：發送暫存資料給父組件
      emit('tempContactAdded', {
        isExisting: !!foundContact.value,
        contactPersonId: finalContactPersonId,
        name: contactForm.name,
        contactNumber: contactForm.contactNumber,
        nickName: contactForm.nickName,
        relationshipType: contactForm.relationshipType,
        sort: contactForm.sort
      })

      toast.add({
        severity: 'success',
        summary: '新增成功',
        detail: '聯絡人已加入列表',
        life: 3000
      })

      contactDialogVisible.value = false
    } else {
      // 編輯模式：建立寵物關聯
      await petRelationApi.createPetRelation({
        petId: props.petId!,
        contactPersonId: finalContactPersonId,
        relationshipType: contactForm.relationshipType,
        sort: contactForm.sort
      })

      toast.add({
        severity: 'success',
        summary: '新增成功',
        detail: '聯絡人關聯已新增',
        life: 3000
      })

      contactDialogVisible.value = false
      await loadPetContacts()
      emit('contactsUpdated')
    }
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

const removeContact = async (relation: PetRelation | TempContactData, index: number) => {
  const contactName = 'contactPerson' in relation
    ? relation.contactPerson?.name
    : (relation as TempContactData).name

  confirm.require({
    message: `確定要移除聯絡人「${contactName}」嗎？`,
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
        if (props.isNewPet) {
          // 新增模式：通知父組件移除
          emit('tempContactRemoved', index)
        } else {
          // 編輯模式：刪除關聯
          const petRelation = relation as PetRelation
          await petRelationApi.deletePetRelation(petRelation.petRelationId)
          toast.add({
            severity: 'success',
            summary: '移除成功',
            detail: '聯絡人關聯已移除',
            life: 3000
          })
          await loadPetContacts()
          emit('contactsUpdated')
        }
      } catch (error: any) {
        console.error('Remove contact error:', error)
        toast.add({
          severity: 'error',
          summary: '移除失敗',
          detail: '移除聯絡人關聯失敗',
          life: 3000
        })
      }
    }
  })
}

// Watch for petId changes
watch(() => props.petId, (newPetId) => {
  if (!props.isNewPet && newPetId && !isNaN(newPetId) && newPetId > 0) {
    loadPetContacts()
  }
}, { immediate: true })

// Lifecycle
onMounted(() => {
  loadRelationshipCodes()
  if (!props.isNewPet) {
    loadPetContacts()
  }
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
  padding: 1rem 0;
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

/* 狀態訊息樣式 */
.status-message {
  margin-bottom: 1.5rem;
}

.found-contact-message {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
}

.found-contact-message i {
  font-size: 1.25rem;
  margin-top: 0.125rem;
}

.message-content {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.message-content strong {
  font-weight: 600;
}

.message-content small {
  color: var(--p-text-muted-color);
  font-size: 0.875rem;
}

.new-contact-message {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.new-contact-message i {
  font-size: 1.25rem;
}

/* 表單提示樣式 */
.form-hint {
  font-size: 0.75rem;
  color: var(--p-primary-color);
  margin-top: 0.25rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.form-hint i {
  font-size: 0.875rem;
}

/* AutoComplete 樣式 */
.contact-form :deep(.p-autocomplete) {
  width: 100%;
}

.contact-form :deep(.p-autocomplete-input) {
  width: 100%;
}
</style>
