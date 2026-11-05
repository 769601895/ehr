/** 与后端 HrUiCulture.Normalize 对齐，供布局译文行使用。 */
export function hrUiCulture(locale?: string) {
	const n = (locale || 'zh-CN').trim().replace('_', '-');
	if (/^zh-cn$/i.test(n) || /^zh-hans$/i.test(n)) return 'zh-CN';
	if (/^zh-tw$/i.test(n) || /^zh-hant/i.test(n)) return 'zh-TW';
	if (/^en/i.test(n)) return 'en';
	if (/^th/i.test(n)) return 'th';
	if (/^vi/i.test(n)) return 'vi';
	if (/^other$/i.test(n) || /^ot$/i.test(n)) return 'other';
	return 'zh-CN';
}

/**
 * 业务译文可维护语种（含「其他」备用槽，非界面切换语言）。
 * 新增/编辑、译文弹窗均按此列表渲染。
 */
export const hrUiCultures = [
	{ value: 'zh-CN', label: '简体中文' },
	{ value: 'zh-TW', label: '繁体中文' },
	{ value: 'en', label: 'English' },
	{ value: 'th', label: 'ไทย' },
	{ value: 'vi', label: 'Tiếng Việt' },
	{ value: 'other', label: '其他 / Other' },
];
