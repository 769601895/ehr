<template>
	<el-dialog v-model="visibleDialog" width="420px" append-to-body draggable :close-on-click-modal="false" destroy-on-close>
		<template #header>
			<div style="color: #fff">
				<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"><ele-OfficeBuilding /></el-icon>
				<span>部门选择</span>
			</div>
		</template>
		<div class="org-pick-body">
			<div class="org-pick-tip">选择部门....</div>
			<el-input v-model="filterText" clearable placeholder="输入部门名称筛选" class="mb10" />
			<div class="org-pick-tree">
				<el-scrollbar height="360px">
					<el-tree
						ref="treeRef"
						:data="treeData"
						node-key="id"
						:props="{ children: 'children', label: 'name' }"
						:filter-node-method="filterNode"
						:default-expanded-keys="defaultExpandedKeys"
						highlight-current
						@node-click="onNodeClick"
					>
						<template #default="{ node, data }">
							<span class="org-pick-node" :class="{ 'is-selected': selectedId === data.id }">
								<el-icon v-if="selectedId === data.id" class="org-pick-check"><ele-Check /></el-icon>
								<span v-else class="org-pick-check-placeholder" />
								<el-icon class="org-pick-folder"><ele-Folder /></el-icon>
								<span class="org-pick-label">{{ node.label }}</span>
								<span v-if="data.children?.length" class="org-pick-count">({{ data.children.length }})</span>
							</span>
						</template>
					</el-tree>
				</el-scrollbar>
			</div>
		</div>
		<template #footer>
			<span class="dialog-footer">
				<el-button type="primary" icon="ele-CircleCheckFilled" @click="confirm">选定</el-button>
				<el-button icon="ele-CircleCloseFilled" @click="closeDialog">取消</el-button>
			</span>
		</template>
	</el-dialog>
</template>

<script lang="ts" setup>
import { computed, nextTick, ref, watch } from 'vue';
import type { ElTree } from 'element-plus';
import { ElMessage } from 'element-plus';
import { SysOrg } from '/@/api-services/system/models';

const props = defineProps({
	visible: { type: Boolean, default: false },
	/** 机构树数据 */
	orgData: { type: Array as () => Array<SysOrg>, default: () => [] },
	/** 当前已选机构 Id */
	modelValue: { type: [Number, String] as unknown as () => number | string | undefined, default: undefined },
});

const emits = defineEmits<{
	(e: 'update:visible', v: boolean): void;
	/** id 可为数字（SysOrg）或字符串（人事 CcCode） */
	(e: 'confirm', payload: { id: number | string; name: string; path: string }): void;
}>();

const treeRef = ref<InstanceType<typeof ElTree>>();
const filterText = ref('');
const selectedId = ref<number | string | undefined>();
const selectedNode = ref<SysOrg | null>(null);

const treeData = computed(() => props.orgData ?? []);
const defaultExpandedKeys = computed(() => {
	const root = treeData.value[0];
	return root?.id != null ? [root.id] : [];
});

const visibleDialog = computed({
	get: () => props.visible,
	set: (v: boolean) => emits('update:visible', v),
});

watch(
	() => props.visible,
	async (open) => {
		if (!open) return;
		filterText.value = '';
		selectedId.value = props.modelValue;
		selectedNode.value = findOrgById(treeData.value, props.modelValue) ?? null;
		await nextTick();
		if (selectedId.value != null) {
			treeRef.value?.setCurrentKey(selectedId.value as any);
		}
	}
);

watch(filterText, (val) => {
	treeRef.value?.filter(val);
});

const filterNode = (value: string, data: SysOrg) => {
	if (!value) return true;
	return (data.name ?? '').includes(value);
};

const onNodeClick = (data: SysOrg) => {
	selectedId.value = data.id;
	selectedNode.value = data;
};

const findOrgById = (nodes: SysOrg[], id: number | string | undefined | null): SysOrg | null => {
	if (id == null || id === '') return null;
	for (const n of nodes) {
		if (n.id == id) return n;
		if (n.children?.length) {
			const found = findOrgById(n.children as SysOrg[], id);
			if (found) return found;
		}
	}
	return null;
};

const buildOrgPath = (nodes: SysOrg[], id: number | string | undefined | null, path: string[] = []): string => {
	if (id == null || id === '') return '';
	for (const n of nodes) {
		const next = [...path, n.name ?? ''];
		if (n.id == id) return next.filter(Boolean).join(' / ');
		if (n.children?.length) {
			const found = buildOrgPath(n.children as SysOrg[], id, next);
			if (found) return found;
		}
	}
	return '';
};

const normalizePickId = (raw: number | string): number | string => {
	if (typeof raw === 'number') return raw;
	const s = String(raw);
	// 纯数字且无前导零时转为 number，保留 CcCode 等带前导零/非数字编码为 string
	if (/^\d+$/.test(s) && !/^0\d/.test(s)) return Number(s);
	return s;
};

const confirm = () => {
	if (selectedId.value == null || selectedId.value === '' || !selectedNode.value) {
		ElMessage.warning('请选择部门');
		return;
	}
	const id = normalizePickId(selectedId.value);
	const name = selectedNode.value.name ?? '';
	const path = buildOrgPath(treeData.value, selectedId.value) || name;
	emits('confirm', { id, name, path });
	emits('update:visible', false);
};

const closeDialog = () => {
	emits('update:visible', false);
};

defineExpose({ buildOrgPath, findOrgById });
</script>

<style lang="scss" scoped>
.org-pick-body {
	.org-pick-tip {
		margin-bottom: 8px;
		color: var(--el-text-color-regular);
	}
	.org-pick-tree {
		border: 1px solid var(--el-border-color);
		border-radius: 4px;
		padding: 6px 4px;
	}
}
.org-pick-node {
	display: inline-flex;
	align-items: center;
	gap: 4px;
	line-height: 22px;
	&.is-selected {
		.org-pick-label {
			font-weight: 600;
		}
	}
}
.org-pick-check {
	color: var(--el-color-danger);
	font-size: 14px;
}
.org-pick-check-placeholder {
	display: inline-block;
	width: 14px;
	height: 14px;
}
.org-pick-folder {
	color: #e6a23c;
	font-size: 16px;
}
.org-pick-count {
	color: var(--el-text-color-secondary);
	margin-left: 2px;
}
.mb10 {
	margin-bottom: 10px;
}
</style>
