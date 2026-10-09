<template>
	<div class="sys-org-container">
		<splitpanes class="default-theme">
			<pane size="15" style="display: flex">
				<OrgTree ref="orgTreeRef" @node-click="handleNodeChange" />
			</pane>
			<pane size="85" style="display: flex; flex-direction: column">
				<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', height: '100%', alignItems: 'start' }">
					<el-form :model="state.queryParams" ref="queryForm" :show-message="false" :inlineMessage="true" label-width="auto" style="flex: 1 1 0%">
						<el-row :gutter="10">
							<el-col class="mb5" :xs="24" :sm="12" :md="8" :lg="6" :xl="6">
								<el-form-item :label="$t('message.list.organizationName')">
									<el-input v-model="state.queryParams.name" :placeholder="$t('message.list.organizationName')" clearable @keyup.enter.native="handleQuery" />
								</el-form-item>
							</el-col>
							<el-col class="mb5" :xs="24" :sm="12" :md="8" :lg="6" :xl="6">
								<el-form-item :label="$t('message.list.organizationCode')">
									<el-input v-model="state.queryParams.code" :placeholder="$t('message.list.organizationCode')" clearable @keyup.enter.native="handleQuery" />
								</el-form-item>
							</el-col>
							<el-col class="mb5" :xs="24" :sm="12" :md="8" :lg="6" :xl="6">
								<el-form-item :label="$t('message.list.organizationType')">
									<el-select v-model="state.queryParams.type" filterable clearable class="w100" @clear="state.queryParams.type = undefined">
										<el-option v-for="item in state.orgTypeList" :key="item.id" :label="item.label" :value="item.value" />
									</el-select>
								</el-form-item>
							</el-col>
						</el-row>
					</el-form>

					<el-divider style="height: calc(100% - 5px); margin: 0 10px" direction="vertical" />

					<el-row>
						<el-col>
							<el-button-group>
								<el-button type="primary" icon="ele-Search" @click="handleQuery(true)" :loading="options.loading"> {{ $t('message.list.query') }} </el-button>
								<el-button icon="ele-Refresh" @click="resetQuery" :loading="options.loading"> {{ $t('message.list.reset') }} </el-button>
							</el-button-group>
						</el-col>
					</el-row>
				</el-card>

				<el-card class="full-table" shadow="hover" style="margin-top: 5px; flex: 1">
					<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options" v-on="gridEvents">
						<template #toolbar_buttons>
							<el-button type="primary" icon="ele-Plus" @click="handleAdd" v-auth="'hrOrg/add'"> {{ $t('message.list.add') }} </el-button>
							<el-button-group style="padding-left: 12px">
								<el-button icon="ele-Expand" @click="handleExpand"> {{ $t('message.list.allExpand') }} </el-button>
								<el-button icon="ele-Fold" @click="handleFold"> {{ $t('message.list.allFold') }} </el-button>
								<el-button icon="ele-Refresh" @click="handleSync" v-auth="'hrOrg/syncFromCcMstr'"> 从人事部门导入 </el-button>
								<el-button icon="ele-EditPen" @click="openBatch" v-auth="'hrOrg/batchUpdateCostKq'"> 批量维护 </el-button>
								<el-button icon="ele-Sort" @click="openMerge" v-auth="'hrOrg/mergeDept'"> 合并部门 </el-button>
								<el-button icon="ele-Rank" @click="openTransfer" v-auth="'hrOrg/transferDept'"> 调动部门 </el-button>
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
						<template #row_buttons="{ row }">
							<el-button icon="ele-Edit" text type="primary" @click="handleEdit(row)" v-auth="'hrOrg/update'"> {{ $t('message.list.edit') }} </el-button>
							<el-button icon="ele-Delete" text type="danger" @click="handleDelete(row)" v-auth="'hrOrg/delete'"> {{ $t('message.list.delete') }} </el-button>
						</template>
					</vxe-grid>
				</el-card>
			</pane>
		</splitpanes>

		<EditOrg ref="editOrgRef" :title="state.title" :orgData="state.treeData" @reload="handleQuery" />
		<BatchMaintenance ref="batchRef" @reload="handleQuery" />

		<!-- 合并部门 -->
		<el-dialog v-model="mergeState.visible" draggable :close-on-click-modal="false" width="560px" title="合并部门">
			<el-form label-width="120px">
				<el-form-item label="合并前部门一" :rules="[{ required: true }]">
					<el-select v-model="mergeState.oldCode1" filterable clearable class="w100">
						<el-option v-for="item in mergeState.ccList" :key="item.ccCode" :label="`${item.ccCode} ${item.ccDesc}`" :value="item.ccCode" />
					</el-select>
				</el-form-item>
				<el-form-item label="合并前部门二">
					<el-select v-model="mergeState.oldCode2" filterable clearable class="w100">
						<el-option v-for="item in mergeState.ccList" :key="item.ccCode" :label="`${item.ccCode} ${item.ccDesc}`" :value="item.ccCode" />
					</el-select>
				</el-form-item>
				<el-form-item label="合并后新部门" :rules="[{ required: true }]">
					<el-select v-model="mergeState.newCode" filterable clearable class="w100">
						<el-option v-for="item in mergeState.ccList" :key="item.ccCode" :label="`${item.ccCode} ${item.ccDesc}`" :value="item.ccCode" />
					</el-select>
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="mergeState.visible = false">{{ $t('message.list.cancelButtonText') }}</el-button>
				<el-button type="primary" :loading="mergeState.loading" @click="submitMerge">{{ $t('message.list.confirmButtonText') }}</el-button>
			</template>
		</el-dialog>

		<!-- 调动部门 -->
		<el-dialog v-model="transferState.visible" draggable :close-on-click-modal="false" width="560px" title="调动部门">
			<el-form label-width="120px">
				<el-form-item label="调动部门" :rules="[{ required: true }]">
					<el-select v-model="transferState.fromOrgId" filterable clearable class="w100">
						<el-option v-for="item in transferState.orgList" :key="item.id" :label="item.name" :value="item.id" />
					</el-select>
				</el-form-item>
				<el-form-item label="调至归属部门" :rules="[{ required: true }]">
					<el-select v-model="transferState.toOrgId" filterable clearable class="w100">
						<el-option v-for="item in transferState.orgList" :key="item.id" :label="item.name" :value="item.id" />
					</el-select>
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="transferState.visible = false">{{ $t('message.list.cancelButtonText') }}</el-button>
				<el-button type="primary" :loading="transferState.loading" @click="submitTransfer">{{ $t('message.list.confirmButtonText') }}</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<!-- 组织架构管理 -->
