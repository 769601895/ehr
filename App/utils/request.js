import {
	http,
	toast
} from './http.js'
import Base64 from 'base-64'
import {
	env
} from '.env';

// token 键定义
export const accessTokenKey = 'access-token';
export const refreshAccessTokenKey = `x-${accessTokenKey}`;
export const currentUserKey = 'current-user';

// 清除 token
export const clearAccessTokens = () => {
	uni.removeStorageSync(accessTokenKey);
	uni.removeStorageSync(refreshAccessTokenKey);
	uni.clearStorageSync();
};

const requestInterceptors = (vm) => {
	/**
	 * 请求拦截
	 * @param {Object} http
	 */
	http.interceptors.request.use((config) => { // 可使用async await 做异步操作
			let accessToken = uni.getStorageSync(accessTokenKey)
			if (accessToken) {
				config['header'] = {
					Authorization: `Bearer ${accessToken}`
				}

				// 判断 accessToken 是否过期
				let jwt = decryptJWT(accessToken);
				let exp = getJWTDate(jwt.exp);
				// token 已经过期 
				if (new Date() >= exp) {
					// 获取刷新 token
					let refreshAccessToken = uni.getStorageSync(refreshAccessTokenKey);
					// 携带刷新 token
					if (refreshAccessToken) {
						config['header'] = {
							'X-Authorization': `Bearer ${refreshAccessToken}`
						}
					}
				}
			}
			// 初始化请求拦截器时，会执行此方法，此时data为undefined，赋予默认{}
			config.data = config.data || {}
			// 可以在此通过vm引用vuex中的变量，具体值在vm.$store.state中 
			return config
		}, (config) => // 可使用async await 做异步操作
		Promise.reject(config))
}

const responseInterceptors = (vm) => {
	/**
	 * 响应拦截
	 * @param {Object} http 
	 */
	http.interceptors.response.use((response) => {
		/* 对响应成功做点什么 可使用async await 做异步操作*/
		let data = response.data
		// 处理 401
		if (data.code === 401) {
			clearAccessTokens();
			// 跳转到登录页面
			uni.redirectTo({
				url: '/pages/login/login'
			});
		}

		// 处理未进行规范化处理的
		if (data.code >= 400) {
			toast(JSON.stringify(data.statusText))
		}

		// 处理规范化结果错误
		if (data && data.hasOwnProperty('errors') && data.errors) {
			toast(JSON.stringify(data.errors))
		}

		// 读取响应报文头 token 信息
		let accessToken = response.header[accessTokenKey];
		let refreshAccessToken = response.header[refreshAccessTokenKey];

		// 判断是否是无效 token
		if (accessToken === 'invalid_token') {
			clearAccessTokens();
		} else if (refreshAccessToken && accessToken && accessToken !== 'invalid_token') {
			// 判断是否存在刷新 token，如果存在则存储在本地
			uni.setStorageSync(accessTokenKey, accessToken)
			uni.setStorageSync(refreshAccessTokenKey, refreshAccessToken)
		}

		// 自定义参数
		let custom = response.config?.custom
		if (data.code !== 200) { // 服务端返回的状态码不等于200，则reject()
			// 如果没有显式定义custom的toast参数为false的话，默认对报错进行toast弹出提示

			if (custom.toast !== false) {
				toast(data.message)
			}
			// 如果需要catch返回，则进行reject
			if (custom?.catch) {
				return Promise.reject(data)
			} else {
				// 否则返回一个pending中的promise
				return new Promise(() => {})
			}
		}
		return data || {}
	}, (response) => {
		/*  对响应错误做点什么 （statusCode !== 200）*/
		if (response.statusCode !== 200) {
			clearAccessTokens();
			return Promise.reject(response)
		} else {
			return response.data || {}
		}
	})
}

/**
 * 解密 JWT token 的信息
 * @param token jwt token 字符串
 * @returns <any>object
 */
export function decryptJWT(token) {
	token = token.replace(/_/g, '/').replace(/-/g, '+');
	var json = decodeURIComponent(escape(Base64.decode(token.split('.')[1])));
	return JSON.parse(json);
}

/**
 * 将 JWT 时间戳转换成 Date
 * @description 主要针对 `exp`，`iat`，`nbf`
 * @param timestamp 时间戳
 * @returns Date 对象
 */
export function getJWTDate(timestamp) {
	return new Date(timestamp * 1000);
}

//  初始化请求配置
const initRequest = () => {
	let accessToken = uni.getStorageSync(accessTokenKey);
	if (accessToken) {
		let jwt = decryptJWT(accessToken);
		let exp = getJWTDate(jwt.exp);
		// token 已经过期 
		if (new Date() >= exp) {
			// 获取刷新 token
			let refreshAccessToken = uni.getStorageSync(refreshAccessTokenKey);
			// 携带刷新 token
			if (refreshAccessToken) {
				env.header['X-Authorization'] = `Bearer ${refreshAccessToken}`;
			}
		} else {
			env.header['Authorization'] = `Bearer ${accessToken}`
		}
	}
	http.setConfig((config) => {
		config.baseURL = env.baseUrl /* 根域名 */
		return config;
	})
	requestInterceptors();
	responseInterceptors();
}

export {
	initRequest
}
