<template>
	<el-dialog
		v-model="state.visible"
		:title="state.id ? `异动单 ${state.form.chgNo || ''}` : '新增异动'"
		width="860px"
		destroy-on-close
		:close-on-click-modal="false"
		@closed="onClosed"
	>
		<el-form ref="formRef" :model="state.form" :rules="rules" label-width="100px" :disabled="!canEdit">
			<el-row :gutter="12">
				<el-col :span="12">
					<el-form-item label="异动单号">
						<el-input :model-value="state.form.chgNo || '保存后自动生成'" disabled />
					</el-form-item>
				</el-col>
				<el-col :span="12">
					<el-form-item label="状态">
						<el-tag size="small">{{ state.form.statusText || statusLabel(state.form.status) || '草稿' }}</el-tag>
					</el-form-item>
				</el-col>
				<el-col :span="12">
					<el-form-item label="工号" prop="empId">
						<div class="pick-row">
							<el-input v-model="state.form.empId" placeholder="工号" readonly />
							<el-button v-if="canEdit" @click="state.empPickVisible = true">选人</el-button>
						</div>
					</el-form-item>
				</el-col>
				<el-col :span="12">
					<el-form-item label="姓名">
						<el-input v-model="state.form.empName" disabled />
					</el-form-item>
				</el-col>
				<el-col :span="12">
					<el-form-item label="异动日期" prop="chgDate">
						<el-date-picker v-model="state.form.chgDate" type="date" value-format="YYYY-MM-DD" class="w100" />
					</el-form-item>
				</el-col>
				<el-col :span="12">
					<el-form-item label="异动类别" prop="chgType">
						<el-select v-model="state.form.chgType" clearable filterable class="w100" placeholder="请选择">
							<el-option v-for="c in typeOptions" :key="c.value" :label="c.label || c.text" :value="c.value" />
						</el-select>
					</el-form-item>
				</el-col>
				<el-col :span="24">
					<el-form-item label="异动原因">
						<el-input v-model="state.form.chgReason" maxlength="500" />
					</el-form-item>
				</el-col>
				<el-col :span="24">
					<el-form-item label="备注">
						<el-input v-model="state.form.remark" type="textarea" :rows="2" maxlength="500" />
					</el-form-item>
				</el-col>
			</el-row>

			<el-divider content-position="left">组织变更（左右对比）</el-divider>
			<el-row :gutter="16">
				<el-col :span="12">
					<div class="compare-title">异动前（只读）</div>
					<el-form-item label="部门">
						<el-input :model-value="beforeText('ccCode')" disabled />
					</el-form-item>
					<el-form-item label="岗位">
						<el-input :model-value="beforeText('postId')" disabled />
					</el-form-item>
				</el-col>
				<el-col :span="12">
					<div class="compare-title">异动后</div>
					<el-form-item label="新部门" required>
						<el-cascader
							v-model="state.afterCc"
							:options="state.deptTree"
							:props="cascaderProps"
							clearable
							filterable
							class="w100"
							:disabled="!canEdit"
							placeholder="选择新部门"
						/>
					</el-form-item>
					<el-form-item label="新岗位">
						<el-select v-model="state.afterPost" clearable filterable class="w100" :disabled="!canEdit" placeholder="可选，空则不改岗位">
							<el-option v-for="p in state.postOptions" :key="p.value" :label="p.label || p.text" :value="p.value" />
						</el-select>
					</el-form-item>
				</el-col>
			</el-row>

			<el-alert v-if="state.form.rejectReason" type="error" :title="`驳回原因：${state.form.rejectReason}`" show-icon :closable="false" class="mb10" />
			<el-alert v-if="state.form.voidReason" type="info" :title="`撤销原因：${state.form.voidReason}`" show-icon :closable="false" class="mb10" />
		</el-form>

		<template #footer>
			<div class="footer-btns">
				<el-button @click="state.visible = false">关闭</el-button>
				<el-button v-if="canEdit" type="primary" :loading="state.saving" v-auth="'hrEmpChg/saveDraft'" @click="save(false)">保存</el-button>
				<el-button v-if="canEdit" type="success" :loading="state.saving" v-auth="'hrEmpChg/submit'" @click="save(true)">保存并提交</el-button>
				<el-button v-if="state.form.status === 'InReview'" type="success" :loading="state.saving" v-auth="'hrEmpChg/approve'" @click="doApprove">通过</el-button>
				<el-button v-if="state.form.status === 'InReview'" type="danger" :loading="state.saving" v-auth="'hrEmpChg/reject'" @click="doReject">驳回</el-button>
				<el-button v-if="state.form.status === 'Effective'" type="warning" :loading="state.saving" v-auth="'hrEmpChg/void'" @click="doVoid">撤销</el-button>
			</div>
		</template>

		<!-- 选人 -->
		<el-dialog v-model="state.empPickVisible" title="选择员工" width="720px" append-to-body destroy-on-close>
			<div class="emp-pick-bar">
				<el-input v-model="state.empKeyword" placeholder="工号/姓名" clearable style="width: 220px" @keyup.enter="loadEmpPage" />
				<el-button type="primary" @click="loadEmpPage">查询</el-button>
			</div>
			<el-table :data="state.empRows" height="360" highlight-current-row @row-dblclick="pickEmp" @current-change="(r: any) => (state.empCurrent = r)">
				<el-table-column prop="empId" label="工号" width="120" />
				<el-table-column prop="empName" label="姓名" width="120" />
				<el-table-column prop="ccDesc" label="部门" min-width="160" />
				<el-table-column prop="postId" label="岗位" width="120" />
			</el-table>
			<template #footer>
				<el-button @click="state.empPickVisible = false">取消</el-button>
				<el-button type="primary" @click="confirmPickEmp">选定</el-button>
			</template>
		</el-dialog>
	</el-dialog>
