<template>
  <div class="pet-dropdown-test">
    <h1>🐾 寵物下拉選單測試頁面</h1>

    <div class="test-sections">
      <!-- 基本用法 -->
      <Card class="test-section">
        <template #title>基本用法</template>
        <template #content>
          <div class="demo-container">
            <label>選擇寵物：</label>
            <PetDropdown v-model="selectedPetId1" @change="handlePetChange1" />

            <div v-if="selectedPetId1" class="result-display">
              <h4>選中的寵物 ID：{{ selectedPetId1 }}</h4>
              <pre>{{ JSON.stringify(selectedPetData1, null, 2) }}</pre>
            </div>
          </div>
        </template>
      </Card>

      <!-- 自定義 placeholder -->
      <Card class="test-section">
        <template #title>自定義 placeholder</template>
        <template #content>
          <div class="demo-container">
            <label>請選擇要預約的寵物：</label>
            <PetDropdown
              v-model="selectedPetId2"
              placeholder="請選擇要預約的寵物"
              @change="handlePetChange2"
            />

            <p v-if="selectedPetId2" class="result-text">
              已選擇：{{ selectedPetData2?.petName }}
            </p>
          </div>
        </template>
      </Card>

      <!-- 禁用狀態 -->
      <Card class="test-section">
        <template #title>禁用狀態</template>
        <template #content>
          <div class="demo-container">
            <div class="checkbox-field">
              <Checkbox v-model="isDisabled" binary input-id="disable-checkbox" />
              <label for="disable-checkbox">禁用下拉選單</label>
            </div>

            <label>選擇寵物：</label>
            <PetDropdown v-model="selectedPetId3" :disabled="isDisabled" />
          </div>
        </template>
      </Card>

      <!-- 不顯示清除按鈕 -->
      <Card class="test-section">
        <template #title>不顯示清除按鈕</template>
        <template #content>
          <div class="demo-container">
            <label>選擇寵物（必選）：</label>
            <PetDropdown v-model="selectedPetId4" :show-clear="false" />
          </div>
        </template>
      </Card>

      <!-- 不啟用搜尋過濾 -->
      <Card class="test-section">
        <template #title>不啟用搜尋過濾</template>
        <template #content>
          <div class="demo-container">
            <label>選擇寵物：</label>
            <PetDropdown v-model="selectedPetId5" :filter="false" />
          </div>
        </template>
      </Card>

      <!-- 表單集成範例 -->
      <Card class="test-section">
        <template #title>表單集成範例</template>
        <template #content>
          <div class="demo-container">
            <div class="form-grid">
              <div class="form-field">
                <label>寵物 *</label>
                <PetDropdown v-model="form.petId" :show-clear="false" />
              </div>

              <div class="form-field">
                <label>預約日期 *</label>
                <Calendar
                  v-model="form.date"
                  date-format="yy/mm/dd"
                  show-icon
                />
              </div>

              <div class="form-field">
                <label>備註</label>
                <Textarea v-model="form.note" rows="3" />
              </div>
            </div>

            <div class="form-actions">
              <Button label="提交表單" icon="pi pi-check" @click="submitForm" />
              <Button
                label="重置"
                icon="pi pi-refresh"
                severity="secondary"
                @click="resetForm"
              />
            </div>

            <div v-if="formSubmitted" class="result-display">
              <h4>表單數據：</h4>
              <pre>{{ JSON.stringify(form, null, 2) }}</pre>
            </div>
          </div>
        </template>
      </Card>

      <!-- 使用說明 -->
      <Card class="test-section">
        <template #title>使用說明</template>
        <template #content>
          <div class="usage-docs">
            <h3>組件特性</h3>
            <ul>
              <li>✅ 支援 v-model 雙向綁定</li>
              <li>✅ 自動載入寵物列表（含聯絡人資訊）</li>
              <li>✅ 圖文並列顯示（左側縮圖 + 右側寵物名稱、品種、毛色）</li>
              <li>✅ 滑鼠懸停顯示聯絡人資訊 Tooltip</li>
              <li>✅ 支援搜尋過濾功能</li>
              <li>✅ 響應式設計（支援手機和桌面）</li>
            </ul>

            <h3>Props</h3>
            <table class="props-table">
              <thead>
                <tr>
                  <th>屬性</th>
                  <th>類型</th>
                  <th>預設值</th>
                  <th>說明</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><code>modelValue</code></td>
                  <td>number | null</td>
                  <td>-</td>
                  <td>選中的寵物 ID（支援 v-model）</td>
                </tr>
                <tr>
                  <td><code>placeholder</code></td>
                  <td>string</td>
                  <td>'請選擇寵物'</td>
                  <td>未選擇時的提示文字</td>
                </tr>
                <tr>
                  <td><code>disabled</code></td>
                  <td>boolean</td>
                  <td>false</td>
                  <td>是否禁用</td>
                </tr>
                <tr>
                  <td><code>showClear</code></td>
                  <td>boolean</td>
                  <td>true</td>
                  <td>是否顯示清除按鈕</td>
                </tr>
                <tr>
                  <td><code>filter</code></td>
                  <td>boolean</td>
                  <td>true</td>
                  <td>是否啟用搜尋過濾</td>
                </tr>
              </tbody>
            </table>

            <h3>Events</h3>
            <table class="props-table">
              <thead>
                <tr>
                  <th>事件名稱</th>
                  <th>參數</th>
                  <th>說明</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td><code>update:modelValue</code></td>
                  <td>(value: number | null)</td>
                  <td>選中值變更時觸發</td>
                </tr>
                <tr>
                  <td><code>change</code></td>
                  <td>(pet: Pet | null)</td>
                  <td>選擇變更時觸發，返回完整的寵物資料</td>
                </tr>
              </tbody>
            </table>

            <h3>基本用法</h3>
            <pre class="code-block"><code>&lt;template&gt;
  &lt;PetDropdown v-model="selectedPetId" @change="handlePetChange" /&gt;
