import request from '/@/utils/request';

enum Api {
	Page = '/api/hrUiFieldDef/page',
	Detail = '/api/hrUiFieldDef/detail',
	Update = '/api/hrUiFieldDef/update',
	SyncFromDb = '/api/hrUiFieldDef/syncFromDb',
	LangList = '/api/hrUiFieldDef/langList',
	SaveLang = '/api/hrUiFieldDef/saveLang',
}

export const pageHrUiFieldDef = (data: any) => request({ url: Api.Page, method: 'post', data });
export const detailHrUiFieldDef = (id: number) => request({ url: Api.Detail, method: 'get', params: { id } });
export const updateHrUiFieldDef = (data: any) => request({ url: Api.Update, method: 'post', data });
export const syncHrUiFieldDef = (moduleCode = 'hrEmp') => request({ url: Api.SyncFromDb, method: 'post', data: { moduleCode } });
export const langListHrUiFieldDef = (moduleCode: string, fieldKey: string) =>
	request({ url: Api.LangList, method: 'get', params: { moduleCode, fieldKey } });
export const saveLangHrUiFieldDef = (data: any) => request({ url: Api.SaveLang, method: 'post', data });