</template>

<script lang="ts" setup>
import { computed, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox, FormInstance, FormRules } from 'element-plus';
import { pageHrEmp } from '/@/api/hrms/hrEmp';
import { getHrOrgList } from '/@/api/hrms/hrOrg';
import { getHrLookupOptions } from '/@/api/hrms/hrLookupDef';
import {
	approveHrEmpChg,
	getHrEmpChgDetail,
	previewHrEmpChgDefaults,
	rejectHrEmpChg,
	saveHrEmpChgDraft,
	submitHrEmpChg,
	voidHrEmpChg,
} from '/@/api/hrms/hrEmpChg';

const props = defineProps<{
	typeOptions: Array<{ value: string; label?: string; text?: string }>;
}>();
const emits = defineEmits<{ saved: [] }>();

const formRef = ref<FormInstance>();
const cascaderProps = { checkStrictly: true, emitPath: false, value: 'ccCode', label: 'ccDesc', children: 'children' };

const state = reactive({
	visible: false,
	saving: false,
	id: undefined as number | undefined,
	form: blankForm(),
	lines: [] as any[],
	afterCc: '' as string | undefined,
	afterPost: '' as string | undefined,
	deptTree: [] as any[],
	postOptions: [] as Array<{ value: string; label?: string; text?: string }>,
	empPickVisible: false,
	empKeyword: '',
	empRows: [] as any[],
	empCurrent: null as any,
});

const canEdit = computed(() => !state.form.status || state.form.status === 'Draft');

const rules: FormRules = {
	empId: [{ required: true, message: '请选择员工', trigger: 'change' }],
	chgDate: [{ required: true, message: '请填写异动日期', trigger: 'change' }],
	chgType: [{ required: true, message: '请选择异动类别', trigger: 'change' }],
};

function blankForm() {
	return {
		chgNo: '',
		empId: '',
		empName: '',
		chgDate: '' as string,
		chgType: '',
		chgReason: '',
		remark: '',
		status: '' as string,
		statusText: '',
		rejectReason: '',
		voidReason: '',
	};
}

const statusLabel = (s?: string) =>
	({ Draft: '草稿', InReview: '审核中', Approved: '待生效', Effective: '已生效', Rejected: '已驳回', Voided: '已作废' } as any)[s || ''] || s;

const lineOf = (key: string) => state.lines.find((u) => u.fieldKey === key);
const beforeText = (key: string) => {
	const l = lineOf(key);
	return l?.valueBeforeText || l?.valueBefore || '';
};

const open = async (id?: number) => {
	state.visible = true;
	state.id = id;
	state.form = blankForm();
	state.lines = [];
	state.afterCc = undefined;
	state.afterPost = undefined;
	await Promise.all([loadDeptTree(), loadPosts()]);
	if (id) {
		const res = await getHrEmpChgDetail(id);
		const d = res.data?.result ?? res.data;
		state.form = {
			chgNo: d.chgNo,
			empId: d.empId,
			empName: d.empName,
			chgDate: (d.chgDate || '').toString().slice(0, 10),
			chgType: d.chgType,
			chgReason: d.chgReason || '',
			remark: d.remark || '',
			status: d.status,
			statusText: d.statusText,
			rejectReason: d.rejectReason || '',
			voidReason: d.voidReason || '',
		};
		state.lines = d.lines || [];
		state.afterCc = lineOf('ccCode')?.valueAfter || undefined;
		state.afterPost = lineOf('postId')?.valueAfter || undefined;
	}
};

