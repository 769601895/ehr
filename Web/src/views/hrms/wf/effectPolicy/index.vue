<template>
	<div class="hr-wf-effect-policy">
		<el-card shadow="hover">
			<template #header>
				<div class="hdr">
					<span>业务生效策略</span>
					<div>
						<el-button type="warning" plain @click="doApplyDue">扫描到期生效</el-button>
					</div>
				</div>
			</template>
			<el-tabs v-model="state.tab">
				<el-tab-pane label="生效策略" name="policy">
					<div class="mb8">
						<el-button type="primary" icon="ele-Plus" @click="openPolicy()">新增策略</el-button>
						<el-button icon="ele-Refresh" @click="loadPolicies">刷新</el-button>
					</div>
					<el-table :data="state.policies" border stripe v-loading="state.loadingPolicy">
						<el-table-column prop="bizType" label="业务类型" min-width="120" />
						<el-table-column prop="baseDateField" label="基准日字段" min-width="120" />
						<el-table-column prop="dayOffset" label="日偏移" width="90" />
						<el-table-column prop="applyMode" label="生效模式" min-width="130" />
						<el-table-column prop="enabled" label="启用" width="80">
							<template #default="{ row }">
								<el-tag :type="row.enabled ? 'success' : 'info'" size="small">{{ row.enabled ? '是' : '否' }}</el-tag>
							</template>
						</el-table-column>
						<el-table-column prop="remark" label="备注" min-width="160" show-overflow-tooltip />
						<el-table-column label="操作" width="120" fixed="right">
							<template #default="{ row }">
								<el-button text type="primary" @click="openPolicy(row)">编辑</el-button>
								<el-button text type="danger" @click="removePolicy(row)">删</el-button>
							</template>
						</el-table-column>
					</el-table>
				</el-tab-pane>

				<el-tab-pane label="字段对照" name="map">
					<div class="mb8">
						<el-select v-model="state.mapBizType" clearable placeholder="按业务类型筛选" style="width: 200px" @change="loadMaps">
							<el-option v-for="p in state.policies" :key="p.bizType" :label="p.bizType" :value="p.bizType" />
						</el-select>
						<el-button type="primary" icon="ele-Plus" class="ml8" @click="openMap()">新增对照</el-button>
						<el-button icon="ele-Refresh" @click="loadMaps">刷新</el-button>
					</div>
					<el-table :data="state.maps" border stripe v-loading="state.loadingMap">
						<el-table-column prop="bizType" label="业务类型" width="110" />
						<el-table-column prop="formFieldKey" label="表单字段" min-width="120" />
						<el-table-column prop="role" label="角色" width="130" />
						<el-table-column prop="dbColumn" label="主档列" width="110" />
						<el-table-column prop="pairGroup" label="配对组" width="90" />
						<el-table-column prop="sortNo" label="排序" width="70" />
						<el-table-column prop="enabled" label="启用" width="70">
							<template #default="{ row }">
								<el-tag :type="row.enabled ? 'success' : 'info'" size="small">{{ row.enabled ? '是' : '否' }}</el-tag>
							</template>
						</el-table-column>
						<el-table-column prop="remark" label="备注" min-width="140" show-overflow-tooltip />
						<el-table-column label="操作" width="120" fixed="right">
							<template #default="{ row }">
								<el-button text type="primary" @click="openMap(row)">编辑</el-button>
								<el-button text type="danger" @click="removeMap(row)">删</el-button>
							</template>
						</el-table-column>
					</el-table>
				</el-tab-pane>
			</el-tabs>
		</el-card>

		<el-dialog v-model="state.policyVisible" :title="state.policyEdit.id ? '编辑策略' : '新增策略'" width="560px" destroy-on-close>
			<el-form :model="state.policyEdit" label-width="110px">
				<el-form-item label="业务类型" required>
					<el-input v-model="state.policyEdit.bizType" placeholder="如 hrEmh / hrEmpLeave" :disabled="!!state.policyEdit.id" />
				</el-form-item>
				<el-form-item label="基准日来源">
					<el-select v-model="state.policyEdit.baseDateSource" style="width: 100%">
						<el-option label="Form（表单）" value="Form" />
						<el-option label="Biz（业务单）" value="Biz" />
					</el-select>
				</el-form-item>
				<el-form-item label="基准日字段" required>
					<el-input v-model="state.policyEdit.baseDateField" placeholder="如 emh_date / leave_date" />
				</el-form-item>
				<el-form-item label="日偏移">
					<el-input-number v-model="state.policyEdit.dayOffset" :min="-30" :max="365" />
					<span class="hint">0=当日，1=次日</span>
				</el-form-item>
				<el-form-item label="生效模式">
					<el-select v-model="state.policyEdit.applyMode" style="width: 100%">
						<el-option label="到期立即生效 ImmediateIfDue" value="ImmediateIfDue" />
						<el-option label="仅定时任务 JobOnly" value="JobOnly" />
					</el-select>
				</el-form-item>
				<el-form-item label="启用"><el-switch v-model="state.policyEdit.enabled" /></el-form-item>
				<el-form-item label="备注"><el-input v-model="state.policyEdit.remark" type="textarea" /></el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="state.policyVisible = false">取消</el-button>
				<el-button type="primary" :loading="state.saving" @click="savePolicy">保存</el-button>
			</template>
		</el-dialog>

		<el-dialog v-model="state.mapVisible" :title="state.mapEdit.id ? '编辑对照' : '新增对照'" width="560px" destroy-on-close>
			<el-form :model="state.mapEdit" label-width="110px">
				<el-form-item label="业务类型" required>
					<el-input v-model="state.mapEdit.bizType" placeholder="如 hrEmh" />
				</el-form-item>
				<el-form-item label="表单字段" required>
					<el-input v-model="state.mapEdit.formFieldKey" placeholder="FormData 键" />
				</el-form-item>
				<el-form-item label="角色" required>
					<el-select v-model="state.mapEdit.role" style="width: 100%">
						<el-option label="Subject 工号" value="Subject" />
						<el-option label="EffectBaseDate 基准日" value="EffectBaseDate" />
						<el-option label="Before 撤销回滚" value="Before" />
						<el-option label="After 写回主档" value="After" />
						<el-option label="Context 仅展示" value="Context" />
					</el-select>
				</el-form-item>
				<el-form-item label="主档列">
					<el-select v-model="state.mapEdit.dbColumn" clearable allow-create filterable style="width: 100%" placeholder="白名单列">
						<el-option label="cc_code 部门" value="cc_code" />
						<el-option label="post_id 岗位" value="post_id" />
						<el-option label="emp_state 状态" value="emp_state" />
						<el-option label="emp_lea_date 离职日" value="emp_lea_date" />
					</el-select>
				</el-form-item>
				<el-form-item label="配对组"><el-input v-model="state.mapEdit.pairGroup" placeholder="如 dept / post" /></el-form-item>
				<el-form-item label="排序"><el-input-number v-model="state.mapEdit.sortNo" :min="0" /></el-form-item>
				<el-form-item label="启用"><el-switch v-model="state.mapEdit.enabled" /></el-form-item>
				<el-form-item label="备注"><el-input v-model="state.mapEdit.remark" type="textarea" /></el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="state.mapVisible = false">取消</el-button>
				<el-button type="primary" :loading="state.saving" @click="saveMap">保存</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="hrWfEffectPolicy">
