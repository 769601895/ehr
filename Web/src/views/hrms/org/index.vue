<template>
	<div class="hr-org-container">
		<splitpanes class="default-theme">
			<pane size="22" style="display: flex">
				<OrgTree ref="orgTreeRef" @node-click="handleNodeChange" />
			</pane>
			<pane size="78" class="hr-org-right-pane">
				<div class="hr-org-right">
					<el-card shadow="hover" :body-style="{ padding: '8px 10px', display: 'flex', width: '100%', alignItems: 'center' }">
						<el-tabs v-model="state.activeTab" class="hr-org-tabs-header" @tab-change="handleTabChange">
							<el-tab-pane label="部门维护" name="maintain" />
							<el-tab-pane label="部门异动" name="change" />
							<el-tab-pane label="部门明细" name="detail" />
						</el-tabs>
					</el-card>

					<!-- 部门维护 -->
					<template v-if="state.activeTab === 'maintain'">
						<el-card shadow="hover" :body-style="{ padding: '8px 10px', display: 'flex', width: '100%', alignItems: 'center', flexWrap: 'wrap', gap: '4px' }" style="margin-top: 5px">
							<el-input v-model="state.keyword" placeholder="按名称/编码筛选" clearable style="width: 220px" @keyup.enter="handleQuery" />
							<el-checkbox v-model="state.includeDisabled" style="margin-left: 8px" @change="handleQuery">含停用</el-checkbox>
							<el-button-group style="margin-left: 8px">
								<el-button type="primary" icon="ele-Search" :loading="maintainOptions.loading" @click="handleQuery">查询</el-button>
								<el-button icon="ele-Refresh" :loading="maintainOptions.loading" @click="resetQuery">重置</el-button>
							</el-button-group>
							<el-button type="primary" icon="ele-Plus" style="margin-left: 8px" v-auth="'hrOrg/add'" @click="handleAdd">新增</el-button>
							<el-button-group style="margin-left: 8px">
								<el-button icon="ele-Expand" @click="expandAll">全部展开</el-button>
								<el-button icon="ele-Fold" @click="foldAll">全部折叠</el-button>
							</el-button-group>
						</el-card>
						<el-card class="full-table" shadow="hover" style="margin-top: 5px; flex: 1">
							<vxe-grid ref="maintainGrid" class="xGrid-style" v-bind="maintainOptions" v-on="maintainGridEvents">
								<template #empty>
									<el-empty :image-size="160" />
								</template>
								<template #row_ccEnable="{ row }">
									<el-tag v-if="row.ccEnable == null || row.ccEnable === 0" type="success">启用</el-tag>
									<el-tag v-else type="danger">停用</el-tag>
								</template>
								<template #row_buttons="{ row }">
									<el-button icon="ele-Edit" text type="primary" v-auth="'hrOrg/update'" :disabled="row.ccEnable === 1" @click="handleEdit(row)">编辑</el-button>
									<el-button icon="ele-Delete" text type="danger" v-auth="'hrOrg/delete'" :disabled="row.ccEnable === 1" @click="handleDisable(row)">停用</el-button>
								</template>
							</vxe-grid>
						</el-card>
					</template>

					<!-- 部门异动 -->
					<el-card v-else-if="state.activeTab === 'change'" class="full-table" shadow="hover" style="margin-top: 5px; flex: 1; overflow: auto">
						<ChangeOrg :tree-data="state.fullTree" @reload="reloadAll" />
					</el-card>

					<!-- 部门明细 -->
					<template v-else-if="state.activeTab === 'detail'">
						<el-card shadow="hover" :body-style="{ padding: '8px 10px', display: 'flex', width: '100%', alignItems: 'center', flexWrap: 'wrap', gap: '4px' }" style="margin-top: 5px">
							<el-input v-model="state.detailKeyword" placeholder="按名称/编码筛选" clearable style="width: 220px" @keyup.enter="handleDetailQuery(true)" />
							<el-checkbox v-model="state.detailIncludeDisabled" style="margin-left: 8px" @change="handleDetailQuery(true)">含停用</el-checkbox>
							<el-button-group style="margin-left: 8px">
								<el-button type="primary" icon="ele-Search" :loading="detailOptions.loading" @click="handleDetailQuery(true)">查询</el-button>
								<el-button icon="ele-Refresh" :loading="detailOptions.loading" @click="resetDetailQuery">重置</el-button>
							</el-button-group>
						</el-card>
						<el-card class="full-table" shadow="hover" style="margin-top: 5px; flex: 1">
							<vxe-grid ref="detailGrid" class="xGrid-style" v-bind="detailOptions" v-on="detailGridEvents">
								<template #empty>
									<el-empty :image-size="160" />
								</template>
								<template #row_ccEnable="{ row }">
									{{ row.ccEnable === 1 ? '是' : '否' }}
								</template>
							</vxe-grid>
						</el-card>
					</template>
				</div>
			</pane>
		</splitpanes>

		<EditOrg ref="editOrgRef" :title="state.title" :tree-data="state.fullTree" @reload="reloadAll" />
	</div>
