import request from '/@/utils/request';

enum Api {
	List = '/api/hrOrg/list',
	Page = '/api/hrOrg/page',
	Children = '/api/hrOrg/children',
	Add = '/api/hrOrg/add',
	Update = '/api/hrOrg/update',
	Delete = '/api/hrOrg/delete',
	EmpById = '/api/hrOrg/empById',
	CodeList = '/api/hrOrg/codeList',
	CostCenterList = '/api/hrOrg/costCenterList',
	ShiftGroupList = '/api/hrOrg/shiftGroupList',
	MergeDept = '/api/hrOrg/mergeDept',
	TransferDept = '/api/hrOrg/transferDept',
}

/** 部门树（默认仅启用；人数默认按在职统计；支持 field/order/descStr） */
export const getHrOrgList = (params?: {
	parentCcCode?: string;
	includeDisabled?: boolean;
	/** 人数统计：1在职 2停职 3离职；与 empStateAll 互斥 */
	empState?: number;
	/** true=人数统计全部人员，忽略 empState */
	empStateAll?: boolean;
	field?: string;
	order?: string;
	descStr?: string;
}) =>
	request({
		url: Api.List,
		method: 'get',
		params,
	});

/** 部门明细分页（服务端排序） */
export const pageHrOrg = (data?: {
	page?: number;
	pageSize?: number;
	field?: string;
	order?: string;
	descStr?: string;
	keyword?: string;
	parentCcCode?: string;
	includeDisabled?: boolean;
}) =>
	request({
		url: Api.Page,
		method: 'post',
		data,
	});

/** 直接下级部门 */
export const getHrOrgChildren = (params?: { parentCcCode?: string; includeDisabled?: boolean }) =>
	request({
		url: Api.Children,
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

/** 停用部门（软删除） */
export const deleteHrOrg = (data: { ccCode: string; remark?: string }) =>
	request({
		url: Api.Delete,
		method: 'post',
		data,
	});

/** 按工号取人事姓名（账号关联带出） */
export const getHrEmpById = (empId: string) =>
	request({
		url: Api.EmpById,
		method: 'get',
		params: { empId },
	});

export const getHrCodeList = (codeField: string) =>
	request({
		url: Api.CodeList,
		method: 'get',
		params: { codeField },
	});

export const getHrCostCenterList = (site?: string) =>
	request({
		url: Api.CostCenterList,
		method: 'get',
		params: { site },
	});

export const getHrShiftGroupList = (tpkSite?: string) =>
	request({
		url: Api.ShiftGroupList,
		method: 'get',
		params: { tpkSite },
	});

export const mergeHrDept = (data: { oldCode1: string; oldCode2: string; newCode: string }) =>
	request({
		url: Api.MergeDept,
		method: 'post',
		data,
	});

export const transferHrDept = (data: { fromCcCode: string; toParentCcCode?: string }) =>
	request({
		url: Api.TransferDept,
		method: 'post',
		data,
	});
