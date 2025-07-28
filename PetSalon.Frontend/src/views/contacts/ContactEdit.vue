<template>
  <div class="contact-edit-container">
    <!-- Header -->
    <div class="page-header">
      <div class="header-left">
        <h2>{{ isEdit ? '編輯聯絡人' : '新增聯絡人' }}</h2>
        <nav class="breadcrumb">
          <span class="breadcrumb-item" @click="$router.push('/contacts')">聯絡人管理</span>
          <i class="pi pi-chevron-right breadcrumb-separator"></i>
          <span class="breadcrumb-current">{{ isEdit ? '編輯聯絡人' : '新增聯絡人' }}</span>
        </nav>
      </div>
      <div class="header-right">
        <Button
          label="返回"
          icon="pi pi-arrow-left"
          severity="secondary"
          @click="$router.back()"
        />
      </div>
    </div>

    <!-- Form -->
    <Card>
      <template #content>
        <form @submit.prevent="handleSubmit">
          <div class="grid">
            <div class="col-6">
              <div class="field">
                <label class="label">姓名 *</label>
                <InputText
                  v-model="form.name"
                  placeholder="請輸入姓名"
                  maxlength="50"
                  :invalid="!!errors.name"
                />
                <small v-if="errors.name" class="p-error">{{ errors.name }}</small>
              </div>
            </div>
            <div class="col-6">
              <div class="field">
                <label class="label">暱稱</label>
                <InputText
                  v-model="form.nickName"
                  placeholder="請輸入暱稱(選填)"
                  maxlength="50"
                />
              </div>
            </div>
          </div>

          <div class="field">
            <label class="label">聯絡電話 *</label>
            <InputText
              v-model="form.contactNumber"
              placeholder="請輸入聯絡電話"
              maxlength="20"
              :invalid="!!errors.contactNumber"
            />
            <small v-if="errors.contactNumber" class="p-error">{{ errors.contactNumber }}</small>
          </div>

          <!-- 關聯寵物 -->
          <div v-if="isEdit && contactId">
            <Divider />
            <div class="related-pets-section">
              <div class="section-header">
                <h3>關聯寵物</h3>
                <Button
                  label="新增寵物關聯"
                  icon="pi pi-plus"
                  size="small"
                  @click="openAddPetDialog"
                />
              </div>

              <div class="pets-list" v-if="petsLoading">
                <div class="loading-container">
                  <ProgressSpinner style="width: 30px; height: 30px" />
                  <span>載入中...</span>
                </div>
              </div>

              <div v-else-if="relatedPets.length > 0" class="pets-list">
                <DataTable
                  :value="relatedPets"
                  size="small"
                  responsive-layout="scroll"
                >
                  <Column field="petName" header="寵物名稱" style="min-width: 150px">
                    <template #body="{ data }">
                      {{ data.pet?.petName || data.petName || '未知寵物' }}
                    </template>
                  </Column>
                  <Column field="breed" header="品種" style="min-width: 120px">
                    <template #body="{ data }">
                      {{ data.pet?.breed || data.breed || '-' }}
                    </template>
                  </Column>
                  <Column field="gender" header="性別" style="min-width: 80px">
                    <template #body="{ data }">
                      {{ getGenderDisplay(data.pet?.gender || data.gender) }}
                    </template>
                  </Column>
                  <Column field="relationship" header="關係" style="min-width: 100px">
                    <template #body="{ data }">
                      {{ data.relationship || '-' }}
                    </template>
                  </Column>
                  <Column field="sort" header="排序" style="min-width: 80px">
                    <template #body="{ data }">
                      <InputNumber
                        v-model="data.sort"
                        size="small"
                        :min="1"
                        :max="99"
                        @update:model-value="updatePetSort(data)"
                      />
                    </template>
                  </Column>
                  <Column header="操作" style="min-width: 120px">
                    <template #body="{ data }">
                      <Button
                        label="移除"
                        icon="pi pi-trash"
                        size="small"
                        severity="danger"
                        @click="removePetRelation(data)"
                      />
                    </template>
                  </Column>
                </DataTable>
              </div>

              <!-- 空狀態 -->
              <div v-else class="empty-pets">
                <div class="empty-content">
                  <i class="pi pi-users" style="font-size: 2rem; color: var(--p-text-color-secondary);"></i>
                  <p>尚未關聯任何寵物</p>
                  <Button
                    label="新增第一個寵物關聯"
                    @click="openAddPetDialog"
                  />
                </div>
              </div>
            </div>
          </div>

          <!-- Submit Buttons -->
          <div class="form-actions">
            <Button
              :label="isEdit ? '更新' : '新增'"
              type="submit"
              :loading="submitting"
            />
            <Button
              label="取消"
              severity="secondary"
              @click="$router.back()"
            />
          </div>
        </form>
      </template>
    </Card>

    <!-- 新增寵物關聯對話框 -->
    <Dialog
      :visible="petDialogVisible"
      header="新增寵物關聯"
      :style="{ width: '500px' }"
      :modal="true"
      @update:visible="petDialogVisible = false"
    >
      <form @submit.prevent="savePetRelation">
        <div class="field">
          <label class="label">寵物 *</label>
          <Select
            v-model="selectedPetId"
            :options="availablePets"
            option-label="displayName"
            option-value="petId"
            placeholder="請選擇寵物"
            filter
            :loading="petSearchLoading"
            @filter="searchPets"
            :invalid="!!petErrors.petId"
          />
          <small v-if="petErrors.petId" class="p-error">{{ petErrors.petId }}</small>
        </div>

        <div class="field">
          <label class="label">關係 *</label>
          <Select
            v-model="petForm.relationshipType"
            :options="relationshipOptions"
            option-label="name"
            option-value="code"
            placeholder="請選擇關係"
            :invalid="!!petErrors.relationshipType"
          />
          <small v-if="petErrors.relationshipType" class="p-error">{{ petErrors.relationshipType }}</small>
        </div>

        <div class="field">
          <label class="label">排序</label>
          <InputNumber
            v-model="petForm.sort"
            :min="1"
            :max="99"
            placeholder="數字越小越優先"
          />
        </div>

        <div class="dialog-footer">
          <Button
            label="取消"
            severity="secondary"
            @click="petDialogVisible = false"
          />
          <Button
            label="新增"
            type="submit"
            :loading="petSaving"
          />
        </div>
      </form>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import type { Contact, ContactCreateRequest, ContactUpdateRequest, LinkContactToPetRequest } from '@/types/contact'
