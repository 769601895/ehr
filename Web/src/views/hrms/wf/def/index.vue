<template>
	<div class="hr-wf-def">
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

		<el-dialog v-model="state.editVisible" :title="state.edit.id ? '编辑流程定义' : '新增流程定义'" width="920px" destroy-on-close top="5vh">
			<el-form :model="state.edit" label-width="120px">
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
					<el-col :span="12">
						<el-form-item label="绑定表单" required>
							<el-select v-model="state.edit.formId" filterable class="w100">
								<el-option v-for="f in state.forms" :key="f.id" :label="`${f.name} (${f.bizType})`" :value="f.id" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :span="6"><el-form-item label="启用"><el-switch v-model="state.edit.enabled" /></el-form-item></el-col>
					<el-col :span="8"><el-form-item label="审批中可改"><el-switch v-model="state.edit.allowEditWhileRunning" /></el-form-item></el-col>
					<el-col :span="8"><el-form-item label="发起人可改"><el-switch v-model="state.edit.editByApplicant" :disabled="!state.edit.allowEditWhileRunning" /></el-form-item></el-col>
					<el-col :span="8"><el-form-item label="审批人可改"><el-switch v-model="state.edit.editByAssignee" :disabled="!state.edit.allowEditWhileRunning" /></el-form-item></el-col>
				</el-row>
			</el-form>
			<div class="mb8"><el-button size="small" icon="ele-Plus" @click="addNode">添加节点</el-button></div>
			<el-table :data="state.edit.nodes" border size="small" max-height="360">
				<el-table-column label="节点键" width="100"><template #default="{ row }"><el-input v-model="row.nodeKey" /></template></el-table-column>
				<el-table-column label="名称" min-width="120"><template #default="{ row }"><el-input v-model="row.name" /></template></el-table-column>
				<el-table-column label="顺序" width="80"><template #default="{ row }"><el-input-number v-model="row.orderNo" :controls="false" /></template></el-table-column>
				<el-table-column label="模式" width="100">
					<template #default="{ row }">
						<el-select v-model="row.nodeMode"><el-option label="或签" value="Or" /><el-option label="会签" value="And" /><el-option label="串行" value="Serial" /></el-select>
					</template>
				</el-table-column>
				<el-table-column label="审批人类型" width="120">
					<template #default="{ row }">
						<el-select v-model="row._ruleType" @change="() => syncRuleJson(row)">
							<el-option label="审核等级" value="Level" />
							<el-option label="指定人" value="FixedUser" />
							<el-option label="岗位" value="Post" />
						</el-select>
					</template>
				</el-table-column>
				<el-table-column label="审批人配置" min-width="220">
					<template #default="{ row }">
						<template v-if="row._ruleType === 'Level'">
							<el-select v-model="row._levelId" filterable placeholder="选等级" class="w100" @change="() => syncRuleJson(row)">
								<el-option v-for="l in state.levels" :key="l.id" :label="`${l.name}`" :value="l.id" />
							</el-select>
							<div class="hint">仅当当事人汇报路径上存在该等级时才会审批</div>
						</template>
						<template v-else-if="row._ruleType === 'FixedUser'">
							<el-select
								v-model="row._userIds"
								multiple
								filterable
								allow-create
								default-first-option
								:reserve-keyword="false"
								placeholder="输入用户 Id 回车添加"
								class="w100"
								@change="() => syncRuleJson(row)"
							>
								<el-option v-for="uid in row._userIds || []" :key="uid" :label="String(uid)" :value="uid" />
							</el-select>
						</template>
						<template v-else>
							<el-input v-model="row._postId" placeholder="人事岗位编码 postId" @change="() => syncRuleJson(row)" />
						</template>
					</template>
				</el-table-column>
				<el-table-column label="条件JSON" min-width="160">
					<template #default="{ row }">
						<el-input v-model="row.conditionJson" placeholder='如 {"field":"amount","op":">","value":1000}' />
					</template>
				</el-table-column>
				<el-table-column label="" width="60" align="center">
					<template #default="{ $index }"><el-button text type="danger" @click="state.edit.nodes.splice($index, 1)">删</el-button></template>
				</el-table-column>
			</el-table>
			<template #footer>
				<el-button @click="state.editVisible = false">取消</el-button>
				<el-button type="primary" :loading="state.saving" @click="saveEdit">保存</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="hrWfDef">
