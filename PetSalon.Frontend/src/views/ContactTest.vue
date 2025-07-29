<template>
  <div class="contact-test">
    <h1>聯絡人 API 測試</h1>
    
    <div class="section">
      <h2>測試聯絡人 API</h2>
      <Button label="取得聯絡人列表" @click="testContactList" />
      <Button label="取得聯絡人詳情 (ID: 2)" @click="() => testContactDetail(2)" />
      <Button label="取得聯絡人詳情 (ID: 4)" @click="() => testContactDetail(4)" />
      
      <h3>結果:</h3>
      <pre>{{ result }}</pre>
    </div>

    <div class="section" v-if="contactDetail">
      <h2>聯絡人詳情</h2>
      <p><strong>姓名:</strong> {{ contactDetail.name }}</p>
      <p><strong>暱稱:</strong> {{ contactDetail.nickName || '無' }}</p>
      <p><strong>電話:</strong> {{ contactDetail.contactNumber }}</p>
      
      <h3>關聯寵物 ({{ contactDetail.relatedPets?.length || 0 }} 隻):</h3>
      <div v-if="contactDetail.relatedPets && contactDetail.relatedPets.length > 0">
        <DataTable :value="contactDetail.relatedPets" size="small">
          <Column field="petName" header="寵物名稱" />
          <Column field="breed" header="品種" />
          <Column field="gender" header="性別">
            <template #body="{ data }">
              {{ getGenderDisplay(data.gender) }}
            </template>
          </Column>
          <Column field="relationshipTypeName" header="關係" />
          <Column field="sort" header="排序" />
        </DataTable>
      </div>
      <div v-else>
        <p>此聯絡人沒有關聯寵物</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { contactApi } from '@/api/contact'
import type { Contact } from '@/types/contact'

const result = ref('')
const contactDetail = ref<Contact | null>(null)

const testContactList = async () => {
  try {
    const response = await contactApi.getContacts({})
    result.value = JSON.stringify(response, null, 2)
    console.log('Contact list:', response)
  } catch (error) {
    result.value = `Error: ${error}`
    console.error('Error:', error)
  }
}

const testContactDetail = async (id: number) => {
  try {
    const contact = await contactApi.getContact(id)
    contactDetail.value = contact
    result.value = JSON.stringify(contact, null, 2)
    console.log(`Contact ${id} detail:`, contact)
    
    if (contact.relatedPets) {
      console.log(`聯絡人 ${id} 有 ${contact.relatedPets.length} 個關聯寵物:`, contact.relatedPets)
    } else {
      console.log(`聯絡人 ${id} 沒有關聯寵物`)
    }
  } catch (error) {
    result.value = `Error: ${error}`
    console.error('Error:', error)
  }
}

const getGenderDisplay = (genderCode: string) => {
  if (!genderCode) return '-'
  switch(genderCode) {
    case '1': return '男'
    case '2': return '女'
    case 'M': return '公'
    case 'F': return '母'
    default: return genderCode
  }
}
</script>

<style scoped>
.contact-test {
  padding: 20px;
}

.section {
  margin: 20px 0;
  padding: 15px;
  border: 1px solid #ddd;
  border-radius: 5px;
}

pre {
  background: #f4f4f4;
  padding: 10px;
  overflow-x: auto;
  white-space: pre-wrap;
  font-size: 12px;
}

.p-button {
  margin: 5px;
}
</style>