import { contactApi } from '@/api/contact'
import { petApi } from '@/api/pet'
import { commonApi } from '@/api/common'

const route = useRoute()
const router = useRouter()
const toast = useToast()
const confirm = useConfirm()

// Refs
const submitting = ref(false)
const petsLoading = ref(false)
const petSearchLoading = ref(false)
const petSaving = ref(false)
const petDialogVisible = ref(false)

// Data
const contactId = ref<number | null>(null)
const relatedPets = ref<any[]>([])
const availablePets = ref<any[]>([])
const genders = ref<any[]>([])
const relationshipOptions = ref<any[]>([])

// Computed
const isEdit = computed(() => !!contactId.value)

// Form data
const form = reactive<ContactCreateRequest>({
  name: '',
  nickName: '',
  contactNumber: ''
})

// Validation errors
const errors = reactive({
  name: '',
  contactNumber: ''
})

const petForm = reactive<LinkContactToPetRequest>({
  relationshipType: '',
  sort: 1
})

const petErrors = reactive({
  petId: '',
  relationshipType: ''
})

const selectedPetId = ref<number>(0)

// Validation
const validateForm = () => {
  let isValid = true
  
  // Reset errors
  errors.name = ''
  errors.contactNumber = ''

  // Validate name
  if (!form.name.trim()) {
    errors.name = '請輸入姓名'
    isValid = false
  } else if (form.name.length > 50) {
    errors.name = '姓名長度不能超過50個字符'
    isValid = false
  }

  // Validate contact number
  if (!form.contactNumber.trim()) {
    errors.contactNumber = '請輸入聯絡電話'
    isValid = false
  } else if (form.contactNumber.length > 20) {
    errors.contactNumber = '聯絡電話長度不能超過20個字符'
    isValid = false
  }

  return isValid
}

