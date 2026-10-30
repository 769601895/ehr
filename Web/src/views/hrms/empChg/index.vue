<template>
	<div class="hr-emp-chg">
		<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', alignItems: 'start' }">
			<el-form :model="state.queryParams" :inline="true" style="flex: 1">
				<el-form-item label="关键字">
					<el-input v-model="state.queryParams.keyword" placeholder="工号/姓名" clearable @keyup.enter="handleQuery(true)" />
				</el-form-item>
				<el-form-item label="状态">
					<el-select v-model="state.queryParams.status" clearable placeholder="全部" style="width: 120px">
						<el-option v-for="s in statusOptions" :key="s.value" :label="s.label" :value="s.value" />
					</el-select>
				</el-form-item>
				<el-form-item label="类别">
					<el-select v-model="state.queryParams.chgType" clearable placeholder="全部" style="width: 140px">
						<el-option v-for="c in state.typeOptions" :key="c.value" :label="c.label || c.text" :value="c.value" />
					</el-select>
				</el-form-item>
				<el-form-item label="异动日期">
					<el-date-picker
						v-model="state.dateRange"
						type="daterange"
						value-format="YYYY-MM-DD"
						start-placeholder="起"
						end-placeholder="止"
						style="width: 240px"
					/>
				</el-form-item>
			</el-form>
			<el-button-group>
				<el-button type="primary" icon="ele-Search" v-auth="'hrEmpChg/page'" @click="handleQuery(true)">查询</el-button>
				<el-button icon="ele-Refresh" @click="resetQuery">重置</el-button>
			</el-button-group>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options" v-on="gridEvents">
				<template #toolbar_buttons>
					<el-button type="primary" icon="ele-Plus" v-auth="'hrEmpChg/saveDraft'" @click="openEdit()">新增</el-button>
				</template>
				<template #empty><el-empty :image-size="160" /></template>
				<template #row_status="{ row }">
					<el-tag size="small" :type="statusTag(row.status)">{{ row.statusText || row.status }}</el-tag>
				</template>
				<template #row_buttons="{ row }">
					<el-button icon="ele-View" text type="primary" v-auth="'hrEmpChg/detail'" @click="openEdit(row)" />
					<el-button
						v-if="row.status === 'Draft'"
						icon="ele-Delete"
						text
						type="danger"
						v-auth="'hrEmpChg/deleteDraft'"
						@click="handleDelete(row)"
					/>
				</template>
			</vxe-grid>
		</el-card>

		<EditChg ref="editRef" :type-options="state.typeOptions" @saved="handleQuery(false)" />
	</div>
</template>

