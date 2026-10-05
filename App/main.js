import App from './App'
import { initRequest } from './utils/request'

// #ifndef VUE3
import Vue from 'vue'
import './uni.promisify.adaptor'

Vue.config.productionTip = false
App.mpType = 'app'
const app = new Vue({
	...App
})
app.$mount()
// #endif

// #ifdef VUE3
import { createSSRApp } from 'vue'
import { registerAuthDirectives } from '@/utils/authFunction'

export function createApp() {
    const app = createSSRApp(App)

    initRequest(app)

    registerAuthDirectives(app)

    return { app }
}
// #endif
