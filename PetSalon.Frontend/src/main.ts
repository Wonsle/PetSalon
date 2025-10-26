import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'

// PrimeVue
import PrimeVue from 'primevue/config'
import Aura from '@primevue/themes/aura'
import 'primeicons/primeicons.css'

// PrimeVue Components
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Select from 'primevue/select'
import Dropdown from 'primevue/dropdown'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Dialog from 'primevue/dialog'
import Toast from 'primevue/toast'
import ToastService from 'primevue/toastservice'
import ConfirmDialog from 'primevue/confirmdialog'
import ConfirmationService from 'primevue/confirmationservice'
import Card from 'primevue/card'
import InputNumber from 'primevue/inputnumber'
import Textarea from 'primevue/textarea'
import FileUpload from 'primevue/fileupload'
import Image from 'primevue/image'
import Tag from 'primevue/tag'
import Paginator from 'primevue/paginator'
import ProgressSpinner from 'primevue/progressspinner'
import Menu from 'primevue/menu'
import Menubar from 'primevue/menubar'
import PanelMenu from 'primevue/panelmenu'
import MegaMenu from 'primevue/megamenu'
import Sidebar from 'primevue/sidebar'
import Divider from 'primevue/divider'
import Password from 'primevue/password'
import Calendar from 'primevue/calendar'
import Message from 'primevue/message'
import Tooltip from 'primevue/tooltip'
import Checkbox from 'primevue/checkbox'

// Permission directive
import permissionDirective from '@/directives/permission'

/**
 * 條件性啟用 MSW Mock
 * 只在 VITE_USE_MOCK=true 時載入
 */
async function enableMocking() {
  // 檢查環境變數
  if (import.meta.env.VITE_USE_MOCK !== 'true') {
    return
  }

  console.log('🚀 Starting MSW in mock mode...')

  // 動態導入 MSW worker
  const { worker } = await import('./mocks/browser')

  // 啟動 Service Worker
  return worker.start({
    onUnhandledRequest: 'bypass', // 未匹配的請求繼續發送到真實服務器
    serviceWorker: {
      url: '/mockServiceWorker.js'
    }
  })
}

// 條件性啟動 MSW 後再創建應用
enableMocking().then(() => {
  const app = createApp(App)

  // 注意：已移除Element Plus相關設定，專案現在僅使用PrimeVue

  // Register PrimeVue components
  app.component('Button', Button)
  app.component('InputText', InputText)
  app.component('Select', Select)
  app.component('Dropdown', Dropdown)
  app.component('DataTable', DataTable)
  app.component('Column', Column)
  app.component('Dialog', Dialog)
  app.component('Toast', Toast)
  app.component('ConfirmDialog', ConfirmDialog)
  app.component('Card', Card)
  app.component('InputNumber', InputNumber)
  app.component('Textarea', Textarea)
  app.component('FileUpload', FileUpload)
  app.component('Image', Image)
  app.component('Tag', Tag)
  app.component('Paginator', Paginator)
  app.component('ProgressSpinner', ProgressSpinner)
  app.component('Menu', Menu)
  app.component('Menubar', Menubar)
  app.component('PanelMenu', PanelMenu)
  app.component('MegaMenu', MegaMenu)
  app.component('Sidebar', Sidebar)
  app.component('Divider', Divider)
  app.component('Password', Password)
  app.component('Calendar', Calendar)
  app.component('Message', Message)
  app.component('Checkbox', Checkbox)

  app.use(createPinia())
  app.use(router)

  // 使用PrimeVue作為主要UI框架
  app.use(PrimeVue, {
    theme: {
      preset: Aura,
      options: {
        prefix: 'p',
        darkModeSelector: '.dark-mode',
        cssLayer: false
      }
    },
    locale: {
      // 中文本地化設定
      startsWith: '開始於',
      contains: '包含',
      notContains: '不包含',
      endsWith: '結束於',
      equals: '等於',
      notEquals: '不等於',
      noFilter: '無篩選',
      lt: '小於',
      lte: '小於等於',
      gt: '大於',
      gte: '大於等於',
      dateIs: '日期為',
      dateIsNot: '日期不為',
      dateBefore: '日期早於',
      dateAfter: '日期晚於',
      clear: '清除',
      apply: '套用',
      matchAll: '符合全部',
      matchAny: '符合任一',
      addRule: '新增規則',
      removeRule: '移除規則',
      accept: '是',
      reject: '否',
      choose: '選擇',
      upload: '上傳',
      cancel: '取消',
      dayNames: ['星期日', '星期一', '星期二', '星期三', '星期四', '星期五', '星期六'],
      dayNamesShort: ['日', '一', '二', '三', '四', '五', '六'],
      dayNamesMin: ['日', '一', '二', '三', '四', '五', '六'],
      monthNames: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
      monthNamesShort: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
      today: '今天',
      weekHeader: '週',
      firstDayOfWeek: 1,
      dateFormat: 'yy/mm/dd',
      weak: '弱',
      medium: '中',
      strong: '強',
      passwordPrompt: '請輸入密碼'
    }
  })

  app.use(ToastService)
  app.use(ConfirmationService)
  app.directive('tooltip', Tooltip)
  app.use(permissionDirective)

  app.mount('#app')
})
