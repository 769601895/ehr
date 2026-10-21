import request from '/@/utils/request';

enum Api {
	Page = '/api/hrCodeMstr/page',
	Save = '/api/hrCodeMstr/save',
	Delete = '/api/hrCodeMstr/delete',
	LangList = '/api/hrCodeMstr/langList',
	SaveLang = '/api/hrCodeMstr/saveLang',
}

export const pageHrCodeMstr = (data: any) => request({ url: Api.Page, method: 'post', data });
export const saveHrCodeMstr = (data: any) => request({ url: Api.Save, method: 'post', data });
export const deleteHrCodeMstr = (data: { codeField: string; codeValue: string }) => request({ url: Api.Delete, method: 'post', data });
export const langListHrCodeMstr = (codeField: string, codeValue: string) =>
	request({ url: Api.LangList, method: 'get', params: { codeField, codeValue } });
export const saveLangHrCodeMstr = (data: any) => request({ url: Api.SaveLang, method: 'post', data });
