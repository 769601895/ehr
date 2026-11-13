import request from '/@/utils/request';

enum Api {
	Page = '/api/hrCodeMstr/page',
	Save = '/api/hrCodeMstr/save',
	Delete = '/api/hrCodeMstr/delete',
	ListDeleted = '/api/hrCodeMstr/listDeleted',
	Restore = '/api/hrCodeMstr/restore',
	LangList = '/api/hrCodeMstr/langList',
	SaveLang = '/api/hrCodeMstr/saveLang',
	ListField = '/api/hrCodeMstr/listField',
	GetField = '/api/hrCodeMstr/getField',
	ListDeletedField = '/api/hrCodeMstr/listDeletedField',
	SaveField = '/api/hrCodeMstr/saveField',
	DeleteField = '/api/hrCodeMstr/deleteField',
	RestoreField = '/api/hrCodeMstr/restoreField',
	FieldLangList = '/api/hrCodeMstr/fieldLangList',
	SaveFieldLang = '/api/hrCodeMstr/saveFieldLang',
}

export const pageHrCodeMstr = (data: any) => request({ url: Api.Page, method: 'post', data });
export const saveHrCodeMstr = (data: any) => request({ url: Api.Save, method: 'post', data });
export const deleteHrCodeMstr = (data: { codeField: string; codeValue: string }) => request({ url: Api.Delete, method: 'post', data });
export const listDeletedHrCodeMstr = (codeField: string, keyword?: string) =>
	request({ url: Api.ListDeleted, method: 'get', params: { codeField, keyword: keyword || undefined } });
export const restoreHrCodeMstr = (data: { codeField: string; codeValue: string }) =>
	request({ url: Api.Restore, method: 'post', data });
export const langListHrCodeMstr = (codeField: string, codeValue: string) =>
	request({ url: Api.LangList, method: 'get', params: { codeField, codeValue } });
export const saveLangHrCodeMstr = (data: any) => request({ url: Api.SaveLang, method: 'post', data });

export const listHrCodeField = (keyword?: string) =>
	request({ url: Api.ListField, method: 'get', params: { keyword: keyword || undefined } });
export const getHrCodeField = (codeField: string) =>
	request({ url: Api.GetField, method: 'get', params: { codeField } });
export const listDeletedHrCodeField = (keyword?: string) =>
	request({ url: Api.ListDeletedField, method: 'get', params: { keyword: keyword || undefined } });
export const saveHrCodeField = (data: any) => request({ url: Api.SaveField, method: 'post', data });
export const deleteHrCodeField = (data: { codeField: string }) => request({ url: Api.DeleteField, method: 'post', data });
export const restoreHrCodeField = (data: { codeField: string }) => request({ url: Api.RestoreField, method: 'post', data });
export const fieldLangListHrCodeField = (codeField: string) =>
	request({ url: Api.FieldLangList, method: 'get', params: { codeField } });
export const saveFieldLangHrCodeField = (data: any) => request({ url: Api.SaveFieldLang, method: 'post', data });
