<template>
	<div class="sys-pos-container">
		<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', height: '100%', alignItems: 'start' }">
			<el-form :model="state.queryParams" ref="queryForm" :show-message="false" :inlineMessage="true" label-width="auto" style="flex: 1 1 0%">
				<el-row :gutter="10">
					<el-col class="mb5" :xs="24" :sm="12" :md="8" :lg="6" :xl="6">
						<el-form-item :label="$t('message.list.jobTitle')">
							<el-input v-model="state.queryParams.name" :placeholder="$t('message.list.jobTitle')" clearable @keyup.enter.native="handleQuery(true)" />
						</el-form-item>
					</el-col>
					<el-col class="mb5" :xs="24" :sm="12" :md="8" :lg="6" :xl="6">
						<el-form-item :label="$t('message.list.positionCode')">
							<el-input v-model="state.queryParams.code" :placeholder="$t('message.list.positionCode')" clearable @keyup.enter.native="handleQuery(true)" />
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
					<el-button-group style="padding-left: 12px">
						<el-button plain icon="ele-Upload" @click="importData" v-auth="'sysPos/import'"> {{ $t('message.list.import') }} </el-button>
						<el-button plain icon="ele-Download" @click="exportData" v-auth="'sysPos/export'"> {{ $t('message.list.export') }} </el-button>
						<el-button plain icon="ele-FolderOpened" @click="downloadTemplate"> {{ $t('message.list.downloadTemplate') }} </el-button>
					</el-button-group>
				</template>
				<template #toolbar_tools> </template>
				<template #empty>
					<el-empty :image-size="200" />
				</template>
				<template #row_status="{ row }">
					<el-tag v-if="row.status === 1" type="success">{{ $t('message.list.enable') }}</el-tag>
					<el-tag v-else type="danger">{{ $t('message.list.disable') }}</el-tag>
				</template>
				<template #row_userCount="{ row }">
					{{ row.userList?.length }}
				</template>
				<template #row_userList="{ row }">
					<el-popover placement="bottom" width="280" trigger="hover" v-if="row.userList?.length">
						<template #reference>
							<el-text type="primary" class="cursor-default">
								<el-icon><ele-InfoFilled /></el-icon>人员明细
							</el-text>
						</template>
						<el-table :data="row.userList" stripe border>
							<el-table-column type="index" label="序号" width="65" align="center" />
							<el-table-column prop="account" label="账号" />
							<el-table-column prop="realName" label="姓名" />
						</el-table>
					</el-popover>
				</template>
				<template #row_record="{ row }">
					<ModifyRecord :data="row" />
				</template>
				<template #row_buttons="{ row }">
					<el-button icon="ele-Edit" text type="primary" v-auth="'sysPos/update'" @click="handleEdit(row)"> {{ $t('message.list.edit') }} </el-button>
					<el-button icon="ele-Delete" text type="danger" v-auth="'sysPos/delete'" @click="handleDelete(row)"> {{ $t('message.list.delete') }} </el-button>
					<el-button icon="ele-CopyDocument" text type="primary" v-auth="'sysPos/add'" @click="openCopyMenu(row)"> {{ $t('message.list.copy') }} </el-button>
				</template>
			</vxe-grid>
		</el-card>

		<el-dialog v-model="state.uploadVisible" :lock-scroll="false" draggable width="400px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-UploadFilled /> </el-icon>
					<span> {{ $t('message.list.import') }} </span>
				</div>
			</template>
			<div>
				<el-upload ref="uploadRef" drag :auto-upload="false" :limit="1" :file-list="state.fileList" action="" :on-change="handleChange" accept=".xlsx">
					<el-icon class="el-icon--upload">
						<ele-UploadFilled />
					</el-icon>
					<div class="el-upload__text">{{ $t('message.list.uploadFile') }}</div>
					<template #tip>
						<div class="el-upload__tip">{{ $t('message.list.pleaseUploadFile') }}</div>
					</template>
				</el-upload>
			</div>
			<template #footer>
				<span class="dialog-footer">
					<el-button icon="ele-CircleCloseFilled" @click="state.uploadVisible = false">{{ $t('message.list.cancelButtonText') }}</el-button>
					<el-button type="primary" icon="ele-CircleCheckFilled" @click="uploadFile">{{ $t('message.list.confirmButtonText') }}</el-button>
				</span>
			</template>
		</el-dialog>

		<EditPos ref="editPosRef" :title="state.title" @handleQuery="handleQuery" />
	</div>
</template>

<!-- 职位管理 -->
<script lang="ts" setup name="sysPos">
import { onMounted, reactive, ref } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { useI18n } from 'vue-i18n';
import { auth } from '/@/utils/authFunction';
import { downloadByData, getFileName } from '/@/utils/download';

import EditPos from '/@/views/system/pos/component/editPos.vue';
import ModifyRecord from '/@/components/table/modifyRecord.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysPosApi } from '/@/api-services/system/api';
import { PagePosOutput, PagePosInput, UpdatePosInput } from '/@/api-services/system/models';

