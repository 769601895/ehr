import request from '/@/utils/request';

enum Api {
	Page = '/api/hrPost/page',
	Get = '/api/hrPost/get',
	ListEnabled = '/api/hrPost/listEnabled',
	Save = '/api/hrPost/save',
	Delete = '/api/hrPost/delete',
}

export const pageHrPost = (data: any) => request({ url: Api.Page, method: 'post', data });
export const getHrPost = (id: number) => request({ url: Api.Get, method: 'get', params: { id } });
export const listEnabledHrPost = () => request({ url: Api.ListEnabled, method: 'get' });
export const saveHrPost = (data: any) => request({ url: Api.Save, method: 'post', data });
export const deleteHrPost = (data: { id: number }) => request({ url: Api.Delete, method: 'post', data });
