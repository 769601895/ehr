<template>
	<div class="sysOAuthUser-container">
		<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', height: '100%', alignItems: 'start' }">
			<el-form :model="state.queryParams" ref="queryForm" :show-message="false" :inlineMessage="true" label-width="auto" style="flex: 1 1 0%">
				<el-row :gutter="10">
					<el-col class="mb5" :xs="24" :sm="12" :md="8" :lg="6" :xl="6">
						<el-form-item label="微信昵称" prop="nickName">
							<el-input v-model="state.queryParams.nickName" placeholder="微信昵称" clearable @keyup.enter.native="handleQuery(true)" />
						</el-form-item>
					</el-col>
					<el-col class="mb5" :xs="24" :sm="12" :md="8" :lg="6" :xl="6">
						<el-form-item label="手机号码" prop="mobile">
							<el-input v-model="state.queryParams.mobile" placeholder="手机号码" clearable @keyup.enter.native="handleQuery(true)" />
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>

			<el-divider style="height: calc(100% - 5px); margin: 0 10px" direction="vertical" />

			<el-row>
				<el-col>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery(true)" v-auth="'sysOAuthUser/page'" :loading="options.loading"> 查询 </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery" :loading="options.loading"> 重置 </el-button>
					</el-button-group>
				</el-col>
			</el-row>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options" v-on="gridEvents">
				<template #toolbar_buttons> </template>
				<template #toolbar_tools> </template>
				<template #empty>
					<el-empty :image-size="200" />
				</template>
				<template #row_platformType="{ row }">
					<el-tag v-if="row.platformType === 1"> 微信公众号 </el-tag>
					<el-tag v-else-if="row.platformType === 2"> 微信小程序 </el-tag>
					<el-tag v-else-if="row.platformType === 3"> QQ </el-tag>
					<el-tag v-else-if="row.platformType === 4"> Gitee </el-tag>
					<el-tag v-else-if="row.platformType === 5"> Alipay </el-tag>
					<el-tag v-else> 未知 </el-tag>
				</template>
				<template #row_avatar="{ row }">
					<el-avatar :src="row.avatar" :size="24" style="vertical-align: middle" />
				</template>
				<template #row_sex="{ row }">
					<el-tag v-if="row.sex === 1">男</el-tag>
					<el-tag v-else-if="row.sex === 2" type="danger">女</el-tag>
				</template>
				<template #row_address="{ row }"> {{ row.country }} {{ row.province }} {{ row.city }} </template>
				<template #row_record="{ row }">
					<ModifyRecord :data="row" />
				</template>
				<template #row_buttons="{ row }">
					<el-tooltip content="编辑" placement="top">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="handleEdit(row)" v-auth="'sysOAuthUser/update'" :disabled="row.status === 1" />
					</el-tooltip>
					<el-tooltip content="删除" placement="top">
						<el-button icon="ele-Delete" size="small" text type="danger" @click="handleDelete(row)" v-auth="'sysOAuthUser/delete'" :disabled="row.status === 1" />
					</el-tooltip>
				</template>
			</vxe-grid>
		</el-card>

		<EditOAuthUser ref="editOAuthUserRef" :title="state.title" @handleQuery="handleQuery" />
	</div>
</template>

<!-- OAuth用户 -->
<script lang="ts" setup name="sysOAuthUser">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { auth } from '/@/utils/authFunction';

import EditOAuthUser from '/@/views/system/oAuthUser/component/editOAuthUser.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysOAuthUserApi } from '/@/api-services/system/api';
import { SysOAuthUser, OAuthUserInput } from '/@/api-services/system/models';

const xGrid = ref<VxeGridInstance>();
const editOAuthUserRef = ref<InstanceType<typeof EditOAuthUser>>();
const state = reactive({
	queryParams: {
		nickName: undefined,
		mobile: undefined,
	},
	localPageParam: {
		pageSize: 50 as number,
		defaultSort: { field: 'orderNo', order: 'asc', descStr: 'desc' },
	},
	title: '',
});

// 本地存储参数
const localPageParamKey = 'localPageParam:oAuthUser';
// 表格参数配置
const options = useVxeTable<SysOAuthUser>(
	{
		id: 'oAuthUser',
		name: '三方账号',
		columns: [
			// { type: 'checkbox', width: 40, fixed: 'left' },
			{ field: 'seq', type: 'seq', title: '序号', width: 60, fixed: 'left' },
			{ field: 'platformType', title: '平台类型', minWidth: 110, showOverflow: 'tooltip', slots: { default: 'row_platformType' } },
			{ field: 'nickName', title: '昵称', minWidth: 160, showOverflow: 'tooltip' },
			{ field: 'avatar', title: '头像', minWidth: 60, slots: { default: 'row_avatar' } },
			{ field: 'sex', title: '性别', minWidth: 60, showOverflow: 'tooltip', slots: { default: 'row_sex' } },
			{ field: 'mobile', title: '手机号码', minWidth: 150, showOverflow: 'tooltip' },
			{ field: 'address', title: '地址', minWidth: 200, showOverflow: 'tooltip', slots: { default: 'row_address' } },
			// { field: 'city', title: '城市', minWidth: 150, showOverflow: 'tooltip' },
			// { field: 'province', title: '省', minWidth: 120, showOverflow: 'tooltip' },
			// { field: 'country', title: '国家', minWidth: 150, showOverflow: 'tooltip' },
			{ field: 'openId', title: 'OpenId', showOverflow: 'tooltip' },
			{ field: 'unionId', title: 'UnionId', showOverflow: 'tooltip' },
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
	const params = Object.assign(state.queryParams, { page: page.currentPage, pageSize: page.pageSize, field: sort.field, order: sort.order, descStr: 'desc' }) as OAuthUserInput;
	return getAPI(SysOAuthUserApi).apiSysOAuthUserPagePost(params);
};

// 查询操作
const handleQuery = async (reset = false) => {
	options.loading = true;
	reset ? await xGrid.value?.commitProxy('reload') : await xGrid.value?.commitProxy('query');
	options.loading = false;
};

// 重置操作
const resetQuery = async () => {
	state.queryParams.nickName = undefined;
	state.queryParams.mobile = undefined;
	await xGrid.value?.commitProxy('reload');
};

// 打开编辑页面
const handleEdit = (row: any) => {
	state.title = '编辑三方账号';
	editOAuthUserRef.value?.openDialog(row);
};

// 删除
const handleDelete = (row: any) => {
	ElMessageBox.confirm(`确定删除三方账号：【${row.nickName}】?`, '提示', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysOAuthUserApi).apiSysOAuthUserDeletePost({ id: row.id });
			handleQuery();
			ElMessage.success('删除成功');
		})
		.catch(() => {});
};

// 表格事件
const gridEvents: VxeGridListeners<SysOAuthUser> = {
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
		if (auth('sysOAuthUser/update')) await handleEdit(row);
	},
};
</script>
