import request from '/@/utils/request';

enum Api {
	FieldPool = '/api/hrEmpImport/fieldPool',
	TplPage = '/api/hrEmpImport/tplPage',
	TplDetail = '/api/hrEmpImport/tplDetail',
	TplByMode = '/api/hrEmpImport/tplByMode',
	TplSave = '/api/hrEmpImport/tplSave',
	TplDelete = '/api/hrEmpImport/tplDelete',
	DownloadTemplate = '/api/hrEmpImport/downloadTemplate',
	Preview = '/api/hrEmpImport/preview',
	Commit = '/api/hrEmpImport/commit',
	BatchPage = '/api/hrEmpImport/batchPage',
	FailRows = '/api/hrEmpImport/failRows',
}

export type ImportMode = 'Add' | 'Update';

export const fieldPoolHrEmpImport = (importMode: ImportMode) =>
	request({ url: Api.FieldPool, method: 'get', params: { importMode } });

export const tplPageHrEmpImport = (data: any) => request({ url: Api.TplPage, method: 'post', data });

export const tplDetailHrEmpImport = (id: number) =>
	request({ url: Api.TplDetail, method: 'get', params: { id } });

/** 本账号按作业类型的唯一模板（自动创建） */
export const tplByModeHrEmpImport = (importMode: ImportMode) =>
	request({ url: Api.TplByMode, method: 'get', params: { importMode } });

export const tplSaveHrEmpImport = (data: any) => request({ url: Api.TplSave, method: 'post', data });

export const tplDeleteHrEmpImport = (id: number) =>
	request({ url: Api.TplDelete, method: 'post', data: { id } });

export const downloadTemplateHrEmpImport = (tplId: number) =>
	request({
		url: Api.DownloadTemplate,
		method: 'get',
		params: { tplId },
		responseType: 'blob',
	});

export const previewHrEmpImport = (tplId: number, file: File) => {
	const form = new FormData();
	form.append('tplId', String(tplId));
	form.append('file', file);
	return request({ url: Api.Preview, method: 'post', data: form, headers: { 'Content-Type': 'multipart/form-data' } });
};

export const commitHrEmpImport = (tplId: number, file: File) => {
	const form = new FormData();
	form.append('tplId', String(tplId));
	form.append('file', file);
	return request({ url: Api.Commit, method: 'post', data: form, headers: { 'Content-Type': 'multipart/form-data' } });
};

export const batchPageHrEmpImport = (data: any) => request({ url: Api.BatchPage, method: 'post', data });

export const failRowsHrEmpImport = (data: any) => request({ url: Api.FailRows, method: 'post', data });
