<template>
	<div class="hr-emp-reinstate">
		<el-alert
			type="info"
			:closable="false"
			show-icon
			class="mb8"
			title="复职：仅服务停职人员，同工号恢复在岗；停职单保留历史。请先在「员工停职处理」办理停职。离职后再聘请走再入职另案。"
		/>
		<el-tabs v-model="state.tab" class="hr-leave-tabs" @tab-change="onTabChange">
			<el-tab-pane label="复职办理" name="process" />
			<el-tab-pane label="复职记录" name="record" />
			<el-tab-pane label="作废失效" name="invalid" />
		</el-tabs>

		<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', alignItems: 'start' }">
			<el-form :model="state.queryParams" :inline="true" style="flex: 1">
				<el-form-item label="关键字">
					<el-input v-model="state.queryParams.keyword" placeholder="工号/姓名/单号" clearable @keyup.enter="handleQuery(true)" />
				</el-form-item>
				<el-form-item v-if="state.tab === 'process'" label="状态">
					<el-select v-model="state.queryParams.status" clearable placeholder="全部在途" style="width: 120px">
						<el-option v-for="s in processStatusOptions" :key="s.value" :label="s.label" :value="s.value" />
					</el-select>
				</el-form-item>
				<el-form-item v-else-if="state.tab === 'invalid'" label="状态">
					<el-select v-model="state.queryParams.status" clearable placeholder="全部失效" style="width: 120px">
						<el-option v-for="s in invalidStatusOptions" :key="s.value" :label="s.label" :value="s.value" />
					</el-select>
				</el-form-item>
				<el-form-item label="复职日">
					<el-date-picker v-model="state.dateRange" type="daterange" value-format="YYYY-MM-DD" start-placeholder="起" end-placeholder="止" style="width: 240px" />
				</el-form-item>
			</el-form>
			<el-button-group>
				<el-button type="primary" icon="ele-Search" v-auth="'hrEmpLeaveRestore/page'" @click="handleQuery(true)">查询</el-button>
				<el-button icon="ele-Refresh" @click="resetQuery">重置</el-button>
			</el-button-group>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<vxe-grid v-if="state.tab === 'process'" ref="xGridProcess" class="xGrid-style" v-bind="processOptions" v-on="processEvents">
				<template #toolbar_buttons>
					<el-button type="primary" icon="ele-Plus" v-auth="'hrEmpLeaveRestore/saveDraft'" @click="openEdit()">新增</el-button>
				</template>
				<template #empty><el-empty description="暂无在途复职单" :image-size="160" /></template>
				<template #row_status="{ row }">
					<el-tag size="small" :type="statusTag(row.status)">{{ row.statusText || row.status }}</el-tag>
				</template>
				<template #row_buttons="{ row }">
					<el-button icon="ele-View" text type="primary" v-auth="'hrEmpLeaveRestore/detail'" @click="openEdit(row)" />
					<el-button
						v-if="row.status === 'Draft'"
						icon="ele-Delete"
						text
						type="danger"
						v-auth="'hrEmpLeaveRestore/deleteDraft'"
						@click="handleDelete(row)"
					/>
				</template>
			</vxe-grid>
			<vxe-grid v-else-if="state.tab === 'record'" ref="xGridRecord" class="xGrid-style" v-bind="recordOptions" v-on="recordEvents">
				<template #empty><el-empty description="暂无已生效复职记录" :image-size="160" /></template>
				<template #row_status="{ row }">
					<el-tag size="small" type="success">{{ row.statusText || '已生效' }}</el-tag>
				</template>
				<template #row_buttons="{ row }">
					<el-button icon="ele-View" text type="primary" v-auth="'hrEmpLeaveRestore/detail'" @click="openEdit(row)" />
				</template>
			</vxe-grid>
			<vxe-grid v-else ref="xGridInvalid" class="xGrid-style" v-bind="invalidOptions" v-on="invalidEvents">
				<template #empty><el-empty description="暂无作废/驳回单据" :image-size="160" /></template>
				<template #row_status="{ row }">
					<el-tag size="small" :type="statusTag(row.status)">{{ row.statusText || row.status }}</el-tag>
				</template>
				<template #row_invalid_time="{ row }">{{ row.voidTime || row.approveTime || '' }}</template>
				<template #row_invalid_reason="{ row }">{{ row.voidReason || row.rejectReason || '' }}</template>
				<template #row_buttons="{ row }">
					<el-button icon="ele-View" text type="primary" v-auth="'hrEmpLeaveRestore/detail'" @click="openEdit(row)" />
				</template>
			</vxe-grid>
		</el-card>

		<EditRestore ref="editRef" @saved="handleQuery(false)" />
	</div>
