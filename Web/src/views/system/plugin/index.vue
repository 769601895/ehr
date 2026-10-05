<template>
	<div class="sys-plugin-container">
		<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', height: '100%', alignItems: 'start' }">
			<el-form :model="state.queryParams" ref="queryForm" :show-message="false" :inlineMessage="true" label-width="auto" style="flex: 1 1 0%">
				<el-row :gutter="10">
					<el-col class="mb5" :xs="24" :sm="12" :md="8" :lg="6" :xl="6">
						<el-form-item label="功能名称" pro="name">
							<el-input v-model="state.queryParams.name" placeholder="功能名称" clearable @keyup.enter.native="handleQuery(true)" />
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>

			<el-divider style="height: calc(100% - 5px); margin: 0 10px" direction="vertical" />

			<el-row>
				<el-col>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery(true)" v-auth="'sysPlugin/page'" :loading="options.loading"> 查询 </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery" :loading="options.loading"> 重置 </el-button>
					</el-button-group>
				</el-col>
			</el-row>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options" v-on="gridEvents">
				<template #toolbar_buttons>
					<el-button type="primary" icon="ele-Plus" @click="handleAdd" v-auth="'sysPlugin/add'"> 新增 </el-button>
				</template>
				<template #toolbar_tools> </template>
				<template #empty>
					<el-empty :image-size="200" />
				</template>
				<template #row_status="{ row }">
					<el-tag v-if="row.status === 1" type="success">启用</el-tag>
					<el-tag v-else type="danger">禁用</el-tag>
				</template>
				<template #row_record="{ row }">
					<ModifyRecord :data="row" />
				</template>
				<template #row_buttons="{ row }">
					<el-tooltip content="编辑" placement="top">
						<el-button icon="ele-Edit" text type="primary" v-auth="'sysPlugin/update'" @click="handleEdit(row)"> </el-button>
					</el-tooltip>
					<el-tooltip content="删除" placement="top">
						<el-button icon="ele-Delete" text type="danger" v-auth="'sysPlugin/delete'" @click="handleDelete(row)"> </el-button>
					</el-tooltip>
				</template>
			</vxe-grid>
		</el-card>

		<EditPlugin ref="editPluginRef" :title="state.title" @handleQuery="handleQuery" />
	</div>
</template>

<!-- 动态插件 -->
<script lang="ts" setup name="sysPlugin">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { auth } from '/@/utils/authFunction';

import EditPlugin from '/@/views/system/plugin/component/editPlugin.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysPluginApi } from '/@/api-services/system/api';
import { SysPlugin, PagePluginInput } from '/@/api-services/system/models';

const xGrid = ref<VxeGridInstance>();
const editPluginRef = ref<InstanceType<typeof EditPlugin>>();
const state = reactive({
	queryParams: {
		name: undefined,
	},
	localPageParam: {
		pageSize: 50 as number,
		defaultSort: { field: 'orderNo', order: 'asc', descStr: 'desc' },
	},
	visible: false,
	title: '',
});

// 本地存储参数
const localPageParamKey = 'localPageParam:sysPlugin';
// 表格参数配置
const options = useVxeTable<SysPlugin>(
	{
		id: 'sysPlugin',
		name: '插件信息',
		columns: [
			// { type: 'checkbox', width: 40, fixed: 'left' },
			{ field: 'seq', type: 'seq', title: '序号', width: 60, fixed: 'left' },
			{ field: 'name', title: '功能名称', minWidth: 200, showOverflow: 'tooltip' },
			{ field: 'assemblyName', title: '动态程序集名称', showOverflow: 'tooltip' },
			{ field: 'status', title: '状态', showOverflow: 'tooltip', slots: { default: 'row_status' } },
			{ field: 'orderNo', title: '排序', showOverflow: 'tooltip' },
			{ field: 'remark', title: '备注', showOverflow: 'tooltip' },
			{ field: 'record', title: '修改记录', width: 100, showOverflow: 'tooltip', slots: { default: 'row_record' } },
			{ field: 'buttons', title: '操作', fixed: 'right', width: 100, showOverflow: true, slots: { default: 'row_buttons' } },
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
		toolbarConfig: { export: true },
	}
);

// 页面初始化
onMounted(() => {
	state.localPageParam = Local.get(localPageParamKey) || state.localPageParam;
});

// 查询api
const handleQueryApi = async (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const params = Object.assign(state.queryParams, { page: page.currentPage, pageSize: page.pageSize, field: sort.field, order: sort.order, descStr: 'desc' }) as PagePluginInput;
	return getAPI(SysPluginApi).apiSysPluginPagePost(params);
};

// 查询操作
const handleQuery = async (reset = false) => {
	options.loading = true;
	reset ? await xGrid.value?.commitProxy('reload') : await xGrid.value?.commitProxy('query');
	options.loading = false;
};

// 重置操作
const resetQuery = async () => {
	state.queryParams.name = undefined;
	await xGrid.value?.commitProxy('reload');
};

// 打开新增页面
const handleAdd = () => {
	state.title = '添加动态插件';
	editPluginRef.value?.openDialog({ status: 1, orderNo: 100 });
};

// 打开编辑页面
const handleEdit = (row: any) => {
	state.title = '编辑动态插件';
	editPluginRef.value?.openDialog(row);
};

// 删除
const handleDelete = (row: any) => {
	ElMessageBox.confirm(`确定删除动态插件：【${row.name}】?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysPluginApi).apiSysPluginDeletePost({ id: row.id });
			handleQuery();
			ElMessage.success('删除成功');
		})
		.catch(() => {});
};

// 表格事件
const gridEvents: VxeGridListeners<SysPlugin> = {
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
		if (auth('sysPlugin/update')) await handleEdit(row);
	},
};
</script>