import { onMounted, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { listEnabledHrWfLevel } from '/@/api/hrms/hrWfLevel';
import { pageHrWfForm } from '/@/api/hrms/hrWfForm';
import { deleteHrWfDef, getHrWfDef, pageHrWfDef, saveHrWfDef } from '/@/api/hrms/hrWfDef';
import { bizTypeLabel, loadHrWfBizTypeOptions, type HrWfBizCodeItem } from '/@/views/hrms/wf/wfBizType';

const xGrid = ref<VxeGridInstance>();
const localPageParamKey = 'localPageParam:hrWfDef';
const state = reactive({
	queryParams: { bizType: '', keyword: '' },
	bizTypeOptions: [] as HrWfBizCodeItem[],
	localPageParam: { pageSize: 50 as number, defaultSort: { field: 'id', order: 'desc', descStr: 'desc' } },
	levels: [] as any[],
	forms: [] as any[],
	editVisible: false,
	saving: false,
	edit: blank(),
});

function blank(bizType = 'hrWfGeneral') {
	return {
		id: undefined as number | undefined,
		bizType,
		code: '',
		name: '',
		formId: undefined as number | undefined,
		enabled: true,
		allowEditWhileRunning: false,
		editByApplicant: false,
		editByAssignee: false,
		defaultTimeoutHours: 0,
		nodes: [] as any[],
	};
}

const parseRule = (json: string) => {
	try {
		const o = JSON.parse(json || '{}');
		const type = o.type || o.Type || 'Level';
		return {
			_ruleType: type,
			_levelId: o.levelId ?? o.LevelId,
			_userIds: (o.userIds || o.UserIds || []).map((x: any) => Number(x)).filter((x: number) => x > 0),
			_postId: o.postId || o.PostId || '',
		};
	} catch {
		return { _ruleType: 'Level', _levelId: undefined, _userIds: [], _postId: '' };
	}
};

const syncRuleJson = (row: any) => {
	const type = row._ruleType || 'Level';
	if (type === 'FixedUser') {
		const userIds = (row._userIds || []).map((x: any) => Number(x)).filter((x: number) => x > 0);
		row.assigneeRuleJson = JSON.stringify({ type: 'FixedUser', userIds });
		return;
	}
	if (type === 'Post') {
		row.assigneeRuleJson = JSON.stringify({ type: 'Post', postId: String(row._postId || '').trim() });
		return;
	}
	row.assigneeRuleJson = JSON.stringify({ type: 'Level', levelId: row._levelId || 0 });
};

const addNode = () => {
	const n = state.edit.nodes.length + 1;
	const levelId = state.levels[0]?.id;
	const row: any = {
		nodeKey: `n${n}`,
		name: `节点${n}`,
		orderNo: n * 10,
		nodeMode: 'Or',
		enabled: true,
		conditionJson: '',
		_ruleType: 'Level',
		_levelId: levelId,
		_userIds: [],
		_postId: '',
	};
	syncRuleJson(row);
	state.edit.nodes.push(row);
};

const options = useVxeTable(
	{
		id: 'hrWfDef',
		name: '流程定义',
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
			{ field: 'formName', title: '表单', minWidth: 120 },
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
	return pageHrWfDef({ ...state.queryParams, page: page.currentPage, pageSize: page.pageSize, field: effective.field, order: effective.order, descStr: 'desc' });
};
const handleQuery = async (reset = false) => {
	reset ? await xGrid.value?.commitProxy('reload') : await xGrid.value?.commitProxy('query');
};
const resetQuery = () => {
	state.queryParams.bizType = '';
	state.queryParams.keyword = '';
	handleQuery(true);
};

const loadMeta = async () => {
	const [lv, fm] = await Promise.all([listEnabledHrWfLevel(), pageHrWfForm({ page: 1, pageSize: 200, enabled: true })]);
	state.levels = lv.data?.result ?? lv.data ?? [];
	state.forms = fm.data?.result?.items ?? fm.data?.result ?? [];
};

const openEdit = async (row?: any) => {
	await loadMeta();
	if (row?.id) {
		const res = await getHrWfDef(row.id);
		const d = res.data?.result ?? res.data;
		state.edit = {
			...d,
			nodes: (d.nodes || []).map((n: any) => ({ ...n, ...parseRule(n.assigneeRuleJson) })),
		};
	} else state.edit = blank(state.bizTypeOptions[0]?.value || 'hrWfGeneral');
	state.editVisible = true;
};

const saveEdit = async () => {
	if (!state.edit.bizType || !state.edit.code || !state.edit.name || !state.edit.formId) {
		ElMessage.warning('请填写完整头信息');
		return;
	}
	if (!state.edit.nodes?.length) {
		ElMessage.warning('请至少添加一个节点');
		return;
	}
	state.saving = true;
	try {
		const payload = {
			...state.edit,
			nodes: state.edit.nodes.map((n: any) => {
				syncRuleJson(n);
				return {
					nodeKey: n.nodeKey,
					name: n.name,
					orderNo: n.orderNo,
					nodeMode: n.nodeMode || 'Or',
					enabled: n.enabled !== false,
					assigneeRuleJson: n.assigneeRuleJson,
					conditionJson: n.conditionJson || null,
					timeoutHours: n.timeoutHours,
				};
			}),
		};
		await saveHrWfDef(payload);
		ElMessage.success('保存成功');
		state.editVisible = false;
		await handleQuery(true);
	} finally {
		state.saving = false;
	}
};

const handleDelete = async (row: any) => {
	await ElMessageBox.confirm(`确认删除「${row.name}」？`, '提示', { type: 'warning' });
	await deleteHrWfDef({ id: row.id });
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
.hint { margin-top: 4px; font-size: 12px; color: var(--el-text-color-secondary); line-height: 1.3; }
</style>
