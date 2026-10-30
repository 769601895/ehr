<template>
	<!-- 栅格与自定义布局一致：无照片 6 列 / 有照片 5 列 -->
	<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="98%" top="1vh" destroy-on-close class="hr-emp-edit-dialog">
		<template #header>
			<div style="color: #fff">
				<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"><ele-Edit /></el-icon>
				<span>{{ dialogTitle }}</span>
			</div>
		</template>
		<div class="hr-emp-shell">
			<nav class="hr-emp-nav">
				<button v-for="item in sections" :key="item.id" type="button" class="hr-emp-nav-item" :class="{ active: state.section === item.id }" @click="state.section = item.id">
					<span>{{ t(item.title) }}</span>
					<em v-if="item.child">{{ state.childCounts[item.id] || 0 }}</em>
				</button>
			</nav>
			<div class="hr-emp-pane">
		<el-form v-show="!isChildSection" :model="state.ruleForm" ref="ruleFormRef" label-width="70px" size="small" class="hr-emp-edit-form">
			<EmpDynamicForm v-if="state.useDynamic" :rule-form="state.ruleForm" :fields="state.dynFields" :code-options="state.codeOptions" :tree-data="props.treeData" :is-edit="state.isEdit" />
			<template v-else>
			<el-divider content-position="left" class="hr-emp-section-divider">{{ t('message.hrms.common.groupBasic') }}</el-divider>
			<div class="hr-emp-group">
			<el-row :gutter="4">
				<el-col :span="4" class="mb-field">
					<el-form-item label="工号" prop="empId" :rules="[{ required: true, message: '工号不能为空', trigger: 'blur' }]">
						<el-input v-model="state.ruleForm.empId" :disabled="state.isEdit" placeholder="工号" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="姓名" prop="empName" :rules="[{ required: true, message: '姓名不能为空', trigger: 'blur' }]">
						<el-input v-model="state.ruleForm.empName" placeholder="姓名" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="性别">
						<el-input v-model="state.ruleForm.empSex" placeholder="性别" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="出生日期">
						<el-date-picker v-model="state.ruleForm.empBirth" type="date" value-format="YYYY-MM-DD" class="w100" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="年龄">
						<el-input :model-value="ageText" disabled placeholder="自动计算" />
					</el-form-item>
				</el-col>
				<el-col :span="8" class="mb-field">
					<el-form-item label="身份证号">
						<el-input v-model="state.ruleForm.empCard" placeholder="身份证号" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="身份证到期日">
						<el-date-picker v-model="state.ruleForm.empCardOverdate" type="date" value-format="YYYY-MM-DD" class="w100" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="发证日期">
						<el-date-picker v-model="state.ruleForm.empCardDate" type="date" value-format="YYYY-MM-DD" class="w100" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="期限">
						<el-input-number v-model="state.ruleForm.empCardLimit" :min="0" controls-position="right" class="w100" />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="联系电话">
						<el-input v-model="state.ruleForm.empAddrter" placeholder="联系电话" clearable />
					</el-form-item>
				</el-col>

				<el-col :span="4" class="mb-field">
					<el-form-item label="户籍类型">
						<el-input v-model="state.ruleForm.empRgt" placeholder="户籍类型" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="民族">
						<el-input v-model="state.ruleForm.empNation" placeholder="民族" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="省份">
						<el-input v-model="state.ruleForm.empProvince" placeholder="省份" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="婚姻状况">
						<el-input v-model="state.ruleForm.empMarry" placeholder="婚姻状况" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="8" class="mb-field">
					<el-form-item label="户口所在地">
						<el-input v-model="state.ruleForm.empResidence" placeholder="户口所在地" clearable />
					</el-form-item>
				</el-col>

				<el-col :span="8" class="mb-field">
					<el-form-item label="现住址">
						<el-input v-model="state.ruleForm.empAddr" placeholder="现住址" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="8" class="mb-field">
					<el-form-item label="家庭住址">
						<el-input v-model="state.ruleForm.empHome" placeholder="家庭住址" clearable />
					</el-form-item>
				</el-col>
			</el-row>
			</div>

			<el-divider content-position="left" class="hr-emp-section-divider">{{ t('message.hrms.common.groupJob') }}</el-divider>
			<el-row :gutter="4">
				<el-col :span="8" class="mb-field">
					<el-form-item label="部门">
						<el-cascader
							:options="props.treeData"
							:props="cascaderProps"
							placeholder="选择部门"
							clearable
							filterable
							class="w100"
							v-model="state.ruleForm.ccCode"
						/>
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="职务">
						<el-input v-model="state.ruleForm.dutyId" placeholder="职务" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="岗位">
						<el-input v-model="state.ruleForm.postId" placeholder="岗位" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="人员类别">
						<el-input v-model="state.ruleForm.empType" placeholder="人员类别" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="薪酬类别">
						<el-input v-model="state.ruleForm.empSal" placeholder="薪酬类别" clearable maxlength="2" />
					</el-form-item>
				</el-col>

				<el-col :span="4" class="mb-field">
					<el-form-item label="参加工作时间">
						<el-date-picker v-model="state.ruleForm.empWorkDate" type="date" value-format="YYYY-MM-DD" class="w100" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="集团入职日">
						<el-date-picker v-model="state.ruleForm.empInDateGroup" type="date" value-format="YYYY-MM-DD" class="w100" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="公司入职日">
						<el-date-picker v-model="state.ruleForm.empInDate" type="date" value-format="YYYY-MM-DD" class="w100" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="试用起始日">
						<el-date-picker v-model="state.ruleForm.empBegDate" type="date" value-format="YYYY-MM-DD" class="w100" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="试用到期日">
						<el-date-picker v-model="state.ruleForm.empTryDate" type="date" value-format="YYYY-MM-DD" class="w100" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="转正日">
						<el-date-picker v-model="state.ruleForm.empFromDate" type="date" value-format="YYYY-MM-DD" class="w100" clearable />
					</el-form-item>
				</el-col>

				<el-col :span="4" class="mb-field">
					<el-form-item label="入司前工龄(月)">
						<el-input-number v-model="state.ruleForm.empLongmon" :min="0" controls-position="right" class="w100" />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="考勤开始日">
						<el-date-picker v-model="state.ruleForm.empKqIndate" type="date" value-format="YYYY-MM-DD" class="w100" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="考勤结束日">
						<el-date-picker v-model="state.ruleForm.empKqLeaDate" type="date" value-format="YYYY-MM-DD" class="w100" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="4" class="mb-field">
					<el-form-item label="人员状态">
						<el-select v-model="state.ruleForm.empState" class="w100">
							<el-option label="在职" :value="1" />
							<el-option label="停职" :value="2" />
							<el-option label="离职" :value="3" />
						</el-select>
					</el-form-item>
				</el-col>
			</el-row>
			</template>
		</el-form>
		<el-empty v-if="isChildSection && !state.ruleForm.empId" :description="t('message.hrms.emp.needSaveMain')" />
		<EmpChildGrid v-else-if="isChildSection" :section="state.section" :emp-id="state.ruleForm.empId" @changed="loadChildCounts" />
			</div>
		</div>
		<template #footer>
			<span class="dialog-footer">
				<el-button icon="ele-CircleCloseFilled" @click="state.isShowDialog = false">{{ t('message.hrms.emp.cancel') }}</el-button>
				<el-button v-if="!isChildSection" type="primary" icon="ele-CircleCheckFilled" @click="submit">{{ t('message.hrms.emp.saveMain') }}</el-button>
			</span>
		</template>
	</el-dialog>
