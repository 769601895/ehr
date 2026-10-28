<template>
	<div class="hr-wf-level">
		<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', alignItems: 'start' }">
			<el-form :model="state.queryParams" :inline="true" label-width="auto" style="flex: 1">
				<el-form-item label="关键字">
					<el-input v-model="state.queryParams.keyword" placeholder="编码/名称" clearable @keyup.enter="handleQuery(true)" />
				</el-form-item>
				<el-form-item label="启用">
					<el-select v-model="state.queryParams.enabled" clearable placeholder="全部" style="width: 110px">
						<el-option label="启用" :value="true" />
						<el-option label="停用" :value="false" />
					</el-select>
				</el-form-item>
			</el-form>
			<el-button-group>
				<el-button type="primary" icon="ele-Search" @click="handleQuery(true)">查询</el-button>
				<el-button icon="ele-Refresh" @click="resetQuery">重置</el-button>
			</el-button-group>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options" v-on="gridEvents">
				<template #toolbar_buttons>
					<el-button type="primary" icon="ele-Plus" @click="openEdit()">新增</el-button>
				</template>
				<template #empty><el-empty :image-size="160" /></template>
				<template #row_enabled="{ row }">
					<el-tag :type="row.enabled ? 'success' : 'info'" size="small">{{ row.enabled ? '启用' : '停用' }}</el-tag>
				</template>
				<template #row_functional="{ row }">
					<el-tag :type="row.isFunctional ? 'warning' : 'info'" size="small">{{ row.isFunctional ? '是' : '否' }}</el-tag>
				</template>
				<template #row_buttons="{ row }">
					<el-button icon="ele-Edit" text type="primary" @click="openEdit(row)" />
					<el-button icon="ele-Delete" text type="danger" @click="handleDelete(row)" />
				</template>
			</vxe-grid>
		</el-card>

		<el-dialog v-model="state.editVisible" :title="state.edit.id ? '编辑审核等级' : '新增审核等级'" width="520px" destroy-on-close>
			<el-form :model="state.edit" label-width="100px">
				<el-form-item label="编码" required><el-input v-model="state.edit.code" maxlength="50" /></el-form-item>
				<el-form-item label="名称" required><el-input v-model="state.edit.name" maxlength="100" /></el-form-item>
				<el-form-item label="审核顺序"><el-input-number v-model="state.edit.auditOrder" :min="0" :max="999" /></el-form-item>
				<el-form-item label="职能审核"><el-switch v-model="state.edit.isFunctional" /></el-form-item>
				<el-form-item label="排序"><el-input-number v-model="state.edit.orderNo" :min="0" /></el-form-item>
				<el-form-item label="启用"><el-switch v-model="state.edit.enabled" /></el-form-item>
				<el-form-item label="备注"><el-input v-model="state.edit.remark" type="textarea" maxlength="256" /></el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="state.editVisible = false">取消</el-button>
				<el-button type="primary" :loading="state.saving" @click="saveEdit">保存</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="hrWfLevel">
import { onMounted, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { deleteHrWfLevel, pageHrWfLevel, saveHrWfLevel } from '/@/api/hrms/hrWfLevel';

const xGrid = ref<VxeGridInstance>();
const localPageParamKey = 'localPageParam:hrWfLevel';
const state = reactive({
	queryParams: { keyword: '', enabled: undefined as boolean | undefined },
	localPageParam: { pageSize: 50 as number, defaultSort: { field: 'auditOrder', order: 'asc', descStr: 'desc' } },
	editVisible: false,
	saving: false,
	edit: { id: undefined as number | undefined, code: '', name: '', auditOrder: 1, isFunctional: false, orderNo: 100, enabled: true, remark: '' },
});

const options = useVxeTable(
	{
		id: 'hrWfLevel',
		name: '审核等级',
		columns: [
			{ field: 'seq', type: 'seq', title: '序号', width: 60 },
			{ field: 'auditOrder', title: '审核顺序', width: 100, sortable: true },
			{ field: 'code', title: '编码', minWidth: 120, sortable: true },
			{ field: 'name', title: '名称', minWidth: 140, sortable: true },
			{ field: 'isFunctional', title: '职能', width: 80, slots: { default: 'row_functional' } },
			{ field: 'orderNo', title: '排序', width: 80, sortable: true },
			{ field: 'enabled', title: '启用', width: 80, slots: { default: 'row_enabled' } },
			{ field: 'remark', title: '备注', minWidth: 160, showOverflow: 'tooltip' },
			{ field: 'buttons', title: '操作', width: 100, fixed: 'right', slots: { default: 'row_buttons' } },
		],
	},
	{
		proxyConfig: {
			autoLoad: true,
			ajax: { query: ({ page, sort }) => handleQueryApi(page, sort) },
		},
		sortConfig: { defaultSort: Local.get(localPageParamKey)?.defaultSort || state.localPageParam.defaultSort },
		pagerConfig: { pageSize: Local.get(localPageParamKey)?.pageSize || state.localPageParam.pageSize },
	}
);

const handleQueryApi = (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const effective = sort?.field ? sort : state.localPageParam.defaultSort;
	return pageHrWfLevel({
		...state.queryParams,
		page: page.currentPage,
		pageSize: page.pageSize,
		field: effective.field,
		order: effective.order,
		descStr: 'desc',
	});
};

const handleQuery = async (reset = false) => {
	reset ? await xGrid.value?.commitProxy('reload') : await xGrid.value?.commitProxy('query');
};
const resetQuery = () => {
	state.queryParams.keyword = '';
	state.queryParams.enabled = undefined;
	handleQuery(true);
};

const openEdit = (row?: any) => {
	state.edit = row
		? {
				id: row.id,
				code: row.code,
				name: row.name,
				auditOrder: row.auditOrder,
				isFunctional: !!row.isFunctional,
				orderNo: row.orderNo ?? 100,
				enabled: row.enabled !== false,
				remark: row.remark || '',
			}
		: { id: undefined, code: '', name: '', auditOrder: 1, isFunctional: false, orderNo: 100, enabled: true, remark: '' };
	state.editVisible = true;
};

const saveEdit = async () => {
	if (!state.edit.code?.trim() || !state.edit.name?.trim()) {
		ElMessage.warning('请填写编码和名称');
		return;
	}
	state.saving = true;
	try {
		await saveHrWfLevel(state.edit);
		ElMessage.success('保存成功');
		state.editVisible = false;
		await handleQuery(true);
	} finally {
		state.saving = false;
	}
};

const handleDelete = async (row: any) => {
	await ElMessageBox.confirm(`确认删除等级「${row.name}」？`, '提示', { type: 'warning' });
	await deleteHrWfLevel({ id: row.id });
	ElMessage.success('已删除');
	await handleQuery(true);
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
