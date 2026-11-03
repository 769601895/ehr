import request from '/@/utils/request';

enum Api {
	Page = '/api/hrCompanyParam/page',
	Detail = '/api/hrCompanyParam/detail',
	Add = '/api/hrCompanyParam/add',
	Update = '/api/hrCompanyParam/update',
	Delete = '/api/hrCompanyParam/delete',
	LangList = '/api/hrCompanyParam/langList',
	SaveLang = '/api/hrCompanyParam/saveLang',
	CompanyOrgPage = '/api/hrCompanyParam/companyOrgPage',
	SaveCompanyOrgSite = '/api/hrCompanyParam/saveCompanyOrgSite',
	SiteOptions = '/api/hrCompanyParam/siteOptions',
}

export const pageHrCompanyParam = (data: any) => request({ url: Api.Page, method: 'post', data });
export const detailHrCompanyParam = (params: { id?: number; siteCode?: string }) =>
	request({ url: Api.Detail, method: 'get', params });
export const addHrCompanyParam = (data: { siteCode: string; siteName: string }) =>
	request({ url: Api.Add, method: 'post', data });
export const updateHrCompanyParam = (data: any) => request({ url: Api.Update, method: 'post', data });
export const deleteHrCompanyParam = (data: { id: number }) => request({ url: Api.Delete, method: 'post', data });
export const langListHrCompanyParam = (id: number) =>
	request({ url: Api.LangList, method: 'get', params: { id } });
export const saveLangHrCompanyParam = (data: any) => request({ url: Api.SaveLang, method: 'post', data });
export const companyOrgPageHrCompanyParam = (data: any) =>
	request({ url: Api.CompanyOrgPage, method: 'post', data });
export const saveCompanyOrgSiteHrCompanyParam = (data: { items: { ccCode: string; belongSiteCode?: string | null }[] }) =>
	request({ url: Api.SaveCompanyOrgSite, method: 'post', data });
export const siteOptionsHrCompanyParam = () => request({ url: Api.SiteOptions, method: 'get' });
