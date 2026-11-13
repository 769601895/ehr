import request from '/@/utils/request';

enum Api {
	Page = '/api/hrEmpMonth/page',
	Get = '/api/hrEmpMonth/get',
	Create = '/api/hrEmpMonth/create',
	Delete = '/api/hrEmpMonth/delete',
	ResolvePeriodPreview = '/api/hrEmpMonth/resolvePeriodPreview',
	Generate = '/api/hrEmpMonth/generate',
	PageLines = '/api/hrEmpMonth/pageLines',
	AdjustLine = '/api/hrEmpMonth/adjustLine',
	Validate = '/api/hrEmpMonth/validate',
	Freeze = '/api/hrEmpMonth/freeze',
	Unfreeze = '/api/hrEmpMonth/unfreeze',
	Export = '/api/hrEmpMonth/export',
	ListMonthSnapFields = '/api/hrEmpMonth/listMonthSnapFields',
	GetChgEnabledFields = '/api/hrEmpMonth/getChgEnabledFields',
}

export const pageHrEmpMonth = (data: any) => request({ url: Api.Page, method: 'post', data });
export const getHrEmpMonth = (id: number) => request({ url: Api.Get, method: 'get', params: { id } });
export const createHrEmpMonth = (data: any) => request({ url: Api.Create, method: 'post', data });
export const deleteHrEmpMonth = (data: { id: number }) => request({ url: Api.Delete, method: 'post', data });
export const resolvePeriodPreview = (data: { siteCode: string; payPeriod: string }) =>
	request({ url: Api.ResolvePeriodPreview, method: 'post', data });
export const generateHrEmpMonth = (data: { batchId: number; fullReset?: boolean }) =>
	request({ url: Api.Generate, method: 'post', data });
export const pageHrEmpMonthLines = (data: any) => request({ url: Api.PageLines, method: 'post', data });
export const adjustHrEmpMonthLine = (data: any) => request({ url: Api.AdjustLine, method: 'post', data });
export const validateHrEmpMonth = (data: { id: number }) => request({ url: Api.Validate, method: 'post', data });
export const freezeHrEmpMonth = (data: { batchId: number; confirmWarn?: boolean }) =>
	request({ url: Api.Freeze, method: 'post', data });
export const unfreezeHrEmpMonth = (data: { batchId: number; reason: string }) =>
	request({ url: Api.Unfreeze, method: 'post', data });
export const exportHrEmpMonthUrl = (batchId: number) => `${Api.Export}?batchId=${batchId}`;
export const listMonthSnapFields = () => request({ url: Api.ListMonthSnapFields, method: 'get' });
export const getChgEnabledFields = () => request({ url: Api.GetChgEnabledFields, method: 'get' });