</template>

<script lang="ts" setup name="hrEditEmp">
import { computed, reactive, ref, watch } from 'vue';
import { ElMessage } from 'element-plus';
import { useI18n } from 'vue-i18n';
import { addHrEmp, updateHrEmp } from '/@/api/hrms/hrEmp';
import { countHrEmpChild } from '/@/api/hrms/hrEmpChild';
import { getHrCodeList } from '/@/api/hrms/hrOrg';
import { getHrLookupOptions } from '/@/api/hrms/hrLookupDef';
import { getHrUiLayoutEffective } from '/@/api/hrms/hrUiLayout';
import EmpDynamicForm from '/@/views/hrms/emp/component/empDynamicForm.vue';
import EmpChildGrid from '/@/views/hrms/emp/component/empChildGrid.vue';

const props = defineProps({
	title: String,
	treeData: { type: Array as () => any[], default: () => [] },
});
const emits = defineEmits(['reload']);
const { locale, t } = useI18n();
const ruleFormRef = ref();
const cascaderProps = { checkStrictly: true, emitPath: false, value: 'ccCode', label: 'ccDesc', children: 'children' };

const sections = [
	{ id: 'profile', title: 'message.hrms.emp.sectionProfile' },
	{ id: 'family', title: 'message.hrms.emp.sectionFamily', child: true },
	{ id: 'edu', title: 'message.hrms.emp.sectionEdu', child: true },
	{ id: 'work', title: 'message.hrms.emp.sectionWork', child: true },
	{ id: 'title', title: 'message.hrms.emp.sectionTitle', child: true },
];

