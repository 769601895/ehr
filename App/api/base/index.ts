// 接口基类
import { service, cancelRequest, RequestConfig } from './request'

/*
使用方式: custom(接口名, 'post'|'get', 自定义的配置项)
get方式默认是FromQuery
调用方式：
1、基础方式：
	方法名(参数集)
	这种方式，会自动捕获异常，接口出来statusCode不等200时，会用uni.showToast弹出提示，后序代码会再进地。
2、自定义异常处理方式：
	方法名(参数集, false, false)
	第二个参数为是否自动捕获异常，默认值为true,现在传出false时，如果 statusCode出现异常，就由自己为处理
*/

export const useBaseApi = (module: string) => {
    const baseUrl = `/api/${module}/`;
    const request = <T>(config: RequestConfig<T>, cancel: boolean = false, autoCatchException: boolean = true) => {
        if (cancel) {
            cancelRequest(config.url || '');
            return Promise.resolve({} as any);
        }
        config.custom = { ...(config.custom || {}), catch: true }
        return service(config).catch((err) => {
			const statusCode = typeof err === 'object' && err ? (err.statusCode ?? err.data?.code ?? '') : ''
			const errMsg = typeof err === 'string'
				? err
				: (err?.data?.message || err?.data?.msg || err?.errMsg || err?.message || '')
			const msg = errMsg || (statusCode ? `接口错误 ${statusCode}` : '接口错误')
			console.log('传入autoCatchException：', autoCatchException)
			err.errMsg = msg;		
            if (config?.custom?.autoCatchException === true || autoCatchException) {
				uni.hideLoading()
            	uni.showToast({ title: String(msg), icon: 'none', duration: 2000 })
				console.log("useBaseApi里捕获, 并不传递下去", err)
                return new Promise(() => {})
            }
			console.log("useBaseApi传递异常下去", err)
            return Promise.reject(err)
        });
    };
	return {
		baseUrl: baseUrl,
		request: request,
		// 自定义的其它接口，使用时 custom(接口名，Method)
		custom: function (apiName:string, method:string='post', custom: {} = {}) {
			return function (data: any, cancel: boolean = false, autoCatchException: boolean = true) {
				return request(
					{
						url: baseUrl + apiName,
						method: method,
						data,
						custom
					},
					cancel,
					autoCatchException
				);
			}
		},
		page: function (data: any, cancel: boolean = false, autoCatchException: boolean = true) {
			return request(
				{
					url: baseUrl + 'page',
					method: 'post',
					data,
				},
				cancel,
				autoCatchException
			);
		},
		list: function (data: any, cancel: boolean = false, autoCatchException: boolean = true) {
			return request(
				{
					url: baseUrl + 'list',
					method: 'post',
					data,
				},
				cancel,
				autoCatchException
			);
		},
		detail: function (id: any, cancel: boolean = false, autoCatchException: boolean = true) {
			return request(
				{
					url: baseUrl + 'detail',
					method: 'get',
					data: { id },
				},
				cancel,
				autoCatchException
			);
		},
		dropdownData: function (data: any, cancel: boolean = false, autoCatchException: boolean = true) {
			return request(
				{
					url: baseUrl + 'dropdownData',
					method: 'post',
					data,
				},
				cancel,
				autoCatchException
			);
		},
		add: function (data: any, cancel: boolean = false, autoCatchException: boolean = true) {
			return request(
				{
					url: baseUrl + 'add',
					method: 'post',
					data,
				},
				cancel,
				autoCatchException
			);
		},
		update: function (data: any, cancel: boolean = false, autoCatchException: boolean = true) {
			return request(
				{
					url: baseUrl + 'update',
					method: 'post',
					data,
				},
				cancel,
				autoCatchException
			);
		},
		setStatus: function (data: any, cancel: boolean = false, autoCatchException: boolean = true) {
			return request(
				{
					url: baseUrl + 'setStatus',
					method: 'post',
					data,
				},
				cancel,
				autoCatchException
			);
		},
		delete: function (data: any, cancel: boolean = false, autoCatchException: boolean = true) {
			return request(
				{
					url: baseUrl + 'delete',
					method: 'post',
					data,
				},
				cancel,
				autoCatchException
			);
		},
		batchDelete: function (data: any, cancel: boolean = false, autoCatchException: boolean = true) {
			return request(
				{
					url: baseUrl + 'batchDelete',
					method: 'post',
					data,
				},
				cancel,
				autoCatchException
			);
		},
		exportData: function (data: any, cancel: boolean = false, autoCatchException: boolean = true) {
			return request(
				{
					responseType: 'arraybuffer',
					url: baseUrl + 'export',
					method: 'post',
					data,
				},
				cancel,
				autoCatchException
			);
		},
		downloadTemplate: function (cancel: boolean = false, autoCatchException: boolean = true) {
			return request(
				{
					responseType: 'arraybuffer',
					url: baseUrl + 'import',
					method: 'get',
				},
				cancel,
				autoCatchException
			);
		},
		importData: function (file: any, cancel: boolean = false, autoCatchException: boolean = true) {
			const formData = new FormData();
			formData.append('file', file);
			return request(
				{
					headers: { 'Content-Type': 'multipart/form-data;charset=UTF-8' },
					responseType: 'arraybuffer',
					url: baseUrl + 'import',
					method: 'post',
					data: formData,
				},
				cancel,
				autoCatchException
			);
		},
		uploadFile: function (params: any, action: string, cancel: boolean = false, autoCatchException: boolean = true) {
			const formData = new FormData();
			formData.append('file', params.file);
			// 自定义参数
			if (params.data) {
				Object.keys(params.data).forEach((key) => {
					const value = params.data![key];
					if (Array.isArray(value)) {
						value.forEach((item) => formData.append(`${key}[]`, item));
						return;
					}
					formData.append(key, params.data![key]);
				});
			}
			return request(
				{
					url: baseUrl + action,
					method: 'POST',
					data: formData,
					headers: {
						'Content-Type': 'multipart/form-data;charset=UTF-8',
						ignoreCancelToken: true,
					},
				},
				cancel,
				autoCatchException
			);
		},
	};
};
