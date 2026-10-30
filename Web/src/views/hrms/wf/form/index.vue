<template>
	<div class="hr-wf-form">
		<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', alignItems: 'start' }">
			<el-form :model="state.queryParams" :inline="true" style="flex: 1">
				<el-form-item label="业务类型">
					<el-select v-model="state.queryParams.bizType" clearable filterable placeholder="全部" style="width: 200px">
						<el-option v-for="item in state.bizTypeOptions" :key="item.value" :label="`${item.label} (${item.value})`" :value="item.value" />
					</el-select>
				</el-form-item>
				<el-form-item label="关键字"><el-input v-model="state.queryParams.keyword" clearable @keyup.enter="handleQuery(true)" /></el-form-item>
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
				<template #row_buttons="{ row }">
					<el-button icon="ele-Edit" text type="primary" @click="openEdit(row)" />
					<el-button icon="ele-Delete" text type="danger" @click="handleDelete(row)" />
				</template>
			</vxe-grid>
		</el-card>

		<el-dialog v-model="state.editVisible" :title="state.edit.id ? '编辑表单方案' : '新增表单方案'" width="1100px" destroy-on-close top="5vh">
			<el-form :model="state.edit" label-width="100px">
				<el-row :gutter="12">
					<el-col :span="8">
						<el-form-item label="业务类型" required>
							<el-select v-model="state.edit.bizType" filterable class="w100" placeholder="人事代码设置 · hr_wf_biz">
								<el-option v-for="item in state.bizTypeOptions" :key="item.value" :label="`${item.label} (${item.value})`" :value="item.value" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="8"><el-form-item label="编码" required><el-input v-model="state.edit.code" /></el-form-item></el-col>
					<el-col :span="8"><el-form-item label="名称" required><el-input v-model="state.edit.name" /></el-form-item></el-col>
					<el-col :span="8"><el-form-item label="模式"><el-select v-model="state.edit.formMode" class="w100"><el-option label="B 自定义" value="B" /><el-option label="A 池字段" value="A" /></el-select></el-form-item></el-col>
					<el-col :span="8"><el-form-item label="启用"><el-switch v-model="state.edit.enabled" /></el-form-item></el-col>
				</el-row>
			</el-form>
			<div class="mb8">
				<el-button size="small" icon="ele-Plus" @click="addField">添加字段</el-button>
			</div>
			<el-table :data="state.edit.fields" border size="small" max-height="360">
				<el-table-column label="键" min-width="120"><template #default="{ row }"><el-input v-model="row.fieldKey" /></template></el-table-column>
				<el-table-column label="标签" min-width="120"><template #default="{ row }"><el-input v-model="row.label" /></template></el-table-column>
				<el-table-column label="控件" width="120">
					<template #default="{ row }">
						<el-select v-model="row.controlType">
							<el-option v-for="c in controls" :key="c" :label="c" :value="c" />
						</el-select>
					</template>
				</el-table-column>
				<el-table-column label="必填" width="70" align="center"><template #default="{ row }"><el-checkbox v-model="row.required" /></template></el-table-column>
				<el-table-column label="排序" width="90"><template #default="{ row }"><el-input-number v-model="row.sortNo" :controls="false" /></template></el-table-column>
				<el-table-column label="占位" width="80"><template #default="{ row }"><el-input-number v-model="row.spanSlots" :controls="false" :min="1" :max="24" /></template></el-table-column>
				<el-table-column label="代码类" width="120">
					<template #default="{ row }">
						<el-input
							v-model="row.codeField"
							placeholder="code_mstr"
							clearable
							:disabled="!!row.lookupSource"
							@change="() => onCodeFieldChange(row)"
						/>
					</template>
				</el-table-column>
				<el-table-column label="查找数据源" width="150">
					<template #default="{ row }">
						<el-select
							v-model="row.lookupSource"
							clearable
							filterable
							placeholder="如 hrPost"
							:disabled="!!row.codeField"
							@change="() => onLookupChange(row)"
						>
							<el-option v-for="s in state.lookupSources" :key="s.code" :label="`${s.code} · ${s.name}`" :value="s.code" />
						</el-select>
					</template>
				</el-table-column>
				<el-table-column label="" width="60" align="center">
					<template #default="{ $index }"><el-button text type="danger" @click="state.edit.fields.splice($index, 1)">删</el-button></template>
				</el-table-column>
			</el-table>
			<template #footer>
				<el-button @click="state.editVisible = false">取消</el-button>
				<el-button type="primary" :loading="state.saving" @click="saveEdit">保存</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="hrWfForm">
