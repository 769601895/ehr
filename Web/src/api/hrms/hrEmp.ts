import request from '/@/utils/request';

enum Api {
	Page = '/api/hrEmp/page',
	Detail = '/api/hrEmp/detail',
	Add = '/api/hrEmp/add',
	Update = '/api/hrEmp/update',
	Delete = '/api/hrEmp/delete',
}

/** 人员资料分页（服务端排序） */
export const pageHrEmp = (data?: {
	page?: number;
	pageSize?: number;
	field?: string;
	order?: string;
	descStr?: string;
	keyword?: string;
	ccCode?: string;
	empState?: number;
	empId?: string;
	empName?: string;
}) =>
	request({
		url: Api.Page,
		method: 'post',
		data,
	});

export const getHrEmpDetail = (empId: string) =>
	request({
		url: Api.Detail,
		method: 'get',
		params: { empId },
	});

export const addHrEmp = (data?: any) =>
	request({
		url: Api.Add,
		method: 'post',
		data,
	});

export const updateHrEmp = (data?: any) =>
	request({
		url: Api.Update,
		method: 'post',
		data,
	});

/** 离职（emp_state=3） */
export const deleteHrEmp = (data: { empId: string }) =>
	request({
		url: Api.Delete,
		method: 'post',
		data,
	});
