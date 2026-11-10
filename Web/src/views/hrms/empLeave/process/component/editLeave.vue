<template>
	<el-dialog
		v-model="state.visible"
		:title="state.id ? `离职单 ${state.form.leaveNo || ''}` : '新增离职'"
		width="720px"
		destroy-on-close
		:close-on-click-modal="false"
		@closed="onClosed"
	>
		<el-form ref="formRef" :model="state.form" :rules="rules" label-width="100px" :disabled="!canEdit">
			<el-row :gutter="12">
				<el-col :span="12">
					<el-form-item label="离职单号">
						<el-input :model-value="state.form.leaveNo || '保存后自动生成'" disabled />
					</el-form-item>
				</el-col>
				<el-col :span="12">
					<el-form-item label="状态">
						<el-tag size="small">{{ state.form.statusText || statusLabel(state.form.status) || '草稿' }}</el-tag>
					</el-form-item>
				</el-col>
				<el-col :span="12">
					<el-form-item label="工号" prop="empId">
						<EmpIdInput
							ref="empIdRef"
							v-model="state.form.empId"
							:disabled="!canEdit"
							:emp-state="1"
							@resolved="onEmpResolved"
						/>
					</el-form-item>
				</el-col>
				<el-col :span="12">
					<el-form-item label="姓名">
						<el-input v-model="state.form.empName" disabled />
					</el-form-item>
				</el-col>
				<el-col :span="12">
					<el-form-item label="部门">
						<el-input v-model="state.form.ccDescSnap" disabled />
					</el-form-item>
				</el-col>
				<el-col :span="12">
					<el-form-item label="离职日" prop="leaveDate">
						<el-date-picker v-model="state.form.leaveDate" type="date" value-format="YYYY-MM-DD" class="w100" />
					</el-form-item>
				</el-col>
				<el-col :span="12">
					<el-form-item label="计划生效日">
						<el-input :model-value="planEffectText" disabled />
					</el-form-item>
				</el-col>
				<el-col :span="12">
					<el-form-item label="离职类型" prop="leaveType">
						<el-select v-model="state.form.leaveType" clearable filterable class="w100">
							<el-option v-for="c in typeOptions" :key="c.value" :label="c.label || c.text" :value="c.value" />
						</el-select>
					</el-form-item>
				</el-col>
				<el-col :span="12">
					<el-form-item label="离职原因" prop="leaveReason">
						<el-select v-model="state.form.leaveReason" clearable filterable class="w100">
							<el-option v-for="c in reasonOptions" :key="c.value" :label="c.label || c.text" :value="c.value" />
						</el-select>
					</el-form-item>
				</el-col>
				<el-col :span="12">
					<el-form-item label="薪资结算" prop="salarySettle">
						<el-select v-model="state.form.salarySettle" clearable filterable class="w100">
							<el-option v-for="c in settleOptions" :key="c.value" :label="c.label || c.text" :value="c.value" />
						</el-select>
					</el-form-item>
				</el-col>
				<el-col :span="12">
					<el-form-item label="黑名单">
						<el-switch v-model="state.form.isBlacklist" :disabled="!canEdit" />
					</el-form-item>
				</el-col>
				<el-col :span="24">
					<el-form-item label="备注">
						<el-input v-model="state.form.remark" type="textarea" :rows="2" maxlength="500" />
					</el-form-item>
				</el-col>
			</el-row>
			<el-alert v-if="state.form.rejectReason" type="error" :title="`驳回原因：${state.form.rejectReason}`" show-icon :closable="false" class="mb10" />
			<el-alert v-if="state.form.voidReason" type="info" :title="`撤销原因：${state.form.voidReason}`" show-icon :closable="false" class="mb10" />
		</el-form>
		<template #footer>
			<div class="footer-btns">
				<el-button @click="state.visible = false">关闭</el-button>
				<el-button v-if="canEdit" type="primary" :loading="state.saving" v-auth="'hrEmpLeave/saveDraft'" @click="save(false)">保存</el-button>
				<el-button v-if="canEdit" type="success" :loading="state.saving" v-auth="'hrEmpLeave/submit'" @click="save(true)">保存并提交</el-button>
				<el-button v-if="state.form.status === 'InReview'" type="success" :loading="state.saving" v-auth="'hrEmpLeave/approve'" @click="doApprove">通过</el-button>
				<el-button v-if="state.form.status === 'InReview'" type="danger" :loading="state.saving" v-auth="'hrEmpLeave/reject'" @click="doReject">驳回</el-button>
				<el-button v-if="state.form.status === 'Effective'" type="warning" :loading="state.saving" v-auth="'hrEmpLeave/void'" @click="doVoid">撤销</el-button>
			</div>
		</template>
	</el-dialog>
</template>

<script lang="ts" setup>
import { computed, nextTick, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox, FormInstance, FormRules } from 'element-plus';
import EmpIdInput, { type ScopedEmp } from '/@/components/selector/empIdInput.vue';
import {
	approveHrEmpLeave,
	getHrEmpLeaveDetail,
	rejectHrEmpLeave,
	saveHrEmpLeaveDraft,
	submitHrEmpLeave,
	voidHrEmpLeave,
} from '/@/api/hrms/hrEmpLeave';

defineProps<{
	typeOptions: Array<{ value: string; label?: string; text?: string }>;
	reasonOptions: Array<{ value: string; label?: string; text?: string }>;
	settleOptions: Array<{ value: string; label?: string; text?: string }>;
}>();
const emits = defineEmits<{ saved: [] }>();
const formRef = ref<FormInstance>();
const empIdRef = ref<InstanceType<typeof EmpIdInput>>();

const state = reactive({
	visible: false,
	saving: false,
	id: undefined as number | undefined,
	form: blankForm(),
});

