<template>
	<div class="hr-emp-leave">
		<el-tabs v-model="state.tab" class="hr-leave-tabs" @tab-change="onTabChange">
			<el-tab-pane label="离职办理" name="process" />
			<el-tab-pane label="离职记录" name="record" />
			<el-tab-pane label="作废失效" name="invalid" />
		</el-tabs>

		<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', alignItems: 'start' }">
			<el-form :model="state.queryParams" :inline="true" style="flex: 1">
				<el-form-item label="关键字">
					<el-input v-model="state.queryParams.keyword" placeholder="工号/姓名" clearable @keyup.enter="handleQuery(true)" />
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
				<el-form-item label="类型">
					<el-select v-model="state.queryParams.leaveType" clearable placeholder="全部" style="width: 140px">
						<el-option v-for="c in state.typeOptions" :key="c.value" :label="c.label || c.text" :value="c.value" />
					</el-select>
				</el-form-item>
				<el-form-item label="离职日">
					<el-date-picker v-model="state.dateRange" type="daterange" value-format="YYYY-MM-DD" start-placeholder="起" end-placeholder="止" style="width: 240px" />
				</el-form-item>
			</el-form>
			<el-button-group>
				<el-button type="primary" icon="ele-Search" v-auth="'hrEmpLeave/page'" @click="handleQuery(true)">查询</el-button>
				<el-button icon="ele-Refresh" @click="resetQuery">重置</el-button>
			</el-button-group>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<vxe-grid v-if="state.tab === 'process'" ref="xGridProcess" class="xGrid-style" v-bind="processOptions" v-on="processEvents">
				<template #toolbar_buttons>
					<el-button type="primary" icon="ele-Plus" v-auth="'hrEmpLeave/saveDraft'" @click="openEdit()">新增</el-button>
				</template>
				<template #empty><el-empty description="暂无在途离职单" :image-size="160" /></template>
				<template #row_status="{ row }">
					<el-tag size="small" :type="statusTag(row.status)">{{ row.statusText || row.status }}</el-tag>
				</template>
				<template #row_buttons="{ row }">
					<el-button icon="ele-View" text type="primary" v-auth="'hrEmpLeave/detail'" @click="openEdit(row)" />
					<el-button
						v-if="row.status === 'Draft'"
						icon="ele-Delete"
						text
						type="danger"
						v-auth="'hrEmpLeave/deleteDraft'"
						@click="handleDelete(row)"
					/>
				</template>
			</vxe-grid>
			<vxe-grid v-else-if="state.tab === 'record'" ref="xGridRecord" class="xGrid-style" v-bind="recordOptions" v-on="recordEvents">
				<template #empty><el-empty description="暂无已生效离职记录" :image-size="160" /></template>
				<template #row_status="{ row }">
					<el-tag size="small" type="success">{{ row.statusText || '已生效' }}</el-tag>
				</template>
				<template #row_buttons="{ row }">
					<el-button icon="ele-View" text type="primary" v-auth="'hrEmpLeave/detail'" @click="openEdit(row)" />
				</template>
			</vxe-grid>
			<vxe-grid v-else ref="xGridInvalid" class="xGrid-style" v-bind="invalidOptions" v-on="invalidEvents">
				<template #empty><el-empty description="暂无作废/驳回单据" :image-size="160" /></template>
				<template #row_status="{ row }">
					<el-tag size="small" :type="statusTag(row.status)">{{ row.statusText || row.status }}</el-tag>
				</template>
				<template #row_invalid_time="{ row }">
					{{ row.voidTime || row.approveTime || row.updateTime || '' }}
				</template>
				<template #row_invalid_reason="{ row }">
					{{ row.voidReason || row.rejectReason || '' }}
				</template>
				<template #row_buttons="{ row }">
					<el-button icon="ele-View" text type="primary" v-auth="'hrEmpLeave/detail'" @click="openEdit(row)" />
				</template>
			</vxe-grid>
		</el-card>

		<EditLeave
			ref="editRef"
			:type-options="state.typeOptions"
			:reason-options="state.reasonOptions"
			:settle-options="state.settleOptions"
			@saved="handleQuery(false)"
		/>
	</div>
