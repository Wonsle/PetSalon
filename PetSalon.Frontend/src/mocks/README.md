# MSW Mock System - Quick Start Guide

This directory contains the Mock Service Worker (MSW) configuration for the PetSalon Frontend application, enabling frontend development without a running backend.

## Quick Start

### 1. Start in Mock Mode

```bash
npm run dev:mock
```

The application will start at `http://127.0.0.1:3000` with all API calls intercepted by MSW.

### 2. Verify It's Working

Open your browser and check:

1. **Console Output**:
   ```
   🚀 Starting MSW in mock mode...
   🔧 MSW Mock Service Worker initialized with 46 handlers
   ```

2. **Network Tab**: Requests show "(from ServiceWorker)"

3. **Test Page**: Visit `http://127.0.0.1:3000/#/msw-test`

## What's Included

### 📦 Modules Covered

- **Pets** (8 handlers) - Full CRUD + photo upload
- **Contacts** (9 handlers) - Full CRUD + pet relationships
- **Reservations** (11 handlers) - CRUD + calendar + calculations
- **Subscriptions** (9 handlers) - CRUD + usage tracking
- **Dashboard** (5 handlers) - Statistics and summaries
- **Common** (6 handlers) - System codes + file upload

### 📁 Directory Structure

```
src/mocks/
├── browser.ts              # MSW worker setup
├── handlers/               # Request handlers
│   ├── petHandlers.ts
│   ├── contactHandlers.ts
│   ├── reservationHandlers.ts
│   ├── subscriptionHandlers.ts
│   ├── dashboardHandlers.ts
│   └── commonHandlers.ts
├── data/                   # Mock data generators
│   ├── pets.ts
│   ├── contacts.ts
│   ├── reservations.ts
│   ├── subscriptions.ts
│   ├── dashboard.ts
│   └── systemCodes.ts
├── README.md              # This file
└── TEST_REPORT.md         # Comprehensive testing documentation
```

## How It Works

```
Vue App ──HTTP Request──> MSW Service Worker ──Mock Response──> Vue App
                              │
                              ├─ Match handler
                              ├─ Generate/fetch mock data
                              └─ Simulate delay
```

MSW intercepts HTTP requests at the **network level** using a Service Worker, making it transparent to your application code. No changes needed to API calls!

## Common Use Cases

### Adding New Mock Data

Edit the data files in `src/mocks/data/`:

```typescript
// Example: Add a new pet
export const mockPets: Pet[] = [
  {
    petId: 21,
    petName: 'Max',
    breed: 'BRDL',
    // ... other properties
  }
]
```

### Adding New Handlers

Create handlers in `src/mocks/handlers/`:

```typescript
// Example: Add new endpoint
export const myHandlers = [
  http.get('/api/my-endpoint', async () => {
    await delay(400)
    return HttpResponse.json({ data: 'mock data' })
  })
]
```

Then register in `browser.ts`:

```typescript
import { myHandlers } from './handlers/myHandlers'

export const worker = setupWorker(
  ...petHandlers,
  ...myHandlers  // Add here
)
```

### Adjusting Response Delays

Change delays in handler files to simulate network conditions:

```typescript
await delay(300)  // Fast read
await delay(500)  // Medium operation
await delay(800)  // Slow write
```

### Simulating Errors

```typescript
http.get('/api/pet/:id', ({ params }) => {
  if (params.id === '999') {
    return new HttpResponse(null, {
      status: 404,
      statusText: 'Pet not found'
    })
  }
  // Normal response...
})
```

## Configuration

### Environment Variables

```bash
# .env.mock
VITE_USE_MOCK=true      # Enable MSW
VITE_API_BASE_URL=      # Empty (no backend needed)
```

### Vite Configuration

The Vite proxy is automatically disabled in mock mode (see `vite.config.ts`):

```typescript
export default defineConfig(({ mode }) => {
  const useMock = mode === 'mock'

  return {
    server: {
      proxy: useMock ? undefined : { /* proxy config */ }
    }
  }
})
```

### Main.ts Integration

MSW is conditionally loaded based on `VITE_USE_MOCK`:

```typescript
async function enableMocking() {
  if (import.meta.env.VITE_USE_MOCK !== 'true') {
    return
  }

  const { worker } = await import('./mocks/browser')
  return worker.start()
}
```

## Testing

### Manual Testing

1. Start mock mode: `npm run dev:mock`
2. Navigate to test page: `http://127.0.0.1:3000/#/msw-test`
3. View results and verify all tests pass

### E2E Testing

MSW works seamlessly with Cypress and Playwright:

```typescript
// Cypress example
beforeEach(() => {
  cy.visit('/')  // MSW automatically intercepts
})
```

## Troubleshooting

### MSW Not Starting

✅ **Check**: `VITE_USE_MOCK=true` in `.env.mock`
✅ **Check**: Started with `npm run dev:mock`
✅ **Check**: No console errors
✅ **Fix**: Clear cache and reload

### Requests Not Intercepted

✅ **Check**: Service Worker registered (DevTools > Application)
✅ **Check**: Handler paths match API exactly
✅ **Check**: Vite proxy is disabled (in mock mode)
✅ **Fix**: Hard refresh (Cmd+Shift+R / Ctrl+Shift+R)

### Data Not Persisting

⚠️ **This is normal** - Mock data resets on page reload
💡 **Solution**: Use localStorage if persistence needed

### Wrong API Response

✅ **Check**: Handler path matches request path
✅ **Check**: HTTP method matches (GET, POST, PUT, DELETE)
✅ **Check**: Response format matches type definitions
✅ **Fix**: Review handler code and MSW console logs

## Best Practices

### DO ✅

- Use mock mode for feature development
- Add new handlers as you build features
- Keep mock data realistic
- Document special mock scenarios
- Test in both mock and real modes

### DON'T ❌

- Ship MSW to production
- Rely on mock data persistence
- Mix mock and real API calls
- Forget to update mocks when API changes
- Use complex logic in handlers

## Advanced Features

### Dynamic Responses

```typescript
http.get('/api/pet/:id', ({ params, request }) => {
  const url = new URL(request.url)
  const includeDetails = url.searchParams.get('details') === 'true'

  // Return different data based on query params
})
```

### Stateful Mocks

```typescript
let counter = 0

http.post('/api/pet', async ({ request }) => {
  const body = await request.json()
  counter++

  return HttpResponse.json({ id: counter, ...body })
})
```

### Conditional Mocking

```typescript
http.get('/api/pet/:id', ({ params }) => {
  // Mock specific scenarios
  if (params.id === 'error') {
    return new HttpResponse(null, { status: 500 })
  }

  if (params.id === 'slow') {
    await delay(5000) // Super slow
  }

  // Normal response
})
```

## Resources

- **MSW Documentation**: https://mswjs.io/
- **Full Test Report**: `TEST_REPORT.md`
- **Project README**: `/CLAUDE.md`

## Support

For issues or questions:

1. Check `TEST_REPORT.md` for detailed documentation
2. Review browser console for MSW logs
3. Verify Service Worker status in DevTools
4. Check handler configuration matches API expectations

---

**Last Updated**: 2025-10-11
**MSW Version**: 2.7.0
**Status**: ✅ Operational
