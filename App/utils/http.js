const defaultConfig = {
  baseURL: '',
  header: {}
}

const interceptors = {
  request: {
    onFulfilled: null,
    onRejected: null,
    use(onFulfilled, onRejected) {
      this.onFulfilled = onFulfilled
      this.onRejected = onRejected
    }
  },
  response: {
    onFulfilled: null,
    onRejected: null,
    use(onFulfilled, onRejected) {
      this.onFulfilled = onFulfilled
      this.onRejected = onRejected
    }
  }
}

function setConfig(updater) {
  const next = updater({ ...defaultConfig }) || {}
  defaultConfig.baseURL = next.baseURL ?? defaultConfig.baseURL
  defaultConfig.header = next.header ?? defaultConfig.header
}

function request(options) {
  let config = {
    url: options.url,
    method: (options.method || 'GET').toUpperCase(),
    data: options.data,
    header: { ...defaultConfig.header, ...(options.header || {}) },
    responseType: options.responseType,
    custom: options.custom || {}
  }
  if (defaultConfig.baseURL && !/^https?:\/\//i.test(config.url)) {
    config.url = defaultConfig.baseURL + config.url
  }

  try {
    if (interceptors.request.onFulfilled) {
      const next = interceptors.request.onFulfilled(config)
      if (next) config = next
    }
  } catch (e) {
    if (interceptors.request.onRejected) {
      return interceptors.request.onRejected(config)
    }
  }

  return new Promise((resolve, reject) => {
    uni.request({
      url: config.url,
      data: config.data,
      header: config.header,
      method: config.method,
      responseType: config.responseType,
      success: (res) => {
        const response = {
          data: res.data,
          statusCode: res.statusCode,
          header: res.header || {},
          config
        }        
        if (response.statusCode && response.statusCode !== 200) {
          if (interceptors.response.onRejected) {
            const rej = interceptors.response.onRejected(response)
            if (rej && typeof rej.then === 'function') {
              rej.then(resolve).catch(reject)
            } else {
              reject(rej)
            }
          } else {
            reject(response)
          }
          return
        }
        if (interceptors.response.onFulfilled) {
          try {
            const result = interceptors.response.onFulfilled(response)
            if (result && typeof result.then === 'function') {
              result.then(resolve).catch(reject)
            } else {
              resolve(result)
            }
          } catch (err) {
            if (interceptors.response.onRejected) {
              const rej = interceptors.response.onRejected(response)
              if (rej && typeof rej.then === 'function') {
                rej.then(resolve).catch(reject)
              } else {
                reject(rej)
              }
            } else {
              reject(err)
            }
          }
        } else {
          console.log('response.data:', response.data)
          resolve(response.data)
        }
      },
      fail: (err) => {
        const errObj = typeof err === 'object' && err ? err : { errMsg: String(err || 'request fail') }
        if (!('statusCode' in errObj)) errObj.statusCode = 0
        if (!('data' in errObj)) errObj.data = {}
        if (interceptors.response.onRejected) {
          const rej = interceptors.response.onRejected(errObj)
          if (rej && typeof rej.then === 'function') {
            rej.then(resolve).catch(reject)
          } else {
            reject(rej)
          }
        } else {
          reject(errObj)
        }
      }
    })
  })
}

function toast(message) {
  if (message === undefined || message === null) return
  uni.showToast({ title: String(message), icon: 'none', duration: 2000 })
}

export const http = { request, setConfig, interceptors }
export { toast }
