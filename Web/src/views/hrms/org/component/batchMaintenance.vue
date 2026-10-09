<template>
	<div class="hr-batch-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="1000px" title="批量维护">
			<el-tabs v-model="state.activeTab">
				<el-tab-pane label="成本中心/考勤" name="costKq">
					<el-table :data="state.costKqList" border height="420" size="small" v-loading="state.loading">
						<el-table-column prop="ccCode" label="部门编码" width="120" fixed show-overflow-tooltip />
						<el-table-column prop="ccDesc" label="部门名称" min-width="180" show-overflow-tooltip />
						<el-table-column label="成本中心" min-width="180">
							<template #default="{ row }">
								<el-select v-model="row.ccCost" filterable clearable class="w100">
									<el-option v-for="item in state.costCenterList" :key="item.value" :label="item.label" :value="item.value" />
								</el-select>
							</template>
						</el-table-column>
						<el-table-column label="工时变更" min-width="140">
							<template #default="{ row }">
								<el-input v-model="row.ccKqWocachange" clearable placeholder="cc_kq_wocachange" />
							</template>
						</el-table-column>
						<el-table-column label="考勤组别" min-width="180">
							<template #default="{ row }">
								<el-select v-model="row.ccKqgroup" filterable clearable class="w100">
									<el-option v-for="item in state.shiftGroupList" :key="item.value" :label="item.label" :value="item.value" />
								</el-select>
							</template>
						</el-table-column>
					</el-table>
				</el-tab-pane>
				<el-tab-pane label="考勤代码" name="kqCode">
					<el-table :data="state.kqCodeList" border height="420" size="small" v-loading="state.loading">
						<el-table-column prop="ccCode" label="部门编码" width="120" fixed show-overflow-tooltip />
						<el-table-column prop="ccDesc" label="部门名称" min-width="180" show-overflow-tooltip />
						<el-table-column label="考勤代码" min-width="200">
							<template #default="{ row }">
								<el-input v-model="row.ccKqCode" clearable placeholder="cc_kq_code" />
							</template>
						</el-table-column>
					</el-table>
				</el-tab-pane>
			</el-tabs>
			<template #footer>
				<span class="dialog-footer">
					<el-button icon="ele-CircleCloseFilled" @click="state.isShowDialog = false">{{ $t('message.list.cancelButtonText') }}</el-button>
					<el-button type="primary" icon="ele-CircleCheckFilled" :loading="state.saveLoading" @click="submit">{{ $t('message.list.confirmButtonText') }}</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="hrBatchMaintenance">
import { reactive, ref } from 'vue';
import { ElMessage } from 'element-plus';
import { useI18n } from 'vue-i18n';

import { batchUpdateCostKq, batchUpdateKqCode, getHrCcList, getHrCostCenterList, getHrShiftGroupList } from '/@/api/hrms/hrOrg';

const emits = defineEmits(['reload']);
const i18n = useI18n();
const state = reactive({
	isShowDialog: false,
	activeTab: 'costKq',
	loading: false,
	saveLoading: false,
	costKqList: [] as any[],
	kqCodeList: [] as any[],
	costCenterList: [] as any[],
	shiftGroupList: [] as any[],
});

const openDialog = async () => {
	state.isShowDialog = true;
	state.activeTab = 'costKq';
	await loadData();
};

const loadData = async () => {
	state.loading = true;
	try {
		const [cc, cost, shift] = await Promise.all([getHrCcList(), getHrCostCenterList(), getHrShiftGroupList()]);
		const list = cc.data.result ?? [];
		state.costKqList = JSON.parse(JSON.stringify(list));
		state.kqCodeList = JSON.parse(JSON.stringify(list));
		state.costCenterList = cost.data.result ?? [];
		state.shiftGroupList = shift.data.result ?? [];
	} finally {
		state.loading = false;
	}
};

const submit = async () => {
	state.saveLoading = true;
	try {
		if (state.activeTab === 'costKq') {
			const rows = state.costKqList
				.filter((u: any) => u.ccCode)
				.map((u: any) => ({ ccCode: u.ccCode, ccCost: u.ccCost ?? '', ccKqWocachange: u.ccKqWocachange ?? '', ccKqgroup: u.ccKqgroup ?? null }));
			await batchUpdateCostKq(rows);
			ElMessage.success('成本中心/考勤 已保存');
		} else {
			const rows = state.kqCodeList
				.filter((u: any) => u.ccCode)
				.map((u: any) => ({ ccCode: u.ccCode, ccKqCode: u.ccKqCode ?? '' }));
			await batchUpdateKqCode(rows);
			ElMessage.success('考勤代码 已保存');
		}
		state.isShowDialog = false;
		emits('reload', true);
	} finally {
		state.saveLoading = false;
	}
};

defineExpose({ openDialog });
</script>
