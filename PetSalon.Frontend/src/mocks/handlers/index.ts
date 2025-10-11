/**
 * MSW Handlers 統一導出
 *
 * 集中管理所有 API Handlers
 */
export { petHandlers } from './petHandlers'
export { contactHandlers } from './contactHandlers'
export { reservationHandlers } from './reservationHandlers'
export { subscriptionHandlers } from './subscriptionHandlers'
export { dashboardHandlers } from './dashboardHandlers'
export { commonHandlers } from './commonHandlers'

// 導出所有 handlers 的合併陣列
import { petHandlers } from './petHandlers'
import { contactHandlers } from './contactHandlers'
import { reservationHandlers } from './reservationHandlers'
import { subscriptionHandlers } from './subscriptionHandlers'
import { dashboardHandlers } from './dashboardHandlers'
import { commonHandlers } from './commonHandlers'

export const allHandlers = [
  ...petHandlers,
  ...contactHandlers,
  ...reservationHandlers,
  ...subscriptionHandlers,
  ...dashboardHandlers,
  ...commonHandlers
]
