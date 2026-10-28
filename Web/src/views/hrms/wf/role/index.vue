<template>
	<div class="hr-wf-role">
		<splitpanes class="default-theme">
			<pane size="22" style="display: flex">
				<el-card shadow="hover" class="tree-card">
					<template #header>
						<div class="tree-header">
							<span>全局汇报树</span>
							<el-button type="primary" link icon="ele-Plus" @click="openEdit(undefined, false, true)">新增组织根</el-button>
						</div>
					</template>
					<el-input v-model="state.treeFilter" placeholder="过滤" clearable class="mb8" />
					<div class="tree-body">
						<el-tree
							ref="treeRef"
							:data="state.treeData"
							node-key="id"
							:props="{ label: 'name', children: 'children' }"
							highlight-current
							default-expand-all
							:filter-node-method="filterTree"
							@node-click="onTreeClick"
						>
							<template #default="{ data }">
								<span :class="{ 'is-disabled': data.enabled === false, 'is-virtual': !data.levelId }">
									{{ data.name }}
									<span class="tree-meta" v-if="data.levelName">（{{ data.levelName }}）</span>
									<span class="tree-meta" v-else>（组织）</span>
								</span>
							</template>
						</el-tree>
					</div>
				</el-card>
			</pane>
			<pane size="78" class="role-right-pane">
				<div class="role-right">
					<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', alignItems: 'start' }">
						<el-form :model="state.queryParams" :inline="true" label-width="auto" style="flex: 1">
							<el-form-item label="当前父节点">
								<span>{{ state.currentParentName || '（请先选左树节点查看其子角色）' }}</span>
							</el-form-item>
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

					<el-card class="full-table" shadow="hover" style="margin-top: 5px; flex: 1">
						<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options" v-on="gridEvents">
							<template #toolbar_buttons>
								<el-button type="primary" icon="ele-Plus" :disabled="!state.queryParams.parentId" @click="openEdit(undefined, true)">
									新增子角色
								</el-button>
							</template>
							<template #empty><el-empty :image-size="120" description="请选择左侧节点以查看其子角色" /></template>
							<template #row_enabled="{ row }">
								<el-tag :type="row.enabled ? 'success' : 'info'" size="small">{{ row.enabled ? '启用' : '停用' }}</el-tag>
							</template>
							<template #row_buttons="{ row }">
								<el-button icon="ele-Edit" text type="primary" @click="openEdit(row)" />
								<el-button icon="ele-Delete" text type="danger" @click="handleDelete(row)" />
							</template>
						</vxe-grid>
					</el-card>
				</div>
			</pane>
		</splitpanes>

		<el-dialog v-model="state.editVisible" :title="state.edit.id ? '编辑岗位角色' : '新增岗位角色'" width="720px" destroy-on-close>
			<el-form :model="state.edit" label-width="100px">
				<el-form-item label="上级节点">
					<el-tree-select
						v-model="state.edit.parentId"
						:data="state.treeData"
						check-strictly
						clearable
						filterable
						:props="{ label: 'name', value: 'id', children: 'children' }"
						placeholder="空=根节点"
						class="w100"
					/>
				</el-form-item>
				<el-form-item label="节点类型">
					<el-radio-group v-model="state.edit.isVirtual" @change="onVirtualChange">
						<el-radio :value="false">岗位角色（参与审核）</el-radio>
						<el-radio :value="true">组织节点（不参与审核）</el-radio>
					</el-radio-group>
				</el-form-item>
				<el-form-item v-if="!state.edit.isVirtual" label="审核等级" required>
					<el-select v-model="state.edit.levelId" filterable clearable class="w100">
						<el-option v-for="l in state.levels" :key="l.id" :label="`${l.name} (${l.code})`" :value="l.id" />
					</el-select>
				</el-form-item>
				<el-alert
					v-else
					type="info"
					:closable="false"
					show-icon
					title="组织节点不赋予审核等级、不绑岗位/管辖，仅用于挂子节点（如「审核流程」）。"
					class="mb12"
				/>
				<el-form-item label="编码" required><el-input v-model="state.edit.code" maxlength="50" /></el-form-item>
				<el-form-item label="名称" required><el-input v-model="state.edit.name" maxlength="100" /></el-form-item>
				<el-form-item label="排序"><el-input-number v-model="state.edit.orderNo" :min="0" /></el-form-item>
				<el-form-item label="启用"><el-switch v-model="state.edit.enabled" /></el-form-item>
				<el-form-item label="备注"><el-input v-model="state.edit.remark" type="textarea" maxlength="256" /></el-form-item>

				<template v-if="!state.edit.isVirtual">
					<el-divider content-position="left">管辖部门（Level 找人时校验；未配置则无人）</el-divider>
					<div class="scope-toolbar">
						<el-button size="small" icon="ele-Plus" @click="addScopeRow">添加</el-button>
					</div>
					<el-table :data="state.edit.scopes" border size="small" class="mb12">
						<el-table-column label="管辖部门" min-width="280">
							<template #default="{ row, $index }">
								<div class="org-pick-field">
									<el-input :model-value="row.ccName || row.ccCode" placeholder="请选择部门" readonly class="org-pick-input" />
									<el-button class="org-pick-btn" @click="openOrgPick($index)">....</el-button>
								</div>
							</template>
						</el-table-column>
						<el-table-column label="含下级" width="100" align="center">
							<template #default="{ row }">
								<el-switch v-model="row.includeChild" />
							</template>
						</el-table-column>
						<el-table-column label="操作" width="80" align="center">
							<template #default="{ $index }">
								<el-button text type="danger" @click="state.edit.scopes.splice($index, 1)">删</el-button>
							</template>
						</el-table-column>
					</el-table>

					<el-divider content-position="left">绑定岗位</el-divider>
					<el-form-item label="岗位编码">
						<el-select
							v-model="state.edit.postIds"
							multiple
							filterable
							allow-create
							default-first-option
							:reserve-keyword="false"
							placeholder="输入岗位编码（与员工 post_id 一致）回车添加"
							class="w100"
						>
							<el-option v-for="pid in state.edit.postIds" :key="pid" :label="pid" :value="pid" />
						</el-select>
					</el-form-item>
				</template>
			</el-form>
			<template #footer>
				<el-button @click="state.editVisible = false">取消</el-button>
				<el-button type="primary" :loading="state.saving" @click="saveEdit">保存</el-button>
			</template>
		</el-dialog>

		<OrgPickDialog
			v-model:visible="state.orgPickVisible"
			:org-data="state.orgPickData"
			:model-value="state.orgPickTargetIndex != null ? state.edit.scopes[state.orgPickTargetIndex]?.ccCode : undefined"
			@confirm="onOrgPickConfirm"
		/>
	</div>
