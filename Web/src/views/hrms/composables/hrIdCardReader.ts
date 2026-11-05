/**
 * 本机二代证读卡 Agent（公安部标准 sdtapi）。
 * 业务开关在「公司参数 → 启用二代身份证」，不由此处 JSON/环境变量控制。
 * Agent 固定本机地址（硬件服务），与公司参数无关。
 */
const AGENT_BASE = 'http://127.0.0.1:19196';

export type HrIdCardDeviceResult = {
	ok: boolean;
	code?: string;
	message?: string;
	name?: string;
	sex?: string;
	nation?: string;
	birth?: string;
	address?: string;
	idNumber?: string;
	agency?: string;
	expireStart?: string;
	expireEnd?: string;
	photoBase64?: string;
};

export function getIdCardAgentBase(): string {
	return AGENT_BASE;
}

export async function idCardAgentHealth(timeoutMs = 2000): Promise<boolean> {
	const ctrl = new AbortController();
	const t = window.setTimeout(() => ctrl.abort(), timeoutMs);
	try {
		const res = await fetch(`${getIdCardAgentBase()}/health`, { signal: ctrl.signal });
		if (!res.ok) return false;
		const j = await res.json();
		return !!j?.ok;
	} catch {
		return false;
	} finally {
		window.clearTimeout(t);
	}
}

export async function idCardAgentRead(timeoutMs = 20000): Promise<HrIdCardDeviceResult> {
	const ctrl = new AbortController();
	const t = window.setTimeout(() => ctrl.abort(), timeoutMs);
	try {
		const res = await fetch(`${getIdCardAgentBase()}/idcard/read`, {
			method: 'POST',
			headers: { Accept: 'application/json' },
			signal: ctrl.signal,
		});
		const j = (await res.json()) as HrIdCardDeviceResult;
		return j ?? { ok: false, code: 'EMPTY', message: '读卡无返回' };
	} catch (e: any) {
		if (e?.name === 'AbortError') return { ok: false, code: 'TIMEOUT', message: '读卡超时' };
		return { ok: false, code: 'AGENT_UNREACHABLE', message: e?.message || '无法连接读卡服务' };
	} finally {
		window.clearTimeout(t);
	}
}

/** base64（可带 dataURL）→ File，供上传照片 */
export function idCardPhotoToFile(photoBase64: string, fileName = 'idcard-photo.bmp'): File | null {
	const raw = photoBase64.trim();
	if (!raw) return null;
	let b64 = raw;
	let mime = 'image/bmp';
	const m = /^data:([^;]+);base64,(.+)$/i.exec(raw);
	if (m) {
		mime = m[1] || mime;
		b64 = m[2];
	}
	try {
		const bin = atob(b64);
		const arr = new Uint8Array(bin.length);
		for (let i = 0; i < bin.length; i++) arr[i] = bin.charCodeAt(i);
		if (arr.length >= 2 && arr[0] === 0xff && arr[1] === 0xd8) {
			mime = 'image/jpeg';
			fileName = fileName.replace(/\.bmp$/i, '.jpg');
		} else if (arr.length >= 8 && arr[0] === 0x89 && arr[1] === 0x50) {
			mime = 'image/png';
			fileName = fileName.replace(/\.bmp$/i, '.png');
		}
		return new File([arr], fileName, { type: mime });
	} catch {
		return null;
	}
}
