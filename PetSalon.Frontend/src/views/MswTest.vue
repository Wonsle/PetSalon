<template>
  <div class="msw-test-page">
    <div class="header">
      <h1>MSW Mock System Test</h1>
      <div class="status-badges">
        <span class="badge" :class="mswStatus">
          {{ mswStatus === 'active' ? '🟢' : '🔴' }} MSW Status: {{ mswStatus }}
        </span>
        <span class="badge">
          Mode: {{ import.meta.env.MODE }}
        </span>
        <span class="badge">
          Use Mock: {{ import.meta.env.VITE_USE_MOCK }}
        </span>
      </div>
    </div>

    <div class="test-section">
      <h2>Test Results</h2>
      <div v-if="loading" class="loading">
        <div class="spinner"></div>
        <p>Running tests...</p>
      </div>
      <div v-else-if="error" class="error">
        <strong>Error:</strong> {{ error }}
      </div>
      <div v-else class="results">
        <div class="summary">
          <div class="summary-item success">
            <div class="count">{{ passedCount }}</div>
            <div class="label">Passed</div>
          </div>
          <div class="summary-item failure">
            <div class="count">{{ failedCount }}</div>
            <div class="label">Failed</div>
          </div>
          <div class="summary-item">
            <div class="count">{{ totalCount }}</div>
            <div class="label">Total</div>
          </div>
        </div>

        <div v-for="(result, key) in testResults" :key="key" class="test-result" :class="{ 'failure': !result.success }">
          <div class="test-header">
            <span :class="result.success ? 'success' : 'failure'">
              {{ result.success ? '✅' : '❌' }}
            </span>
            <strong>{{ key }}</strong>
            <span class="test-time">{{ result.duration }}ms</span>
          </div>
          <div class="test-message">{{ result.message }}</div>
          <div v-if="result.data" class="result-data">
            <button @click="toggleData(key)" class="toggle-btn">
              {{ expandedTests.has(key) ? '▼ Hide Data' : '▶ Show Data' }}
            </button>
            <pre v-if="expandedTests.has(key)">{{ JSON.stringify(result.data, null, 2) }}</pre>
          </div>
          <div v-if="result.error" class="error-details">
            <strong>Error:</strong> {{ result.error }}
          </div>
        </div>
      </div>
    </div>

    <div class="actions">
      <button @click="runTests" :disabled="loading" class="btn-primary">
        {{ loading ? 'Testing...' : 'Run Tests Again' }}
      </button>
      <button @click="checkServiceWorker" class="btn-secondary">
        Check Service Worker
      </button>
      <button @click="clearResults" class="btn-secondary">
        Clear Results
      </button>
    </div>

    <div class="info-section">
      <h3>How to Verify MSW is Working:</h3>
      <ol>
        <li>Open DevTools (F12)</li>
        <li>Check the Console tab for MSW initialization messages</li>
        <li>Check the Network tab - intercepted requests should show "(from ServiceWorker)"</li>
        <li>Check Application > Service Workers - should see mockServiceWorker.js running</li>
      </ol>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { petApi } from '@/api/pet'
import { contactApi } from '@/api/contact'
import { reservationApi } from '@/api/reservation'
import { subscriptionApi } from '@/api/subscription'
import { dashboardApi } from '@/api/dashboard'
import { commonApi } from '@/api/common'

const loading = ref(false)
const error = ref('')
const testResults = ref<Record<string, any>>({})
const expandedTests = ref(new Set<string>())
const mswStatus = ref('unknown')

const passedCount = computed(() => Object.values(testResults.value).filter((r: any) => r.success).length)
const failedCount = computed(() => Object.values(testResults.value).filter((r: any) => !r.success).length)
const totalCount = computed(() => Object.keys(testResults.value).length)

function toggleData(key: string) {
  if (expandedTests.value.has(key)) {
    expandedTests.value.delete(key)
  } else {
    expandedTests.value.add(key)
  }
}

function clearResults() {
  testResults.value = {}
  error.value = ''
  expandedTests.value.clear()
}

async function checkServiceWorker() {
  if ('serviceWorker' in navigator) {
    const registrations = await navigator.serviceWorker.getRegistrations()
    const mswWorker = registrations.find(reg => reg.active?.scriptURL.includes('mockServiceWorker'))

    if (mswWorker) {
      mswStatus.value = 'active'
      alert('✅ MSW Service Worker is active!\n\nScript: ' + mswWorker.active?.scriptURL)
    } else {
      mswStatus.value = 'inactive'
      alert('❌ MSW Service Worker not found!\n\nMake sure you started the app with: npm run dev:mock')
    }
  } else {
    alert('❌ Service Workers are not supported in this browser')
  }
}

