import request from '/@/utils/request';

enum Api {
	Page = '/api/hrProvince/page',
	Get = '/api/hrProvince/get',
	ListEnabled = '/api/hrProvince/listEnabled',
	Save = '/api/hrProvince/save',
	Delete = '/api/hrProvince/delete',
	LangList = '/api/hrProvince/langList',
	SaveLang = '/api/hrProvince/saveLang',
}

export const pageHrProvince = (data: any) => request({ url: Api.Page, method: 'post', data });
export const getHrProvince = (id: number) => request({ url: Api.Get, method: 'get', params: { id } });
export const listEnabledHrProvince = () => request({ url: Api.ListEnabled, method: 'get' });
export const saveHrProvince = (data: any) => request({ url: Api.Save, method: 'post', data });
export const deleteHrProvince = (data: { id: number }) => request({ url: Api.Delete, method: 'post', data });
export const langListHrProvince = (provinceCode: string) =>
	request({ url: Api.LangList, method: 'get', params: { provinceCode } });
export const saveLangHrProvince = (data: any) => request({ url: Api.SaveLang, method: 'post', data });
