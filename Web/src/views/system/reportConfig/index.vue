<template>
	<div>
		<splitpanes class="default-theme" style="height: 100%">
			<pane size="16" class="pane-left">
				<ReportGroupPanel @nodeClick="handleGroupNodeClick" />
			</pane>
			<pane size="84" class="pane-right">
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
								<el-button type="primary" icon="ele-Search" @click="handleQuery(true)" v-auth="'sysReportConfig/page'" :loading="options.loading"> {{ $t('查询') }} </el-button>
								<el-button icon="ele-Refresh" @click="resetQuery" :loading="options.loading"> {{ $t('重置') }} </el-button>
							</el-button-group>
						</el-col>
					</el-row>
				</el-card>

				<el-card class="full-table" shadow="hover" style="margin-top: 5px">
					<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options" v-on="gridEvents">
						<template #toolbar_buttons>
							<el-button type="primary" icon="ele-Plus" @click="handleAdd" v-auth="'sysReportConfig/add'"> {{ $t('新增') }} </el-button>
						</template>
						<template #toolbar_tools> </template>
						<template #empty>
							<el-empty :image-size="200" />
						</template>
						<template #row_dsType="{ row }">
							<g-sys-dict v-model="row.dsType" code="ReportConfigDsTypeEnum" />
						</template>
						<template #row_dataSource="{ row }">
							<span> {{ state.dataSourceList.find((r) => r.id == row.dataSource)?.name }}</span>
						</template>
						<template #row_record="{ row }">
							<ModifyRecord :data="row" />
						</template>
						<template #row_buttons="{ row }">
							<el-tooltip :content="$t('编辑')" placement="top">
								<el-button icon="ele-Edit" size="small" text type="primary" @click="handleEdit(row)" v-auth="'sysReportConfig/update'" :disabled="row.status === 1" />
							</el-tooltip>
							<el-tooltip :content="$t('删除')" placement="top">
								<el-button icon="ele-Delete" size="small" text type="danger" @click="handleDelete(row)" v-auth="'sysReportConfig/delete'" :disabled="row.status === 1" />
							</el-tooltip>
							<el-button icon="ele-CopyDocument" size="small" text type="primary" @click="copy(row)"> {{ $t('复制') }} </el-button>
							<el-button icon="ele-ScaleToOriginal" size="small" text type="primary" @click="preview(row)"> {{ $t('预览') }} </el-button>
							<el-button icon="ele-Position" size="small" text type="primary" @click="publicToMenu(row)"> {{ $t('发布到菜单') }} </el-button>
						</template>
					</vxe-grid>
				</el-card>
			</pane>
		</splitpanes>

		<EditReportConfig ref="editRef" :title="state.title" @handleQuery="handleQuery" />
		<EditMenu ref="editMenuRef" :title="state.title" :menuData="state.menuData" @handleQuery="handleEditMenuQuery" />
	</div>
</template>

<!-- 报表开发配置 -->
<script setup lang="ts" name="sysReportConfig">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';
import { auth } from '/@/utils/authFunction';

import ReportGroupPanel from './component/reportGroupPanel/index.vue';
import EditReportConfig from '/@/views/system/reportConfig/component/editReportConfig.vue';
import EditMenu from '/@/views/system/menu/component/editMenu.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysMenuApi, SysReportConfigApi, SysReportDataSourceApi } from '/@/api-services/system/api';
import { MenuTypeEnum, PageReportConfigInput, ReportConfigDsTypeEnum, ReportConfigOutput, ReportDataSourceOutput, StatusEnum, SysMenu, SysReportGroup } from '/@/api-services/system/models';

const { t } = useI18n();

const router = useRouter();

const xGrid = ref<VxeGridInstance>();
const editRef = ref<InstanceType<typeof EditReportConfig>>();
const editMenuRef = ref<InstanceType<typeof EditMenu>>();
const state = reactive({
	queryParams: {
		name: undefined,
	} as EmptyObjectType,
	localPageParam: {
		pageSize: 50 as number,
		defaultSort: { field: 'id', order: 'desc', descStr: 'desc' },
	},
	title: '',
	/** 已选择的分组Id */
	selectedGroupId: 0,
	/** 数据源列表数据 */
	dataSourceList: [] as ReportDataSourceOutput[],
	/** 菜单树数据 */
	menuData: [] as Array<SysMenu>,
});