async function runTest(name: string, testFn: () => Promise<any>) {
  const startTime = performance.now()
  try {
    const result = await testFn()
    const duration = Math.round(performance.now() - startTime)

    return {
      success: true,
      message: 'Test passed',
      data: result,
      duration
    }
  } catch (e: any) {
    const duration = Math.round(performance.now() - startTime)
    return {
      success: false,
      message: e.message || 'Test failed',
      error: e.response?.data || e.message,
      duration
    }
  }
}

async function runTests() {
  loading.value = true
  error.value = ''
  testResults.value = {}

  console.log('🧪 Starting MSW Test Suite...')

  try {
    // Test 1: Get Pets
    console.log('Testing: Get Pets')
    testResults.value['1. Get Pets (paginated)'] = await runTest('Get Pets', async () => {
      const result = await petApi.getPets({ page: 1, pageSize: 5 })
      if (!result.data || result.data.length === 0) {
        throw new Error('No pets returned')
      }
      return {
        total: result.total,
        returned: result.data.length,
        firstPet: result.data[0]
      }
    })

    // Test 2: Get Single Pet
    console.log('Testing: Get Single Pet')
    testResults.value['2. Get Pet by ID'] = await runTest('Get Pet by ID', async () => {
      const result = await petApi.getPet(1)
      if (!result || !result.petId) {
        throw new Error('Invalid pet data')
      }
      return result
    })

    // Test 3: Get Contacts
    console.log('Testing: Get Contacts')
    testResults.value['3. Get Contacts (paginated)'] = await runTest('Get Contacts', async () => {
      const result = await contactApi.getContacts({ page: 1, pageSize: 5 })
      if (!result.data || result.data.length === 0) {
        throw new Error('No contacts returned')
      }
      return {
        total: result.total,
        returned: result.data.length,
        firstContact: result.data[0]
      }
    })

    // Test 4: Get Single Contact
    console.log('Testing: Get Single Contact')
    testResults.value['4. Get Contact by ID'] = await runTest('Get Contact by ID', async () => {
      const result = await contactApi.getContact(1)
      if (!result || !result.contactPersonId) {
        throw new Error('Invalid contact data')
      }
      return result
    })

    // Test 5: Get Reservations
    console.log('Testing: Get Reservations')
    testResults.value['5. Get Reservations (paginated)'] = await runTest('Get Reservations', async () => {
      const result = await reservationApi.getReservations({ page: 1, pageSize: 5 })
      return {
        total: result.total,
        returned: result.data?.length || 0
      }
    })

    // Test 6: Get Subscriptions
    console.log('Testing: Get Subscriptions')
    testResults.value['6. Get Subscriptions'] = await runTest('Get Subscriptions', async () => {
      const result = await subscriptionApi.getSubscriptions()
      if (!Array.isArray(result)) {
        throw new Error('Subscriptions should be an array')
      }
      return {
        count: result.length,
        firstSubscription: result[0] || null
      }
    })

    // Test 7: Get Dashboard Statistics
    console.log('Testing: Get Dashboard Statistics')
    testResults.value['7. Dashboard Statistics'] = await runTest('Dashboard Statistics', async () => {
      const result = await dashboardApi.getStatistics()
      if (!result || typeof result.totalPets === 'undefined') {
        throw new Error('Invalid statistics data')
      }
      return result
    })

    // Test 8: Get System Code Types
    console.log('Testing: Get System Code Types')
    testResults.value['8. System Code Types'] = await runTest('System Code Types', async () => {
      const result = await commonApi.getSystemCodeTypes()
      if (!Array.isArray(result) || result.length === 0) {
        throw new Error('No system code types returned')
      }
      return { types: result }
    })

    // Test 9: Get Breed Codes
    console.log('Testing: Get Breed Codes')
    testResults.value['9. Breed System Codes'] = await runTest('Breed Codes', async () => {
      const result = await commonApi.getSystemCodes('Breed')
      if (!Array.isArray(result) || result.length === 0) {
        throw new Error('No breed codes returned')
      }
      return {
        count: result.length,
        breeds: result.slice(0, 3).map(b => b.name)
      }
    })

    // Test 10: Get Today Reservations
    console.log('Testing: Get Today Reservations')
    testResults.value['10. Today Reservations'] = await runTest('Today Reservations', async () => {
      const result = await dashboardApi.getTodayReservations()
      return {
        count: Array.isArray(result) ? result.length : 0
      }
    })

    console.log('✅ All tests completed!')

  } catch (e: any) {
    console.error('❌ Test suite failed:', e)
    error.value = e.message
  } finally {
    loading.value = false
  }
}

