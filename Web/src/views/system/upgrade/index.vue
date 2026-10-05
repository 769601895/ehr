<template>
	<div class="sys-upgrade-container">
		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options" v-on="gridEvents">
				<template #toolbar_buttons>
					<el-button type="primary" icon="ele-Plus" @click="handleAdd"> 新增 </el-button>
				</template>
				<template #toolbar_tools> </template>
				<template #empty>
					<el-empty :image-size="200" />
				</template>
				<template #row_record="{ row }">
					<ModifyRecord :data="row" />
				</template>
				<template #row_buttons="{ row }">
					<el-button icon="ele-Edit" size="small" text type="primary" @click="handleEdit(row)" />
					<!-- <el-button icon="ele-Delete" size="small" text type="danger" @click="handleDelete(row)" /> -->
				</template>
			</vxe-grid>
		</el-card>

		<EditUpgrade ref="editUpgradeRef" :title="state.title" @handleQuery="handleQuery" />
	</div>
</template>

<!-- 更新日志 -->
<script lang="ts" setup name="sysUpgrade">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';

import EditUpgrade from './component/editUpgrade.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysUpgradeApi } from '/@/api-services/system/api';
import { SysUpgrade, PageUpgradeInput } from '/@/api-services/system/models';

const xGrid = ref<VxeGridInstance>();
const editUpgradeRef = ref<InstanceType<typeof EditUpgrade>>();
const state = reactive({
	queryParams: {
		title: undefined,
		type: undefined,
	},
	localPageParam: {
		pageSize: 50 as number,
		defaultSort: { field: 'orderNo', order: 'asc', descStr: 'desc' },
	},
	visible: false,
	title: '',
});

// 本地存储参数
const localPageParamKey = 'localPageParam:sysUpgrade';
// 表格参数配置
const options = useVxeTable<SysUpgrade>(
	{
		id: 'sysUpgrade',
		name: '更新日志',
		columns: [
			{ field: 'seq', type: 'seq', title: '序号', width: 60, fixed: 'left' },
			{ field: 'content', title: '系统更新日志', align: 'left', headerAlign: 'center', type: 'html', showOverflow: 'tooltip' },
			{ field: 'createTime', title: '创建时间', width: 160, showOverflow: 'tooltip' },
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
		// 行设置
		rowConfig: { useKey: true },
		cellConfig: { height: 120 },
		columnConfig: { useKey: true },
	}
);

// 页面初始化
onMounted(() => {
	state.localPageParam = Local.get(localPageParamKey) || state.localPageParam;
});

// 查询api
const handleQueryApi = async (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const params = Object.assign(state.queryParams, { page: page.currentPage, pageSize: page.pageSize, field: sort.field, order: sort.order, descStr: 'desc' }) as PageUpgradeInput;
	return getAPI(SysUpgradeApi).apiSysUpgradePagePost(params);
};

// 查询操作
const handleQuery = async (reset = false) => {
	options.loading = true;
	reset ? await xGrid.value?.commitProxy('reload') : await xGrid.value?.commitProxy('query');
	options.loading = false;
};

// 打开新增页面
const handleAdd = () => {
	state.title = '添加系统更新日志';
	editUpgradeRef.value?.openDialog({ type: 1 });
};

// 打开编辑页面
const handleEdit = (row: any) => {
	state.title = '编辑系统更新日志';
	editUpgradeRef.value?.openDialog(row);
};

// 删除
const handleDelete = (row: any) => {
	ElMessageBox.confirm(`确定删除系统更新日志：【${row.createTime}】?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysUpgradeApi).apiSysUpgradeDeletePost({ id: row.id });
			handleQuery();
			ElMessage.success('删除成功');
		})
		.catch(() => {});
};

// 表格事件
const gridEvents: VxeGridListeners<SysUpgrade> = {
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
		await handleEdit(row);
	},
};
</script>
