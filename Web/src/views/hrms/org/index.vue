<template>
	<div class="hr-org-container">
		<splitpanes class="default-theme">
			<pane size="22" style="display: flex">
				<OrgTree ref="orgTreeRef" @node-click="handleNodeChange" @loaded="handleTreeLoaded" />
			</pane>
			<pane size="78" class="hr-org-right-pane">
				<div class="hr-org-right">
					<el-card shadow="hover" :body-style="{ padding: '8px 10px', display: 'flex', width: '100%', alignItems: 'center' }">
						<el-tabs v-model="state.activeTab" class="hr-org-tabs-header" @tab-change="handleTabChange">
							<el-tab-pane :label="t('message.hrms.org.tabMaintain')" name="maintain" />
							<el-tab-pane :label="t('message.hrms.org.tabChange')" name="change" />
							<el-tab-pane :label="t('message.hrms.org.tabDetail')" name="detail" />
						</el-tabs>
					</el-card>

					<!-- 部门维护 -->
					<template v-if="state.activeTab === 'maintain'">
						<el-card shadow="hover" :body-style="{ padding: '8px 10px', display: 'flex', width: '100%', alignItems: 'center', flexWrap: 'wrap', gap: '4px' }" style="margin-top: 5px">
							<el-input v-model="state.keyword" :placeholder="t('message.hrms.common.filterNameCode')" clearable style="width: 220px" @keyup.enter="handleQuery" />
							<el-checkbox v-model="state.includeDisabled" style="margin-left: 8px" @change="() => handleQuery(true)">{{ t('message.hrms.common.includeDisabled') }}</el-checkbox>
							<el-button-group style="margin-left: 8px">
								<el-button type="primary" icon="ele-Search" :loading="maintainOptions.loading" @click="() => handleQuery(false)">{{ t('message.list.query') }}</el-button>
								<el-button icon="ele-Refresh" :loading="maintainOptions.loading" @click="resetQuery">{{ t('message.list.reset') }}</el-button>
							</el-button-group>
							<el-button type="primary" icon="ele-Plus" style="margin-left: 8px" v-auth="'hrOrg/add'" @click="handleAdd">{{ t('message.list.add') }}</el-button>
							<el-button-group style="margin-left: 8px">
								<el-button icon="ele-Expand" @click="expandAll">{{ t('message.list.allExpand') }}</el-button>
								<el-button icon="ele-Fold" @click="foldAll">{{ t('message.list.allFold') }}</el-button>
							</el-button-group>
						</el-card>
						<el-card class="full-table" shadow="hover" style="margin-top: 5px; flex: 1">
							<vxe-grid ref="maintainGrid" class="xGrid-style" v-bind="maintainOptions" v-on="maintainGridEvents">
								<template #empty>
									<el-empty :image-size="160" />
								</template>
								<template #row_ccEnable="{ row }">
									<el-tag v-if="row.ccEnable == null || row.ccEnable === 0" type="success">{{ t('message.hrms.common.enabled') }}</el-tag>
									<el-tag v-else type="danger">{{ t('message.hrms.common.disabled') }}</el-tag>
								</template>
								<template #row_buttons="{ row }">
									<el-button icon="ele-Edit" text type="primary" v-auth="'hrOrg/update'" :disabled="row.ccEnable === 1" @click="handleEdit(row)">{{ t('message.list.edit') }}</el-button>
									<el-button icon="ele-Delete" text type="danger" v-auth="'hrOrg/delete'" :disabled="row.ccEnable === 1" @click="handleDisable(row)">{{ t('message.hrms.common.disabled') }}</el-button>
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
							<el-input v-model="state.detailKeyword" :placeholder="t('message.hrms.common.filterNameCode')" clearable style="width: 220px" @keyup.enter="handleDetailQuery(true)" />
							<el-checkbox v-model="state.detailIncludeDisabled" style="margin-left: 8px" @change="handleDetailQuery(true)">{{ t('message.hrms.common.includeDisabled') }}</el-checkbox>
							<el-button-group style="margin-left: 8px">
								<el-button type="primary" icon="ele-Search" :loading="detailOptions.loading" @click="handleDetailQuery(true)">{{ t('message.list.query') }}</el-button>
								<el-button icon="ele-Refresh" :loading="detailOptions.loading" @click="resetDetailQuery">{{ t('message.list.reset') }}</el-button>
							</el-button-group>
						</el-card>
						<el-card class="full-table" shadow="hover" style="margin-top: 5px; flex: 1">
							<vxe-grid ref="detailGrid" class="xGrid-style" v-bind="detailOptions" v-on="detailGridEvents">
								<template #empty>
									<el-empty :image-size="160" />
								</template>
								<template #row_ccEnable="{ row }">
									{{ row.ccEnable === 1 ? t('message.hrms.common.yes') : t('message.hrms.common.no') }}
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
import { nextTick, onMounted, reactive, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
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

const { t, locale } = useI18n();
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

const buildMaintainColumns = () => [
	{ field: 'ccDesc', title: t('message.hrms.org.ccDesc'), minWidth: 180, treeNode: true, align: 'left', headerAlign: 'center', showOverflow: 'tooltip', sortable: true },
	{ field: 'ccCode', title: t('message.hrms.org.ccCode'), minWidth: 110, showOverflow: 'tooltip', sortable: true },
	{ field: 'ccDept', title: t('message.hrms.org.ccDept'), minWidth: 100, showOverflow: 'tooltip', sortable: true },
	{ field: 'ccLevel', title: t('message.hrms.org.ccLevel'), minWidth: 110, showOverflow: 'tooltip', sortable: true },
	{ field: 'ccSite', title: t('message.hrms.org.ccSite'), minWidth: 90, showOverflow: 'tooltip', sortable: true },
	{ field: 'ccCost', title: t('message.hrms.org.ccCost'), minWidth: 90, showOverflow: 'tooltip', sortable: true },
	{ field: 'ccEnable', title: t('message.hrms.common.status'), width: 90, align: 'center', sortable: true, slots: { default: 'row_ccEnable' } },
	{ field: 'ccUser', title: t('message.hrms.org.ccUser'), minWidth: 90, showOverflow: 'tooltip', sortable: true },
	{ field: 'ccDate', title: t('message.hrms.org.ccDate'), minWidth: 160, showOverflow: 'tooltip', sortable: true },
	{ field: 'buttons', title: t('message.list.operation'), fixed: 'right', width: 160, showOverflow: true, slots: { default: 'row_buttons' } },
];

const buildDetailColumns = () => [
	{ field: 'seq', type: 'seq', title: '#', width: 55 },
	{ field: 'ccCode', title: t('message.hrms.org.code'), minWidth: 100, sortable: true },
	{ field: 'ccDesc', title: t('message.hrms.org.name'), minWidth: 140, sortable: true },
	{ field: 'ccDept', title: t('message.hrms.org.alias'), minWidth: 90, sortable: true },
	{ field: 'ccLevel', title: t('message.hrms.org.ccLevel'), minWidth: 110, sortable: true },
	{ field: 'ccCodeParent', title: t('message.hrms.org.parentCode'), minWidth: 100, sortable: true },
	{ field: 'ccSite', title: t('message.hrms.org.ccSite'), minWidth: 80, sortable: true },
	{ field: 'ccCost', title: t('message.hrms.org.cost'), minWidth: 80, sortable: true },
	{ field: 'ccWoca', title: t('message.hrms.org.ccWoca'), minWidth: 90, sortable: true },
	{ field: 'ccXzGroup', title: t('message.hrms.org.ccXzGroup'), minWidth: 90, sortable: true },
	{ field: 'ccEnable', title: t('message.hrms.org.stopped'), width: 70, align: 'center', sortable: true, slots: { default: 'row_ccEnable' } },
	{ field: 'ccDeleRemark', title: t('message.hrms.org.ccDeleRemark'), minWidth: 140, showOverflow: 'tooltip' },
];

const maintainOptions = useVxeTable<any>(
	{
		id: 'hrOrgMaintain',
		name: t('message.hrms.org.tabMaintain'),
		columns: buildMaintainColumns(),
	},
	{
		stripe: false,
		height: '100%',
		proxyConfig: {
			// 由左树 loaded 回填，避免与 OrgTree 并发打两次 GetList（切语言整页刷新时易超时）
			autoLoad: false,
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
		name: t('message.hrms.org.tabDetail'),
		columns: buildDetailColumns(),
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

/** 用已缓存的 fullTree 刷新右侧维护表，不再二次请求 */
const applyMaintainFromCache = async () => {
	await nextTick();
	if (state.activeTab !== 'maintain' || !maintainGrid.value) return;
	const scoped = filterTree(pickSubtree(state.fullTree, state.parentCcCode), state.keyword);
	maintainGrid.value.reloadData?.(scoped);
	await expandMaintainTree();
};

const handleTreeLoaded = async (tree: any[], meta: { includeDisabled: boolean; empState: number }) => {
	state.fullTree = tree ?? [];
	// 与左树筛选对齐，避免右侧再按另一套 includeDisabled 重查
	state.includeDisabled = !!meta?.includeDisabled;
	await applyMaintainFromCache();
};

const handleMaintainQueryApi = async (sort?: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const effectiveSort = resolveSort(sort, Local.get(localPageParamKey)?.defaultSort || state.localPageParam.defaultSort);
	// 已有缓存且非强制重查时，直接用内存树（排序对树形展示影响有限，以左树一次加载为准）
	if (state.fullTree.length) {
		const scoped = filterTree(pickSubtree(state.fullTree, state.parentCcCode), state.keyword);
		return { data: { result: scoped } };
	}
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

const handleQuery = async (forceRemote = false) => {
	if (state.activeTab !== 'maintain') return;
	await nextTick();
	if (!maintainGrid.value) {
		setTimeout(() => handleQuery(forceRemote), 50);
		return;
	}
	if (!forceRemote && state.fullTree.length) {
		await applyMaintainFromCache();
		return;
	}
	maintainOptions.loading = true;
	try {
		state.fullTree = [];
		await maintainGrid.value.commitProxy('reload');
		await expandMaintainTree();
	} finally {
		maintainOptions.loading = false;
	}
};

const resetQuery = async () => {
	state.keyword = '';
	state.includeDisabled = false;
	state.parentCcCode = '';
	await orgTreeRef.value?.fetchTreeData();
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
	// fetchTreeData → loaded 已回填维护表，勿再 commitProxy 打第二次 GetList
	await orgTreeRef.value?.fetchTreeData();
	if (state.activeTab === 'detail') await handleDetailQuery(true);
};

const handleNodeChange = async (node: any) => {
	state.parentCcCode = node?.ccCode ?? '';
	if (state.activeTab === 'maintain') await applyMaintainFromCache();
	if (state.activeTab === 'detail') await handleDetailQuery(true);
};

const handleAdd = () => {
	state.title = t('message.hrms.org.addTitle');
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
	state.title = t('message.hrms.org.editTitle');
	editOrgRef.value?.openDialog(row, true);
};

const handleDisable = (row: any) => {
	ElMessageBox.prompt(t('message.hrms.org.confirmDisable', { name: row.ccDesc }), t('message.hrms.org.disableTitle'), {
		confirmButtonText: t('message.list.confirmButtonText'),
		cancelButtonText: t('message.list.cancelButtonText'),
		inputPlaceholder: t('message.hrms.org.disableRemark'),
		type: 'warning',
	})
		.then(async ({ value }) => {
			await deleteHrOrg({ ccCode: row.ccCode, remark: value || undefined });
			ElMessage.success(t('message.hrms.common.disabledDone'));
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

watch(locale, () => {
	maintainOptions.name = t('message.hrms.org.tabMaintain');
	detailOptions.name = t('message.hrms.org.tabDetail');
	const mCols = buildMaintainColumns();
	const dCols = buildDetailColumns();
	maintainOptions.columns = mCols;
	detailOptions.columns = dCols;
	maintainGrid.value?.reloadColumn?.(mCols);
	detailGrid.value?.reloadColumn?.(dCols);
});

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