</template>

<script lang="ts" setup name="hrEmpLeaveProcess">
import { nextTick, onMounted, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { getHrCodeList } from '/@/api/hrms/hrOrg';
import { deleteHrEmpLeaveDraft, pageHrEmpLeave } from '/@/api/hrms/hrEmpLeave';
import EditLeave from './component/editLeave.vue';

type LeaveTab = 'process' | 'record' | 'invalid';

const xGridProcess = ref<VxeGridInstance>();
const xGridRecord = ref<VxeGridInstance>();
const xGridInvalid = ref<VxeGridInstance>();
const editRef = ref<InstanceType<typeof EditLeave>>();
const processKey = 'localPageParam:hrEmpLeave';
const recordKey = 'localPageParam:hrEmpLeaveRecord';
const invalidKey = 'localPageParam:hrEmpLeaveInvalid';

/** 在途：草稿 / 审核中 / 待生效 */
const openStatuses = ['Draft', 'InReview', 'Approved'];
/** 作废失效：已驳回 / 已作废（含撤销） */
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
	tab: 'process' as LeaveTab,
	queryParams: {
		keyword: '',
		status: undefined as string | undefined,
		leaveType: undefined as string | undefined,
	},
	dateRange: null as string[] | null,
	processSort: {
		pageSize: 50 as number,
		defaultSort: { field: 'leaveDate', order: 'desc', descStr: 'desc' },
	},
	recordSort: {
		pageSize: 50 as number,
		defaultSort: { field: 'effectiveTime', order: 'desc', descStr: 'desc' },
	},
	invalidSort: {
		pageSize: 50 as number,
		defaultSort: { field: 'voidTime', order: 'desc', descStr: 'desc' },
	},
	typeOptions: [] as Array<{ value: string; label?: string; text?: string }>,
	reasonOptions: [] as Array<{ value: string; label?: string; text?: string }>,
	settleOptions: [] as Array<{ value: string; label?: string; text?: string }>,
});

const statusTag = (s: string) =>
	({ Draft: 'info', InReview: 'warning', Approved: '', Effective: 'success', Rejected: 'danger', Voided: 'info' } as any)[s] || 'info';

const baseColumns = [
	{ field: 'seq', type: 'seq', title: '序号', width: 60 },
	{ field: 'leaveNo', title: '离职单号', minWidth: 150, sortable: true },
	{ field: 'empId', title: '工号', minWidth: 100, sortable: true },
	{ field: 'empName', title: '姓名', minWidth: 100, sortable: true },
	{ field: 'leaveDate', title: '离职日', minWidth: 110, sortable: true },
	{ field: 'planEffectDate', title: '计划生效日', minWidth: 110, sortable: true },
	{ field: 'leaveTypeText', title: '类型', minWidth: 100 },
	{ field: 'leaveReasonText', title: '原因', minWidth: 100 },
	{ field: 'salarySettleText', title: '薪资结算', minWidth: 120 },
];