const loadDeptTree = async () => {
	try {
		const res = await getHrOrgList({});
		state.deptTree = res.data?.result ?? res.data ?? [];
	} catch {
		state.deptTree = [];
	}
};

const loadPosts = async () => {
	try {
		const res = await getHrLookupOptions('hrPost');
		state.postOptions = res.data?.result ?? res.data ?? [];
	} catch {
		state.postOptions = [];
	}
};

const loadEmpPage = async () => {
	const res = await pageHrEmp({
		page: 1,
		pageSize: 50,
		keyword: state.empKeyword || undefined,
		empState: 1,
		field: 'empId',
		order: 'asc',
		descStr: 'desc',
	});
	const page = res.data?.result ?? res.data;
	state.empRows = page?.items ?? [];
};

const pickEmp = async (row: any) => {
	state.empCurrent = row;
	await confirmPickEmp();
};

const confirmPickEmp = async () => {
	const row = state.empCurrent;
	if (!row?.empId) {
		ElMessage.warning('请选择员工');
		return;
	}
	state.form.empId = row.empId;
	state.form.empName = row.empName || '';
	state.empPickVisible = false;
	const res = await previewHrEmpChgDefaults(row.empId);
	state.lines = res.data?.result ?? res.data ?? [];
	state.afterCc = lineOf('ccCode')?.valueAfter || undefined;
	state.afterPost = lineOf('postId')?.valueAfter || undefined;
};

const buildPayload = () => ({
	id: state.id,
	empId: state.form.empId,
	chgDate: state.form.chgDate,
	chgType: state.form.chgType,
	chgReason: state.form.chgReason,
	remark: state.form.remark,
	reloadFromEmp: false,
	lines: [
		{ fieldKey: 'ccCode', valueAfter: state.afterCc || '' },
		{ fieldKey: 'postId', valueAfter: state.afterPost || '' },
	],
});

const save = async (andSubmit: boolean) => {
	await formRef.value?.validate();
	if (!state.afterCc) {
		ElMessage.warning('新部门不能为空');
		return;
	}
	state.saving = true;
	try {
		const res = await saveHrEmpChgDraft(buildPayload());
		const newId = res.data?.result ?? res.data;
		state.id = typeof newId === 'number' ? newId : state.id;
		if (andSubmit && state.id) {
			const sub = await submitHrEmpChg({ id: state.id });
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
		await approveHrEmpChg({ id: state.id });
		ElMessage.success('已通过');
		emits('saved');
		await open(state.id);
	} finally {
		state.saving = false;
	}
};

const doReject = async () => {
	if (!state.id) return;
	const { value } = await ElMessageBox.prompt('请输入驳回原因（可空）', '驳回', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		inputPlaceholder: '驳回原因',
	}).catch(() => ({ value: null as any }));
	if (value === null) return;
	state.saving = true;
	try {
		await rejectHrEmpChg({ id: state.id, rejectReason: value || undefined });
		ElMessage.success('已驳回');
		emits('saved');
		await open(state.id);
	} finally {
		state.saving = false;
	}
};

const doVoid = async () => {
	if (!state.id) return;
	const { value } = await ElMessageBox.prompt('请输入撤销原因（可空）', '撤销异动', {
		confirmButtonText: '确定',
		cancelButtonText: '取消',
		inputPlaceholder: '撤销原因',
		type: 'warning',
	}).catch(() => ({ value: null as any }));
	if (value === null) return;
	state.saving = true;
	try {
		await voidHrEmpChg({ id: state.id, voidReason: value || undefined });
		ElMessage.success('已撤销并回滚主档');
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
.pick-row {
	display: flex;
	gap: 8px;
	width: 100%;
}
.compare-title {
	font-weight: 600;
	margin-bottom: 8px;
	color: var(--el-text-color-primary);
}
.footer-btns {
	display: flex;
	flex-wrap: wrap;
	justify-content: flex-end;
	gap: 8px;
}
.emp-pick-bar {
	display: flex;
	gap: 8px;
	margin-bottom: 10px;
}
.mb10 {
	margin-bottom: 10px;
}
</style>