<script lang="ts" setup name="hrEmpChg">
import { onMounted, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { getHrCodeList } from '/@/api/hrms/hrOrg';
import { deleteHrEmpChgDraft, pageHrEmpChg } from '/@/api/hrms/hrEmpChg';
import EditChg from './component/editChg.vue';

const xGrid = ref<VxeGridInstance>();
const editRef = ref<InstanceType<typeof EditChg>>();
const localPageParamKey = 'localPageParam:hrEmpChg';

const statusOptions = [
	{ value: 'Draft', label: '草稿' },
	{ value: 'InReview', label: '审核中' },
	{ value: 'Approved', label: '待生效' },
	{ value: 'Effective', label: '已生效' },
	{ value: 'Rejected', label: '已驳回' },
	{ value: 'Voided', label: '已作废' },
];

const state = reactive({
	queryParams: {
		keyword: '',
		status: undefined as string | undefined,
		chgType: undefined as string | undefined,
	},
	dateRange: null as string[] | null,
	localPageParam: {
		pageSize: 50 as number,
		defaultSort: { field: 'chgDate', order: 'desc', descStr: 'desc' },
	},
	typeOptions: [] as Array<{ value: string; label?: string; text?: string }>,
});

const statusTag = (s: string) => {
	const m: Record<string, string> = {
		Draft: 'info',
		InReview: 'warning',
		Approved: '',
		Effective: 'success',
		Rejected: 'danger',
		Voided: 'info',
	};
	return m[s] || 'info';
};

const options = useVxeTable(
	{
		id: 'hrEmpChg',
		name: '异动管理',
		columns: [
			{ field: 'seq', type: 'seq', title: '序号', width: 60 },
			{ field: 'chgNo', title: '异动单号', minWidth: 150, sortable: true },
			{ field: 'empId', title: '工号', minWidth: 100, sortable: true },
			{ field: 'empName', title: '姓名', minWidth: 100, sortable: true },
			{ field: 'chgDate', title: '异动日期', minWidth: 110, sortable: true },
			{ field: 'chgTypeText', title: '类别', minWidth: 100 },
			{ field: 'occText', title: '原部门', minWidth: 120 },
			{ field: 'nccText', title: '新部门', minWidth: 120 },
			{ field: 'status', title: '状态', width: 100, sortable: true, slots: { default: 'row_status' } },
			{ field: 'createUserName', title: '创建人', minWidth: 90 },
			{ field: 'buttons', title: '操作', width: 100, fixed: 'right', slots: { default: 'row_buttons' } },
		],
	},
	{
		proxyConfig: { autoLoad: true, ajax: { query: ({ page, sort }) => handleQueryApi(page, sort) } },
		sortConfig: { defaultSort: Local.get(localPageParamKey)?.defaultSort || state.localPageParam.defaultSort },
		pagerConfig: { pageSize: Local.get(localPageParamKey)?.pageSize || state.localPageParam.pageSize },
	}
);

const handleQueryApi = (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const effective = sort?.field ? sort : Local.get(localPageParamKey)?.defaultSort || state.localPageParam.defaultSort;
	const [begin, end] = state.dateRange || [];
	return pageHrEmpChg({
		...state.queryParams,
		chgDateBegin: begin,
		chgDateEnd: end,
		page: page.currentPage,
		pageSize: page.pageSize,
		field: effective.field,
		order: effective.order,
		descStr: 'desc',
	});
};

const gridEvents: VxeGridListeners = {
	sortChange({ field, order }) {
		state.localPageParam.defaultSort = { field, order: order || 'asc', descStr: 'desc' };
		Local.set(localPageParamKey, {
			pageSize: options.pagerConfig?.pageSize || state.localPageParam.pageSize,
			defaultSort: state.localPageParam.defaultSort,
		});
	},
	pageChange({ pageSize }) {
		state.localPageParam.pageSize = pageSize;
		Local.set(localPageParamKey, {
			pageSize,
			defaultSort: Local.get(localPageParamKey)?.defaultSort || state.localPageParam.defaultSort,
		});
	},
};

const handleQuery = async (reset = false) => {
	reset ? await xGrid.value?.commitProxy('reload') : await xGrid.value?.commitProxy('query');
};

const resetQuery = () => {
	state.queryParams.keyword = '';
	state.queryParams.status = undefined;
	state.queryParams.chgType = undefined;
	state.dateRange = null;
	handleQuery(true);
};

const openEdit = (row?: any) => {
	editRef.value?.open(row?.id);
};

const handleDelete = (row: any) => {
	ElMessageBox.confirm(`确定删除草稿【${row.chgNo}】？`, '提示', { type: 'warning' })
		.then(async () => {
			await deleteHrEmpChgDraft({ id: row.id });
			ElMessage.success('已删除');
			handleQuery(false);
		})
		.catch(() => undefined);
};

onMounted(async () => {
	state.localPageParam = Local.get(localPageParamKey) || state.localPageParam;
	try {
		const res = await getHrCodeList('EmpChgType');
		state.typeOptions = res.data?.result ?? res.data ?? [];
	} catch {
		state.typeOptions = [];
	}
});
</script>
