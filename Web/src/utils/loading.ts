import { nextTick } from 'vue';
import '/@/theme/loading.scss';

/**
 * 页面全局 Loading
 * @method start 创建 loading
 * @method done 移除 loading
 */
// 动态注入遮罩层样式
function injectMaskStyle() {
	if (document.getElementById('full-mask-style')) return;
	const style = document.createElement('style');
	style.id = 'full-mask-style';
	style.innerHTML = `
	.loading-next {
		position: fixed;
		inset: 0;
		// z-index: 4000;
		// background: var(--el-color-primary-light-9);
		backdrop-filter: blur(2px);
		display: flex;
		align-items: center;
		justify-content: center;
		cursor: pointer;
	}
	`;
	document.head.appendChild(style);
}

export const NextLoading = {
	// 创建 loading
	start: () => {
		if (document.querySelector('.full-mask')) return;
		injectMaskStyle();
		const div = document.createElement('div');
		div.setAttribute('class', 'loading-next');
		div.innerHTML = `
			<div class="loading-next-box">
				<div class="loading-next-box-warp">
					<div class="loading-next-box-item"></div>
					<div class="loading-next-box-item"></div>
					<div class="loading-next-box-item"></div>
					<div class="loading-next-box-item"></div>
					<div class="loading-next-box-item"></div>
					<div class="loading-next-box-item"></div>
					<div class="loading-next-box-item"></div>
					<div class="loading-next-box-item"></div>
					<div class="loading-next-box-item"></div>
				</div>
			</div>
		`;
		document.body.appendChild(div);
		window.nextLoading = true;
	},
	// 移除 loading
	done: (time: number = 0) => {
		nextTick(() => {
			setTimeout(() => {
				window.nextLoading = false;
				const el = document.querySelector('.loading-next');
				el?.parentNode?.removeChild(el);
			}, time);
		});
	},
};
