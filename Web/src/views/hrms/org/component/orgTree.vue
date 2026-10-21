<template>
	<el-card class="box-card" shadow="hover" body-style="height:100%;overflow:auto;padding:5px;width:100%;">
		<template #header>
			<div class="tree-toolbar">
				<div class="tree-toolbar-row">
					<span class="tree-toolbar-title">{{ t('message.hrms.common.person') }}</span>
					<el-select v-model="state.empState" size="small" style="width: 96px" @change="reloadTree">
						<el-option :label="t('message.hrms.common.empStateActive')" :value="1" />
						<el-option :label="t('message.hrms.common.empStateSuspend')" :value="2" />
						<el-option :label="t('message.hrms.common.empStateLeave')" :value="3" />
						<el-option :label="t('message.hrms.common.empStateAll')" :value="-1" />
					</el-select>
					<div class="tree-h-right">
						<el-dropdown @command="handleCommand">
							<el-button size="small" style="width: 34px">
								<el-icon class="el-icon--center"><more-filled /></el-icon>
							</el-button>
							<template #dropdown>
								<el-dropdown-menu>
									<el-dropdown-item command="expandAll">{{ t('message.list.allExpand') }}</el-dropdown-item>
									<el-dropdown-item command="collapseAll">{{ t('message.list.allFold') }}</el-dropdown-item>
									<el-dropdown-item command="rootNode">{{ t('message.hrms.common.rootNode') }}</el-dropdown-item>
									<el-dropdown-item command="refresh">{{ t('message.list.refresh') }}</el-dropdown-item>
								</el-dropdown-menu>
							</template>
						</el-dropdown>
					</div>
				</div>
				<div class="tree-toolbar-row">
					<el-radio-group v-model="state.includeDisabled" size="small" @change="reloadTree">
						<el-radio :value="false">{{ t('message.hrms.common.activeDepts') }}</el-radio>
						<el-radio :value="true">{{ t('message.hrms.common.allHistoryDepts') }}</el-radio>
					</el-radio-group>
				</div>
				<div class="tree-toolbar-row">
					<el-input :prefix-icon="Search" v-model.lazy="filterText" clearable size="small" :placeholder="t('message.hrms.common.queryDept')" />
				</div>
			</div>
		</template>
		<div class="tree-body" v-loading="state.loading">
			<el-scrollbar>
				<el-tree
					ref="treeRef"
					class="filter-tree hr-org-tree"
					:data="state.treeData"
					node-key="ccCode"
					:props="{ children: 'children', label: 'ccDesc' }"
					:filter-node-method="filterNode"
					@node-click="nodeClick"
					:default-expanded-keys="state.expandedKeys"
					highlight-current
				>
					<template #default="{ node, data }">
						<span class="tree-node">
							<el-icon v-if="node.level === 1" class="tree-node-icon root">
								<ele-OfficeBuilding />
							</el-icon>
							<el-icon v-else-if="node.expanded && data.children?.length" class="tree-node-icon">
								<ele-FolderOpened />
							</el-icon>
							<el-icon v-else class="tree-node-icon">
								<ele-Folder />
							</el-icon>
							<span class="tree-node-label">{{ node.label }}</span>
							<span class="tree-node-count">{{ data.empCount ?? 0 }}{{ t('message.hrms.common.personUnit') }}</span>
						</span>
					</template>
				</el-tree>
			</el-scrollbar>
		</div>
	</el-card>
</template>

<script lang="ts" setup>
import { nextTick, onMounted, reactive, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import type { ElTree } from 'element-plus';
import { Search, MoreFilled } from '@element-plus/icons-vue';
import { getHrOrgList } from '/@/api/hrms/hrOrg';

const { t } = useI18n();
const filterText = ref('');
const treeRef = ref<InstanceType<typeof ElTree>>();
const emits = defineEmits<{
	(e: 'node-click', node: any): void;
	/** 树数据加载完成（供右侧表格复用，避免再打一次 GetList） */
	(e: 'loaded', tree: any[], meta: { includeDisabled: boolean; empState: number }): void;
}>();
const state = reactive({
	loading: false,
	treeData: [] as any[],
	expandedKeys: [] as string[],
	/** 人数统计：1在职 2停职 3离职 -1全部 */
	empState: 1,
	/** false=现有有效部门 true=所有历史部门 */
	includeDisabled: false,
});

onMounted(async () => {
	await fetchTreeData();
});

watch(filterText, (val) => {
	treeRef.value?.filter(val);
});

const fetchTreeData = async (showLoading = true) => {
	if (showLoading) state.loading = true;
	try {
		const res = await getHrOrgList({
			includeDisabled: state.includeDisabled,
			empState: state.empState === -1 ? undefined : state.empState,
			empStateAll: state.empState === -1,
		});
		state.treeData = res.data.result ?? [];
		emits('loaded', state.treeData, { includeDisabled: state.includeDisabled, empState: state.empState });
		const rootCode = state.treeData[0]?.ccCode as string | undefined;
		state.expandedKeys = rootCode ? [rootCode] : [];
		await nextTick();
		if (rootCode) {
			treeRef.value?.setExpandedKeys(state.expandedKeys);
			treeRef.value?.setCurrentKey(rootCode);
			emits('node-click', state.treeData[0]);
		} else {
			emits('node-click', { ccCode: '', ccDesc: '', ccLevel: '' });
		}
	} finally {
		if (showLoading) state.loading = false;
	}
	return state.treeData;
};

const reloadTree = async () => {
	await fetchTreeData();
};

const filterNode = (value: string, data: any) => {
	if (!value) return true;
	return (data.ccDesc ?? '').includes(value) || (data.ccCode ?? '').includes(value) || (data.ccDept ?? '').includes(value);
};

const nodeClick = (node: any) => {
	emits('node-click', node);
};

const handleCommand = async (command: string | number | object) => {
	if (command === 'expandAll') {
		treeRef.value?.store._getAllNodes().forEach((n: any) => (n.expanded = true));
	} else if (command === 'collapseAll') {
		treeRef.value?.store._getAllNodes().forEach((n: any) => (n.expanded = false));
	} else if (command === 'refresh') {
		await fetchTreeData();
	} else if (command === 'rootNode') {
		treeRef.value?.setCurrentKey(undefined as any);
		emits('node-click', { ccCode: '', ccDesc: '', ccLevel: '' });
	}
};

defineExpose({ fetchTreeData });
</script>

<style lang="scss" scoped>
.box-card {
	flex: 1;
	> :deep(.el-card__header) {
		padding: 8px;
	}
}
.tree-toolbar {
	display: flex;
	flex-direction: column;
	gap: 8px;
}
.tree-toolbar-row {
	display: flex;
	align-items: center;
	gap: 8px;
	flex-wrap: wrap;
}
.tree-toolbar-title {
	font-weight: 600;
	font-size: 13px;
	color: var(--el-text-color-primary);
	margin-right: 4px;
}
.tree-h-right {
	margin-left: auto;
}
.tree-body {
	margin-bottom: 45px;
	min-height: 120px;
}
.tree-node {
	display: inline-flex;
	align-items: center;
	gap: 4px;
	line-height: 1.4;
}
.tree-node-icon {
	color: #e6a23c;
	font-size: 16px;
	&.root {
		color: var(--el-color-primary);
	}
}
.tree-node-label {
	font-size: 13px;
}
.tree-node-count {
	margin-left: 4px;
	color: var(--el-text-color-secondary);
	font-size: 12px;
}
.hr-org-tree {
	:deep(.el-tree-node__content) {
		height: 28px;
	}
}
</style>
