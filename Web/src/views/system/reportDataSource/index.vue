<template>
	<div>
		<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', height: '100%', alignItems: 'start' }">
			<el-form :model="state.queryParams" ref="queryForm" :show-message="false" :inlineMessage="true" label-width="auto" style="flex: 1 1 0%">
				<el-row :gutter="10">
					<el-col class="mb5" :xs="24" :sm="12" :md="8" :lg="6" :xl="6">
						<el-form-item :label="$t('名称')" prop="name">
							<el-input v-model="state.queryParams.name" clearable @keyup.enter.native="handleQuery(true)" />
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>

			<el-divider style="height: calc(100% - 5px); margin: 0 10px" direction="vertical" />

			<el-row>
				<el-col>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery(true)" v-auth="'sysReportDataSource/page'" :loading="options.loading"> {{ $t('查询') }} </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery" :loading="options.loading"> {{ $t('重置') }} </el-button>
					</el-button-group>
				</el-col>
			</el-row>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options" v-on="gridEvents">
				<template #toolbar_buttons>
					<el-button type="primary" icon="ele-Plus" @click="handleAdd" v-auth="'sysReportDataSource/add'"> {{ $t('新增') }} </el-button>
				</template>
				<template #toolbar_tools> </template>
				<template #empty>
					<el-empty :image-size="200" />
				</template>
				<template #row_dbType="{ row }">
					<g-sys-dict v-model="row.dbType" code="db_type" render-as="tag" />
				</template>
				<template #row_record="{ row }">
					<ModifyRecord :data="row" />
				</template>
				<template #row_buttons="{ row }">
					<el-tooltip :content="$t('编辑')" placement="top">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="handleEdit(row)" v-auth="'sysReportDataSource/update'" :disabled="row.status === 1" />
					</el-tooltip>
					<el-tooltip :content="$t('删除')" placement="top">
						<el-button icon="ele-Delete" size="small" text type="danger" @click="handleDelete(row)" v-auth="'sysReportDataSource/delete'" :disabled="row.status === 1" />
					</el-tooltip>
				</template>
			</vxe-grid>
		</el-card>

		<EditReportDataSource ref="editRef" :title="state.title" @handleQuery="handleQuery" />
	</div>
</template>

<!-- 报表开发数据源 -->
<script setup lang="ts" name="sysReportDataSource">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { useI18n } from 'vue-i18n';
import { auth } from '/@/utils/authFunction';

import EditReportDataSource from '/@/views/system/reportDataSource/component/editReportDataSource.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysReportDataSourceApi } from '/@/api-services/system/api';
import { PageReportDataSourceInput, SysReportDataSource } from '/@/api-services/system/models';

const { t } = useI18n();

const xGrid = ref<VxeGridInstance>();
const editRef = ref<InstanceType<typeof EditReportDataSource>>();
const state = reactive({
	queryParams: {
		name: undefined,
	},
	localPageParam: {
		pageSize: 50 as number,
		defaultSort: { field: 'id', order: 'asc', descStr: 'desc' },
	},
	title: '',
});

// 本地存储参数
const localPageParamKey = 'localPageParam:sysReportDataSource';
// 表格参数配置
const options = useVxeTable<SysReportDataSource>(
	{
		id: 'sysReportDataSource',
		name: '报表数据源',
		columns: [
			{ field: 'seq', type: 'seq', title: t('序号'), width: 60, fixed: 'left' },
			{ field: 'name', title: t('名称'), showOverflow: 'tooltip' },
			{ field: 'dbType', title: t('数据库类型'), showOverflow: 'tooltip', slots: { default: 'row_dbType' } },
			{ field: 'connectionString', title: t('连接字符串'), showOverflow: 'tooltip' },
			{ field: 'memo', title: t('备注'), showOverflow: 'tooltip' },

			{ field: 'record', title: t('修改记录'), width: 100, showOverflow: 'tooltip', slots: { default: 'row_record' } },
			{ field: 'buttons', title: t('操作'), fixed: 'right', width: 100, showOverflow: true, slots: { default: 'row_buttons' } },
		],
	},
	// vxeGrid配置参数(此处可覆写任何参数)，参考vxe-table官方文档
	{
		// 代理配置
		proxyConfig: { autoLoad: true, ajax: { query: ({ page, sort }) => handleQueryApi(page, sort) } },
		// 排序配置
		sortConfig: { defaultSort: Local.get(localPageParamKey)?.defaultSort || state.localPageParam.defaultSort },
		// 分页配置
		pagerConfig: { pageSize: Local.get(localPageParamKey)?.pageSize || state.localPageParam.pageSize },
		// 工具栏配置
		toolbarConfig: { export: false, print: false },
	}
);

// 页面初始化
onMounted(() => {
	state.localPageParam = Local.get(localPageParamKey) || state.localPageParam;
});

// 查询api
const handleQueryApi = async (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const params = Object.assign(state.queryParams, { page: page.currentPage, pageSize: page.pageSize, field: sort.field, order: sort.order, descStr: 'desc' }) as PageReportDataSourceInput;
	return getAPI(SysReportDataSourceApi).apiSysReportDataSourcePagePost(params);
};

// 查询操作
const handleQuery = async (reset = false) => {
	options.loading = true;
	reset ? await xGrid.value?.commitProxy('reload') : await xGrid.value?.commitProxy('query');
	options.loading = false;
};

// 重置操作
const resetQuery = async () => {
	state.queryParams = {} as any;
	await xGrid.value?.commitProxy('reload');
};

// 打开新增页面
const handleAdd = () => {
	state.title = t('添加报表数据源');
	editRef.value?.openDialog({});
};

// 打开编辑页面
const handleEdit = (row: any) => {
	state.title = t('编辑报表数据源');
	editRef.value?.openDialog(row);
};

// 删除
const handleDelete = (row: any) => {
	ElMessageBox.confirm(t('确定删除？'), t('提示'), {
		confirmButtonText: t('确定'),
		cancelButtonText: t('取消'),
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysReportDataSourceApi).apiSysReportDataSourceDeletePost({ id: row.id });
			handleQuery();
			ElMessage.success(t('删除成功'));
		})
		.catch(() => {});
};

// 表格事件
const gridEvents: VxeGridListeners<SysReportDataSource> = {
	// 只对 pager-config 配置时有效，分页发生改变时会触发该事件
	async pageChange({ pageSize }) {
		state.localPageParam.pageSize = pageSize;
		Local.set(localPageParamKey, state.localPageParam);
	},
	// 当排序条件发生变化时会触发该事件
	async sortChange({ field, order }) {
		state.localPageParam.defaultSort = { field: field, order: order!, descStr: 'desc' };
		Local.set(localPageParamKey, state.localPageParam);
	},
	// 双击行事件
	async cellDblclick({ row }) {
		if (auth('sysReportDataSource/update')) await handleEdit(row);
	},
};
</script>

<style scoped lang="scss"></style>
