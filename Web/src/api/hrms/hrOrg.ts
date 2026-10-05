import request from '/@/utils/request';

enum Api {
	List = '/api/hrOrg/list',
	Add = '/api/hrOrg/add',
	Update = '/api/hrOrg/update',
	Delete = '/api/hrOrg/delete',
	Sync = '/api/hrOrg/syncFromCcMstr',
	CodeList = '/api/hrOrg/codeList',
}

export const getHrOrgList = (params?: any) =>
	request({
		url: Api.List,
		method: 'get',
		params,
	});

export const addHrOrg = (data?: any) =>
	request({
		url: Api.Add,
		method: 'post',
		data,
	});

export const updateHrOrg = (data?: any) =>
	request({
		url: Api.Update,
		method: 'post',
		data,
	});

export const deleteHrOrg = (data?: any) =>
	request({
		url: Api.Delete,
		method: 'post',
		data,
	});

export const syncHrOrgFromCcMstr = () =>
	request({
		url: Api.Sync,
		method: 'post',
	});

export const getHrCodeList = (codeField: string) =>
	request({
		url: Api.CodeList,
		method: 'get',
		params: { codeField },
	});