</template>

<script lang="ts" setup name="hrWfRole">
import { onMounted, reactive, ref, watch } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { listEnabledHrWfLevel } from '/@/api/hrms/hrWfLevel';
import { getHrOrgList } from '/@/api/hrms/hrOrg';
import { deleteHrWfRole, getHrWfRole, pageHrWfRole, saveHrWfRole, treeHrWfRole } from '/@/api/hrms/hrWfRole';
import OrgPickDialog from '/@/components/selector/orgPickDialog.vue';

type ScopeRow = { ccCode: string; ccName?: string; includeChild: boolean };

const xGrid = ref<VxeGridInstance>();
const treeRef = ref();
const localPageParamKey = 'localPageParam:hrWfRole';
const state = reactive({
	queryParams: {
		parentId: undefined as number | undefined,
		keyword: '',
		enabled: undefined as boolean | undefined,
	},
	localPageParam: { pageSize: 50 as number, defaultSort: { field: 'orderNo', order: 'asc', descStr: 'desc' } },
	levels: [] as any[],
	treeData: [] as any[],
	treeFilter: '',
	currentParentName: '',
	editVisible: false,
	saving: false,
	edit: blankEdit(),
	orgPickVisible: false,
	orgPickTargetIndex: null as number | null,
	orgPickData: [] as any[],
	hrOrgRaw: [] as any[],
});

function blankEdit(parentId?: number, asVirtualRoot = false) {
	return {
		id: undefined as number | undefined,
		parentId: parentId as number | undefined,
		isVirtual: asVirtualRoot,
		levelId: undefined as number | undefined,
		code: asVirtualRoot ? 'WF_TREE_ROOT' : '',
		name: asVirtualRoot ? '审核流程' : '',
		orderNo: asVirtualRoot ? 0 : 100,
		enabled: true,
		remark: asVirtualRoot ? '组织虚拟根，不参与审核' : '',
		scopes: [] as ScopeRow[],
		postIds: [] as string[],
	};
}