<script lang="ts" setup name="hrOrg">
import { onMounted, reactive, ref, nextTick } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { VxeGridInstance, VxeGridListeners } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';
import { useI18n } from 'vue-i18n';
import { auth } from '/@/utils/authFunction';

import OrgTree from './component/orgTree.vue';
import EditOrg from './component/editOrg.vue';
import BatchMaintenance from './component/batchMaintenance.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysDictDataApi } from '/@/api-services/system/api';
import { SysOrg } from '/@/api-services/system/models';
import { deleteHrOrg, getHrCcList, getHrOrgList, mergeHrDept, syncHrOrgFromCcMstr, transferHrDept } from '/@/api/hrms/hrOrg';

const xGrid = ref<VxeGridInstance>();
const editOrgRef = ref<InstanceType<typeof EditOrg>>();
const orgTreeRef = ref<InstanceType<typeof OrgTree>>();
const batchRef = ref<InstanceType<typeof BatchMaintenance>>();
const state = reactive({
	treeData: [] as Array<SysOrg>,
	queryParams: {
		id: 0,
		name: undefined,
		code: undefined,
		type: undefined,
	},
	title: '',
	orgTypeList: [] as any,
});

const i18n = useI18n();
const options = useVxeTable<SysOrg>(
	{
		id: 'hrOrg',
		name: i18n.t('message.list.orgStructure'),
		columns: [
			{ field: 'seq', type: 'seq', title: i18n.t('message.list.seq'), width: 60, fixed: 'left' },
			{ field: 'name', title: i18n.t('message.list.organizationName'), minWidth: 200, showOverflow: 'tooltip', treeNode: true, align: 'left', headerAlign: 'center' },
			{ field: 'code', title: i18n.t('message.list.organizationCode'), minWidth: 140, showOverflow: 'tooltip' },
			{ field: 'ccDept', title: '部门代号', minWidth: 100, showOverflow: 'tooltip' },
			{ field: 'ccLevel', title: '层级码', minWidth: 120, showOverflow: 'tooltip' },
			{ field: 'ccSite', title: '厂区', minWidth: 90, showOverflow: 'tooltip' },
			{ field: 'level', title: i18n.t('message.list.level'), minWidth: 70, showOverflow: 'tooltip' },
			{ field: 'type', title: i18n.t('message.list.organizationType'), minWidth: 80, formatter: ({ cellValue }: any) => state.orgTypeList.find((u: any) => u.value == cellValue)?.label, showOverflow: 'tooltip' },
			{ field: 'orderNo', title: i18n.t('message.list.orderNo'), width: 80, showOverflow: 'tooltip' },
			{ field: 'status', title: i18n.t('message.list.status'), width: 80, showOverflow: 'tooltip', slots: { default: 'row_status' } },
			{ field: 'buttons', title: i18n.t('message.list.operation'), fixed: 'right', width: 160, showOverflow: true, slots: { default: 'row_buttons' } },
		],
	},
	{
		stripe: false,
		checkboxConfig: { range: false },
		proxyConfig: { autoLoad: true, ajax: { query: () => handleQueryApi() } },
		pagerConfig: { enabled: false },
		toolbarConfig: { export: true },
		treeConfig: { expandAll: false },
	}
);

