/**
 * 单个权限验证
 * @param value 权限值
 * @returns 有权限，返回 `true`，反之则反
 */
import { VNode, withDirectives, resolveDirective } from 'vue'
import { currentUserKey } from '@/utils/request.js'

function getUserApis(): Set<string> {
  try {
    const u: any = uni.getStorageSync(currentUserKey)
    if (!u) return new Set()
    const obj = typeof u === 'string' ? JSON.parse(u || '{}') : u
    const list: Array<string> = Array.isArray(obj?.apis) ? obj.apis : []
    return new Set(list.map(s => String(s || '').toLowerCase()))
  } catch {
    return new Set()
  }
}

export function auth(value: string): boolean {
  if (!value || typeof value !== 'string') return false
  const apis = getUserApis()
  console.log("apis:" + value, apis)
  return apis.has(value.toLowerCase())
}

/**
 * 多个权限验证，满足一个则为 true
 * @param value 权限值
 * @returns 有权限，返回 `true`，反之则反
 */
export function auths(value: Array<string>): boolean {
  if (!Array.isArray(value) || value.length === 0) return false
  const apis = getUserApis()
  for (const v of value) {
    if (typeof v === 'string' && apis.has(v.toLowerCase())) return true
  }
  return false
}

/**
 * 多个权限验证，全部满足则为 true
 * @param value 权限值
 * @returns 有权限，返回 `true`，反之则反
 */
export function authAll(value: Array<string>): boolean {
  if (!Array.isArray(value) || value.length === 0) return false
  const apis = getUserApis()
  for (const v of value) {
    if (typeof v !== 'string' || !apis.has(v.toLowerCase())) return false
  }
  return true
}

function applyDisplay(el: any, show: boolean) {
  try {
    el.style.display = show ? '' : 'none'
  } catch {}
}

export function registerAuthDirectives(app: any) {
  app.directive('auth', {
    mounted(el: any, binding: any) {
      const res = auth(binding?.value)
      ;(el as any).__auth_result__ = res
      el.setAttribute('data-auth-result', res ? '1' : '0')
      applyDisplay(el, res)
    },
    updated(el: any, binding: any) {
      const res = auth(binding?.value)
      ;(el as any).__auth_result__ = res
      el.setAttribute('data-auth-result', res ? '1' : '0')
      applyDisplay(el, res)
    }
  })
  app.directive('auth-else', {
    mounted(el: any, binding: any) {
      const prev: any = el.previousElementSibling
      const res = prev && (prev.__auth_result__ ?? (prev.getAttribute('data-auth-result') === '1')) ? true : false
      applyDisplay(el, !res)
    },
    updated(el: any, binding: any) {
      const prev: any = el.previousElementSibling
      const res = prev && (prev.__auth_result__ ?? (prev.getAttribute('data-auth-result') === '1')) ? true : false
      applyDisplay(el, !res)
    }
  })
  app.directive('auths', {
    mounted(el: any, binding: any) {
      const res = auths(binding?.value)
      ;(el as any).__auth_result__ = res
      el.setAttribute('data-auth-result', res ? '1' : '0')
      applyDisplay(el, res)
    },
    updated(el: any, binding: any) {
      const res = auths(binding?.value)
      ;(el as any).__auth_result__ = res
      el.setAttribute('data-auth-result', res ? '1' : '0')
      applyDisplay(el, res)
    }
  })
  app.directive('auths-else', {
    mounted(el: any, binding: any) {
      const prev: any = el.previousElementSibling
      const res = prev && (prev.__auth_result__ ?? (prev.getAttribute('data-auth-result') === '1')) ? true : false
      applyDisplay(el, !res)
    },
    updated(el: any, binding: any) {
      const prev: any = el.previousElementSibling
      const res = prev && (prev.__auth_result__ ?? (prev.getAttribute('data-auth-result') === '1')) ? true : false
      applyDisplay(el, !res)
    }
  })
  app.directive('auth-all', {
    mounted(el: any, binding: any) {
      const res = authAll(binding?.value)
      ;(el as any).__auth_result__ = res
      el.setAttribute('data-auth-result', res ? '1' : '0')
      applyDisplay(el, res)
    },
    updated(el: any, binding: any) {
      const res = authAll(binding?.value)
      ;(el as any).__auth_result__ = res
      el.setAttribute('data-auth-result', res ? '1' : '0')
      applyDisplay(el, res)
    }
  })
  app.directive('auth-all-else', {
    mounted(el: any, binding: any) {
      const prev: any = el.previousElementSibling
      const res = prev && (prev.__auth_result__ ?? (prev.getAttribute('data-auth-result') === '1')) ? true : false
      applyDisplay(el, !res)
    },
    updated(el: any, binding: any) {
      const prev: any = el.previousElementSibling
      const res = prev && (prev.__auth_result__ ?? (prev.getAttribute('data-auth-result') === '1')) ? true : false
      applyDisplay(el, !res)
    }
  })
}
/**
 * 单个权限验证，是否满足，返回VNode
 * @param VNode 元素
 * @param value 权限值
 * @returns VNode
 */
export function hAuth<T extends VNode>(el: T, value: string): T {
	return withDirectives(el, [[resolveDirective('auth'), value]]);
}
/**
 * 多个权限验证，判断是否满足一个，返回VNode
 * @param VNode 元素
 * @param value 权限值
 * @returns VNode
 */
export function hAuths<T extends VNode>(el: T, value: Array<string>): T {
	return withDirectives(el, [[resolveDirective('auths'), value]]);
}
/**
 * 多个权限验证，判断是否全部满足，返回VNode
 * @param VNode 元素
 * @param value 权限值
 * @returns VNode
 */
export function hAuthAll<T extends VNode>(el: T, value: Array<string>): T {
	return withDirectives(el, [[resolveDirective('auth-all'), value]]);
}
