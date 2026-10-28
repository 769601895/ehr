<template>
	<div class="hr-post">
		<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', alignItems: 'start' }">
			<el-form :model="state.queryParams" :inline="true" style="flex: 1">
				<el-form-item label="关键字">
					<el-input v-model="state.queryParams.keyword" placeholder="岗位代码/名称" clearable @keyup.enter="handleQuery(true)" />
				</el-form-item>
				<el-form-item label="状态">
					<el-select v-model="state.queryParams.status" clearable placeholder="全部" style="width: 120px">
						<el-option label="现用" value="Active" />
						<el-option label="停用" value="Inactive" />
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
					<el-button type="primary" icon="ele-Plus" v-auth="'hrPost/save'" @click="openEdit()">新增</el-button>
				</template>
				<template #empty><el-empty :image-size="160" /></template>
				<template #row_status="{ row }">
					<el-tag :type="row.status === 'Active' ? 'success' : 'info'" size="small">
						{{ row.status === 'Active' ? '现用' : '停用' }}
					</el-tag>
				</template>
				<template #row_need_audit="{ row }">
					<el-checkbox :model-value="!!row.needAudit" disabled />
				</template>
				<template #row_buttons="{ row }">
					<el-button icon="ele-Edit" text type="primary" v-auth="'hrPost/save'" @click="openEdit(row)" />
					<el-button icon="ele-Delete" text type="danger" v-auth="'hrPost/delete'" @click="handleDelete(row)" />
				</template>
			</vxe-grid>
		</el-card>

		<el-dialog v-model="state.editVisible" :title="state.edit.id ? '编辑岗位' : '新增岗位'" width="520px" destroy-on-close>
			<el-form :model="state.edit" label-width="110px">
				<el-form-item label="岗位代码" required>
					<el-input v-model="state.edit.postCode" maxlength="50" :disabled="!!state.edit.id" />
				</el-form-item>
				<el-form-item label="岗位名称" required>
					<el-input v-model="state.edit.postName" maxlength="100" />
				</el-form-item>
				<el-form-item label="等级">
					<el-input-number v-model="state.edit.grade" :min="0" :max="999" />
				</el-form-item>
				<el-form-item label="状态">
					<el-select v-model="state.edit.status" class="w100">
						<el-option label="现用" value="Active" />
						<el-option label="停用" value="Inactive" />
					</el-select>
				</el-form-item>
				<el-form-item label="所属部门">
					<div class="org-pick-field">
						<el-input :model-value="state.edit.ccName" placeholder="所属部门" readonly class="org-pick-input" />
						<el-button class="org-pick-btn" @click="openOrgPick">....</el-button>
						<el-button v-if="state.edit.ccCode" text type="danger" @click="clearOrg">清除</el-button>
					</div>
				</el-form-item>
				<el-form-item label="岗位是否审核">
					<el-checkbox v-model="state.edit.needAudit" />
				</el-form-item>
				<el-form-item label="备注">
					<el-input v-model="state.edit.remark" type="textarea" maxlength="256" />
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="state.editVisible = false">取消</el-button>
				<el-button type="primary" :loading="state.saving" @click="saveEdit">保存</el-button>
			</template>
		</el-dialog>

		<OrgPickDialog
			v-model:visible="state.orgPickVisible"
			:org-data="state.orgPickData"
			:model-value="state.edit.ccCode"
			@confirm="onOrgPickConfirm"
		/>
	</div>
</template>