// Check MSW status on mount
checkServiceWorker()

// Auto-run tests on mount
runTests()
</script>

<style scoped>
.msw-test-page {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
}

.header {
  margin-bottom: 2rem;
}

h1 {
  color: #2c3e50;
  margin-bottom: 1rem;
}

.status-badges {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
}

.badge {
  padding: 0.5rem 1rem;
  background: #f0f0f0;
  border-radius: 4px;
  font-size: 0.875rem;
  font-weight: 500;
}

.badge.active {
  background: #d4edda;
  color: #155724;
}

.badge.inactive {
  background: #f8d7da;
  color: #721c24;
}

h2 {
  color: #42b983;
  margin-bottom: 1rem;
}

h3 {
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.test-section {
  background: #f8f9fa;
  padding: 1.5rem;
  border-radius: 8px;
  margin-bottom: 1rem;
}

.loading {
  text-align: center;
  padding: 2rem;
}

.spinner {
  width: 40px;
  height: 40px;
  margin: 0 auto 1rem;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #42b983;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.summary {
  display: flex;
  gap: 1rem;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
}

.summary-item {
  flex: 1;
  min-width: 100px;
  background: white;
  padding: 1rem;
  border-radius: 8px;
  text-align: center;
  border: 2px solid #e0e0e0;
}

.summary-item.success {
  border-color: #67c23a;
}

.summary-item.failure {
  border-color: #f56c6c;
}

.summary-item .count {
  font-size: 2rem;
  font-weight: bold;
  color: #2c3e50;
}

.summary-item .label {
  font-size: 0.875rem;
  color: #666;
  margin-top: 0.25rem;
}

.test-result {
  padding: 1rem;
  margin-bottom: 1rem;
  background: white;
  border-radius: 4px;
  border-left: 4px solid #67c23a;
}

.test-result.failure {
  border-left-color: #f56c6c;
}

.test-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
}

.test-header strong {
  flex: 1;
}

.test-time {
  font-size: 0.75rem;
  color: #999;
  padding: 0.25rem 0.5rem;
  background: #f5f5f5;
  border-radius: 3px;
}

.test-message {
  color: #666;
  font-size: 0.875rem;
  margin-bottom: 0.5rem;
}

.success {
  color: #67c23a;
  font-size: 1.2rem;
}

.failure {
  color: #f56c6c;
  font-size: 1.2rem;
}

.error {
  color: #f56c6c;
  padding: 1rem;
  background: #fef0f0;
  border-radius: 4px;
  border-left: 4px solid #f56c6c;
}

.error-details {
  margin-top: 0.5rem;
  padding: 0.5rem;
  background: #fef0f0;
  border-radius: 4px;
  font-size: 0.875rem;
  color: #f56c6c;
}

.result-data {
  margin-top: 0.5rem;
}

.toggle-btn {
  background: #f5f5f5;
  border: 1px solid #ddd;
  padding: 0.25rem 0.5rem;
  border-radius: 3px;
  cursor: pointer;
  font-size: 0.75rem;
  margin-bottom: 0.5rem;
}

.toggle-btn:hover {
  background: #e0e0e0;
}

.result-data pre {
  margin: 0;
  padding: 0.5rem;
  background: #f5f5f5;
  border-radius: 4px;
  font-size: 0.75rem;
  overflow-x: auto;
  max-height: 300px;
  overflow-y: auto;
}

.actions {
  display: flex;
  gap: 1rem;
  margin-bottom: 1rem;
  flex-wrap: wrap;
}

button {
  padding: 0.75rem 1.5rem;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
  border: none;
  transition: all 0.3s;
}

.btn-primary {
  background: #42b983;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background: #35a372;
}

.btn-secondary {
  background: #6c757d;
  color: white;
}

.btn-secondary:hover {
  background: #5a6268;
}

button:disabled {
  background: #ccc;
  cursor: not-allowed;
  opacity: 0.6;
}

.info-section {
  background: #e7f3ff;
  padding: 1.5rem;
  border-radius: 8px;
  border-left: 4px solid #2196F3;
}

.info-section ol {
  margin: 0.5rem 0 0 1.5rem;
}

.info-section li {
  margin-bottom: 0.5rem;
}
</style>
