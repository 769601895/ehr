import request from '/@/utils/request';

enum Api {
	PageMyTodo = '/api/hrWfTodo/pageMyTodo',
	PageMyDone = '/api/hrWfTodo/pageMyDone',
	PageMyApplied = '/api/hrWfTodo/pageMyApplied',
}

export const pageMyTodo = (data: any) => request({ url: Api.PageMyTodo, method: 'post', data });
export const pageMyDone = (data: any) => request({ url: Api.PageMyDone, method: 'post', data });
export const pageMyApplied = (data: any) => request({ url: Api.PageMyApplied, method: 'post', data });
