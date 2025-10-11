/**
 * MSW Browser Configuration
 *
 * 此檔案配置 Mock Service Worker 在瀏覽器環境中運行
 * 會在 main.ts 中條件性載入
 */
import { setupWorker } from 'msw/browser'
import { petHandlers } from './handlers/petHandlers'
import { contactHandlers } from './handlers/contactHandlers'
import { reservationHandlers } from './handlers/reservationHandlers'
import { subscriptionHandlers } from './handlers/subscriptionHandlers'
import { dashboardHandlers } from './handlers/dashboardHandlers'
import { commonHandlers } from './handlers/commonHandlers'

// 註冊所有 handlers
export const worker = setupWorker(
  ...petHandlers,
  ...contactHandlers,
  ...reservationHandlers,
  ...subscriptionHandlers,
  ...dashboardHandlers,
  ...commonHandlers
)

// 開發環境日誌
if (import.meta.env.DEV) {
  const totalHandlers =
    petHandlers.length +
    contactHandlers.length +
    reservationHandlers.length +
    subscriptionHandlers.length +
    dashboardHandlers.length +
    commonHandlers.length

  console.log(`🔧 MSW Mock Service Worker initialized with ${totalHandlers} handlers`)
  console.log('Handler breakdown:')
  console.log(`  - Pet handlers: ${petHandlers.length}`)
  console.log(`  - Contact handlers: ${contactHandlers.length}`)
  console.log(`  - Reservation handlers: ${reservationHandlers.length}`)
  console.log(`  - Subscription handlers: ${subscriptionHandlers.length}`)
  console.log(`  - Dashboard handlers: ${dashboardHandlers.length}`)
  console.log(`  - Common handlers: ${commonHandlers.length}`)
}