const state = reactive({
	isShowDialog: false,
	isEdit: false,
	section: 'profile',
	ruleForm: {} as any,
	useDynamic: false,
	dynFields: [] as any[],
	codeOptions: {} as Record<string, { value: string; label?: string; text?: string }[]>,
	childCounts: { family: 0, edu: 0, work: 0, title: 0 } as Record<string, number>,
});

const isChildSection = computed(() => sections.some((s) => s.child && s.id === state.section));

const dialogTitle = computed(() => {
	const empId = String(state.ruleForm?.empId ?? '').trim();
	const empName = String(state.ruleForm?.empName ?? '').trim();
	if (empId && empName) return `${empId} ${empName}`;
	if (empId) return empId;
	if (empName) return empName;
	return props.title || '';
});

const loadChildCounts = async () => {
	const empId = state.ruleForm.empId;
	if (!empId) {
		state.childCounts = { family: 0, edu: 0, work: 0, title: 0 };
		return;
	}
	try {
		const res = await countHrEmpChild(empId);
		const c = res.data.result ?? {};
		state.childCounts = {
			family: c.family ?? 0,
			edu: c.edu ?? 0,
			work: c.work ?? 0,
			title: c.title ?? 0,
		};
	} catch {
		state.childCounts = { family: 0, edu: 0, work: 0, title: 0 };
	}
};

const registered = new Set(['ccCode', 'empPic', 'age', 'empState']);

const loadDynamic = async () => {
	try {
		const res = await getHrUiLayoutEffective('hrEmp', 'form');
		const fields = (res.data.result?.fields ?? []).filter((f: any) => f.visible !== false && !f.isStale);
		if (!fields.length) {
			state.useDynamic = false;
			return;
		}
		const codes = [...new Set(fields.filter((f: any) => f.codeField && !f.lookupSource && !registered.has(f.fieldKey)).map((f: any) => f.codeField as string))];
		const lookups = [...new Set(fields.filter((f: any) => f.lookupSource && !registered.has(f.fieldKey)).map((f: any) => String(f.lookupSource)))];
		const codeEntries = await Promise.all(codes.map(async (c) => [c, (await getHrCodeList(c)).data.result ?? []] as const));
		const lookupEntries = await Promise.all(
			lookups.map(async (s) => [`lookup:${s}`, (await getHrLookupOptions(s)).data?.result ?? []] as const)
		);
		state.codeOptions = Object.fromEntries([...codeEntries, ...lookupEntries]);
		state.dynFields = fields;
		state.useDynamic = true;
	} catch {
		state.useDynamic = false;
	}
};

const ageText = computed(() => {
	const birth = state.ruleForm.empBirth;
	if (!birth) return '';
	const d = new Date(birth);
	if (Number.isNaN(d.getTime())) return '';
	const today = new Date();
	let age = today.getFullYear() - d.getFullYear();
	const m = today.getMonth() - d.getMonth();
	if (m < 0 || (m === 0 && today.getDate() < d.getDate())) age--;
	return age < 0 ? '0' : String(age);
});

