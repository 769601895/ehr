import request from '/@/utils/request';

enum Api {
	FieldPool = '/api/hrUiLayout/fieldPool',
	GetEffective = '/api/hrUiLayout/getEffective',
	GetDefault = '/api/hrUiLayout/getDefault',
	SaveDefault = '/api/hrUiLayout/saveDefault',
	SaveMy = '/api/hrUiLayout/saveMy',
	ResetMy = '/api/hrUiLayout/resetMy',
}

export type HrUiLayoutScene = 'list' | 'form';

export const getHrUiFieldPool = (moduleCode = 'hrEmp', scene?: HrUiLayoutScene) =>
	request({
		url: Api.FieldPool,
		method: 'get',
		params: { moduleCode, scene },
	});

export const getHrUiLayoutEffective = (moduleCode = 'hrEmp', scene: HrUiLayoutScene = 'list') =>
	request({
		url: Api.GetEffective,
		method: 'get',
		params: { moduleCode, scene },
	});

export const getHrUiLayoutDefault = (moduleCode = 'hrEmp', scene: HrUiLayoutScene = 'list') =>
	request({
		url: Api.GetDefault,
		method: 'get',
		params: { moduleCode, scene },
	});

export const saveHrUiLayoutDefault = (data: any) =>
	request({
		url: Api.SaveDefault,
		method: 'post',
		data,
	});

export const saveHrUiLayoutMy = (data: any) =>
	request({
		url: Api.SaveMy,
		method: 'post',
		data,
	});

export const resetHrUiLayoutMy = (moduleCode = 'hrEmp', scene: HrUiLayoutScene = 'list') =>
	request({
		url: Api.ResetMy,
		method: 'post',
		data: { moduleCode, scene },
	});
