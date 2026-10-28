import request from '/@/utils/request';

enum Api {
	Page = '/api/hrWfRole/page',
	Tree = '/api/hrWfRole/tree',
	Get = '/api/hrWfRole/get',
	Save = '/api/hrWfRole/save',
	Delete = '/api/hrWfRole/delete',
}

export const pageHrWfRole = (data: any) => request({ url: Api.Page, method: 'post', data });
export const treeHrWfRole = () => request({ url: Api.Tree, method: 'get' });
export const getHrWfRole = (id: number) => request({ url: Api.Get, method: 'get', params: { id } });
export const saveHrWfRole = (data: any) => request({ url: Api.Save, method: 'post', data });
export const deleteHrWfRole = (data: { id: number }) => request({ url: Api.Delete, method: 'post', data });