<script lang="ts" setup name="hrPost">
import { onMounted, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { getHrOrgList } from '/@/api/hrms/hrOrg';
import { deleteHrPost, getHrPost, pageHrPost, saveHrPost } from '/@/api/hrms/hrPost';
import OrgPickDialog from '/@/components/selector/orgPickDialog.vue';

const xGrid = ref<VxeGridInstance>();
const localPageParamKey = 'localPageParam:hrPost';
const state = reactive({
	queryParams: { keyword: '', status: '' as string },
	localPageParam: { pageSize: 50 as number, defaultSort: { field: 'postCode', order: 'asc', descStr: 'desc' } },
	editVisible: false,
	saving: false,
	edit: blank(),
	orgPickVisible: false,
	orgPickData: [] as any[],
});

function blank() {
	return {
		id: undefined as number | undefined,
		postCode: '',
		postName: '',
		grade: 0,
		status: 'Active',
		ccCode: '' as string,
		ccName: '' as string,
		needAudit: true,
		remark: '',
	};
}

const options = useVxeTable(
	{
		id: 'hrPost',
		name: '岗位代码设置',
		columns: [
			{ field: 'seq', type: 'seq', title: '序号', width: 60 },
			{ field: 'postCode', title: '岗位代码', minWidth: 110, sortable: true },
			{ field: 'postName', title: '岗位名称', minWidth: 140, sortable: true },
			{ field: 'grade', title: '等级', width: 80, sortable: true },
			{ field: 'status', title: '状态', width: 90, sortable: true, slots: { default: 'row_status' } },
			{ field: 'ccName', title: '所属部门', minWidth: 140, showOverflow: 'tooltip' },
			{ field: 'needAudit', title: '岗位是否审核', width: 120, slots: { default: 'row_need_audit' } },
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
	return pageHrPost({
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
	state.queryParams.status = '';
	handleQuery(true);
};

const mapHrOrgToPickNodes = (nodes: any[]): any[] =>
	(nodes || []).map((n) => ({
		id: n.ccCode,
		name: n.ccDesc || n.ccCode,
		children: mapHrOrgToPickNodes(n.children || []),
	}));

const ensureOrgTree = async () => {
	if (state.orgPickData.length) return;
	const res = await getHrOrgList({ includeDisabled: false, empState: 1 });
	state.orgPickData = mapHrOrgToPickNodes(res.data?.result ?? res.data ?? []);
};

const openOrgPick = async () => {
	await ensureOrgTree();
	state.orgPickVisible = true;
};
const clearOrg = () => {
	state.edit.ccCode = '';
	state.edit.ccName = '';
};
const onOrgPickConfirm = (payload: { id: string | number; name: string }) => {
	state.edit.ccCode = String(payload.id ?? '');
	state.edit.ccName = payload.name || state.edit.ccCode;
};

const openEdit = async (row?: any) => {
	if (row?.id) {
		const res = await getHrPost(row.id);
		const d = res.data?.result ?? res.data;
		state.edit = {
			id: d.id,
			postCode: d.postCode,
			postName: d.postName,
			grade: d.grade ?? 0,
			status: d.status || 'Active',
			ccCode: d.ccCode || '',
			ccName: d.ccName || d.ccCode || '',
			needAudit: d.needAudit !== false,
			remark: d.remark || '',
		};
	} else state.edit = blank();
	state.editVisible = true;
};

const saveEdit = async () => {
	if (!state.edit.postCode?.trim() || !state.edit.postName?.trim()) {
		ElMessage.warning('请填写岗位代码、岗位名称');
		return;
	}
	state.saving = true;
	try {
		await saveHrPost({
			id: state.edit.id,
			postCode: state.edit.postCode.trim(),
			postName: state.edit.postName.trim(),
			grade: state.edit.grade ?? 0,
			status: state.edit.status,
			ccCode: state.edit.ccCode || null,
			needAudit: !!state.edit.needAudit,
			remark: state.edit.remark || null,
		});
		ElMessage.success('保存成功');
		state.editVisible = false;
		await handleQuery(true);
	} finally {
		state.saving = false;
	}
};

const handleDelete = async (row: any) => {
	await ElMessageBox.confirm(`确认删除岗位「${row.postName}」？`, '提示', { type: 'warning' });
	await deleteHrPost({ id: row.id });
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

<style scoped>
.w100 { width: 100%; }
.org-pick-field {
	display: flex;
	align-items: center;
	gap: 6px;
	width: 100%;
}
.org-pick-input { flex: 1; }
.org-pick-btn { flex-shrink: 0; }
</style>