const validatePetForm = () => {
  let isValid = true
  
  // Reset errors
  petErrors.petId = ''
  petErrors.relationshipType = ''

  if (!selectedPetId.value) {
    petErrors.petId = '請選擇寵物'
    isValid = false
  }

  if (!petForm.relationshipType) {
    petErrors.relationshipType = '請選擇關係'
    isValid = false
  }

  return isValid
}

// Methods
const loadGenders = async () => {
  try {
    genders.value = await commonApi.getSystemCodes('Gender')
  } catch (error) {
    console.error('Load genders error:', error)
  }
}

const loadRelationships = async () => {
  try {
    relationshipOptions.value = await commonApi.getSystemCodes('Relationship')
  } catch (error) {
    console.error('Load relationships error:', error)
  }
}

const getGenderDisplay = (genderCode: string) => {
  if (!genderCode) return '-'
  if (genders.value.length > 0) {
    const gender = genders.value.find(g => g.code === genderCode || g.id === genderCode)
    return gender?.name || genderCode
  }
  return genderCode === 'M' ? '公' : genderCode === 'F' ? '母' : genderCode
}

const loadContact = async (id: number) => {
  try {
    const contact = await contactApi.getContact(id)
    Object.assign(form, {
      name: contact.name,
      nickName: contact.nickName || '',
      contactNumber: contact.contactNumber
    })
    
    if (contact.relatedPets) {
      relatedPets.value = contact.relatedPets
    }
  } catch (error) {
    console.error('Load contact error:', error)
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: '載入聯絡人資料失敗',
      life: 3000
    })
  }
}

const loadRelatedPets = async () => {
  if (!contactId.value) return

  petsLoading.value = true
  try {
    const contact = await contactApi.getContact(contactId.value)
    relatedPets.value = contact.relatedPets || []
  } catch (error) {
    console.error('Load related pets error:', error)
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: '載入關聯寵物失敗',
      life: 3000
    })
  } finally {
    petsLoading.value = false
  }
}

const searchPets = async (event: any) => {
  const query = event.value
  if (!query || query.length < 2) {
    availablePets.value = []
    return
  }

  petSearchLoading.value = true
  try {
    const response = await petApi.getPets({
      keyword: query,
      pageSize: 20
    })
    availablePets.value = response.data.map(pet => ({
      ...pet,
      displayName: `${pet.petName} (${pet.breed})`
    }))
  } catch (error) {
    console.error('Search pets error:', error)
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: '搜尋寵物失敗',
      life: 3000
    })
  } finally {
    petSearchLoading.value = false
  }
}

const openAddPetDialog = () => {
  selectedPetId.value = 0
  Object.assign(petForm, {
    relationshipType: '',
    sort: relatedPets.value.length + 1
  })
  
  // Reset errors
  petErrors.petId = ''
  petErrors.relationshipType = ''
  
  petDialogVisible.value = true
}

const savePetRelation = async () => {
  if (!validatePetForm() || !contactId.value) return

  petSaving.value = true

  try {
    await contactApi.linkContactToPet(contactId.value, selectedPetId.value, petForm)
    toast.add({
      severity: 'success',
      summary: '成功',
      detail: '新增成功',
      life: 3000
    })
    petDialogVisible.value = false
    await loadRelatedPets()
  } catch (error: any) {
    console.error('Save pet relation error:', error)
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: error.response?.data?.message || '新增失敗',
      life: 3000
    })
  } finally {
    petSaving.value = false
  }
}

