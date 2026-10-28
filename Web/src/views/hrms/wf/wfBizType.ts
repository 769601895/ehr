import { getHrCodeList } from '/@/api/hrms/hrOrg';

/** 审批业务类型对应的人事代码类（code_mstr.code_field） */
export const HR_WF_BIZ_CODE_FIELD = 'hr_wf_biz';

export type HrWfBizCodeItem = { value: string; label: string; text?: string };

export async function loadHrWfBizTypeOptions(): Promise<HrWfBizCodeItem[]> {
	try {
		const res = await getHrCodeList(HR_WF_BIZ_CODE_FIELD);
		const list = (res.data?.result ?? res.data ?? []) as HrWfBizCodeItem[];
		return Array.isArray(list) ? list : [];
	} catch {
		return [];
	}
}

export function bizTypeLabel(options: HrWfBizCodeItem[], bizType: string | null | undefined): string {
	const v = (bizType || '').trim();
	if (!v) return '';
	const hit = options.find((o) => o.value === v);
	return hit ? `${hit.label} (${v})` : v;
}
