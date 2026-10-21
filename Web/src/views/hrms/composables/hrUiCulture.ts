/** 与后端 HrUiCulture.Normalize 对齐，供布局译文行使用。 */
export function hrUiCulture(locale?: string) {
	const n = (locale || 'zh-CN').trim().replace('_', '-');
	if (/^zh-cn$/i.test(n) || /^zh-hans$/i.test(n)) return 'zh-CN';
	if (/^zh-tw$/i.test(n) || /^zh-hant/i.test(n)) return 'zh-TW';
	if (/^en/i.test(n)) return 'en';
	if (/^th/i.test(n)) return 'th';
	if (/^vi/i.test(n)) return 'vi';
	return 'zh-CN';
}

export const hrUiCultures = [
	{ value: 'zh-CN', label: '简体中文' },
	{ value: 'zh-TW', label: '繁体中文' },
	{ value: 'en', label: 'English' },
	{ value: 'th', label: 'ไทย' },
	{ value: 'vi', label: 'Tiếng Việt' },
];