const updatePetSort = async (relation: any) => {
  try {
    // Note: This would require a separate API to update sort order
    toast.add({
      severity: 'info',
      summary: '提示',
      detail: '排序功能待實作',
      life: 3000
    })
  } catch (error: any) {
    console.error('Update sort error:', error)
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: '更新排序失敗',
      life: 3000
    })
  }
}

const removePetRelation = async (relation: any) => {
  confirm.require({
    message: `確定要移除寵物「${relation.petName}」的關聯嗎？`,
    header: '確認移除',
    icon: 'pi pi-exclamation-triangle',
    accept: async () => {
      try {
        if (contactId.value) {
          await contactApi.unlinkContactFromPet(contactId.value, relation.petId)
          toast.add({
            severity: 'success',
            summary: '成功',
            detail: '移除成功',
            life: 3000
          })
          await loadRelatedPets()
        }
      } catch (error: any) {
        console.error('Remove pet relation error:', error)
        toast.add({
          severity: 'error',
          summary: '錯誤',
          detail: '移除失敗',
          life: 3000
        })
      }
    }
  })
}

const handleSubmit = async () => {
  if (!validateForm()) return

  submitting.value = true

  try {
    if (isEdit.value && contactId.value) {
      const updateData: ContactUpdateRequest = {
        ...form,
        contactPersonId: contactId.value
      }
      await contactApi.updateContact(updateData)
      toast.add({
        severity: 'success',
        summary: '成功',
        detail: '更新成功',
        life: 3000
      })
    } else {
      await contactApi.createContact(form)
      toast.add({
        severity: 'success',
        summary: '成功',
        detail: '新增成功',
        life: 3000
      })
    }

    router.push('/contacts')
  } catch (error: any) {
    console.error('Submit error:', error)
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: error.response?.data?.message || '操作失敗',
      life: 3000
    })
  } finally {
    submitting.value = false
  }
}

// Lifecycle
onMounted(async () => {
  await loadGenders()
  await loadRelationships()
  
  const id = route.params.id as string
  if (id && id !== 'create') {
    contactId.value = parseInt(id)
    await loadContact(contactId.value)
    await loadRelatedPets()
  }
})
</script>

<style scoped>
.contact-edit-container {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 20px;
}

.header-left h2 {
  margin: 0 0 8px 0;
  color: var(--p-text-color);
}

.breadcrumb {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
  color: var(--p-text-color-secondary);
}

.breadcrumb-item {
  cursor: pointer;
  color: var(--p-primary-color);
  text-decoration: none;
}

.breadcrumb-item:hover {
  text-decoration: underline;
}

.breadcrumb-separator {
  font-size: 0.75rem;
}

.breadcrumb-current {
  color: var(--p-text-color);
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
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
  margin-bottom: 1rem;
}

.col-6 {
  grid-column: span 1;
}

@media (max-width: 768px) {
  .grid {
    grid-template-columns: 1fr;
  }
  
  .col-6 {
    grid-column: span 1;
  }
  
  .page-header {
    flex-direction: column;
    gap: 1rem;
    align-items: stretch;
  }
}

.form-actions {
  display: flex;
  gap: 12px;
  margin-top: 2rem;
  padding-top: 1rem;
  border-top: 1px solid var(--p-surface-200);
}

.related-pets-section {
  margin-top: 20px;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.section-header h3 {
  margin: 0;
  color: var(--p-text-color);
  font-size: 16px;
}

.pets-list {
  background: var(--p-surface-50);
  padding: 16px;
  border-radius: var(--p-border-radius);
  border: 1px solid var(--p-surface-200);
}

.loading-container {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  padding: 2rem;
  color: var(--p-text-color-secondary);
}

.empty-pets {
  padding: 20px;
  text-align: center;
}

.empty-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}

.empty-content p {
  margin: 0;
  color: var(--p-text-color-secondary);
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 1.5rem;
  padding-top: 1rem;
  border-top: 1px solid var(--p-surface-200);
}

.p-error {
  color: var(--p-red-500);
  font-size: 0.875rem;
}
</style>