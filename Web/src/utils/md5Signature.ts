import CryptoJS from 'crypto-js';

// 递归处理对象，标准化数据以进行严谨的脏检查
function normalizeData(obj: any): any {
	if (obj === null || obj === undefined) {
		return '';
	}

	// 处理日期对象
	if (obj instanceof Date) {
		return obj.toISOString();
	}

	if (Array.isArray(obj)) {
		return obj.map((item) => normalizeData(item));
	}

	if (typeof obj === 'object') {
		const normalized: any = {};
		// 获取所有 key 并排序，确保 JSON.stringify 结果稳定
		const keys = Object.keys(obj).sort();
		for (const key of keys) {
			normalized[key] = normalizeData(obj[key]);
		}
		return normalized;
	}

	// 将所有基本类型转换为字符串，避免数字和字符串比较导致的误判
	return String(obj);
}

export default function md5Signature(data: any) {
	if (!data) return '';
	try {
		const normalizedData = normalizeData(data);
		return CryptoJS.MD5(JSON.stringify(normalizedData)).toString();
	} catch (e) {
		console.error('计算数据签名失败:', e);
		return '';
	}
}
