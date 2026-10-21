import request from '/@/utils/request';

export const countHrEmpChild = (empId: string) =>
	request({
		url: '/api/hrEmpChild/count',
		method: 'get',
		params: { empId },
	});

export const listHrEmpChild = (section: string, empId: string) =>
	request({
		url: '/api/hrEmpChild/list',
		method: 'get',
		params: { section, empId },
	});

export const saveHrEmpChild = (data: { section: string; empId: string; id?: string; values: Record<string, string> }) =>
	request({
		url: '/api/hrEmpChild/save',
		method: 'post',
		data,
	});

export const deleteHrEmpChild = (data: { section: string; empId: string; id: string }) =>
	request({
		url: '/api/hrEmpChild/delete',
		method: 'post',
		data,
	});

export const sortHrEmpChild = (data: { section: string; empId: string; ids: string[] }) =>
	request({
		url: '/api/hrEmpChild/sort',
		method: 'post',
		data,
	});
