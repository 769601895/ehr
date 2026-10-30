<template>
	<div class="hr-lookup-def">
		<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', alignItems: 'start' }">
			<el-form :model="state.queryParams" :inline="true" style="flex: 1">
				<el-form-item label="关键字">
					<el-input v-model="state.queryParams.keyword" placeholder="编码/名称/表名" clearable @keyup.enter="handleQuery(true)" />
				</el-form-item>
				<el-form-item label="启用">
					<el-select v-model="state.queryParams.enabled" clearable placeholder="全部" style="width: 100px">
						<el-option label="是" :value="true" />
						<el-option label="否" :value="false" />
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
					<el-button type="primary" icon="ele-Plus" v-auth="'hrLookupDef/save'" @click="openEdit()">新增</el-button>
				</template>
				<template #empty><el-empty :image-size="160" /></template>
				<template #row_enabled="{ row }">
					<el-tag :type="row.enabled ? 'success' : 'info'" size="small">{{ row.enabled ? '启用' : '停用' }}</el-tag>
				</template>
				<template #row_buttons="{ row }">
					<el-button icon="ele-Edit" text type="primary" v-auth="'hrLookupDef/save'" @click="openEdit(row)" />
					<el-button icon="ele-Delete" text type="danger" v-auth="'hrLookupDef/delete'" @click="handleDelete(row)" />
				</template>
			</vxe-grid>
		</el-card>

		<el-dialog v-model="state.editVisible" :title="state.edit.id ? '编辑查找数据源' : '新增查找数据源'" width="640px" destroy-on-close>
			<el-form :model="state.edit" label-width="120px">
				<el-form-item label="编码" required><el-input v-model="state.edit.code" maxlength="64" :disabled="!!state.edit.id" /></el-form-item>
				<el-form-item label="名称" required><el-input v-model="state.edit.name" maxlength="100" /></el-form-item>
				<el-form-item label="表名" required><el-input v-model="state.edit.tableName" maxlength="128" placeholder="如 hr_post" /></el-form-item>
				<el-form-item label="值列" required><el-input v-model="state.edit.valueColumn" maxlength="64" placeholder="如 post_code" /></el-form-item>
				<el-form-item label="显示列" required><el-input v-model="state.edit.labelColumn" maxlength="64" placeholder="如 post_name" /></el-form-item>
				<el-form-item label="排序列"><el-input v-model="state.edit.orderColumn" maxlength="64" placeholder="空则用值列" /></el-form-item>
				<el-divider content-position="left">常用过滤</el-divider>
				<el-form-item label="未删除"><el-checkbox v-model="state.edit.filterUseIsDelete">追加 IsDelete=0</el-checkbox></el-form-item>
				<el-form-item label="启用列"><el-input v-model="state.edit.filterEnabledColumn" maxlength="64" placeholder="如 status" /></el-form-item>
				<el-form-item label="启用值"><el-input v-model="state.edit.filterEnabledValue" maxlength="64" placeholder="如 Active" /></el-form-item>
				<el-form-item label="等值列"><el-input v-model="state.edit.filterEqualColumn" maxlength="64" /></el-form-item>
				<el-form-item label="等值"><el-input v-model="state.edit.filterEqualValue" maxlength="128" /></el-form-item>
				<el-divider content-position="left">多语言（可选）</el-divider>
				<el-form-item label="翻译表"><el-input v-model="state.edit.langTable" maxlength="128" placeholder="如 hr_post_lang" /></el-form-item>
				<el-form-item label="译文值列"><el-input v-model="state.edit.langValueColumn" maxlength="64" placeholder="默认同值列" /></el-form-item>
				<el-form-item label="文化列"><el-input v-model="state.edit.langCultureColumn" maxlength="64" placeholder="默认 culture" /></el-form-item>
				<el-form-item label="译文显示列"><el-input v-model="state.edit.langLabelColumn" maxlength="64" placeholder="默认同显示列" /></el-form-item>
				<el-form-item label="维护页路由"><el-input v-model="state.edit.maintainPath" maxlength="256" placeholder="如 /sysCode/hrPost" /></el-form-item>
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

