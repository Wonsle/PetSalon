<template>
  <div class="contact-edit-container">
    <!-- Header -->
    <div class="page-header">
      <div class="header-left">
        <h2>{{ isEdit ? '編輯聯絡人' : '新增聯絡人' }}</h2>
        <el-breadcrumb separator=">">
          <el-breadcrumb-item :to="{ path: '/contacts' }">聯絡人管理</el-breadcrumb-item>
          <el-breadcrumb-item>{{ isEdit ? '編輯聯絡人' : '新增聯絡人' }}</el-breadcrumb-item>
        </el-breadcrumb>
      </div>
      <div class="header-right">
        <el-button @click="$router.back()">
          <el-icon><ArrowLeft /></el-icon>
          返回
        </el-button>
      </div>
    </div>

    <!-- Form -->
    <el-card>
      <el-form
        ref="formRef"
        :model="form"
        :rules="rules"
        label-width="120px"
        @submit.prevent="handleSubmit"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="姓名" prop="name">
              <el-input v-model="form.name" placeholder="請輸入姓名" maxlength="50" show-word-limit />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="暱稱" prop="nickName">
              <el-input v-model="form.nickName" placeholder="請輸入暱稱(選填)" maxlength="50" show-word-limit />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="聯絡電話" prop="contactNumber">
          <el-input v-model="form.contactNumber" placeholder="請輸入聯絡電話" maxlength="20" show-word-limit />
        </el-form-item>

        <!-- 關聯寵物 -->
        <div v-if="isEdit && contactId">
          <el-divider />
          <div class="related-pets-section">
            <div class="section-header">
              <h3>關聯寵物</h3>
              <el-button type="primary" size="small" @click="openAddPetDialog">
                <el-icon><Plus /></el-icon>
                新增寵物關聯
              </el-button>
            </div>

            <div class="pets-list" v-loading="petsLoading">
              <el-table :data="relatedPets" border size="small">
                <el-table-column prop="petName" label="寵物名稱" width="150">
                  <template #default="{ row }">
                    {{ row.pet?.petName || row.petName || '未知寵物' }}
                  </template>
                </el-table-column>
                <el-table-column prop="breed" label="品種" width="120">
                  <template #default="{ row }">
                    {{ row.pet?.breed || row.breed || '-' }}
                  </template>
                </el-table-column>
                <el-table-column prop="gender" label="性別" width="80">
                  <template #default="{ row }">
                    {{ getGenderDisplay(row.pet?.gender || row.gender) }}
                  </template>
                </el-table-column>
                <el-table-column prop="relationship" label="關係" width="100">
                  <template #default="{ row }">
                    {{ row.relationship || '-' }}
                  </template>
                </el-table-column>
                <el-table-column prop="sort" label="排序" width="80">
                  <template #default="{ row }">
                    <el-input-number
                      v-model="row.sort"
                      size="small"
                      :min="1"
                      :max="99"
                      @change="updatePetSort(row)"
                    />
                  </template>
                </el-table-column>
                <el-table-column label="操作" width="120">
                  <template #default="{ row }">
                    <el-button size="small" type="danger" @click="removePetRelation(row)">
                      移除
                    </el-button>
                  </template>
                </el-table-column>
              </el-table>

              <!-- 空狀態 -->
              <div v-if="relatedPets.length === 0" class="empty-pets">
                <el-empty description="尚未關聯任何寵物" size="small">
                  <el-button type="primary" @click="openAddPetDialog">
                    新增第一個寵物關聯
                  </el-button>
                </el-empty>
              </div>
            </div>
          </div>
        </div>

        <!-- Submit Buttons -->
        <el-form-item>
          <el-button type="primary" :loading="submitting" @click="handleSubmit">
            {{ isEdit ? '更新' : '新增' }}
          </el-button>
          <el-button @click="$router.back()">取消</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 新增寵物關聯對話框 -->
    <el-dialog
      v-model="petDialogVisible"
      title="新增寵物關聯"
      width="500px"
    >
      <el-form ref="petFormRef" :model="petFormData" :rules="petRules" label-width="80px">
        <el-form-item label="寵物" prop="petId">
          <el-select
            v-model="selectedPetId"
            placeholder="請選擇寵物"
            filterable
            remote
            :remote-method="searchPets"
            :loading="petSearchLoading"
            style="width: 100%"
          >
            <el-option
              v-for="pet in availablePets"
              :key="pet.petId"
              :label="`${pet.petName} (${pet.breed})`"
              :value="pet.petId"
            />
          </el-select>
        </el-form-item>

        <el-form-item label="關係" prop="relationshipType">
          <SystemCodeSelect
            v-model="petForm.relationshipType"
            code-type="Relationship"
            placeholder="請選擇關係"
            clearable
          />
        </el-form-item>

        <el-form-item label="排序" prop="sort">
          <el-input-number
            v-model="petForm.sort"
            :min="1"
            :max="99"
            placeholder="數字越小越優先"
          />
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button @click="petDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="savePetRelation" :loading="petSaving">
          新增
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import { ArrowLeft, Plus } from '@element-plus/icons-vue'
import type { Contact, ContactCreateRequest, ContactUpdateRequest, LinkContactToPetRequest } from '@/types/contact'
import { contactApi } from '@/api/contact'
import { petApi } from '@/api/pet'
import { commonApi } from '@/api/common'
import { SystemCodeSelect } from '@/components/common'