const openDialog = (row: any, isEdit = false) => {
	state.isEdit = isEdit;
	state.section = 'profile';
	state.ruleForm = JSON.parse(JSON.stringify(row ?? {}));
	if (state.ruleForm.empState == null) state.ruleForm.empState = 1;
	if (!isEdit && row?.ccCode) state.ruleForm.ccCode = row.ccCode;
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();
	loadDynamic();
	loadChildCounts();
};

watch(locale, () => {
	if (state.isShowDialog) loadDynamic();
});

const submit = () => {
	if (!String(state.ruleForm.empId || '').trim() || !String(state.ruleForm.empName || '').trim()) {
		state.section = 'profile';
		ElMessage.warning(t('message.hrms.emp.mainRequired'));
		return;
	}
	ruleFormRef.value?.validate(async (valid: boolean) => {
		if (!valid) return;
		if (state.isEdit) {
			await updateHrEmp(state.ruleForm);
		} else {
			await addHrEmp(state.ruleForm);
			state.isEdit = true;
		}
		ElMessage.success(t('message.hrms.emp.mainSaved'));
		emits('reload');
	});
};

defineExpose({ openDialog });
</script>

<style scoped>
.w100 {
	width: 100%;
}
.mb-field {
	margin-bottom: 2px;
}
.hr-emp-section-divider {
	margin: 8px 0 10px;
}
/* 收紧表单项间距，输入区拉满列宽 */
.hr-emp-edit-form :deep(.el-form-item) {
	margin-bottom: 4px;
}
.hr-emp-edit-form :deep(.el-form-item__label) {
	padding-right: 6px;
	height: auto;
	line-height: 1.2;
	white-space: normal;
	overflow: hidden;
	display: inline-flex;
	align-items: center;
	justify-content: flex-end;
}
.hr-emp-edit-form :deep(.el-date-editor),
.hr-emp-edit-form :deep(.el-date-editor.el-input),
.hr-emp-edit-form :deep(.el-date-editor.el-input__wrapper),
.hr-emp-edit-form :deep(.el-input-number),
.hr-emp-edit-form :deep(.el-select),
.hr-emp-edit-form :deep(.el-cascader) {
	width: 100%;
	max-width: 100%;
}
.hr-emp-edit-form :deep(.el-form-item__content) {
	min-width: 0;
	flex: 1;
}
.hr-emp-edit-form :deep(.el-date-editor .el-input__wrapper) {
	padding-left: 4px;
	padding-right: 4px;
}
.hr-emp-edit-form :deep(.el-date-editor .el-input__inner) {
	font-variant-numeric: tabular-nums;
}
.hr-emp-shell {
	display: flex;
	min-height: calc(100vh - 140px);
}
.hr-emp-nav {
	width: 88px;
	flex-shrink: 0;
	border-right: 1px solid var(--el-border-color-lighter);
	padding: 4px 0;
}
.hr-emp-nav-item {
	width: 100%;
	display: flex;
	align-items: center;
	justify-content: space-between;
	gap: 2px;
	border: 0;
	border-left: 3px solid transparent;
	background: transparent;
	padding: 7px 4px 7px 6px;
	cursor: pointer;
	color: var(--el-text-color-primary);
	font-size: 12px;
	line-height: 1.2;
	text-align: left;
}
.hr-emp-nav-item span {
	flex: 1;
	min-width: 0;
}
.hr-emp-nav-item em {
	font-style: normal;
	color: var(--el-color-primary);
	font-size: 12px;
	flex-shrink: 0;
}
.hr-emp-nav-item.active {
	color: var(--el-color-primary);
	background: var(--el-color-primary-light-9);
	border-left-color: var(--el-color-primary);
}
.hr-emp-pane {
	flex: 1;
	min-width: 0;
	padding-left: 12px;
	max-height: calc(100vh - 110px);
	overflow: auto;
}
</style>

<style>
/* dialog 非 scoped */
.hr-emp-edit-dialog {
	max-width: 1880px;
	margin-top: 1vh !important;
}
.hr-emp-edit-dialog .el-dialog__body {
	padding: 6px 12px 4px;
	max-height: calc(100vh - 90px);
	overflow: hidden;
}
</style>
