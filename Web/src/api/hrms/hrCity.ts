import request from '/@/utils/request';

enum Api {
	Page = '/api/hrCity/page',
	Get = '/api/hrCity/get',
	ListByProvince = '/api/hrCity/listByProvince',
	Save = '/api/hrCity/save',
	Delete = '/api/hrCity/delete',
	LangList = '/api/hrCity/langList',
	SaveLang = '/api/hrCity/saveLang',
}

export const pageHrCity = (data: any) => request({ url: Api.Page, method: 'post', data });
export const getHrCity = (id: number) => request({ url: Api.Get, method: 'get', params: { id } });
export const listHrCityByProvince = (provinceCode: string) =>
	request({ url: Api.ListByProvince, method: 'get', params: { provinceCode } });
export const saveHrCity = (data: any) => request({ url: Api.Save, method: 'post', data });
export const deleteHrCity = (data: { id: number }) => request({ url: Api.Delete, method: 'post', data });
export const langListHrCity = (cityCode: string) => request({ url: Api.LangList, method: 'get', params: { cityCode } });
export const saveLangHrCity = (data: any) => request({ url: Api.SaveLang, method: 'post', data });