const buildPageParams = (
	page: VxeGridPropTypes.ProxyAjaxQueryPageParams,
	sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams,
	localKey: string,
	fallback: { field: string; order: string; descStr: string },
	extra: Record<string, any>
) => {
	const effective = sort?.field ? sort : Local.get(localKey)?.defaultSort || fallback;
	const [begin, end] = state.dateRange || [];
	return pageHrEmpLeave({
		keyword: state.queryParams.keyword || undefined,
		leaveType: state.queryParams.leaveType,
		leaveDateBegin: begin,
		leaveDateEnd: end,
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
		id: 'hrEmpLeave',
		name: '员工离职处理',
		columns: [
			...baseColumns,
			{ field: 'status', title: '状态', width: 100, sortable: true, slots: { default: 'row_status' } },
			{ field: 'createUserName', title: '创建人', minWidth: 90 },
			{ field: 'buttons', title: '操作', width: 100, fixed: 'right', slots: { default: 'row_buttons' } },
		],
	},
	{
		proxyConfig: { autoLoad: true, ajax: { query: ({ page, sort }) => queryProcess(page, sort) } },
		sortConfig: { defaultSort: Local.get(processKey)?.defaultSort || state.processSort.defaultSort },
		pagerConfig: { pageSize: Local.get(processKey)?.pageSize || state.processSort.pageSize },
	}
);

const recordOptions = useVxeTable(
	{
		id: 'hrEmpLeaveRecord',
		name: '离职记录',
		columns: [
			...baseColumns,
			{ field: 'effectiveTime', title: '生效时间', minWidth: 150, sortable: true },
			{ field: 'status', title: '状态', width: 100, slots: { default: 'row_status' } },
			{ field: 'createUserName', title: '创建人', minWidth: 90 },
			{ field: 'buttons', title: '操作', width: 80, fixed: 'right', slots: { default: 'row_buttons' } },
		],
	},
	{
		proxyConfig: { autoLoad: false, ajax: { query: ({ page, sort }) => queryRecord(page, sort) } },
		sortConfig: { defaultSort: Local.get(recordKey)?.defaultSort || state.recordSort.defaultSort },
		pagerConfig: { pageSize: Local.get(recordKey)?.pageSize || state.recordSort.pageSize },
	}
);

const invalidOptions = useVxeTable(
	{
		id: 'hrEmpLeaveInvalid',
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
		proxyConfig: { autoLoad: false, ajax: { query: ({ page, sort }) => queryInvalid(page, sort) } },
		sortConfig: { defaultSort: Local.get(invalidKey)?.defaultSort || state.invalidSort.defaultSort },
		pagerConfig: { pageSize: Local.get(invalidKey)?.pageSize || state.invalidSort.pageSize },
	}
);

/** 离职办理：仅在途 */
const queryProcess = (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const status = state.queryParams.status;
	return buildPageParams(page, sort, processKey, state.processSort.defaultSort, {
		status: status && openStatuses.includes(status) ? status : undefined,
		statusList: status && openStatuses.includes(status) ? undefined : openStatuses,
	});
};

/** 离职记录：仅已生效 */
const queryRecord = (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) =>
	buildPageParams(page, sort, recordKey, state.recordSort.defaultSort, { status: 'Effective' });

/** 作废失效：驳回 / 作废 */
const queryInvalid = (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const status = state.queryParams.status;
	return buildPageParams(page, sort, invalidKey, state.invalidSort.defaultSort, {
		status: status && invalidStatuses.includes(status) ? status : undefined,
		statusList: status && invalidStatuses.includes(status) ? undefined : invalidStatuses,
	});
};

const bindLocalEvents = (
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

const processEvents = bindLocalEvents(processKey, state.processSort, processOptions);
const recordEvents = bindLocalEvents(recordKey, state.recordSort, recordOptions);
const invalidEvents = bindLocalEvents(invalidKey, state.invalidSort, invalidOptions);

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
	state.queryParams.leaveType = undefined;
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
	ElMessageBox.confirm(`确定删除草稿【${row.leaveNo}】？`, '提示', { type: 'warning' })
		.then(async () => {
			await deleteHrEmpLeaveDraft({ id: row.id });
			ElMessage.success('已删除');
			handleQuery(false);
		})
		.catch(() => undefined);
};

onMounted(async () => {
	state.processSort = Local.get(processKey) || state.processSort;
	state.recordSort = Local.get(recordKey) || state.recordSort;
	state.invalidSort = Local.get(invalidKey) || state.invalidSort;
	const load = async (field: string) => {
		try {
			return (await getHrCodeList(field)).data?.result ?? [];
		} catch {
			return [];
		}
	};
	state.typeOptions = await load('LeaveType');
	state.reasonOptions = await load('LeaveReason');
	state.settleOptions = await load('LeaveSalarySettle');
});
</script>

<style scoped>
.hr-leave-tabs {
	padding: 0 8px;
}
</style>