import { onMounted, reactive } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import {
	applyDueHrWfEffect,
	deleteHrWfEffectFieldMap,
	deleteHrWfEffectPolicy,
	listHrWfEffectFieldMaps,
	listHrWfEffectPolicies,
	saveHrWfEffectFieldMap,
	saveHrWfEffectPolicy,
} from '/@/api/hrms/hrWfEffectPolicy';

const blankPolicy = () => ({
	id: undefined as number | undefined,
	bizType: '',
	enabled: true,
	baseDateSource: 'Form',
	baseDateField: '',
	dayOffset: 0,
	applyMode: 'ImmediateIfDue',
	targetEntity: 'HrEmployee',
	remark: '',
});

const blankMap = () => ({
	id: undefined as number | undefined,
	bizType: '',
	formFieldKey: '',
	role: 'After',
	targetEntity: 'HrEmployee',
	dbColumn: '' as string | undefined,
	pairGroup: '',
	enabled: true,
	sortNo: 100,
	remark: '',
});

const state = reactive({
	tab: 'policy',
	loadingPolicy: false,
	loadingMap: false,
	saving: false,
	policies: [] as any[],
	maps: [] as any[],
	mapBizType: '' as string,
	policyVisible: false,
	mapVisible: false,
	policyEdit: blankPolicy(),
	mapEdit: blankMap(),
});

