/** 证件照：3:4，输出 480×640，JPEG 压到 100KB 以内。 */
export const EMP_PHOTO_WIDTH = 480;
export const EMP_PHOTO_HEIGHT = 640;
export const EMP_PHOTO_MAX_BYTES = 100 * 1024;

const canvasToBlob = (canvas: HTMLCanvasElement, quality: number) =>
	new Promise<Blob>((resolve, reject) => {
		canvas.toBlob(
			(blob) => (blob ? resolve(blob) : reject(new Error('toBlob'))),
			'image/jpeg',
			quality
		);
	});

/** 居中裁切为 3:4，缩放到 480×640，再降质量直到不超过 100KB。 */
export async function normalizeEmpPhoto(file: File): Promise<File> {
	const bmp = await createImageBitmap(file);
	try {
		const canvas = document.createElement('canvas');
		canvas.width = EMP_PHOTO_WIDTH;
		canvas.height = EMP_PHOTO_HEIGHT;
		const ctx = canvas.getContext('2d');
		if (!ctx) throw new Error('canvas');
		ctx.fillStyle = '#fff';
		ctx.fillRect(0, 0, EMP_PHOTO_WIDTH, EMP_PHOTO_HEIGHT);

		const dstRatio = EMP_PHOTO_WIDTH / EMP_PHOTO_HEIGHT;
		const srcRatio = bmp.width / bmp.height;
		let sx = 0;
		let sy = 0;
		let sw = bmp.width;
		let sh = bmp.height;
		if (srcRatio > dstRatio) {
			sw = bmp.height * dstRatio;
			sx = (bmp.width - sw) / 2;
		} else {
			sh = bmp.width / dstRatio;
			sy = (bmp.height - sh) / 2;
		}
		ctx.drawImage(bmp, sx, sy, sw, sh, 0, 0, EMP_PHOTO_WIDTH, EMP_PHOTO_HEIGHT);

		let quality = 0.9;
		let blob = await canvasToBlob(canvas, quality);
		while (blob.size > EMP_PHOTO_MAX_BYTES && quality > 0.35) {
			quality = Math.round((quality - 0.08) * 100) / 100;
			blob = await canvasToBlob(canvas, quality);
		}
		// 480×640 在极低质量仍超限时，按比例再缩小画布，直到 ≤100KB
		let scale = 1;
		while (blob.size > EMP_PHOTO_MAX_BYTES && scale > 0.4) {
			scale = Math.round((scale - 0.1) * 100) / 100;
			const w = Math.round(EMP_PHOTO_WIDTH * scale);
			const h = Math.round(EMP_PHOTO_HEIGHT * scale);
			const small = document.createElement('canvas');
			small.width = w;
			small.height = h;
			const sctx = small.getContext('2d');
			if (!sctx) break;
			sctx.drawImage(canvas, 0, 0, w, h);
			blob = await canvasToBlob(small, 0.7);
		}
		return new File([blob], 'emp-photo.jpg', { type: 'image/jpeg', lastModified: Date.now() });
	} finally {
		bmp.close();
	}
}
