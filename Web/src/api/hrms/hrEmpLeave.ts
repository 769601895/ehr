import request from '/@/utils/request';

enum LeaveApi {
	Page = '/api/hrEmpLeave/page',
	Detail = '/api/hrEmpLeave/detail',
	SaveDraft = '/api/hrEmpLeave/saveDraft',
	DeleteDraft = '/api/hrEmpLeave/deleteDraft',
	Submit = '/api/hrEmpLeave/submit',
	Approve = '/api/hrEmpLeave/approve',
	Reject = '/api/hrEmpLeave/reject',
	Void = '/api/hrEmpLeave/void',
}

enum RestoreApi {
	Page = '/api/hrEmpLeaveRestore/page',
	Detail = '/api/hrEmpLeaveRestore/detail',
	SaveDraft = '/api/hrEmpLeaveRestore/saveDraft',
	DeleteDraft = '/api/hrEmpLeaveRestore/deleteDraft',
	Submit = '/api/hrEmpLeaveRestore/submit',
	Approve = '/api/hrEmpLeaveRestore/approve',
	Reject = '/api/hrEmpLeaveRestore/reject',
	Void = '/api/hrEmpLeaveRestore/void',
}

export const pageHrEmpLeave = (data: any) => request({ url: LeaveApi.Page, method: 'post', data });
export const getHrEmpLeaveDetail = (id: number) => request({ url: LeaveApi.Detail, method: 'get', params: { id } });
export const saveHrEmpLeaveDraft = (data: any) => request({ url: LeaveApi.SaveDraft, method: 'post', data });
export const deleteHrEmpLeaveDraft = (data: { id: number }) => request({ url: LeaveApi.DeleteDraft, method: 'post', data });
export const submitHrEmpLeave = (data: { id: number }) => request({ url: LeaveApi.Submit, method: 'post', data });
export const approveHrEmpLeave = (data: { id: number }) => request({ url: LeaveApi.Approve, method: 'post', data });
export const rejectHrEmpLeave = (data: { id: number; rejectReason?: string }) =>
	request({ url: LeaveApi.Reject, method: 'post', data });
export const voidHrEmpLeave = (data: { id: number; voidReason?: string }) =>
	request({ url: LeaveApi.Void, method: 'post', data });

export const pageHrEmpLeaveRestore = (data: any) => request({ url: RestoreApi.Page, method: 'post', data });
export const getHrEmpLeaveRestoreDetail = (id: number) => request({ url: RestoreApi.Detail, method: 'get', params: { id } });
export const saveHrEmpLeaveRestoreDraft = (data: any) => request({ url: RestoreApi.SaveDraft, method: 'post', data });
export const deleteHrEmpLeaveRestoreDraft = (data: { id: number }) =>
	request({ url: RestoreApi.DeleteDraft, method: 'post', data });
export const submitHrEmpLeaveRestore = (data: { id: number }) => request({ url: RestoreApi.Submit, method: 'post', data });
export const approveHrEmpLeaveRestore = (data: { id: number }) => request({ url: RestoreApi.Approve, method: 'post', data });
export const rejectHrEmpLeaveRestore = (data: { id: number; rejectReason?: string }) =>
	request({ url: RestoreApi.Reject, method: 'post', data });
export const voidHrEmpLeaveRestore = (data: { id: number; voidReason?: string }) =>
	request({ url: RestoreApi.Void, method: 'post', data });
