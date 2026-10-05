import { http } from '@/utils/http.js'
import { initRequest } from '@/utils/request.js'

initRequest()

export type RequestConfig<T = any> = {
  url: string
  method?: string
  data?: any
  headers?: Record<string, string>
  responseType?: 'text' | 'arraybuffer'
  custom?: any
}

export function service<T = any>(config: RequestConfig<T>) {
  const { url, method = 'GET', data, headers, responseType, custom } = config
  return http.request({
    url,
    method: method.toUpperCase(),
    data,
    header: headers,
    responseType,
    custom
  })
}

const canceled = new Set<string>()

export function cancelRequest(url: string) {
  canceled.add(url)
}
