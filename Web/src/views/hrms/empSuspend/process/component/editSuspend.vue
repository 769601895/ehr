<template>
	<el-dialog
		v-model="state.visible"
		:title="state.id ? `停职单 ${state.form.suspendNo || ''}` : '新增停职'"
		width="640px"
		destroy-on-close
		:close-on-click-modal="false"
		@closed="onClosed"
	>
		<el-form ref="formRef" :model="state.form" :rules="rules" label-width="100px" :disabled="!canEdit">
			<el-row :gutter="12">
				<el-col :span="12">
					<el-form-item label="停职单号">
						<el-input :model-value="state.form.suspendNo || '保存后自动生成'" disabled />
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
							pick-title="选择在职员工"
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
					<el-form-item label="停职日" prop="suspendDate">
						<el-date-picker v-model="state.form.suspendDate" type="date" value-format="YYYY-MM-DD" class="w100" />
					</el-form-item>
				</el-col>
				<el-col :span="24">
					<el-form-item label="停职原因">
						<el-input v-model="state.form.suspendReason" maxlength="200" />
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
				<el-button v-if="canEdit" type="primary" :loading="state.saving" v-auth="'hrEmpSuspend/saveDraft'" @click="save(false)">保存</el-button>
				<el-button v-if="canEdit" type="success" :loading="state.saving" v-auth="'hrEmpSuspend/submit'" @click="save(true)">保存并提交</el-button>
				<el-button v-if="state.form.status === 'InReview'" type="success" :loading="state.saving" v-auth="'hrEmpSuspend/approve'" @click="doApprove">通过</el-button>
				<el-button v-if="state.form.status === 'InReview'" type="danger" :loading="state.saving" v-auth="'hrEmpSuspend/reject'" @click="doReject">驳回</el-button>
				<el-button v-if="state.form.status === 'Effective'" type="warning" :loading="state.saving" v-auth="'hrEmpSuspend/void'" @click="doVoid">撤销</el-button>
			</div>
		</template>
	</el-dialog>
</template>

<script lang="ts" setup>
import { computed, nextTick, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox, FormInstance, FormRules } from 'element-plus';
import EmpIdInput, { type ScopedEmp } from '/@/components/selector/empIdInput.vue';
import {
	approveHrEmpSuspend,
	getHrEmpSuspendDetail,
	rejectHrEmpSuspend,
	saveHrEmpSuspendDraft,
	submitHrEmpSuspend,
	voidHrEmpSuspend,
} from '/@/api/hrms/hrEmpSuspend';

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
const rules: FormRules = {
	empId: [{ required: true, message: '请输入工号', trigger: 'blur' }],
	suspendDate: [{ required: true, message: '请填写停职日', trigger: 'change' }],
};

function blankForm() {
	return {
		suspendNo: '',
		empId: '',
		empName: '',
		ccDescSnap: '',
		suspendDate: '' as string,
		suspendReason: '',
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
	state.form.empName = emp?.empName || '';
	state.form.ccDescSnap = emp?.ccDesc || '';
};

const open = async (id?: number) => {
	state.visible = true;
	state.id = id;
	state.form = blankForm();
	if (id) {
		const res = await getHrEmpSuspendDetail(id);
		const d = res.data?.result ?? res.data;
		state.form = {
			suspendNo: d.suspendNo,
			empId: d.empId,
			empName: d.empName,
			ccDescSnap: d.ccDescSnap || '',
			suspendDate: (d.suspendDate || '').toString().slice(0, 10),
			suspendReason: d.suspendReason || '',
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
	suspendDate: state.form.suspendDate,
	suspendReason: state.form.suspendReason,
	remark: state.form.remark,
});

const save = async (andSubmit: boolean) => {
	await formRef.value?.validate();
	if (!state.form.empName) {
		await empIdRef.value?.resolve(true);
		if (!state.form.empName) {
			ElMessage.warning('请输入管辖范围内的有效在职工号');
			return;
		}
	}
	state.saving = true;
	try {
		const res = await saveHrEmpSuspendDraft(buildPayload());
		const newId = res.data?.result ?? res.data;
		state.id = typeof newId === 'number' ? newId : state.id;
		if (andSubmit && state.id) {
			const sub = await submitHrEmpSuspend({ id: state.id });
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
		await approveHrEmpSuspend({ id: state.id });
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
		await rejectHrEmpSuspend({ id: state.id, rejectReason: value });
		ElMessage.success('已驳回');
		emits('saved');
		await open(state.id);
	} finally {
		state.saving = false;
	}
};

const doVoid = async () => {
	if (!state.id) return;
	const { value } = await ElMessageBox.prompt('请输入撤销原因', '撤销停职', { confirmButtonText: '确定', cancelButtonText: '取消' }).catch(() => ({ value: null }));
	if (value == null) return;
	state.saving = true;
	try {
		await voidHrEmpSuspend({ id: state.id, voidReason: value });
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
