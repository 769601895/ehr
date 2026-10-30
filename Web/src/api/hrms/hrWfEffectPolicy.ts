import request from '/@/utils/request';

enum Api {
	ListPolicies = '/api/hrWfEffectPolicy/listPolicies',
	GetByBizType = '/api/hrWfEffectPolicy/getByBizType',
	SavePolicy = '/api/hrWfEffectPolicy/savePolicy',
	DeletePolicy = '/api/hrWfEffectPolicy/deletePolicy',
	ListFieldMaps = '/api/hrWfEffectPolicy/listFieldMaps',
	SaveFieldMap = '/api/hrWfEffectPolicy/saveFieldMap',
	DeleteFieldMap = '/api/hrWfEffectPolicy/deleteFieldMap',
	ApplyDue = '/api/hrWfEffectPolicy/applyDue',
	Void = '/api/hrWfEffectPolicy/void',
}

export const listHrWfEffectPolicies = () => request({ url: Api.ListPolicies, method: 'get' });
export const getHrWfEffectPolicyByBizType = (bizType: string) =>
	request({ url: Api.GetByBizType, method: 'get', params: { bizType } });
export const saveHrWfEffectPolicy = (data: any) => request({ url: Api.SavePolicy, method: 'post', data });
export const deleteHrWfEffectPolicy = (id: number) =>
	request({ url: Api.DeletePolicy, method: 'post', params: { id } });
export const listHrWfEffectFieldMaps = (bizType?: string) =>
	request({ url: Api.ListFieldMaps, method: 'get', params: { bizType } });
export const saveHrWfEffectFieldMap = (data: any) => request({ url: Api.SaveFieldMap, method: 'post', data });
export const deleteHrWfEffectFieldMap = (id: number) =>
	request({ url: Api.DeleteFieldMap, method: 'post', params: { id } });
export const applyDueHrWfEffect = () => request({ url: Api.ApplyDue, method: 'post' });
export const voidHrWfEffect = (data: { bizType: string; bizId: string; reason?: string }) =>
	request({ url: Api.Void, method: 'post', data });
