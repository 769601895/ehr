import request from '/@/utils/request';

enum Api {
	Page = '/api/hrWfForm/page',
	Get = '/api/hrWfForm/get',
	GetByBizType = '/api/hrWfForm/getByBizType',
	Save = '/api/hrWfForm/save',
	Delete = '/api/hrWfForm/delete',
}

export const pageHrWfForm = (data: any) => request({ url: Api.Page, method: 'post', data });
export const getHrWfForm = (id: number) => request({ url: Api.Get, method: 'get', params: { id } });
export const getHrWfFormByBizType = (bizType: string) => request({ url: Api.GetByBizType, method: 'get', params: { bizType } });
export const saveHrWfForm = (data: any) => request({ url: Api.Save, method: 'post', data });
export const deleteHrWfForm = (data: { id: number }) => request({ url: Api.Delete, method: 'post', data });