/** 人事部门树 → OrgPickDialog 节点（id=ccCode, name=ccDesc） */
const mapHrOrgToPickNodes = (nodes: any[]): any[] =>
	(nodes || []).map((n) => ({
		id: n.ccCode,
		name: n.ccDesc || n.ccCode,
		children: mapHrOrgToPickNodes(n.children || []),
	}));

const findHrOrgName = (nodes: any[], ccCode: string): string => {
	if (!ccCode) return '';
	for (const n of nodes || []) {
		if (n.ccCode === ccCode) return n.ccDesc || n.ccCode;
		const hit = findHrOrgName(n.children || [], ccCode);
		if (hit) return hit;
	}
	return '';
};

const ensureOrgTree = async () => {
	if (state.orgPickData.length) return;
	const res = await getHrOrgList({ includeDisabled: false, empState: 1 });
	state.hrOrgRaw = res.data?.result ?? res.data ?? [];
	state.orgPickData = mapHrOrgToPickNodes(state.hrOrgRaw);
};

const addScopeRow = () => {
	state.edit.scopes.push({ ccCode: '', ccName: '', includeChild: true });
};

const openOrgPick = async (index: number) => {
	await ensureOrgTree();
	state.orgPickTargetIndex = index;
	state.orgPickVisible = true;
};

const onOrgPickConfirm = (payload: { id: number | string; name: string; path: string }) => {
	const idx = state.orgPickTargetIndex;
	if (idx == null || !state.edit.scopes[idx]) return;
	state.edit.scopes[idx].ccCode = String(payload.id);
	state.edit.scopes[idx].ccName = payload.path || payload.name;
	state.orgPickTargetIndex = null;
};

const hydrateScopeNames = () => {
	for (const row of state.edit.scopes) {
		if (row.ccCode && !row.ccName) {
			row.ccName = findHrOrgName(state.hrOrgRaw, row.ccCode) || row.ccCode;
		}
	}
};

/** 仅切换展示；不在切换时清空等级/管辖/岗位，避免来回切换丢数据。真正落库时再按类型剔除。 */
const onVirtualChange = (isVirtual: boolean | string | number) => {
	state.edit.isVirtual = !!isVirtual;
};

watch(
	() => state.treeFilter,
	(v) => treeRef.value?.filter(v)
);

const filterTree = (value: string, data: any) => {
	if (!value) return true;
	return String(data.name || '').includes(value) || String(data.code || '').includes(value);
};

const options = useVxeTable(
	{
		id: 'hrWfRole',
		name: '岗位角色子节点',
		columns: [
			{ field: 'seq', type: 'seq', title: '序号', width: 60 },
			{ field: 'name', title: '岗位角色名称', minWidth: 140, sortable: true },
			{ field: 'levelName', title: '审核角色(等级)', minWidth: 130, sortable: true },
			{ field: 'code', title: '编码', minWidth: 110, sortable: true },
			{ field: 'orderNo', title: '排序', width: 80, sortable: true },
			{ field: 'enabled', title: '启用', width: 80, slots: { default: 'row_enabled' } },
			{ field: 'remark', title: '备注', minWidth: 120, showOverflow: 'tooltip' },
			{ field: 'buttons', title: '操作', width: 100, fixed: 'right', slots: { default: 'row_buttons' } },
		],
	},
	{
		height: '100%',
		proxyConfig: {
			autoLoad: false,
			ajax: { query: ({ page, sort }) => handleQueryApi(page, sort) },
		},
		sortConfig: { defaultSort: Local.get(localPageParamKey)?.defaultSort || state.localPageParam.defaultSort },
		pagerConfig: { pageSize: Local.get(localPageParamKey)?.pageSize || state.localPageParam.pageSize },
	}
);

