import request from '/@/utils/request';

enum Api {
	List = '/api/hrAdvQueryScheme/list',
	Detail = '/api/hrAdvQueryScheme/detail',
	Save = '/api/hrAdvQueryScheme/save',
	Delete = '/api/hrAdvQueryScheme/delete',
}

export const listHrAdvQueryScheme = (moduleCode: string) =>
	request({ url: Api.List, method: 'get', params: { moduleCode } });

export const getHrAdvQuerySchemeDetail = (id: number) =>
	request({ url: Api.Detail, method: 'get', params: { id } });

export const saveHrAdvQueryScheme = (data: {
	id?: number;
	moduleCode: string;
	name: string;
	filterJson: string;
	isShared?: boolean;
	shareRoleIds?: number[];
}) => request({ url: Api.Save, method: 'post', data });

export const deleteHrAdvQueryScheme = (id: number) =>
	request({ url: Api.Delete, method: 'post', data: { id } });