</template>

<script lang="ts" setup name="hrOrg">
import { nextTick, onMounted, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { auth } from '/@/utils/authFunction';

import OrgTree from '/@/views/hrms/org/component/orgTree.vue';
import EditOrg from '/@/views/hrms/org/component/editOrg.vue';
import ChangeOrg from '/@/views/hrms/org/component/changeOrg.vue';
import { deleteHrOrg, getHrOrgList, pageHrOrg } from '/@/api/hrms/hrOrg';

const orgTreeRef = ref<InstanceType<typeof OrgTree>>();
const editOrgRef = ref<InstanceType<typeof EditOrg>>();
const maintainGrid = ref<VxeGridInstance>();
const detailGrid = ref<VxeGridInstance>();

const localPageParamKey = 'localPageParam:hrOrg';
const localDetailParamKey = 'localPageParam:hrOrgDetail';

const state = reactive({
	activeTab: 'maintain',
	keyword: '',
	includeDisabled: false,
	parentCcCode: '' as string,
	title: '',
	fullTree: [] as any[],
	detailKeyword: '',
	detailIncludeDisabled: false,
	localPageParam: {
		defaultSort: { field: 'ccLevel', order: 'asc' as string, descStr: 'desc' },
	},
	localDetailParam: {
		pageSize: 50 as number,
		defaultSort: { field: 'ccLevel', order: 'asc' as string, descStr: 'desc' },
	},
});

const filterTree = (nodes: any[], keyword: string): any[] => {
	if (!keyword) return nodes;
	const kw = keyword.trim().toLowerCase();
	const walk = (list: any[]): any[] => {
		const result: any[] = [];
		for (const n of list) {
			const children = n.children?.length ? walk(n.children) : [];
			const hit = (n.ccDesc ?? '').toLowerCase().includes(kw) || (n.ccCode ?? '').toLowerCase().includes(kw) || (n.ccDept ?? '').toLowerCase().includes(kw);
			if (hit || children.length) result.push({ ...n, children });
		}
		return result;
	};
	return walk(nodes);
};

/** 左侧选中节点时：返回该节点自身及其下级（整棵子树） */
const pickSubtree = (nodes: any[], parentCcCode: string): any[] => {
	if (!parentCcCode) return nodes;
	const find = (list: any[]): any[] | null => {
		for (const n of list) {
			if (n.ccCode === parentCcCode) return [n];
			if (n.children?.length) {
				const found = find(n.children);
				if (found) return found;
			}
		}
		return null;
	};
	return find(nodes) ?? [];
};

const resolveSort = (sort?: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams | null, fallback?: { field: string; order: string; descStr: string }) => {
	const fb = fallback || state.localPageParam.defaultSort;
	const effective = sort?.field && sort?.order ? sort : { field: fb.field, order: fb.order };
	return { field: effective.field!, order: String(effective.order), descStr: 'desc' as const };
};

const pickProxySort = (params: { sort?: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams; sorts?: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams[] }) =>
	params.sort?.field ? params.sort : params.sorts?.[0];

const expandMaintainTree = async () => {
	await nextTick();
	await nextTick();
	const grid = maintainGrid.value;
	if (!grid) return;
	grid.setAllTreeExpand(true);
	// 代理加载后偶发未展开，再补一次
	setTimeout(() => grid.setAllTreeExpand(true), 80);
};

const maintainOptions = useVxeTable<any>(
	{
		id: 'hrOrgMaintain',
		name: '部门维护',
		columns: [
			{ field: 'ccDesc', title: '部门名称', minWidth: 180, treeNode: true, align: 'left', headerAlign: 'center', showOverflow: 'tooltip', sortable: true },
			{ field: 'ccCode', title: '部门编码', minWidth: 110, showOverflow: 'tooltip', sortable: true },
			{ field: 'ccDept', title: '部门代号', minWidth: 100, showOverflow: 'tooltip', sortable: true },
			{ field: 'ccLevel', title: '层级码', minWidth: 110, showOverflow: 'tooltip', sortable: true },
			{ field: 'ccSite', title: '厂区', minWidth: 90, showOverflow: 'tooltip', sortable: true },
			{ field: 'ccCost', title: '成本类型', minWidth: 90, showOverflow: 'tooltip', sortable: true },
			{ field: 'ccEnable', title: '状态', width: 90, align: 'center', sortable: true, slots: { default: 'row_ccEnable' } },
			{ field: 'ccUser', title: '维护人', minWidth: 90, showOverflow: 'tooltip', sortable: true },
			{ field: 'ccDate', title: '维护时间', minWidth: 160, showOverflow: 'tooltip', sortable: true },
			{ field: 'buttons', title: '操作', fixed: 'right', width: 160, showOverflow: true, slots: { default: 'row_buttons' } },
		],
	},
	{
		stripe: false,
		height: '100%',
		proxyConfig: {
			autoLoad: true,
			ajax: {
				query: (params) => handleMaintainQueryApi(pickProxySort(params)),
			},
		},
		sortConfig: {
			remote: true,
			defaultSort: Local.get(localPageParamKey)?.defaultSort || state.localPageParam.defaultSort,
		},
		pagerConfig: { enabled: false },
		toolbarConfig: { enabled: false },
		rowConfig: { keyField: 'ccCode' },
		treeConfig: { transform: false, childrenField: 'children', expandAll: true, reserve: true },
	}
);

const detailOptions = useVxeTable<any>(
	{
		id: 'hrOrgDetail',
		name: '部门明细',
		columns: [
			{ field: 'seq', type: 'seq', title: '#', width: 55 },
			{ field: 'ccCode', title: '编码', minWidth: 100, sortable: true },
			{ field: 'ccDesc', title: '名称', minWidth: 140, sortable: true },
			{ field: 'ccDept', title: '代号', minWidth: 90, sortable: true },
			{ field: 'ccLevel', title: '层级码', minWidth: 110, sortable: true },
			{ field: 'ccCodeParent', title: '父编码', minWidth: 100, sortable: true },
			{ field: 'ccSite', title: '厂区', minWidth: 80, sortable: true },
			{ field: 'ccCost', title: '成本', minWidth: 80, sortable: true },
			{ field: 'ccWoca', title: '工时类别', minWidth: 90, sortable: true },
			{ field: 'ccXzGroup', title: '薪酬组', minWidth: 90, sortable: true },
			{ field: 'ccEnable', title: '停用', width: 70, align: 'center', sortable: true, slots: { default: 'row_ccEnable' } },
			{ field: 'ccDeleRemark', title: '停用备注', minWidth: 140, showOverflow: 'tooltip' },
		],
	},
	{
		height: '100%',
		proxyConfig: {
			autoLoad: false,
			ajax: {
				query: (params) => handleDetailQueryApi(params.page, pickProxySort(params)),
			},
		},
		sortConfig: {
			remote: true,
			defaultSort: Local.get(localDetailParamKey)?.defaultSort || state.localDetailParam.defaultSort,
		},
		pagerConfig: { pageSize: Local.get(localDetailParamKey)?.pageSize || state.localDetailParam.pageSize },
		toolbarConfig: { enabled: false },
		rowConfig: { keyField: 'ccCode' },
	}
);

const handleMaintainQueryApi = async (sort?: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const effectiveSort = resolveSort(sort, Local.get(localPageParamKey)?.defaultSort || state.localPageParam.defaultSort);
	const res = await getHrOrgList({
		includeDisabled: state.includeDisabled,
		field: effectiveSort.field,
		order: effectiveSort.order,
		descStr: effectiveSort.descStr,
	});
	state.fullTree = res.data.result ?? [];
	const scoped = pickSubtree(state.fullTree, state.parentCcCode);
	return { data: { result: filterTree(scoped, state.keyword) } };
};

const handleDetailQueryApi = async (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const effectiveSort = resolveSort(sort, Local.get(localDetailParamKey)?.defaultSort || state.localDetailParam.defaultSort);
	return pageHrOrg({
		page: page.currentPage,
		pageSize: page.pageSize,
		field: effectiveSort.field,
		order: effectiveSort.order,
		descStr: effectiveSort.descStr,
		keyword: state.detailKeyword || undefined,
		parentCcCode: state.parentCcCode || undefined,
		includeDisabled: state.detailIncludeDisabled,
	});
};

const handleQuery = async () => {
	if (state.activeTab !== 'maintain') return;
	await nextTick();
	if (!maintainGrid.value) {
		// 表格尚未挂载时稍后再查
		setTimeout(() => handleQuery(), 50);
		return;
	}
	maintainOptions.loading = true;
	await maintainGrid.value.commitProxy('reload');
	await expandMaintainTree();
	maintainOptions.loading = false;
};

const resetQuery = async () => {
	state.keyword = '';
	state.includeDisabled = false;
	state.parentCcCode = '';
	await handleQuery();
};

const handleDetailQuery = async (reset = false) => {
	await nextTick();
	if (!detailGrid.value) return;
	detailOptions.loading = true;
	reset ? await detailGrid.value.commitProxy('reload') : await detailGrid.value.commitProxy('query');
	detailOptions.loading = false;
};

const resetDetailQuery = async () => {
	state.detailKeyword = '';
	state.detailIncludeDisabled = false;
	await handleDetailQuery(true);
};

const handleTabChange = async (name: string | number) => {
	if (name === 'maintain') {
		await nextTick();
		await handleQuery();
	} else if (name === 'detail') {
		await handleDetailQuery(true);
	}
};

const reloadAll = async () => {
	await orgTreeRef.value?.fetchTreeData();
	if (state.activeTab === 'maintain') await handleQuery();
	if (state.activeTab === 'detail') await handleDetailQuery(true);
};

const handleNodeChange = async (node: any) => {
	state.parentCcCode = node?.ccCode ?? '';
	if (state.activeTab === 'maintain') await handleQuery();
	if (state.activeTab === 'detail') await handleDetailQuery(true);
};

const handleAdd = () => {
	state.title = '新增部门';
	editOrgRef.value?.openDialog(
		{
			ccCodeParent: state.parentCcCode || undefined,
			ccKqgroup: 1,
			ccSub: 0,
			ccWageRate: 0,
		},
		false
	);
};

const handleEdit = (row: any) => {
	state.title = '编辑部门';
	editOrgRef.value?.openDialog(row, true);
};

const handleDisable = (row: any) => {
	ElMessageBox.prompt(`确认停用部门「${row.ccDesc}」及其下级？`, '停用部门', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		inputPlaceholder: '停用备注（可选）',
		type: 'warning',
	})
		.then(async ({ value }) => {
			await deleteHrOrg({ ccCode: row.ccCode, remark: value || undefined });
			ElMessage.success('已停用');
			await reloadAll();
		})
		.catch(() => {});
};