// 本地存储参数
const localPageParamKey = 'localPageParam:sysReportConfig';
// 表格参数配置
const options = useVxeTable<ReportConfigOutput>(
	{
		id: 'sysReportConfig',
		name: '报表数据源',
		columns: [
			{ field: 'seq', type: 'seq', title: t('序号'), width: 60, fixed: 'left' },
			{ field: 'name', title: t('名称'), showOverflow: 'tooltip' },
			{ field: 'dsType', title: t('数据源类型'), showOverflow: 'tooltip', slots: { default: 'row_dsType' } },
			{ field: 'dataSource', title: t('数据源'), showOverflow: 'tooltip', slots: { default: 'row_dataSource' } },
			{ field: 'groupName', title: t('分组名称'), showOverflow: 'tooltip' },

			{ field: 'record', title: t('修改记录'), width: 100, showOverflow: 'tooltip', slots: { default: 'row_record' } },
			{ field: 'buttons', title: t('操作'), fixed: 'right', width: 320, showOverflow: true, slots: { default: 'row_buttons' } },
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
	getAPI(SysReportDataSourceApi)
		.apiSysReportDataSourceDataSourceListGet()
		.then((result) => {
			if (result.data.type !== 'success') {
				return;
			}

			state.dataSourceList = result.data.result!;
		});

	state.localPageParam = Local.get(localPageParamKey) || state.localPageParam;
});

// 查询api
const handleQueryApi = async (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	state.queryParams.groupId = state.selectedGroupId;
	const params = Object.assign(state.queryParams, { page: page.currentPage, pageSize: page.pageSize, field: sort.field, order: sort.order, descStr: 'desc' }) as PageReportConfigInput;
	return getAPI(SysReportConfigApi).apiSysReportConfigPagePost(params);
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
	state.queryParams.groupId = state.selectedGroupId;
	await xGrid.value?.commitProxy('reload');
};

// 打开新增页面
const handleAdd = () => {
	state.title = t('添加报表数据源');
	editRef.value?.openDialog({ dsType: ReportConfigDsTypeEnum.NUMBER_0 });
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
	}).then(async () => {
		await getAPI(SysReportConfigApi).apiSysReportConfigDeletePost({ id: row.id });
		handleQuery();
		ElMessage.success(t('删除成功'));
	});
};

// 表格事件
const gridEvents: VxeGridListeners<ReportConfigOutput> = {
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
		if (auth('sysReportConfig/update')) await handleEdit(row);
	},
};

const handleGroupNodeClick = (g: SysReportGroup) => {
	state.selectedGroupId = g.id!;
	handleQuery(true);
};

/** 复制 */
const copy = (row: any) => {
	ElMessageBox.confirm(t('确定复制？'), t('提示'), {
		confirmButtonText: t('确定'),
		cancelButtonText: t('取消'),
		type: 'warning',
	}).then(async () => {
		await getAPI(SysReportConfigApi).apiSysReportConfigCopyPost({ id: row.id });
		handleQuery();
		ElMessage.success(t('复制成功'));
	});
};

/** 预览 */
const preview = (row: any) => {
	router.push(`/report/view/${row.id}/${encodeURIComponent(row.name)}`);
};

/** 发布到菜单 */
const publicToMenu = async (row: ReportConfigOutput) => {
	if (state.menuData.length == 0) {
		await loadMenuList();
	}
	state.title = t('发布到菜单');
	editMenuRef.value?.openDialog({
		id: 0,
		type: MenuTypeEnum.NUMBER_2,
		name: '',
		title: row.name,
		icon: 'ele-Tickets',
		path: `/report/view/${row.id}/${row.name}`,
		orderNo: 100,
		isHide: false,
		isKeepAlive: true,
		isAffix: false,
		isIframe: false,
		status: StatusEnum.NUMBER_1,
	} as SysMenu);
};

/** 处理发布菜单后查询 */
const handleEditMenuQuery = () => {
	loadMenuList();
};

/** 获取菜单列表 */
const loadMenuList = () => {
	return getAPI(SysMenuApi)
		.apiSysMenuListGet()
		.then((res) => {
			state.menuData = res.data.result!;
			return res;
		});
};
</script>

<style scoped lang="scss">
.pane-left {
	display: flex;
	flex-direction: column;
}

.pane-right {
	display: flex;
	flex-direction: column;
}
</style>