const loadPolicies = async () => {
	state.loadingPolicy = true;
	try {
		const res = await listHrWfEffectPolicies();
		state.policies = res.data?.result ?? res.data ?? [];
	} finally {
		state.loadingPolicy = false;
	}
};

const loadMaps = async () => {
	state.loadingMap = true;
	try {
		const res = await listHrWfEffectFieldMaps(state.mapBizType || undefined);
		state.maps = res.data?.result ?? res.data ?? [];
	} finally {
		state.loadingMap = false;
	}
};

const openPolicy = (row?: any) => {
	state.policyEdit = row
		? {
				id: row.id,
				bizType: row.bizType,
				enabled: row.enabled !== false,
				baseDateSource: row.baseDateSource || 'Form',
				baseDateField: row.baseDateField || '',
				dayOffset: row.dayOffset ?? 0,
				applyMode: row.applyMode || 'ImmediateIfDue',
				targetEntity: row.targetEntity || 'HrEmployee',
				remark: row.remark || '',
			}
		: blankPolicy();
	state.policyVisible = true;
};

const savePolicy = async () => {
	if (!state.policyEdit.bizType?.trim() || !state.policyEdit.baseDateField?.trim()) {
		ElMessage.warning('请填写业务类型与基准日字段');
		return;
	}
	state.saving = true;
	try {
		await saveHrWfEffectPolicy(state.policyEdit);
		ElMessage.success('已保存');
		state.policyVisible = false;
		await loadPolicies();
	} finally {
		state.saving = false;
	}
};

const removePolicy = async (row: any) => {
	await ElMessageBox.confirm(`删除策略「${row.bizType}」？`, '提示', { type: 'warning' });
	await deleteHrWfEffectPolicy(row.id);
	ElMessage.success('已删除');
	await loadPolicies();
};

const openMap = (row?: any) => {
	state.mapEdit = row
		? {
				id: row.id,
				bizType: row.bizType,
				formFieldKey: row.formFieldKey,
				role: row.role,
				targetEntity: row.targetEntity || 'HrEmployee',
				dbColumn: row.dbColumn || '',
				pairGroup: row.pairGroup || '',
				enabled: row.enabled !== false,
				sortNo: row.sortNo ?? 100,
				remark: row.remark || '',
			}
		: { ...blankMap(), bizType: state.mapBizType || '' };
	state.mapVisible = true;
};

const saveMap = async () => {
	if (!state.mapEdit.bizType?.trim() || !state.mapEdit.formFieldKey?.trim() || !state.mapEdit.role?.trim()) {
		ElMessage.warning('请填写业务类型、表单字段、角色');
		return;
	}
	state.saving = true;
	try {
		await saveHrWfEffectFieldMap({
			...state.mapEdit,
			dbColumn: state.mapEdit.dbColumn || null,
		});
		ElMessage.success('已保存');
		state.mapVisible = false;
		await loadMaps();
	} finally {
		state.saving = false;
	}
};

const removeMap = async (row: any) => {
	await ElMessageBox.confirm(`删除对照「${row.formFieldKey}」？`, '提示', { type: 'warning' });
	await deleteHrWfEffectFieldMap(row.id);
	ElMessage.success('已删除');
	await loadMaps();
};

const doApplyDue = async () => {
	const res = await applyDueHrWfEffect();
	const n = res.data?.result ?? res.data ?? 0;
	ElMessage.success(`扫描完成，成功生效 ${n} 条`);
};

onMounted(async () => {
	await loadPolicies();
	await loadMaps();
});
</script>

<style scoped>
.hdr { display: flex; align-items: center; justify-content: space-between; }
.mb8 { margin-bottom: 8px; }
.ml8 { margin-left: 8px; }
.hint { margin-left: 8px; color: #909399; font-size: 12px; }
</style>