&lt;/template&gt;

&lt;script setup lang="ts"&gt;
import { ref } from 'vue'
import PetDropdown from '@/components/common/PetDropdown.vue'
import type { Pet } from '@/types/pet'

const selectedPetId = ref&lt;number | null&gt;(null)

const handlePetChange = (pet: Pet | null) => {
  console.log('選中的寵物:', pet)
}
&lt;/script&gt;</code></pre>

            <h3>表單中使用</h3>
            <pre class="code-block"><code>&lt;div class="form-field"&gt;
  &lt;label&gt;寵物 *&lt;/label&gt;
  &lt;PetDropdown
    v-model="form.petId"
    placeholder="請選擇要預約的寵物"
    :show-clear="false"
  /&gt;
&lt;/div&gt;</code></pre>
          </div>
        </template>
      </Card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import PetDropdown from '@/components/common/PetDropdown.vue'
import type { Pet } from '@/types/pet'
import { useToast } from 'primevue/usetoast'

const toast = useToast()

// 基本用法
const selectedPetId1 = ref<number | null>(null)
const selectedPetData1 = ref<Pet | null>(null)

const handlePetChange1 = (pet: Pet | null) => {
  selectedPetData1.value = pet
  console.log('選中的寵物 1:', pet)
}

// 自定義 placeholder
const selectedPetId2 = ref<number | null>(null)
const selectedPetData2 = ref<Pet | null>(null)

const handlePetChange2 = (pet: Pet | null) => {
  selectedPetData2.value = pet
  toast.add({
    severity: 'success',
    summary: '選擇成功',
    detail: `已選擇：${pet?.petName}`,
    life: 3000
  })
}

// 禁用狀態
const selectedPetId3 = ref<number | null>(null)
const isDisabled = ref(false)

// 不顯示清除按鈕
const selectedPetId4 = ref<number | null>(null)

// 不啟用搜尋
const selectedPetId5 = ref<number | null>(null)

