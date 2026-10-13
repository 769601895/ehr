<template>
	<div class="hr-emp-container">
		<splitpanes class="default-theme">
			<pane size="22" style="display: flex">
				<OrgTree ref="orgTreeRef" @node-click="handleNodeChange" />
			</pane>
			<pane size="78" class="hr-emp-right-pane">
				<div class="hr-emp-right">
					<el-card shadow="hover" :body-style="{ padding: '8px 10px', display: 'flex', width: '100%', alignItems: 'center', flexWrap: 'wrap', gap: '4px' }">
						<el-input v-model="state.queryParams.keyword" placeholder="工号/姓名/身份证/电话" clearable style="width: 220px" @keyup.enter="handleQuery(true)" />
						<el-input v-model="state.queryParams.empId" placeholder="工号" clearable style="width: 120px" @keyup.enter="handleQuery(true)" />
						<el-input v-model="state.queryParams.empName" placeholder="姓名" clearable style="width: 120px" @keyup.enter="handleQuery(true)" />
						<el-select v-model="state.queryParams.empState" clearable placeholder="人员状态" style="width: 110px" @change="handleQuery(true)">
							<el-option label="在职" :value="1" />
							<el-option label="停职" :value="2" />
							<el-option label="离职" :value="3" />
						</el-select>
						<el-button-group style="margin-left: 4px">
							<el-button type="primary" icon="ele-Search" :loading="options.loading" @click="handleQuery(true)">查询</el-button>
							<el-button icon="ele-Refresh" :loading="options.loading" @click="resetQuery">重置</el-button>
						</el-button-group>
						<el-button type="primary" icon="ele-Plus" style="margin-left: 8px" v-auth="'hrEmp/add'" @click="handleAdd">新增</el-button>
					</el-card>

					<el-card class="full-table" shadow="hover" style="margin-top: 5px; flex: 1">
						<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options" v-on="gridEvents">
							<template #empty>
								<el-empty :image-size="160" />
							</template>
							<template #row_empState="{ row }">
								<el-tag v-if="row.empState === 1" type="success">在职</el-tag>
								<el-tag v-else-if="row.empState === 2" type="warning">停职</el-tag>
								<el-tag v-else-if="row.empState === 3" type="info">离职</el-tag>
								<span v-else>{{ row.empState ?? '-' }}</span>
							</template>
							<template #row_buttons="{ row }">
								<el-button icon="ele-Edit" text type="primary" v-auth="'hrEmp/update'" @click="handleEdit(row)">编辑</el-button>
								<el-button icon="ele-Delete" text type="danger" v-auth="'hrEmp/delete'" :disabled="row.empState === 3" @click="handleLeave(row)">离职</el-button>
							</template>
						</vxe-grid>
					</el-card>
				</div>
			</pane>
		</splitpanes>

		<EditEmp ref="editEmpRef" :title="state.title" :tree-data="state.fullTree" @reload="handleQuery(false)" />
	</div>
</template>

