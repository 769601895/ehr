import request from '/@/utils/request';

enum Api {
	Page = '/api/hrEmpChg/page',
	Detail = '/api/hrEmpChg/detail',
	PreviewDefaults = '/api/hrEmpChg/previewDefaults',
	SaveDraft = '/api/hrEmpChg/saveDraft',
	DeleteDraft = '/api/hrEmpChg/deleteDraft',
	Submit = '/api/hrEmpChg/submit',
	Approve = '/api/hrEmpChg/approve',
	Reject = '/api/hrEmpChg/reject',
	Void = '/api/hrEmpChg/void',
}

export const pageHrEmpChg = (data: any) => request({ url: Api.Page, method: 'post', data });
export const getHrEmpChgDetail = (id: number) => request({ url: Api.Detail, method: 'get', params: { id } });
export const previewHrEmpChgDefaults = (empId: string) =>
	request({ url: Api.PreviewDefaults, method: 'get', params: { empId } });
export const saveHrEmpChgDraft = (data: any) => request({ url: Api.SaveDraft, method: 'post', data });
export const deleteHrEmpChgDraft = (data: { id: number }) => request({ url: Api.DeleteDraft, method: 'post', data });
export const submitHrEmpChg = (data: { id: number }) => request({ url: Api.Submit, method: 'post', data });
export const approveHrEmpChg = (data: { id: number }) => request({ url: Api.Approve, method: 'post', data });
export const rejectHrEmpChg = (data: { id: number; rejectReason?: string }) =>
	request({ url: Api.Reject, method: 'post', data });
export const voidHrEmpChg = (data: { id: number; voidReason?: string }) =>
	request({ url: Api.Void, method: 'post', data });
