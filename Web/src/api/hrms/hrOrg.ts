import request from '/@/utils/request';

enum Api {
	List = '/api/hrOrg/list',
	Add = '/api/hrOrg/add',
	Update = '/api/hrOrg/update',
	Delete = '/api/hrOrg/delete',
	Sync = '/api/hrOrg/syncFromCcMstr',
	CodeList = '/api/hrOrg/codeList',
	CostCenterList = '/api/hrOrg/costCenterList',
	ShiftGroupList = '/api/hrOrg/shiftGroupList',
	CcList = '/api/hrOrg/ccList',
	BatchUpdateCostKq = '/api/hrOrg/batchUpdateCostKq',
	BatchUpdateKqCode = '/api/hrOrg/batchUpdateKqCode',
	MergeDept = '/api/hrOrg/mergeDept',
	TransferDept = '/api/hrOrg/transferDept',
}

// 机构树（含 cc_mstr 全字段）
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

// 从人事部门（cc_mstr）导入/对齐系统机构
export const syncHrOrgFromCcMstr = () =>
	request({
		url: Api.Sync,
		method: 'post',
	});

// 人事代码下拉
export const getHrCodeList = (codeField: string) =>
	request({
		url: Api.CodeList,
		method: 'get',
		params: { codeField },
	});

// 成本中心下拉
export const getHrCostCenterList = (site?: string) =>
	request({
		url: Api.CostCenterList,
		method: 'get',
		params: { site },
	});

// 考勤组别下拉
export const getHrShiftGroupList = (tpkSite?: string) =>
	request({
		url: Api.ShiftGroupList,
		method: 'get',
		params: { tpkSite },
	});

// 部门列表（按层级码过滤）
export const getHrCcList = (ccLevel?: string) =>
	request({
		url: Api.CcList,
		method: 'get',
		params: { ccLevel },
	});

// 批量维护成本中心/工时变更/考勤组
export const batchUpdateCostKq = (data?: any) =>
	request({
		url: Api.BatchUpdateCostKq,
		method: 'post',
		data,
	});

// 批量维护考勤代码
export const batchUpdateKqCode = (data?: any) =>
	request({
		url: Api.BatchUpdateKqCode,
		method: 'post',
		data,
	});

// 合并部门
export const mergeHrDept = (data?: any) =>
	request({
		url: Api.MergeDept,
		method: 'post',
		data,
	});

// 调动部门
export const transferHrDept = (data?: any) =>
	request({
		url: Api.TransferDept,
		method: 'post',
		data,
	});
