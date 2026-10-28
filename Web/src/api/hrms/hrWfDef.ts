import request from '/@/utils/request';

enum Api {
	Page = '/api/hrWfDef/page',
	Get = '/api/hrWfDef/get',
	GetEnabledByBizType = '/api/hrWfDef/getEnabledByBizType',
	Save = '/api/hrWfDef/save',
	Delete = '/api/hrWfDef/delete',
}

export const pageHrWfDef = (data: any) => request({ url: Api.Page, method: 'post', data });
export const getHrWfDef = (id: number) => request({ url: Api.Get, method: 'get', params: { id } });
export const getEnabledHrWfDefByBizType = (bizType: string) =>
	request({ url: Api.GetEnabledByBizType, method: 'get', params: { bizType } });
export const saveHrWfDef = (data: any) => request({ url: Api.Save, method: 'post', data });
export const deleteHrWfDef = (data: { id: number }) => request({ url: Api.Delete, method: 'post', data });