const canEdit = computed(() => !state.form.status || state.form.status === 'Draft');
const planEffectText = computed(() => {
	const d = state.form.leaveDate;
	if (!d) return '';
	const dt = new Date(d);
	if (Number.isNaN(dt.getTime())) return state.form.planEffectDate || '';
	dt.setDate(dt.getDate() + 1);
	return dt.toISOString().slice(0, 10);
});

const rules: FormRules = {
	empId: [{ required: true, message: '请输入工号', trigger: 'blur' }],
	leaveDate: [{ required: true, message: '请填写离职日', trigger: 'change' }],
	leaveType: [{ required: true, message: '请选择离职类型', trigger: 'change' }],
	leaveReason: [{ required: true, message: '请选择离职原因', trigger: 'change' }],
	salarySettle: [{ required: true, message: '请选择薪资结算', trigger: 'change' }],
};

function blankForm() {
	return {
		leaveNo: '',
		empId: '',
		empName: '',
		ccDescSnap: '',
		leaveDate: '' as string,
		planEffectDate: '',
		leaveType: '',
		leaveReason: '',
		salarySettle: 'NormalPay',
		isBlacklist: false,
		remark: '',
		status: '' as string,
		statusText: '',
		rejectReason: '',
		voidReason: '',
	};
}

const statusLabel = (s?: string) =>
	({ Draft: '草稿', InReview: '审核中', Approved: '待生效', Effective: '已生效', Rejected: '已驳回', Voided: '已作废' } as any)[s || ''] || s;

const onEmpResolved = (emp: ScopedEmp | null) => {
	if (!emp) {
		state.form.empName = '';
		state.form.ccDescSnap = '';
		return;
	}
	state.form.empName = emp.empName || '';
	state.form.ccDescSnap = emp.ccDesc || '';
};

const open = async (id?: number) => {
	state.visible = true;
	state.id = id;
	state.form = blankForm();
	if (id) {
		const res = await getHrEmpLeaveDetail(id);
		const d = res.data?.result ?? res.data;
		state.form = {
			leaveNo: d.leaveNo,
			empId: d.empId,
			empName: d.empName,
			ccDescSnap: d.ccDescSnap || '',
			leaveDate: (d.leaveDate || '').toString().slice(0, 10),
			planEffectDate: (d.planEffectDate || '').toString().slice(0, 10),
			leaveType: d.leaveType,
			leaveReason: d.leaveReason,
			salarySettle: d.salarySettle || 'NormalPay',
			isBlacklist: !!d.isBlacklist,
			remark: d.remark || '',
			status: d.status,
			statusText: d.statusText,
			rejectReason: d.rejectReason || '',
			voidReason: d.voidReason || '',
		};
		await nextTick();
		empIdRef.value?.markResolved(d.empId);
	}
};

const buildPayload = () => ({
	id: state.id,
	empId: state.form.empId,
	leaveDate: state.form.leaveDate,
	leaveType: state.form.leaveType,
	leaveReason: state.form.leaveReason,
	salarySettle: state.form.salarySettle,
	isBlacklist: state.form.isBlacklist,
	remark: state.form.remark,
});

const save = async (andSubmit: boolean) => {
	await formRef.value?.validate();
	if (!state.form.empName) {
		await empIdRef.value?.resolve(true);
		if (!state.form.empName) {
			ElMessage.warning('请输入管辖范围内的有效工号');
			return;
		}
	}
	state.saving = true;
	try {
		const res = await saveHrEmpLeaveDraft(buildPayload());
		const newId = res.data?.result ?? res.data;
		state.id = typeof newId === 'number' ? newId : state.id;
		if (andSubmit && state.id) {
			const sub = await submitHrEmpLeave({ id: state.id });
			const result = sub.data?.result ?? sub.data;
			if (result?.autoApproved) {
				ElMessage.success(result.status === 'Effective' ? '已提交并自动生效' : '已提交并自动通过（待生效）');
			} else {
				ElMessage.success('已提交，请等待审核');
			}
		} else {
			ElMessage.success('已保存');
		}
		emits('saved');
		if (state.id) await open(state.id);
	} finally {
		state.saving = false;
	}
};

const doApprove = async () => {
	if (!state.id) return;
	state.saving = true;
	try {
		await approveHrEmpLeave({ id: state.id });
		ElMessage.success('已通过');
		emits('saved');
		await open(state.id);
	} finally {
		state.saving = false;
	}
};

const doReject = async () => {
	if (!state.id) return;
	const { value } = await ElMessageBox.prompt('请输入驳回原因', '驳回', { confirmButtonText: '确定', cancelButtonText: '取消' }).catch(() => ({ value: null }));
	if (value == null) return;
	state.saving = true;
	try {
		await rejectHrEmpLeave({ id: state.id, rejectReason: value });
		ElMessage.success('已驳回');
		emits('saved');
		await open(state.id);
	} finally {
		state.saving = false;
	}
};

const doVoid = async () => {
	if (!state.id) return;
	const { value } = await ElMessageBox.prompt('请输入撤销原因', '撤销', { confirmButtonText: '确定', cancelButtonText: '取消' }).catch(() => ({ value: null }));
	if (value == null) return;
	state.saving = true;
	try {
		await voidHrEmpLeave({ id: state.id, voidReason: value });
		ElMessage.success('已撤销');
		emits('saved');
		await open(state.id);
	} finally {
		state.saving = false;
	}
};

const onClosed = () => {
	state.id = undefined;
	state.form = blankForm();
};

defineExpose({ open });
</script>

<style scoped>
.w100 {
	width: 100%;
}
.mb10 {
	margin-bottom: 10px;
}
.footer-btns {
	display: flex;
	justify-content: flex-end;
	gap: 8px;
	flex-wrap: wrap;
}
</style>