// 表單集成
interface ReservationForm {
  petId: number | null
  date: Date | null
  note: string
}

const form = ref<ReservationForm>({
  petId: null,
  date: null,
  note: ''
})

const formSubmitted = ref(false)

const submitForm = () => {
  if (!form.value.petId) {
    toast.add({
      severity: 'warn',
      summary: '驗證失敗',
      detail: '請選擇寵物',
      life: 3000
    })
    return
  }

  if (!form.value.date) {
    toast.add({
      severity: 'warn',
      summary: '驗證失敗',
      detail: '請選擇預約日期',
      life: 3000
    })
    return
  }

  formSubmitted.value = true
  toast.add({
    severity: 'success',
    summary: '提交成功',
    detail: '表單數據已提交',
    life: 3000
  })

  console.log('表單數據:', form.value)
}

const resetForm = () => {
  form.value = {
    petId: null,
    date: null,
    note: ''
  }
  formSubmitted.value = false

  toast.add({
    severity: 'info',
    summary: '已重置',
    detail: '表單已重置',
    life: 3000
  })
}
</script>

<style scoped>
.pet-dropdown-test {
  padding: 24px;
  max-width: 1200px;
  margin: 0 auto;
}

h1 {
  margin-bottom: 32px;
  color: #333;
}

.test-sections {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.test-section {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.demo-container {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

label {
  font-weight: 600;
  color: #333;
  margin-bottom: 4px;
  display: block;
}

.result-display {
  background-color: #f5f5f5;
  padding: 16px;
  border-radius: 8px;
  border-left: 4px solid #3b82f6;
  margin-top: 16px;
}

.result-display h4 {
  margin-top: 0;
  margin-bottom: 12px;
  color: #333;
}

.result-display pre {
  margin: 0;
  white-space: pre-wrap;
  word-wrap: break-word;
  background-color: #fff;
  padding: 12px;
  border-radius: 4px;
  font-size: 13px;
  overflow-x: auto;
}

.result-text {
  color: #10b981;
  font-weight: 600;
  margin: 0;
}

.checkbox-field {
  display: flex;
  align-items: center;
  gap: 8px;
}

.checkbox-field label {
  margin: 0;
  font-weight: 400;
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 16px;
}

.form-field {
  display: flex;
  flex-direction: column;
}

.form-actions {
  display: flex;
  gap: 12px;
  margin-top: 8px;
}

.usage-docs {
  line-height: 1.6;
}

.usage-docs h3 {
  margin-top: 24px;
  margin-bottom: 12px;
  color: #333;
  border-bottom: 2px solid #e5e7eb;
  padding-bottom: 8px;
}

.usage-docs h3:first-child {
  margin-top: 0;
}

.usage-docs ul {
  margin: 12px 0;
  padding-left: 24px;
}

.usage-docs li {
  margin: 8px 0;
}

.props-table {
  width: 100%;
  border-collapse: collapse;
  margin: 16px 0;
  font-size: 14px;
}

.props-table th,
.props-table td {
  border: 1px solid #e5e7eb;
  padding: 12px;
  text-align: left;
}

.props-table th {
  background-color: #f9fafb;
  font-weight: 600;
  color: #374151;
}

.props-table code {
  background-color: #f3f4f6;
  padding: 2px 6px;
  border-radius: 3px;
  font-size: 13px;
  font-family: 'Courier New', monospace;
  color: #dc2626;
}

.code-block {
  background-color: #1e293b;
  padding: 16px;
  border-radius: 8px;
  overflow-x: auto;
  margin: 16px 0;
}

.code-block code {
  color: #e2e8f0;
  font-family: 'Courier New', monospace;
  font-size: 13px;
  line-height: 1.6;
}

@media (max-width: 768px) {
  .pet-dropdown-test {
    padding: 16px;
  }

  h1 {
    font-size: 24px;
  }

  .form-actions {
    flex-direction: column;
  }

  .props-table {
    font-size: 12px;
  }

  .props-table th,
  .props-table td {
    padding: 8px;
  }
}
</style>