const handleQueryApi = (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	if (!state.queryParams.parentId) {
		return Promise.resolve({ data: { result: { items: [], total: 0 } } });
	}
	const effective = sort?.field ? sort : state.localPageParam.defaultSort;
	return pageHrWfRole({
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

const loadLevels = async () => {
	const res = await listEnabledHrWfLevel();
	state.levels = res.data?.result ?? res.data ?? [];
};

const loadTree = async () => {
	const res = await treeHrWfRole();
	state.treeData = res.data?.result ?? res.data ?? [];
};

const onTreeClick = (data: any) => {
	state.queryParams.parentId = data.id;
	state.currentParentName = data.name;
	handleQuery(true);
};

/** asChild=右表新增子角色；asVirtualRoot=左上新增组织根（默认「审核流程」） */
const openEdit = async (row?: any, asChild = false, asVirtualRoot = false) => {
	await Promise.all([loadLevels(), loadTree(), ensureOrgTree()]);
	if (row?.id) {
		const res = await getHrWfRole(row.id);
		const d = res.data?.result ?? res.data;
		const isVirtual = !(d.levelId > 0);
		state.edit = {
			id: d.id,
			parentId: d.parentId,
			isVirtual,
			levelId: d.levelId,
			code: d.code,
			name: d.name,
			orderNo: d.orderNo ?? 100,
			enabled: d.enabled !== false,
			remark: d.remark || '',
			scopes: (d.scopes || []).map((s: any) => ({
				ccCode: s.ccCode,
				ccName: '',
				includeChild: s.includeChild !== false,
			})),
			postIds: (d.postIds || []).map((x: any) => String(x)),
		};
		hydrateScopeNames();
	} else {
		const parentId = asChild ? state.queryParams.parentId : undefined;
		const virtual = asVirtualRoot || (!asChild && !parentId);
		state.edit = blankEdit(parentId, virtual);
		if (!virtual && state.levels[0]) state.edit.levelId = state.levels[0].id;
	}
	state.editVisible = true;
};

const saveEdit = async () => {
	if (!state.edit.code?.trim() || !state.edit.name?.trim()) {
		ElMessage.warning('请填写编码和名称');
		return;
	}
	if (!state.edit.isVirtual && !state.edit.levelId) {
		ElMessage.warning('岗位角色必须选择审核等级');
		return;
	}
	const isVirtual = !!state.edit.isVirtual;
	const postIds = isVirtual ? [] : (state.edit.postIds || []).map((x) => String(x).trim()).filter((x) => !!x);
	const scopes = isVirtual
		? []
		: (state.edit.scopes || [])
				.filter((s) => s.ccCode?.trim())
				.map((s) => ({ ccCode: s.ccCode.trim(), includeChild: s.includeChild !== false }));
	state.saving = true;
	try {
		await saveHrWfRole({
			id: state.edit.id,
			parentId: state.edit.parentId,
			levelId: isVirtual ? null : state.edit.levelId,
			code: state.edit.code,
			name: state.edit.name,
			orderNo: state.edit.orderNo,
			enabled: state.edit.enabled,
			remark: state.edit.remark,
			postIds,
			scopes,
		});
		ElMessage.success('保存成功');
		state.editVisible = false;
		await loadTree();
		await handleQuery(true);
	} finally {
		state.saving = false;
	}
};

const handleDelete = async (row: any) => {
	await ElMessageBox.confirm(`确认删除角色「${row.name}」？`, '提示', { type: 'warning' });
	await deleteHrWfRole({ id: row.id });
	ElMessage.success('已删除');
	await loadTree();
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
	await Promise.all([loadLevels(), loadTree()]);
});
</script>

<style scoped>
.hr-wf-role {
	height: 100%;
}
.role-right-pane {
	display: flex;
}
.role-right {
	display: flex;
	flex-direction: column;
	width: 100%;
	height: 100%;
	padding: 0 0 0 5px;
	overflow: hidden;
}
.tree-card {
	width: 100%;
	height: 100%;
	display: flex;
	flex-direction: column;
}
.tree-card :deep(.el-card__body) {
	flex: 1;
	min-height: 0;
	display: flex;
	flex-direction: column;
	overflow: hidden;
}
.tree-body {
	flex: 1;
	min-height: 0;
	overflow: auto;
}
.full-table {
	display: flex;
	flex-direction: column;
	min-height: 0;
}
.full-table :deep(.el-card__body) {
	flex: 1;
	min-height: 0;
	display: flex;
	flex-direction: column;
}
.tree-header {
	display: flex;
	justify-content: space-between;
	align-items: center;
}
.tree-meta {
	color: var(--el-text-color-secondary);
	font-size: 12px;
}
.is-disabled {
	opacity: 0.55;
}
.is-virtual {
	font-weight: 600;
}
.w100 {
	width: 100%;
}
.scope-toolbar {
	margin-bottom: 8px;
}
.mb8 {
	margin-bottom: 8px;
}
.mb12 {
	margin-bottom: 12px;
}
.org-pick-field {
	display: flex;
	align-items: center;
	width: 100%;
	gap: 6px;
}
.org-pick-field .org-pick-input {
	flex: 1;
	min-width: 0;
}
.org-pick-field .org-pick-btn {
	flex-shrink: 0;
	padding: 8px 10px;
	letter-spacing: 1px;
}
</style>