const mergeState = reactive({
	visible: false,
	loading: false,
	ccList: [] as any[],
	oldCode1: undefined as any,
	oldCode2: undefined as any,
	newCode: undefined as any,
});

const transferState = reactive({
	visible: false,
	loading: false,
	orgList: [] as any[],
	fromOrgId: undefined as any,
	toOrgId: undefined as any,
});

onMounted(async () => {
	let resDicData = await getAPI(SysDictDataApi).apiSysDictDataDataListCodeGet('org_type');
	state.orgTypeList = resDicData.data.result;
	nextTick(async () => {
		if (state.treeData?.length < 100) await xGrid.value?.setAllTreeExpand(true);
	});
});

const handleQueryApi = async () => {
	const params = Object.assign(state.queryParams);
	return getHrOrgList({ id: params.id, name: params.name, code: params.code, type: params.type });
};

const handleQuery = async (updateTree: boolean = false) => {
	options.loading = true;
	var res = await handleQueryApi();
	xGrid.value?.loadData(res.data.result ?? []);
	options.loading = false;
	if (updateTree == true) {
		const data = await orgTreeRef.value?.fetchTreeData();
		state.treeData = data ?? [];
	}
	if (state.queryParams.id == 0 && state.queryParams.name == undefined && state.queryParams.code == undefined && state.queryParams.type == undefined && updateTree == false)
		state.treeData = res.data.result ?? [];
};

const resetQuery = async () => {
	state.queryParams.id = 0;
	state.queryParams.name = undefined;
	state.queryParams.code = undefined;
	state.queryParams.type = undefined;
	await xGrid.value?.commitProxy('reload');
};

const handleAdd = () => {
	state.title = i18n.t('message.list.addOrg');
	editOrgRef.value?.openDialog({ status: 1, orderNo: 100 });
};