<script lang="ts" setup name="hrEmp">
import { onMounted, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';

import OrgTree from '/@/views/hrms/org/component/orgTree.vue';
import EditEmp from '/@/views/hrms/emp/component/editEmp.vue';
import { deleteHrEmp, pageHrEmp } from '/@/api/hrms/hrEmp';
import { getHrOrgList } from '/@/api/hrms/hrOrg';
import { getHrUiLayoutEffective } from '/@/api/hrms/hrUiLayout';

const orgTreeRef = ref<InstanceType<typeof OrgTree>>();
const editEmpRef = ref<InstanceType<typeof EditEmp>>();
const xGrid = ref<VxeGridInstance>();

const localPageParamKey = 'localPageParam:hrEmp';

const state = reactive({
	title: '',
	parentCcCode: '' as string,
	fullTree: [] as any[],
	queryParams: {
		keyword: '',
		empId: '',
		empName: '',
		empState: 1 as number | undefined,
	},
	localPageParam: {
		pageSize: 50 as number,
		defaultSort: { field: 'empId', order: 'asc' as string, descStr: 'desc' },
	},
});

const resolveSort = (sort?: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams | null, fallback?: { field: string; order: string; descStr: string }) => {
	const fb = fallback || state.localPageParam.defaultSort;
	const effective = sort?.field && sort?.order ? sort : { field: fb.field, order: fb.order };
	return { field: effective.field!, order: String(effective.order), descStr: 'desc' as const };
};

const pickProxySort = (params: { sort?: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams; sorts?: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams[] }) =>
	params.sort?.field ? params.sort : params.sorts?.[0];

const defaultColumns = () => [
	{ field: 'seq', type: 'seq', title: '#', width: 55 },
	{ field: 'empId', title: '工号', minWidth: 100, sortable: true },
	{ field: 'empName', title: '姓名', minWidth: 100, sortable: true },
	{ field: 'ccDesc', title: '部门', minWidth: 140, showOverflow: 'tooltip' },
	{ field: 'empSex', title: '性别', width: 70, sortable: true },
	{ field: 'age', title: '年龄', width: 70 },
	{ field: 'empAddrter', title: '联系电话', minWidth: 120, showOverflow: 'tooltip', sortable: true },
	{ field: 'dutyId', title: '职务', minWidth: 90, sortable: true },
	{ field: 'postId', title: '岗位', minWidth: 90, sortable: true },
	{ field: 'empInDate', title: '公司入职日', minWidth: 120, sortable: true },
	{ field: 'empType', title: '人员类别', minWidth: 100, sortable: true },
	{ field: 'empSal', title: '薪酬类别', width: 90, sortable: true },
	{ field: 'empState', title: '状态', width: 80, align: 'center', sortable: true, slots: { default: 'row_empState' } },
	{ field: 'buttons', title: '操作', fixed: 'right', width: 140, showOverflow: true, slots: { default: 'row_buttons' } },
];

const buildColumnsFromLayout = (fields: any[]) => {
	const cols: any[] = [{ field: 'seq', type: 'seq', title: '#', width: 55 }];
	for (const f of fields.filter((x) => x.visible !== false)) {
		const col: any = {
			field: f.fieldKey,
			title: f.label,
			showOverflow: 'tooltip',
			sortable: f.sortable !== false && !['age', 'ccDesc'].includes(f.fieldKey),
		};
		if (f.widthPx) col.width = f.widthPx;
		else col.minWidth = 90;
		if (f.fieldKey === 'empState') {
			col.align = 'center';
			col.slots = { default: 'row_empState' };
		}
		cols.push(col);
	}
	cols.push({ field: 'buttons', title: '操作', fixed: 'right', width: 140, showOverflow: true, slots: { default: 'row_buttons' } });
	return cols;
};

const options = useVxeTable<any>(
	{
		id: 'hrEmp',
		name: '人员资料',
		columns: defaultColumns(),
	},
	{
		height: '100%',
		proxyConfig: {
			autoLoad: false,
			ajax: {
				query: (params) => handleQueryApi(params.page, pickProxySort(params)),
			},
		},
		sortConfig: {
			remote: true,
			defaultSort: Local.get(localPageParamKey)?.defaultSort || state.localPageParam.defaultSort,
		},
		pagerConfig: { pageSize: Local.get(localPageParamKey)?.pageSize || state.localPageParam.pageSize },
		toolbarConfig: { enabled: false },
		rowConfig: { keyField: 'empId' },
	}
);

const applyListLayout = async () => {
	try {
		const res = await getHrUiLayoutEffective('hrEmp', 'list');
		const fields = res.data.result?.fields ?? [];
		if (fields.length) {
			const cols = buildColumnsFromLayout(fields);
			options.columns = cols;
			xGrid.value?.reloadColumn?.(cols);
			const sortField = state.localPageParam.defaultSort.field;
			if (sortField && !fields.some((f: any) => f.visible !== false && f.fieldKey === sortField)) {
				state.localPageParam.defaultSort = { field: 'empId', order: 'asc', descStr: 'desc' };
				Local.set(localPageParamKey, state.localPageParam);
			}
		}
	} catch {
		/* 布局服务未就绪时沿用内置列 */
	}
};

const handleQueryApi = async (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const effectiveSort = resolveSort(sort, Local.get(localPageParamKey)?.defaultSort || state.localPageParam.defaultSort);
	return pageHrEmp({
		page: page.currentPage,
		pageSize: page.pageSize,
		field: effectiveSort.field,
		order: effectiveSort.order,
		descStr: effectiveSort.descStr,
		keyword: state.queryParams.keyword || undefined,
		empId: state.queryParams.empId || undefined,
		empName: state.queryParams.empName || undefined,
		empState: state.queryParams.empState,
		ccCode: state.parentCcCode || undefined,
	});
};

const handleQuery = async (resetPage = false) => {
	const $grid = xGrid.value;
	if (!$grid) return;
	if (resetPage) await $grid.commitProxy('reload');
	else await $grid.commitProxy('query');
};

const resetQuery = () => {
	state.queryParams.keyword = '';
	state.queryParams.empId = '';
	state.queryParams.empName = '';
	state.queryParams.empState = 1;
	handleQuery(true);
};

const handleNodeChange = (node: any) => {
	state.parentCcCode = node?.ccCode || '';
	handleQuery(true);
};

const loadTree = async () => {
	const res = await getHrOrgList({ includeDisabled: false, empState: 1 });
	state.fullTree = res.data.result ?? [];
};

const handleAdd = () => {
	state.title = '新增人员';
	editEmpRef.value?.openDialog({ ccCode: state.parentCcCode || undefined, empState: 1 }, false);
};

const handleEdit = async (row: any) => {
	state.title = '编辑人员';
	editEmpRef.value?.openDialog(row, true);
};

const handleLeave = (row: any) => {
	ElMessageBox.confirm(`确认将【${row.empName || row.empId}】设为离职？`, '提示', { type: 'warning' })
		.then(async () => {
			await deleteHrEmp({ empId: row.empId });
			ElMessage.success('已设为离职');
			handleQuery(false);
		})
		.catch(() => {});
};

const gridEvents: VxeGridListeners = {
	async sortChange({ field, order }) {
		if (!field || !order) return;
		state.localPageParam.defaultSort = { field, order, descStr: 'desc' };
		Local.set(localPageParamKey, state.localPageParam);
	},
	async pageChange({ pageSize }) {
		state.localPageParam.pageSize = pageSize;
		Local.set(localPageParamKey, state.localPageParam);
	},
};

onMounted(async () => {
	state.localPageParam = Local.get(localPageParamKey) || state.localPageParam;
	await Promise.all([loadTree(), applyListLayout()]);
	await handleQuery(true);
});
</script>

<style scoped>
.hr-emp-container {
	height: 100%;
}
.hr-emp-right-pane {
	display: flex;
}
.hr-emp-right {
	display: flex;
	flex-direction: column;
	width: 100%;
	height: 100%;
	padding: 0 0 0 5px;
	overflow: hidden;
}
.full-table {
	display: flex;
	flex-direction: column;
	min-height: 0;
}
.full-table :deep(.el-card__body) {
	flex: 1;
	min-height: 0;
	display: flex;
	flex-direction: column;
}
</style>
