<template>
	<div class="hr-wf-todo">
		<el-card shadow="hover">
			<el-tabs v-model="state.tab" @tab-change="handleQuery(true)">
				<el-tab-pane label="我的待办" name="todo" />
				<el-tab-pane label="我的已办" name="done" />
				<el-tab-pane label="我发起的" name="applied" />
			</el-tabs>
			<el-form :inline="true" class="mb8">
				<el-form-item label="关键字"><el-input v-model="state.keyword" clearable @keyup.enter="handleQuery(true)" /></el-form-item>
				<el-button type="primary" icon="ele-Search" @click="handleQuery(true)">查询</el-button>
			</el-form>
			<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options" v-on="gridEvents">
				<template #empty><el-empty :image-size="160" /></template>
				<template #row_buttons="{ row }">
					<el-button type="primary" link @click="openDetail(row)">查看</el-button>
				</template>
			</vxe-grid>
		</el-card>

		<el-drawer v-model="state.detailVisible" title="审批详情" size="640px" destroy-on-close>
			<template v-if="state.detail">
				<p><b>标题：</b>{{ state.detail.title || '—' }}</p>
				<p><b>状态：</b>{{ state.detail.status }} · <b>节点：</b>{{ state.detail.currentNodeKey || '—' }}</p>
				<wf-form-renderer
					v-model="state.detailForm"
					:fields="state.detailFields"
					:readonly="!state.detail.canEditForm"
				/>
				<div v-if="state.detail.canApprove" class="actions">
					<el-input v-model="state.opinion" type="textarea" :rows="2" placeholder="审批意见（驳回必填）" class="mb8" />
					<el-button type="success" :loading="state.acting" @click="doApprove">同意</el-button>
					<el-button type="danger" :loading="state.acting" @click="doReject">驳回</el-button>
				</div>
			</template>
		</el-drawer>
	</div>
</template>

<script lang="ts" setup name="hrWfTodo">
import { onMounted, reactive, ref } from 'vue';
import { useRoute } from 'vue-router';
import { ElMessage, ElMessageBox } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { pageMyApplied, pageMyDone, pageMyTodo } from '/@/api/hrms/hrWfTodo';
import { approveHrWf, getHrWfDetail, rejectHrWf } from '/@/api/hrms/hrWfRuntime';
import WfFormRenderer from '/@/views/hrms/wf/components/wfFormRenderer.vue';

const route = useRoute();
const xGrid = ref<VxeGridInstance>();
const localPageParamKey = 'localPageParam:hrWfTodo';
const state = reactive({
	tab: (route.query.tab as string) || 'todo',
	keyword: '',
	localPageParam: { pageSize: 50 as number, defaultSort: { field: 'startedTime', order: 'desc', descStr: 'desc' } },
	detailVisible: false,
	detail: null as any,
	detailFields: [] as any[],
	detailForm: {} as Record<string, any>,
	opinion: '',
	acting: false,
});

const options = useVxeTable(
	{
		id: 'hrWfTodo',
		name: '审批待办',
		columns: [
			{ field: 'seq', type: 'seq', title: '序号', width: 60 },
			{ field: 'bizType', title: '业务类型', minWidth: 120, sortable: true },
			{ field: 'title', title: '标题', minWidth: 180, showOverflow: 'tooltip', sortable: true },
			{ field: 'status', title: '状态', width: 100, sortable: true },
			{ field: 'nodeKey', title: '节点', width: 100 },
			{ field: 'startedTime', title: '发起时间', minWidth: 160, sortable: true },
			{ field: 'buttons', title: '操作', width: 80, fixed: 'right', slots: { default: 'row_buttons' } },
		],
	},
	{
		proxyConfig: { autoLoad: true, ajax: { query: ({ page, sort }) => handleQueryApi(page, sort) } },
		sortConfig: { defaultSort: Local.get(localPageParamKey)?.defaultSort || state.localPageParam.defaultSort },
		pagerConfig: { pageSize: Local.get(localPageParamKey)?.pageSize || state.localPageParam.pageSize },
	}
);

const handleQueryApi = (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const effective = sort?.field ? sort : state.localPageParam.defaultSort;
	const params = {
		keyword: state.keyword,
		page: page.currentPage,
		pageSize: page.pageSize,
		field: effective.field,
		order: effective.order,
		descStr: 'desc',
	};
	if (state.tab === 'done') return pageMyDone(params);
	if (state.tab === 'applied') return pageMyApplied(params);
	return pageMyTodo(params);
};

const handleQuery = async (reset = false) => {
	reset ? await xGrid.value?.commitProxy('reload') : await xGrid.value?.commitProxy('query');
};

const openDetail = async (row: any) => {
	const res = await getHrWfDetail(row.instId);
	const d = res.data?.result ?? res.data;
	state.detail = d;
	state.detailFields = d.formSchema?.fields || [];
	state.detailForm = { ...(d.formData || {}) };
	state.opinion = '';
	state.detailVisible = true;
};

const doApprove = async () => {
	if (!state.detail?.myPendingTaskId) return;
	state.acting = true;
	try {
		await approveHrWf({
			taskId: state.detail.myPendingTaskId,
			opinion: state.opinion,
			formDataPatch: state.detail.canEditForm ? state.detailForm : undefined,
		});
		ElMessage.success('已同意');
		state.detailVisible = false;
		await handleQuery(true);
	} finally {
		state.acting = false;
	}
};

const doReject = async () => {
	if (!state.detail?.myPendingTaskId) return;
	if (!state.opinion?.trim()) {
		ElMessage.warning('驳回意见必填');
		return;
	}
	await ElMessageBox.confirm('确认驳回？', '提示', { type: 'warning' });
	state.acting = true;
	try {
		await rejectHrWf({
			taskId: state.detail.myPendingTaskId,
			opinion: state.opinion.trim(),
			formDataPatch: state.detail.canEditForm ? state.detailForm : undefined,
		});
		ElMessage.success('已驳回');
		state.detailVisible = false;
		await handleQuery(true);
	} finally {
		state.acting = false;
	}
};

const gridEvents: VxeGridListeners = {
	sortChange({ field, order }) {
		state.localPageParam.defaultSort = { field, order: order || 'asc', descStr: 'desc' };
		Local.set(localPageParamKey, { pageSize: state.localPageParam.pageSize, defaultSort: state.localPageParam.defaultSort });
	},
	pageChange({ pageSize }) {
		state.localPageParam.pageSize = pageSize;
		Local.set(localPageParamKey, { pageSize, defaultSort: state.localPageParam.defaultSort });
	},
};

onMounted(() => {
	const cached = Local.get(localPageParamKey);
	if (cached?.pageSize) state.localPageParam.pageSize = cached.pageSize;
	if (cached?.defaultSort) state.localPageParam.defaultSort = cached.defaultSort;
});
</script>

<style scoped>
.mb8 { margin-bottom: 8px; }
.actions { margin-top: 16px; }
</style>
