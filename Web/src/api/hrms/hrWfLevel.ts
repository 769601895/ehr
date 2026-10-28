import request from '/@/utils/request';

enum Api {
	Page = '/api/hrWfLevel/page',
	Get = '/api/hrWfLevel/get',
	ListEnabled = '/api/hrWfLevel/listEnabled',
	Save = '/api/hrWfLevel/save',
	Delete = '/api/hrWfLevel/delete',
}

export const pageHrWfLevel = (data: any) => request({ url: Api.Page, method: 'post', data });
export const getHrWfLevel = (id: number) => request({ url: Api.Get, method: 'get', params: { id } });
export const listEnabledHrWfLevel = () => request({ url: Api.ListEnabled, method: 'get' });
export const saveHrWfLevel = (data: any) => request({ url: Api.Save, method: 'post', data });
export const deleteHrWfLevel = (data: { id: number }) => request({ url: Api.Delete, method: 'post', data });
