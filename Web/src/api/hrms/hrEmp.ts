import request from '/@/utils/request';

enum Api {
	Page = '/api/hrEmp/page',
	Detail = '/api/hrEmp/detail',
	LookupByEmpId = '/api/hrEmp/lookupByEmpId',
	Add = '/api/hrEmp/add',
	Update = '/api/hrEmp/update',
	Delete = '/api/hrEmp/delete',
	PreviewEmpId = '/api/hrEmp/previewEmpId',
	ParseIdCard = '/api/hrEmp/parseIdCard',
	IdCardFeature = '/api/hrEmp/idCardFeature',
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

/** 按工号精确查管辖内员工（表单输入回填；不在管辖则失败） */
export const lookupHrEmpByEmpId = (data: { empId: string; empState?: number | null }) =>
	request({
		url: Api.LookupByEmpId,
		method: 'post',
		data,
	});

/** 按部门预览工号（不占号） */
export const previewHrEmpId = (data?: { ccCode?: string | null }) =>
	request({
		url: Api.PreviewEmpId,
		method: 'post',
		data: data ?? {},
	});

/** 解析身份证：中国国籍校验并带出生日/性别/籍贯省市码 */
export const parseHrEmpIdCard = (data?: { empCard?: string | null; empCountry?: string | null; ccCode?: string | null }) =>
	request({
		url: Api.ParseIdCard,
		method: 'post',
		data: data ?? {},
	});

/** 是否启用二代证读卡（公司参数 IdCardV2Enabled，按当前用户管辖部门） */
export const getHrEmpIdCardFeature = () =>
	request({
		url: Api.IdCardFeature,
		method: 'post',
		data: {},
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
