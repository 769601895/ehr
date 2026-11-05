<template>
	<div class="cp-site-list">
		<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', alignItems: 'start' }">
			<el-form :model="queryParams" :inline="true" style="flex: 1">
				<el-form-item label="关键字">
					<el-input v-model="queryParams.keyword" placeholder="公司代码/名称" clearable @keyup.enter="reload(true)" />
				</el-form-item>
			</el-form>
			<el-button-group>
				<el-button type="primary" icon="ele-Search" @click="reload(true)">查询</el-button>
				<el-button icon="ele-Refresh" @click="reset">重置</el-button>
				<el-button type="danger" icon="ele-Delete" v-auth="'hrCompanyParam/delete'" :loading="deleting" @click="handleDelete">删除</el-button>
			</el-button-group>
		</el-card>
		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options" v-on="gridEvents">
				<template #empty><el-empty :image-size="120" /></template>
			</vxe-grid>
		</el-card>
	</div>
</template>

<script lang="ts" setup name="hrCompanyParamSiteList">
import { reactive, ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { deleteHrCompanyParam, pageHrCompanyParam } from '/@/api/hrms/hrCompanyParam';

const emit = defineEmits<{
	(e: 'select', row: any): void;
	(e: 'open-detail', row: any): void;
	(e: 'deleted', id: number): void;
}>();

const xGrid = ref<VxeGridInstance>();
const deleting = ref(false);
const localPageParamKey = 'localPageParam:hrCompanyParam';
const queryParams = reactive({ keyword: '' });
const localPageParam = reactive({
	pageSize: 50 as number,
	defaultSort: { field: 'siteCode', order: 'asc', descStr: 'desc' },
});

const options = useVxeTable(
	{
		id: 'hrCompanyParam',
		name: '公司参数设置',
		columns: [
			{ field: 'seq', type: 'seq', title: '序号', width: 60 },
			{ field: 'siteCode', title: '公司代码', minWidth: 120, sortable: true },
			{ field: 'siteName', title: '公司名称', minWidth: 160, sortable: true },
		],
	},
	{
		proxyConfig: { autoLoad: true, ajax: { query: ({ page, sort }) => handleQueryApi(page, sort) } },
		sortConfig: { defaultSort: Local.get(localPageParamKey)?.defaultSort || localPageParam.defaultSort },
		pagerConfig: { pageSize: Local.get(localPageParamKey)?.pageSize || localPageParam.pageSize },
		rowConfig: { isCurrent: true, isHover: true },
	}
);

const handleQueryApi = (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const effective = sort?.field ? sort : localPageParam.defaultSort;
	return pageHrCompanyParam({
		...queryParams,
		page: page.currentPage,
		pageSize: page.pageSize,
		field: effective.field,
		order: effective.order,
		descStr: 'desc',
	});
};

const gridEvents: VxeGridListeners = {
	currentChange({ row }) {
		if (row) emit('select', row);
	},
	cellDblclick({ row }) {
		if (row) emit('open-detail', row);
	},
	sortChange({ field, order }) {
		localPageParam.defaultSort = { field, order: order || 'asc', descStr: 'desc' };
		Local.set(localPageParamKey, { pageSize: localPageParam.pageSize, defaultSort: localPageParam.defaultSort });
	},
	pageChange({ pageSize }) {
		localPageParam.pageSize = pageSize;
		Local.set(localPageParamKey, { pageSize, defaultSort: localPageParam.defaultSort });
	},
};

const reload = async (reset = false) => {
	reset ? await xGrid.value?.commitProxy('reload') : await xGrid.value?.commitProxy('query');
};
const reset = () => {
	queryParams.keyword = '';
	reload(true);
};
const getCurrent = () => xGrid.value?.getCurrentRecord?.() ?? null;

const handleDelete = async () => {
	const row = getCurrent();
	if (!row?.id) {
		ElMessage.warning('请先选择要删除的公司参数');
		return;
	}
	await ElMessageBox.confirm(`确认删除公司参数「${row.siteCode} ${row.siteName}」？`, '提示', { type: 'warning' });
	deleting.value = true;
	try {
		await deleteHrCompanyParam({ id: row.id });
		ElMessage.success('删除成功');
		emit('deleted', row.id);
		await reload(true);
	} finally {
		deleting.value = false;
	}
};

defineExpose({ reload, getCurrent });
</script>

<style scoped>
.cp-site-list {
	height: 100%;
	display: flex;
	flex-direction: column;
	overflow: hidden;
	min-height: 0;
}
.cp-site-list > .el-card:first-child {
	flex-shrink: 0;
}
.full-table {
	flex: 1;
	min-height: 0;
	display: flex;
	flex-direction: column;
	margin-top: 5px;
}
.full-table :deep(.el-card__body) {
	flex: 1;
	min-height: 0;
	height: 100%;
	display: flex;
	flex-direction: column;
	padding: 5px;
	overflow: hidden;
}
.xGrid-style {
	flex: 1;
	min-height: 0;
	height: 100%;
}
</style>