</template>

<script lang="ts" setup name="hrEmpLeaveRestore">
import { nextTick, onMounted, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { deleteHrEmpLeaveRestoreDraft, pageHrEmpLeaveRestore } from '/@/api/hrms/hrEmpLeave';
import EditRestore from './component/editRestore.vue';

type Tab = 'process' | 'record' | 'invalid';

const xGridProcess = ref<VxeGridInstance>();
const xGridRecord = ref<VxeGridInstance>();
const xGridInvalid = ref<VxeGridInstance>();
const editRef = ref<InstanceType<typeof EditRestore>>();
const processKey = 'localPageParam:hrEmpReinstate';
const recordKey = 'localPageParam:hrEmpReinstateRecord';
const invalidKey = 'localPageParam:hrEmpReinstateInvalid';

const openStatuses = ['Draft', 'InReview', 'Approved'];
const invalidStatuses = ['Rejected', 'Voided'];

const processStatusOptions = [
	{ value: 'Draft', label: '草稿' },
	{ value: 'InReview', label: '审核中' },
	{ value: 'Approved', label: '待生效' },
];
const invalidStatusOptions = [
	{ value: 'Rejected', label: '已驳回' },
	{ value: 'Voided', label: '已作废' },
];

const state = reactive({
	tab: 'process' as Tab,
	queryParams: { keyword: '', status: undefined as string | undefined },
	dateRange: null as string[] | null,
	processSort: { pageSize: 50 as number, defaultSort: { field: 'restoreDate', order: 'desc', descStr: 'desc' } },
	recordSort: { pageSize: 50 as number, defaultSort: { field: 'effectiveTime', order: 'desc', descStr: 'desc' } },
	invalidSort: { pageSize: 50 as number, defaultSort: { field: 'voidTime', order: 'desc', descStr: 'desc' } },
});

const statusTag = (s: string) =>
	({ Draft: 'info', InReview: 'warning', Approved: '', Effective: 'success', Rejected: 'danger', Voided: 'info' } as any)[s] || 'info';

const baseColumns = [
	{ field: 'seq', type: 'seq', title: '序号', width: 60 },
	{ field: 'restoreNo', title: '复职单号', minWidth: 150, sortable: true },
	{ field: 'empId', title: '工号', minWidth: 100, sortable: true },
	{ field: 'empName', title: '姓名', minWidth: 100, sortable: true },
	{ field: 'restoreDate', title: '复职日', minWidth: 110, sortable: true },
	{ field: 'planEffectDate', title: '计划生效日', minWidth: 110, sortable: true },
	{ field: 'clearBlacklist', title: '清黑名单', width: 90, formatter: ({ cellValue }: any) => (cellValue ? '是' : '否') },
];

const buildPage = (
	page: VxeGridPropTypes.ProxyAjaxQueryPageParams,
	sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams,
	localKey: string,
	fallback: { field: string; order: string; descStr: string },
	extra: Record<string, any>
) => {
	const effective = sort?.field ? sort : Local.get(localKey)?.defaultSort || fallback;
	const [begin, end] = state.dateRange || [];
	return pageHrEmpLeaveRestore({
		keyword: state.queryParams.keyword || undefined,
		kind: 'Reinstate',
		restoreDateBegin: begin,
		restoreDateEnd: end,
		page: page.currentPage,
		pageSize: page.pageSize,
		field: effective.field,
		order: effective.order,
		descStr: 'desc',
		...extra,
	});
};

const processOptions = useVxeTable(
	{
		id: 'hrEmpReinstate',
		name: '复职办理',
		columns: [
			...baseColumns,
			{ field: 'status', title: '状态', width: 100, sortable: true, slots: { default: 'row_status' } },
			{ field: 'createUserName', title: '创建人', minWidth: 90 },
			{ field: 'buttons', title: '操作', width: 100, fixed: 'right', slots: { default: 'row_buttons' } },
		],
	},
	{
		proxyConfig: {
			autoLoad: true,
			ajax: {
				query: ({ page, sort }) => {
					const status = state.queryParams.status;
					return buildPage(page, sort, processKey, state.processSort.defaultSort, {
						status: status && openStatuses.includes(status) ? status : undefined,
						statusList: status && openStatuses.includes(status) ? undefined : openStatuses,
					});
				},
			},
		},
		sortConfig: { defaultSort: Local.get(processKey)?.defaultSort || state.processSort.defaultSort },
		pagerConfig: { pageSize: Local.get(processKey)?.pageSize || state.processSort.pageSize },
	}
);

const recordOptions = useVxeTable(
	{
		id: 'hrEmpReinstateRecord',
		name: '复职记录',
		columns: [
			...baseColumns,
			{ field: 'effectiveTime', title: '生效时间', minWidth: 150, sortable: true },
			{ field: 'status', title: '状态', width: 100, slots: { default: 'row_status' } },
			{ field: 'createUserName', title: '创建人', minWidth: 90 },
			{ field: 'buttons', title: '操作', width: 80, fixed: 'right', slots: { default: 'row_buttons' } },
		],
	},
	{
		proxyConfig: {
			autoLoad: false,
			ajax: { query: ({ page, sort }) => buildPage(page, sort, recordKey, state.recordSort.defaultSort, { status: 'Effective' }) },
		},
		sortConfig: { defaultSort: Local.get(recordKey)?.defaultSort || state.recordSort.defaultSort },
		pagerConfig: { pageSize: Local.get(recordKey)?.pageSize || state.recordSort.pageSize },
	}
);

const invalidOptions = useVxeTable(
	{
		id: 'hrEmpReinstateInvalid',
		name: '作废失效',
		columns: [
			...baseColumns,
			{ field: 'status', title: '状态', width: 100, sortable: true, slots: { default: 'row_status' } },
			{ field: 'voidTime', title: '失效时间', minWidth: 150, sortable: true, slots: { default: 'row_invalid_time' } },
			{ field: 'voidReason', title: '作废/驳回原因', minWidth: 160, slots: { default: 'row_invalid_reason' } },
			{ field: 'createUserName', title: '创建人', minWidth: 90 },
			{ field: 'buttons', title: '操作', width: 80, fixed: 'right', slots: { default: 'row_buttons' } },
		],
	},
	{
		proxyConfig: {
			autoLoad: false,
			ajax: {
				query: ({ page, sort }) => {
					const status = state.queryParams.status;
					return buildPage(page, sort, invalidKey, state.invalidSort.defaultSort, {
						status: status && invalidStatuses.includes(status) ? status : undefined,
						statusList: status && invalidStatuses.includes(status) ? undefined : invalidStatuses,
					});
				},
			},
		},
		sortConfig: { defaultSort: Local.get(invalidKey)?.defaultSort || state.invalidSort.defaultSort },
		pagerConfig: { pageSize: Local.get(invalidKey)?.pageSize || state.invalidSort.pageSize },
	}
);

const bindLocal = (
	localKey: string,
	sortState: { pageSize: number; defaultSort: { field: string; order: string; descStr: string } },
	options: { pagerConfig?: { pageSize?: number } }
): VxeGridListeners => ({
	sortChange({ field, order }) {
		sortState.defaultSort = { field, order: order || 'asc', descStr: 'desc' };
		Local.set(localKey, { pageSize: options.pagerConfig?.pageSize || sortState.pageSize, defaultSort: sortState.defaultSort });
	},
	pageChange({ pageSize }) {
		sortState.pageSize = pageSize;
		Local.set(localKey, { pageSize, defaultSort: Local.get(localKey)?.defaultSort || sortState.defaultSort });
	},
});

const processEvents = bindLocal(processKey, state.processSort, processOptions);
const recordEvents = bindLocal(recordKey, state.recordSort, recordOptions);
const invalidEvents = bindLocal(invalidKey, state.invalidSort, invalidOptions);

const activeGrid = () => {
	if (state.tab === 'process') return xGridProcess.value;
	if (state.tab === 'record') return xGridRecord.value;
	return xGridInvalid.value;
};

const handleQuery = async (reset = false) => {
	const grid = activeGrid();
	reset ? await grid?.commitProxy('reload') : await grid?.commitProxy('query');
};

const resetQuery = () => {
	state.queryParams.keyword = '';
	state.queryParams.status = undefined;
	state.dateRange = null;
	handleQuery(true);
};

const onTabChange = async () => {
	state.queryParams.status = undefined;
	await nextTick();
	handleQuery(true);
};

const openEdit = (row?: any) => editRef.value?.open(row?.id);
const handleDelete = (row: any) => {
	ElMessageBox.confirm(`确定删除草稿【${row.restoreNo}】？`, '提示', { type: 'warning' })
		.then(async () => {
			await deleteHrEmpLeaveRestoreDraft({ id: row.id });
			ElMessage.success('已删除');
			handleQuery(false);
		})
		.catch(() => undefined);
};

onMounted(() => {
	state.processSort = Local.get(processKey) || state.processSort;
	state.recordSort = Local.get(recordKey) || state.recordSort;
	state.invalidSort = Local.get(invalidKey) || state.invalidSort;
});
</script>

<style scoped>
.mb8 {
	margin: 8px 8px 0;
}
.hr-leave-tabs {
	padding: 0 8px;
}
</style>