<script lang="ts" setup name="hrLookupDef">
import { onMounted, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { deleteHrLookupDef, getHrLookupDef, pageHrLookupDef, saveHrLookupDef } from '/@/api/hrms/hrLookupDef';

const xGrid = ref<VxeGridInstance>();
const localPageParamKey = 'localPageParam:hrLookupDef';
const state = reactive({
	queryParams: { keyword: '', enabled: undefined as boolean | undefined },
	localPageParam: { pageSize: 50 as number, defaultSort: { field: 'code', order: 'asc', descStr: 'desc' } },
	editVisible: false,
	saving: false,
	edit: blank(),
});

function blank() {
	return {
		id: undefined as number | undefined,
		code: '',
		name: '',
		tableName: '',
		valueColumn: '',
		labelColumn: '',
		orderColumn: '',
		filterUseIsDelete: true,
		filterEnabledColumn: '',
		filterEnabledValue: '',
		filterEqualColumn: '',
		filterEqualValue: '',
		langTable: '',
		langValueColumn: '',
		langCultureColumn: '',
		langLabelColumn: '',
		maintainPath: '',
		enabled: true,
		remark: '',
	};
}

const options = useVxeTable(
	{
		id: 'hrLookupDef',
		name: '查找数据源设置',
		columns: [
			{ field: 'seq', type: 'seq', title: '序号', width: 60 },
			{ field: 'code', title: '编码', minWidth: 110, sortable: true },
			{ field: 'name', title: '名称', minWidth: 120, sortable: true },
			{ field: 'tableName', title: '表名', minWidth: 120, sortable: true },
			{ field: 'valueColumn', title: '值列', minWidth: 100, sortable: true },
			{ field: 'labelColumn', title: '显示列', minWidth: 100, sortable: true },
			{ field: 'langTable', title: '翻译表', minWidth: 120 },
			{ field: 'enabled', title: '启用', width: 80, sortable: true, slots: { default: 'row_enabled' } },
			{ field: 'buttons', title: '操作', width: 100, fixed: 'right', slots: { default: 'row_buttons' } },
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
	return pageHrLookupDef({
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

const openEdit = async (row?: any) => {
	if (row?.id) {
		const res = await getHrLookupDef(row.id);
		const d = res.data?.result ?? res.data;
		state.edit = {
			id: d.id,
			code: d.code,
			name: d.name,
			tableName: d.tableName,
			valueColumn: d.valueColumn,
			labelColumn: d.labelColumn,
			orderColumn: d.orderColumn || '',
			filterUseIsDelete: d.filterUseIsDelete !== false,
			filterEnabledColumn: d.filterEnabledColumn || '',
			filterEnabledValue: d.filterEnabledValue || '',
			filterEqualColumn: d.filterEqualColumn || '',
			filterEqualValue: d.filterEqualValue || '',
			langTable: d.langTable || '',
			langValueColumn: d.langValueColumn || '',
			langCultureColumn: d.langCultureColumn || '',
			langLabelColumn: d.langLabelColumn || '',
			maintainPath: d.maintainPath || '',
			enabled: d.enabled !== false,
			remark: d.remark || '',
		};
	} else state.edit = blank();
	state.editVisible = true;
};

const saveEdit = async () => {
	if (!state.edit.code?.trim() || !state.edit.name?.trim() || !state.edit.tableName?.trim() || !state.edit.valueColumn?.trim() || !state.edit.labelColumn?.trim()) {
		ElMessage.warning('请填写编码、名称、表名、值列、显示列');
		return;
	}
	state.saving = true;
	try {
		await saveHrLookupDef({
			...state.edit,
			code: state.edit.code.trim(),
			name: state.edit.name.trim(),
			tableName: state.edit.tableName.trim(),
			valueColumn: state.edit.valueColumn.trim(),
			labelColumn: state.edit.labelColumn.trim(),
			orderColumn: state.edit.orderColumn?.trim() || null,
			filterEnabledColumn: state.edit.filterEnabledColumn?.trim() || null,
			filterEnabledValue: state.edit.filterEnabledValue?.trim() || null,
			filterEqualColumn: state.edit.filterEqualColumn?.trim() || null,
			filterEqualValue: state.edit.filterEqualValue?.trim() || null,
			langTable: state.edit.langTable?.trim() || null,
			langValueColumn: state.edit.langValueColumn?.trim() || null,
			langCultureColumn: state.edit.langCultureColumn?.trim() || null,
			langLabelColumn: state.edit.langLabelColumn?.trim() || null,
			maintainPath: state.edit.maintainPath?.trim() || null,
			remark: state.edit.remark?.trim() || null,
		});
		ElMessage.success('保存成功');
		state.editVisible = false;
		await handleQuery(true);
	} finally {
		state.saving = false;
	}
};

const handleDelete = async (row: any) => {
	await ElMessageBox.confirm(`确认删除数据源「${row.name}」？`, '提示', { type: 'warning' });
	await deleteHrLookupDef({ id: row.id });
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
