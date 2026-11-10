import request from '/@/utils/request';

enum Api {
	Page = '/api/hrEmpSuspend/page',
	Detail = '/api/hrEmpSuspend/detail',
	SaveDraft = '/api/hrEmpSuspend/saveDraft',
	DeleteDraft = '/api/hrEmpSuspend/deleteDraft',
	Submit = '/api/hrEmpSuspend/submit',
	Approve = '/api/hrEmpSuspend/approve',
	Reject = '/api/hrEmpSuspend/reject',
	Void = '/api/hrEmpSuspend/void',
}

export const pageHrEmpSuspend = (data: any) => request({ url: Api.Page, method: 'post', data });
export const getHrEmpSuspendDetail = (id: number) => request({ url: Api.Detail, method: 'get', params: { id } });
export const saveHrEmpSuspendDraft = (data: any) => request({ url: Api.SaveDraft, method: 'post', data });
export const deleteHrEmpSuspendDraft = (data: { id: number }) => request({ url: Api.DeleteDraft, method: 'post', data });
export const submitHrEmpSuspend = (data: { id: number }) => request({ url: Api.Submit, method: 'post', data });
export const approveHrEmpSuspend = (data: { id: number }) => request({ url: Api.Approve, method: 'post', data });
export const rejectHrEmpSuspend = (data: { id: number; rejectReason?: string }) =>
	request({ url: Api.Reject, method: 'post', data });
export const voidHrEmpSuspend = (data: { id: number; voidReason?: string }) =>
	request({ url: Api.Void, method: 'post', data });
