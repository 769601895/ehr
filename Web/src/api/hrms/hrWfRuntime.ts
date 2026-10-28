import request from '/@/utils/request';

enum Api {
	Start = '/api/hrWfRuntime/start',
	Approve = '/api/hrWfRuntime/approve',
	Reject = '/api/hrWfRuntime/reject',
	PatchForm = '/api/hrWfRuntime/patchForm',
	GetDetail = '/api/hrWfRuntime/getDetail',
}

export const startHrWf = (data: any) => request({ url: Api.Start, method: 'post', data });
export const approveHrWf = (data: any) => request({ url: Api.Approve, method: 'post', data });
export const rejectHrWf = (data: any) => request({ url: Api.Reject, method: 'post', data });
export const patchHrWfForm = (data: any) => request({ url: Api.PatchForm, method: 'post', data });
export const getHrWfDetail = (instId: number) => request({ url: Api.GetDetail, method: 'get', params: { instId } });
