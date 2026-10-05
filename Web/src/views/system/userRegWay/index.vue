<template>
	<div class="sysUserRegWay-container">
		<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', height: '100%', alignItems: 'start' }">
			<el-form :model="state.queryParams" ref="queryForm" :show-message="false" :inlineMessage="true" label-width="auto" style="flex: 1 1 0%">
				<el-row :gutter="10">
					<el-col class="mb5" :xs="24" :sm="12" :md="8" :lg="6" :xl="6">
						<el-form-item label="名称">
							<el-input v-model="state.queryParams.name" clearable />
						</el-form-item>
					</el-col>
					<el-col class="mb5" :xs="24" :sm="12" :md="8" :lg="6" :xl="6">
						<el-form-item label="租户">
							<TenantSelect v-model="state.queryParams.tenantId" clearable />
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>

			<el-divider style="height: calc(100% - 5px); margin: 0 10px" direction="vertical" />

			<el-row>
				<el-col>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery(true)" v-auth="'sysPos/page'"> {{ $t('message.list.query') }} </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> {{ $t('message.list.reset') }} </el-button>
					</el-button-group>
				</el-col>
			</el-row>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options" v-on="gridEvents">
				<template #toolbar_buttons>
					<el-button type="primary" icon="ele-Plus" @click="handleAdd" v-auth="'sysPos/add'"> {{ $t('message.list.add') }} </el-button>
				</template>
				<template #toolbar_tools> </template>
				<template #empty>
					<el-empty :image-size="200" />
				</template>
				<template #row_record="{ row }">
					<ModifyRecord :data="row" />
				</template>
				<template #row_buttons="{ row }">
					<el-button icon="ele-Edit" text type="primary" v-auth="'sysPos/update'" @click="handleEdit(row)"> {{ $t('message.list.edit') }} </el-button>
					<el-button icon="ele-Delete" text type="danger" v-auth="'sysPos/delete'" @click="handleDelete(row)"> {{ $t('message.list.delete') }} </el-button>
				</template>
			</vxe-grid>
		</el-card>

		<EditUserRegWay ref="editUserRegWayRef" :title="state.title" @handleQuery="handleQuery" />
	</div>
</template>

<!-- 用户注册方案 -->
<script lang="ts" setup name="sysUserRegWay">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { useI18n } from 'vue-i18n';
import { auth } from '/@/utils/authFunction';

import TenantSelect from '/@/views/system/tenant/component/tenantSelect.vue';
import EditUserRegWay from '/@/views/system/userRegWay/component/editUserRegWay.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysUserRegWayApi } from '/@/api-services/system/api';
import { PageUserRegWayOutput, PageUserRegWayInput } from '/@/api-services/system/models';

const i18n = useI18n();
const xGrid = ref<VxeGridInstance>();
const editUserRegWayRef = ref<InstanceType<typeof EditUserRegWay>>();
const state = reactive({
	queryParams: {
		name: undefined,
		tenantId: undefined,
	},
	localPageParam: {
		pageSize: 50 as number,
		defaultSort: { field: 'orderNo', order: 'asc', descStr: 'desc' },
	},
	visible: false,
	title: '',
	uploadVisible: false,
	fileList: [] as any,
});

// 本地存储参数
const localPageParamKey = 'localPageParam:sysUserRegWay';
// 表格参数配置
const options = useVxeTable<PageUserRegWayOutput>(
	{
		id: 'sysUserRegWay',
		columns: [
			{ field: 'seq', type: 'seq', title: '序号', width: 60, fixed: 'left' },
			{ field: 'name', title: '名称', minWidth: 200, showOverflow: 'tooltip' },
			{ field: 'tenantName', title: '租户', showOverflow: 'tooltip' },
			{ field: 'orgName', title: '默认机构', showOverflow: 'tooltip' },
			{ field: 'roleName', title: '默认角色', showOverflow: 'tooltip' },
			{ field: 'posName', title: '默认职位', showOverflow: 'tooltip' },
			{ field: 'orderNo', title: '排序', width: 80, showOverflow: 'tooltip' },
			{ field: 'record', title: i18n.t('message.list.record'), width: 100, showOverflow: 'tooltip', slots: { default: 'row_record' } },
			{ field: 'buttons', title: i18n.t('message.list.operation'), fixed: 'right', width: 150, showOverflow: true, slots: { default: 'row_buttons' } },
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

// 查询分页api
const handleQueryApi = async (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const params = Object.assign(state.queryParams, { page: page.currentPage, pageSize: page.pageSize, field: sort.field, order: sort.order, descStr: 'desc' }) as PageUserRegWayInput;
	return getAPI(SysUserRegWayApi).apiSysUserRegWayPagePost(params);
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
	state.queryParams.tenantId = undefined;
	await xGrid.value?.commitProxy('reload');
};

// 打开新增页面
const handleAdd = () => {
	state.title = '增加注册方案';
	editUserRegWayRef.value?.openDialog({ status: 1, orderNo: 100 });
};

// 打开编辑页面
const handleEdit = (row: any) => {
	state.title = '编辑注册方案';
	editUserRegWayRef.value?.openDialog(row);
};

// 删除操作
const handleDelete = (row: any) => {
	ElMessageBox.confirm(i18n.t('message.list.confirmDelete', { name: row.name }), i18n.t('message.list.hint'), {
		confirmButtonText: i18n.t('message.list.confirmButtonText'),
		cancelButtonText: i18n.t('message.list.cancelButtonText'),
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysUserRegWayApi).apiSysUserRegWayDeletePost({ id: row.id });
			handleQuery();
			ElMessage.success(i18n.t('message.list.successDelete'));
		})
		.catch(() => {});
};

// 表格事件
const gridEvents: VxeGridListeners<PageUserRegWayOutput> = {
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
		if (auth('sysUserRegWay/update')) handleEdit(row);
	},
};
</script>