const i18n = useI18n();
const xGrid = ref<VxeGridInstance>();
const editPosRef = ref<InstanceType<typeof EditPos>>();
const state = reactive({
	queryParams: {
		name: undefined,
		code: undefined,
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
const localPageParamKey = 'localPageParam:sysPos';
// 表格参数配置
const options = useVxeTable<PagePosOutput>(
	{
		id: 'sysPos',
		name: i18n.t('message.list.jobTitle'),
		columns: [
			// { type: 'checkbox', width: 40, fixed: 'left' },
			{ field: 'seq', type: 'seq', title: i18n.t('message.list.seq'), width: 60, fixed: 'left' },
			{ field: 'name', title: i18n.t('message.list.jobTitle'), minWidth: 200, showOverflow: 'tooltip' },
			{ field: 'code', title: i18n.t('message.list.positionCode'), minWidth: 200, showOverflow: 'tooltip' },
			{ field: 'type', title: i18n.t('message.list.positionType'), minWidth: 200, showOverflow: 'tooltip' },
			{ field: 'userCount', title: i18n.t('message.list.staffCount'), showOverflow: 'tooltip', slots: { default: 'row_userCount' } },
			{ field: 'userList', title: i18n.t('message.list.staffDetails'), showOverflow: 'tooltip', slots: { default: 'row_userList' } },
			{ field: 'orderNo', title: i18n.t('message.list.orderNo'), width: 80, showOverflow: 'tooltip' },
			{ field: 'status', title: i18n.t('message.list.status'), width: 80, showOverflow: 'tooltip', slots: { default: 'row_status' } },
			{ field: 'record', title: i18n.t('message.list.record'), width: 100, showOverflow: 'tooltip', slots: { default: 'row_record' } },
			{ field: 'buttons', title: i18n.t('message.list.operation'), fixed: 'right', width: 210, showOverflow: true, slots: { default: 'row_buttons' } },
		],
	},
	// vxeGrid配置参数(此处可覆写任何参数)，参考vxe-table官方文档
	{
		// 代理配置
		proxyConfig: { autoLoad: true, ajax: { query: ({ page, sort }) => handleQueryApi(page, sort), queryAll: () => handleQueryAllApi() } },
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
	const params = Object.assign(state.queryParams, { page: page.currentPage, pageSize: page.pageSize, field: sort.field, order: sort.order, descStr: 'desc' }) as PagePosInput;
	return getAPI(SysPosApi).apiSysPosPagePost(params);
};

// 查询所有api
const handleQueryAllApi = async () => {
	return getAPI(SysPosApi).apiSysPosListGet();
};

// 查询操作
const handleQuery = async (reset = false) => {
	options.loading = true;
	reset ? await xGrid.value?.commitProxy('reload') : await xGrid.value?.commitProxy('query');
	options.loading = false;
};

// 重置操作
const resetQuery = async () => {
	state.queryParams.code = undefined;
	state.queryParams.name = undefined;
	await xGrid.value?.commitProxy('reload');
};

// 打开新增页面
const handleAdd = () => {
	state.title = i18n.t('message.list.addPosition');
	editPosRef.value?.openDialog({ status: 1, orderNo: 100 });
};

// 打开编辑页面
const handleEdit = (row: any) => {
	state.title = i18n.t('message.list.editPosition');
	editPosRef.value?.openDialog(row);
};

// 打开复制页面
const openCopyMenu = (row: any) => {
	state.title = i18n.t('message.list.copyPosition');
	var copyRow = JSON.parse(JSON.stringify(row)) as UpdatePosInput;
	copyRow.id = 0;
	copyRow.name = '';
	editPosRef.value?.openDialog(copyRow);
};

// 删除操作
const handleDelete = (row: any) => {
	ElMessageBox.confirm(i18n.t('message.list.confirmDeletePosition', { name: row.name }), i18n.t('message.list.hint'), {
		confirmButtonText: i18n.t('message.list.confirmButtonText'),
		cancelButtonText: i18n.t('message.list.cancelButtonText'),
		type: 'warning',
	})
		.then(async () => {
			await getAPI(SysPosApi).apiSysPosDeletePost({ id: row.id });
			handleQuery();
			ElMessage.success(i18n.t('message.list.successDelete'));
		})
		.catch(() => {});
};

// 表格事件
const gridEvents: VxeGridListeners<PagePosOutput> = {
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
		if (auth('sysPos/update')) await handleEdit(row);
	},
};

// 通过onChanne方法获得文件列表
const handleChange = (file: any, fileList: []) => {
	state.fileList = fileList;
};

// 上传
const uploadFile = async () => {
	if (state.fileList.length < 1) return;

	state.uploadVisible = false;
	options.loading = true;
	await getAPI(SysPosApi).apiSysPosImportPostForm(state.fileList[0].raw);
	handleQuery();
	ElMessage.success('上传成功');
	state.fileList = [];
	options.loading = false;
};

// 导入
const importData = () => {
	state.uploadVisible = true;
};

// 导出
const exportData = async () => {
	options.loading = true;
	var res = await getAPI(SysPosApi).apiSysPosExportPost(state.queryParams, { responseType: 'blob' });
	options.loading = false;

	var fileName = getFileName(res.headers);
	downloadByData(res.data as any, fileName);
};

// 下载模板
const downloadTemplate = async () => {
	var res = await getAPI(SysPosApi).apiSysPosDownloadTemplatePost({ responseType: 'blob' });

	var fileName = getFileName(res.headers);
	downloadByData(res.data as any, fileName);
};
</script>