const expandAll = () => {
	maintainGrid.value?.setAllTreeExpand(true);
};

const foldAll = () => {
	maintainGrid.value?.clearTreeExpand();
};

const maintainGridEvents: VxeGridListeners = {
	async proxyQuery() {
		await expandMaintainTree();
	},
	async sortChange({ field, order }) {
		if (!field || !order) return;
		state.localPageParam.defaultSort = { field, order, descStr: 'desc' };
		Local.set(localPageParamKey, state.localPageParam);
	},
	async cellDblclick({ row }) {
		if (auth('hrOrg/update') && row.ccEnable !== 1) await handleEdit(row);
	},
};

const detailGridEvents: VxeGridListeners = {
	async pageChange({ pageSize }) {
		state.localDetailParam.pageSize = pageSize;
		Local.set(localDetailParamKey, state.localDetailParam);
	},
	async sortChange({ field, order }) {
		if (!field || !order) return;
		state.localDetailParam.defaultSort = { field, order, descStr: 'desc' };
		Local.set(localDetailParamKey, state.localDetailParam);
	},
};

onMounted(async () => {
	state.localPageParam = Local.get(localPageParamKey) || state.localPageParam;
	state.localDetailParam = Local.get(localDetailParamKey) || state.localDetailParam;
});
</script>

<style lang="scss" scoped>
.hr-org-container {
	height: 100%;
	display: flex;
	flex-direction: column;
	overflow: hidden;

	:deep(.splitpanes) {
		flex: 1;
		min-height: 0;
		height: 100%;
	}

	:deep(.splitpanes__pane) {
		min-height: 0;
		overflow: hidden;
	}
}

.hr-org-right-pane {
	display: flex !important;
}

.hr-org-right {
	flex: 1;
	min-height: 0;
	width: 100%;
	display: flex;
	flex-direction: column;
	overflow: hidden;
}

.hr-org-tabs-header {
	width: 100%;
	:deep(.el-tabs__header) {
		margin: 0;
	}
	:deep(.el-tabs__nav-wrap::after) {
		display: none;
	}
	:deep(.el-tabs__content) {
		display: none;
	}
}

:deep(.full-table) {
	min-height: 0;
	display: flex;
	flex-direction: column;

	.el-card__body {
		flex: 1;
		min-height: 0;
		height: 100%;
		display: flex;
		flex-direction: column;
		padding: 5px;
		overflow: hidden;
	}
}

.xGrid-style {
	flex: 1;
	min-height: 0;
	height: 100%;
}
</style>
