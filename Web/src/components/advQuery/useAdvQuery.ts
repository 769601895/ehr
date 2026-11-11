import { ref } from 'vue';
import type { AdvFilterNode, AdvQueryField } from './types';

/**
 * 列表页接入高级查询的通用状态与操作。
 * 控件：AdvQueryDrawer；方案 API 按 moduleCode 隔离。
 */
export function useAdvQuery(moduleCode: string) {
	const drawerRef = ref<{ open: (f?: AdvFilterNode | null) => void; resetTree?: () => void } | null>(null);
	const fields = ref<AdvQueryField[]>([]);
	const filter = ref<AdvFilterNode | null>(null);
	const activeCount = ref(0);

	const open = () => drawerRef.value?.open(filter.value);
	const reset = () => {
		filter.value = null;
		activeCount.value = 0;
		drawerRef.value?.resetTree?.();
	};
	const applySearch = (next: AdvFilterNode | null) => {
		filter.value = next;
	};
	const setActiveCount = (n: number) => {
		activeCount.value = n;
	};
	const setFields = (list: AdvQueryField[]) => {
		fields.value = list ?? [];
	};

	return {
		moduleCode,
		drawerRef,
		fields,
		filter,
		activeCount,
		open,
		reset,
		applySearch,
		setActiveCount,
		setFields,
	};
}