import { onMounted, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { deleteHrWfForm, getHrWfForm, pageHrWfForm, saveHrWfForm } from '/@/api/hrms/hrWfForm';
import { listEnabledHrLookupDef } from '/@/api/hrms/hrLookupDef';
import { bizTypeLabel, loadHrWfBizTypeOptions, type HrWfBizCodeItem } from '/@/views/hrms/wf/wfBizType';

const controls = ['text', 'textarea', 'number', 'date', 'select', 'dept', 'user', 'attachment'];
const xGrid = ref<VxeGridInstance>();
const localPageParamKey = 'localPageParam:hrWfForm';
const state = reactive({
	queryParams: { bizType: '', keyword: '' },
	bizTypeOptions: [] as HrWfBizCodeItem[],
	lookupSources: [] as { code: string; name: string }[],
	localPageParam: { pageSize: 50 as number, defaultSort: { field: 'id', order: 'desc', descStr: 'desc' } },
	editVisible: false,
	saving: false,
	edit: blank(),
});

function blank(bizType = 'hrWfGeneral') {
	return { id: undefined as number | undefined, bizType, code: '', name: '', formMode: 'B', enabled: true, remark: '', fields: [] as any[] };
}
function addField() {
	state.edit.fields.push({
		fieldKey: '',
		label: '',
		controlType: 'text',
		fieldSource: 'Custom',
		required: false,
		readonly: false,
		sortNo: (state.edit.fields.length + 1) * 10,
		spanSlots: 12,
		codeField: '',
		lookupSource: '',
	});
}
const onCodeFieldChange = (row: any) => {
	if (String(row.codeField || '').trim()) {
		row.lookupSource = '';
		row.controlType = 'select';
	}
};
const onLookupChange = (row: any) => {
	if (String(row.lookupSource || '').trim()) {
		row.codeField = '';
		row.controlType = 'select';
	}
};
const loadLookupSources = async () => {
	if (state.lookupSources.length) return;
	try {
		const res = await listEnabledHrLookupDef();
		state.lookupSources = res.data?.result ?? res.data ?? [];
	} catch {
		state.lookupSources = [];
	}
};

const options = useVxeTable(
	{
		id: 'hrWfForm',
		name: '审批表单',
		columns: [
			{ field: 'seq', type: 'seq', title: '序号', width: 60 },
			{
				field: 'bizType',
				title: '业务类型',
				minWidth: 160,
				sortable: true,
				formatter: ({ cellValue }: { cellValue: string }) => bizTypeLabel(state.bizTypeOptions, cellValue),
			},
			{ field: 'code', title: '编码', minWidth: 120, sortable: true },
			{ field: 'name', title: '名称', minWidth: 140, sortable: true },
			{ field: 'formMode', title: '模式', width: 80 },
			{ field: 'version', title: '版本', width: 70, sortable: true },
			{ field: 'enabled', title: '启用', width: 80, slots: { default: 'row_enabled' } },
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
	return pageHrWfForm({ ...state.queryParams, page: page.currentPage, pageSize: page.pageSize, field: effective.field, order: effective.order, descStr: 'desc' });
};
const handleQuery = async (reset = false) => {
	reset ? await xGrid.value?.commitProxy('reload') : await xGrid.value?.commitProxy('query');
};
const resetQuery = () => {
	state.queryParams.bizType = '';
	state.queryParams.keyword = '';
	handleQuery(true);
};

const openEdit = async (row?: any) => {
	await loadLookupSources();
	if (row?.id) {
		const res = await getHrWfForm(row.id);
		const d = res.data?.result ?? res.data;
		state.edit = {
			...d,
			fields: (d.fields || []).map((f: any) => ({
				...f,
				codeField: f.codeField || '',
				lookupSource: f.lookupSource || '',
			})),
		};
	} else state.edit = blank(state.bizTypeOptions[0]?.value || 'hrWfGeneral');
	state.editVisible = true;
};

const saveEdit = async () => {
	if (!state.edit.bizType?.trim() || !state.edit.code?.trim() || !state.edit.name?.trim()) {
		ElMessage.warning('请填写业务类型、编码、名称');
		return;
	}
	for (const f of state.edit.fields || []) {
		if (String(f.codeField || '').trim() && String(f.lookupSource || '').trim()) {
			ElMessage.warning(`字段「${f.label || f.fieldKey}」不能同时设置代码类与查找数据源`);
			return;
		}
	}
	state.saving = true;
	try {
		const payload = {
			...state.edit,
			fields: (state.edit.fields || []).map((f: any) => ({
				...f,
				codeField: String(f.codeField || '').trim() || null,
				lookupSource: String(f.lookupSource || '').trim() || null,
			})),
		};
		await saveHrWfForm(payload);
		ElMessage.success('保存成功');
		state.editVisible = false;
		await handleQuery(true);
	} finally {
		state.saving = false;
	}
};

const handleDelete = async (row: any) => {
	await ElMessageBox.confirm(`确认删除「${row.name}」？`, '提示', { type: 'warning' });
	await deleteHrWfForm({ id: row.id });
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

onMounted(async () => {
	const cached = Local.get(localPageParamKey);
	if (cached?.pageSize) state.localPageParam.pageSize = cached.pageSize;
	if (cached?.defaultSort) state.localPageParam.defaultSort = cached.defaultSort;
	state.bizTypeOptions = await loadHrWfBizTypeOptions();
	if (!state.bizTypeOptions.length) {
		ElMessage.warning('未配置业务类型代码（hr_wf_biz），请先在「人事代码设置」维护或执行种子 SQL');
	}
});
</script>

<style scoped>
.w100 { width: 100%; }
.mb8 { margin-bottom: 8px; }
</style>
