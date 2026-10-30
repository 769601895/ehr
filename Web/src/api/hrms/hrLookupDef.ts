import request from '/@/utils/request';

enum Api {
	Page = '/api/hrLookupDef/page',
	Get = '/api/hrLookupDef/get',
	ListEnabled = '/api/hrLookupDef/listEnabled',
	GetOptions = '/api/hrLookupDef/getOptions',
	Save = '/api/hrLookupDef/save',
	Delete = '/api/hrLookupDef/delete',
}

export const pageHrLookupDef = (data: any) => request({ url: Api.Page, method: 'post', data });
export const getHrLookupDef = (id: number) => request({ url: Api.Get, method: 'get', params: { id } });
export const listEnabledHrLookupDef = () => request({ url: Api.ListEnabled, method: 'get' });
export const getHrLookupOptions = (source: string) => request({ url: Api.GetOptions, method: 'get', params: { source } });
export const saveHrLookupDef = (data: any) => request({ url: Api.Save, method: 'post', data });
export const deleteHrLookupDef = (data: { id: number }) => request({ url: Api.Delete, method: 'post', data });