const handleEdit = (row: any) => {
	state.title = i18n.t('message.list.editOrg');
	editOrgRef.value?.openDialog(row);
};

const handleDelete = (row: any) => {
	ElMessageBox.confirm(i18n.t('message.list.confirmDeleteOrg', { name: row.name }), i18n.t('message.list.hint'), {
		confirmButtonText: i18n.t('message.list.confirmButtonText'),
		cancelButtonText: i18n.t('message.list.cancelButtonText'),
		type: 'warning',
	})
		.then(async () => {
			deleteHrOrg({ id: row.id }).then(async (res: any) => {
				if (res.data.code == 200) {
					ElMessage.success(i18n.t('message.list.successDelete'));
					await handleQuery(true);
				}
			});
		})
		.catch(() => {});
};

const gridEvents: VxeGridListeners<SysOrg> = {
	async proxyQuery() {
		state.treeData = xGrid.value?.getTableData().tableData ?? [];
	},
	async cellDblclick({ row }) {
		if (auth('hrOrg/update')) await handleEdit(row);
	},
};

const handleNodeChange = async (node: any) => {
	state.queryParams.id = node.id;
	state.queryParams.name = undefined;
	state.queryParams.code = undefined;
	state.queryParams.type = undefined;
	orgTreeRef?.value?.setCurrentKey();
	await handleQuery();
};

const handleExpand = () => {
	xGrid.value?.setAllTreeExpand(true);
};

const handleFold = () => {
	xGrid.value?.clearTreeExpand();
};

const handleSync = () => {
	ElMessageBox.confirm('将按 cc_mstr 导入或更新系统机构（以 SysOrg 为后续结构主表，不会删除已有机构）。是否继续？', i18n.t('message.list.hint'), {
		confirmButtonText: i18n.t('message.list.confirmButtonText'),
		cancelButtonText: i18n.t('message.list.cancelButtonText'),
		type: 'warning',
	})
		.then(async () => {
			const res = await syncHrOrgFromCcMstr();
			if (res.data.code == 200) {
				ElMessage.success(`已同步 ${res.data.result ?? 0} 条人事部门`);
				await handleQuery(true);
			}
		})
		.catch(() => {});
};

const openBatch = () => {
	batchRef.value?.openDialog();
};

const openMerge = async () => {
	const res = await getHrCcList();
	mergeState.ccList = res.data.result ?? [];
	mergeState.oldCode1 = undefined;
	mergeState.oldCode2 = undefined;
	mergeState.newCode = undefined;
	mergeState.visible = true;
};

const submitMerge = async () => {
	if (!mergeState.oldCode1 || !mergeState.newCode) {
		ElMessage.warning('请选择合并前部门一与合并后新部门');
		return;
	}
	mergeState.loading = true;
	try {
		await mergeHrDept({ oldCode1: mergeState.oldCode1, oldCode2: mergeState.oldCode2 ?? '', newCode: mergeState.newCode });
		ElMessage.success('部门合并完成');
		mergeState.visible = false;
		await handleQuery(true);
	} finally {
		mergeState.loading = false;
	}
};

const openTransfer = () => {
	const list: any[] = [];
	const walk = (nodes: any[]) => {
		nodes?.forEach((n) => {
			list.push({ id: n.id, name: n.name });
			walk(n.children);
		});
	};
	walk(state.treeData);
	transferState.orgList = list;
	transferState.fromOrgId = undefined;
	transferState.toOrgId = undefined;
	transferState.visible = true;
};

const submitTransfer = async () => {
	if (!transferState.fromOrgId || !transferState.toOrgId) {
		ElMessage.warning('请选择调动部门与调至归属部门');
		return;
	}
	transferState.loading = true;
	try {
		await transferHrDept({ fromOrgId: transferState.fromOrgId, toOrgId: transferState.toOrgId });
		ElMessage.success('部门调动完成');
		transferState.visible = false;
		await handleQuery(true);
	} finally {
		transferState.loading = false;
	}
};
</script>
