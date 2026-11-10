/**
 * 身份证有效期（月）= 到期日 − 发证日期 的日历月差。
 * 两端任一缺失或无法解析时返回 null（不改动已有期限）。
 */
export function calcIdCardLimitMonths(
	issueDate: string | Date | null | undefined,
	expireDate: string | Date | null | undefined
): number | null {
	if (issueDate == null || expireDate == null || issueDate === '' || expireDate === '') return null;
	const start = new Date(issueDate);
	const end = new Date(expireDate);
	if (Number.isNaN(start.getTime()) || Number.isNaN(end.getTime())) return null;
	const months = (end.getFullYear() - start.getFullYear()) * 12 + (end.getMonth() - start.getMonth());
	return months < 0 ? 0 : months;
}