const route = useRoute()
const router = useRouter()

// Refs
const formRef = ref<FormInstance>()
const petFormRef = ref<FormInstance>()
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

// Computed
const isEdit = computed(() => !!contactId.value)

// Form data
const form = reactive<ContactCreateRequest>({
  name: '',
  nickName: '',
  contactNumber: ''
})

const petForm = reactive<LinkContactToPetRequest>({
  relationshipType: '',
  sort: 1
})

const selectedPetId = ref<number>(0)

const petFormData = reactive({
  petId: 0,
  relationshipType: '',
  sort: 1
})

// Form rules
const rules: FormRules = {
  name: [
    { required: true, message: '請輸入姓名', trigger: 'blur' },
    { min: 1, max: 50, message: '姓名長度應為 1-50 個字符', trigger: 'blur' }
  ],
  nickName: [
    { max: 50, message: '暱稱長度不能超過50個字符', trigger: 'blur' }
  ],
  contactNumber: [
    { required: true, message: '請輸入聯絡電話', trigger: 'blur' },
    { min: 1, max: 20, message: '聯絡電話長度應為 1-20 個字符', trigger: 'blur' }
  ]
}

const petRules: FormRules = {
  petId: [
    { required: true, message: '請選擇寵物', trigger: 'change' }
  ],
  relationshipType: [
    { required: true, message: '請選擇關係', trigger: 'change' }
  ]
}

// Methods
const loadGenders = async () => {
  try {
    genders.value = await commonApi.getSystemCodes('Gender')
  } catch (error) {
    console.error('Load genders error:', error)
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
    ElMessage.error('載入聯絡人資料失敗')
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
    ElMessage.error('載入關聯寵物失敗')
  } finally {
    petsLoading.value = false
  }
}

const searchPets = async (query: string) => {
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
    availablePets.value = response.data
  } catch (error) {
    console.error('Search pets error:', error)
    ElMessage.error('搜尋寵物失敗')
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
  Object.assign(petFormData, {
    petId: 0,
    relationshipType: '',
    sort: relatedPets.value.length + 1
  })
  petDialogVisible.value = true
}

const savePetRelation = async () => {
  if (!petFormRef.value || !contactId.value) return

  try {
    const valid = await petFormRef.value.validate()
    if (!valid) return

    petSaving.value = true

    await contactApi.linkContactToPet(contactId.value, selectedPetId.value, petForm)
    ElMessage.success('新增成功')
    petDialogVisible.value = false
    await loadRelatedPets()
  } catch (error: any) {
    console.error('Save pet relation error:', error)
    ElMessage.error(error.response?.data?.message || '新增失敗')
  } finally {
    petSaving.value = false
  }
}

const updatePetSort = async (relation: any) => {
  try {
    // Note: This would require a separate API to update sort order
    ElMessage.info('排序功能待實作')
  } catch (error: any) {
    console.error('Update sort error:', error)
    ElMessage.error('更新排序失敗')
  }
}

const removePetRelation = async (relation: any) => {
  try {
    await ElMessageBox.confirm(
      `確定要移除寵物「${relation.petName}」的關聯嗎？`,
      '確認移除',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )

    if (contactId.value) {
      await contactApi.unlinkContactFromPet(contactId.value, relation.petId)
      ElMessage.success('移除成功')
      await loadRelatedPets()
    }
  } catch (error: any) {
    if (error !== 'cancel') {
      console.error('Remove pet relation error:', error)
      ElMessage.error('移除失敗')
    }
  }
}

const handleSubmit = async () => {
  if (!formRef.value) return

  try {
    const valid = await formRef.value.validate()
    if (!valid) return

    submitting.value = true

    if (isEdit.value && contactId.value) {
      const updateData: ContactUpdateRequest = {
        ...form,
        contactPersonId: contactId.value
      }
      await contactApi.updateContact(updateData)
      ElMessage.success('更新成功')
    } else {
      await contactApi.createContact(form)
      ElMessage.success('新增成功')
    }

    router.push('/contacts')
  } catch (error: any) {
    console.error('Submit error:', error)
    ElMessage.error(error.response?.data?.message || '操作失敗')
  } finally {
    submitting.value = false
  }
}

// Lifecycle
onMounted(async () => {
  await loadGenders()
  
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
  color: #303133;
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
  color: #303133;
  font-size: 16px;
}

.pets-list {
  background: #f8f9fa;
  padding: 16px;
  border-radius: 6px;
  border: 1px solid #e4e7ed;
}

.empty-pets {
  padding: 20px;
  text-align: center;
}
</style>