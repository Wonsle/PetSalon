/**
 * MSW Testing Script
 *
 * This script tests the MSW mock system by making HTTP requests
 * to the local development server running in mock mode.
 */

const http = require('http');
const https = require('https');

const BASE_URL = 'http://127.0.0.1:3000';
const RESULTS = [];

function makeRequest(path, method = 'GET', data = null) {
  return new Promise((resolve, reject) => {
    const url = new URL(path, BASE_URL);
    const protocol = url.protocol === 'https:' ? https : http;

    const options = {
      hostname: url.hostname,
      port: url.port,
      path: url.pathname + url.search,
      method: method,
      headers: {
        'Content-Type': 'application/json',
      }
    };

    const req = protocol.request(options, (res) => {
      let body = '';

      res.on('data', (chunk) => {
        body += chunk;
      });

      res.on('end', () => {
        try {
          const jsonBody = JSON.parse(body);
          resolve({
            status: res.statusCode,
            headers: res.headers,
            body: jsonBody
          });
        } catch (e) {
          resolve({
            status: res.statusCode,
            headers: res.headers,
            body: body
          });
        }
      });
    });

    req.on('error', (err) => {
      reject(err);
    });

    if (data) {
      req.write(JSON.stringify(data));
    }

    req.end();
  });
}

async function testEndpoint(name, path, expectedStatus = 200) {
  console.log(`\n🧪 Testing: ${name}`);
  console.log(`   URL: ${path}`);

  try {
    const result = await makeRequest(path);
    const success = result.status === expectedStatus;

    console.log(`   Status: ${result.status} ${success ? '✅' : '❌'}`);

    if (result.body) {
      const bodyPreview = typeof result.body === 'object'
        ? JSON.stringify(result.body, null, 2).substring(0, 200)
        : result.body.substring(0, 200);
      console.log(`   Response preview: ${bodyPreview}...`);

      // Additional validation
      if (typeof result.body === 'object') {
        if (Array.isArray(result.body)) {
          console.log(`   Array length: ${result.body.length}`);
        } else if (result.body.data && Array.isArray(result.body.data)) {
          console.log(`   Data array length: ${result.body.data.length}`);
          console.log(`   Total: ${result.body.total || 'N/A'}`);
        }
      }
    }

    RESULTS.push({
      name,
      path,
      success,
      status: result.status,
      expectedStatus
    });

    return success;
  } catch (err) {
    console.log(`   Error: ${err.message} ❌`);
    RESULTS.push({
      name,
      path,
      success: false,
      error: err.message
    });
    return false;
  }
}

async function runTests() {
  console.log('='.repeat(60));
  console.log('MSW Mock System Test Suite');
  console.log('='.repeat(60));
  console.log(`Base URL: ${BASE_URL}`);
  console.log(`Time: ${new Date().toISOString()}`);

  // Wait for server to be ready
  console.log('\n⏳ Waiting for server to be ready...');
  await new Promise(resolve => setTimeout(resolve, 3000));

  // Test 1: Pets API
  console.log('\n' + '─'.repeat(60));
  console.log('📦 Testing Pets API');
  console.log('─'.repeat(60));
  await testEndpoint('Get Pets (paginated)', '/api/pet?page=1&pageSize=5');
  await testEndpoint('Get Pet by ID', '/api/pet/1');
  await testEndpoint('Get Pet by ID (not found)', '/api/pet/999', 404);

  // Test 2: Contacts API
  console.log('\n' + '─'.repeat(60));
  console.log('👥 Testing Contacts API');
  console.log('─'.repeat(60));
  await testEndpoint('Get Contacts (paginated)', '/api/contactperson?page=1&pageSize=5');
  await testEndpoint('Get Contact by ID', '/api/contactperson/1');
  await testEndpoint('Get Contact by ID (not found)', '/api/contactperson/999', 404);

  // Test 3: Reservations API
  console.log('\n' + '─'.repeat(60));
  console.log('📅 Testing Reservations API');
  console.log('─'.repeat(60));
  await testEndpoint('Get Reservations (paginated)', '/api/reservation?page=1&pageSize=5');
  await testEndpoint('Get Reservation by ID', '/api/reservation/1');

  // Test 4: Subscriptions API
  console.log('\n' + '─'.repeat(60));
  console.log('💳 Testing Subscriptions API');
  console.log('─'.repeat(60));
  await testEndpoint('Get All Subscriptions', '/api/subscription');
  await testEndpoint('Get Subscription by ID', '/api/subscription/1');
  await testEndpoint('Get Subscriptions by Pet ID', '/api/subscription/pet/1');

  // Test 5: Dashboard API
  console.log('\n' + '─'.repeat(60));
  console.log('📊 Testing Dashboard API');
  console.log('─'.repeat(60));
  await testEndpoint('Get Dashboard Statistics', '/api/dashboard/statistics');
  await testEndpoint('Get Today Reservations', '/api/dashboard/today-reservations');
  await testEndpoint('Get Monthly Revenue', '/api/dashboard/monthly-revenue');

  // Test 6: Common/System Codes API
  console.log('\n' + '─'.repeat(60));
  console.log('🔧 Testing Common API');
  console.log('─'.repeat(60));
  await testEndpoint('Get System Code Types', '/api/Common/systemcode-types');
  await testEndpoint('Get Breed Codes', '/api/Common/systemcodes/Breed');
  await testEndpoint('Get Gender Codes', '/api/Common/systemcodes/Gender');
  await testEndpoint('Get Service Type Codes', '/api/Common/systemcodes/ServiceType');

  // Summary
  console.log('\n' + '='.repeat(60));
  console.log('📊 Test Summary');
  console.log('='.repeat(60));

  const passed = RESULTS.filter(r => r.success).length;
  const failed = RESULTS.filter(r => !r.success).length;
  const total = RESULTS.length;

  console.log(`Total Tests: ${total}`);
  console.log(`Passed: ${passed} ✅`);
  console.log(`Failed: ${failed} ❌`);
  console.log(`Success Rate: ${((passed / total) * 100).toFixed(2)}%`);

  if (failed > 0) {
    console.log('\n❌ Failed Tests:');
    RESULTS.filter(r => !r.success).forEach(r => {
      console.log(`   - ${r.name}`);
      console.log(`     Path: ${r.path}`);
      if (r.error) {
        console.log(`     Error: ${r.error}`);
      } else {
        console.log(`     Expected: ${r.expectedStatus}, Got: ${r.status}`);
      }
    });
  }

  console.log('\n' + '='.repeat(60));

  process.exit(failed > 0 ? 1 : 0);
}

// Run the tests
runTests().catch(err => {
  console.error('Fatal error:', err);
  process.exit(1);
});
