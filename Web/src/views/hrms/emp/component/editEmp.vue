<template>
	<!-- 栅格：一行 6 列 → span=4；长字段占 2 列 → span=8；加宽以保证日期完整显示 -->
	<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="94%" top="4vh" destroy-on-close class="hr-emp-edit-dialog">
		<template #header>
			<div style="color: #fff">
				<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"><ele-Edit /></el-icon>
				<span>{{ props.title }}</span>
			</div>
		</template>
		<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="96px" size="small" class="hr-emp-edit-form">
			<el-divider content-position="left" class="hr-emp-section-divider">基本信息</el-divider>
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
				<el-col :span="4" class="mb-field">
					<div class="emp-pic-box">
						<el-upload
							class="emp-pic-uploader"
							action
							:show-file-list="false"
							:auto-upload="false"
							accept=".jpg,.jpeg,.png,.bmp,.gif"
							:disabled="state.picUploading"
							:on-change="handlePicChange"
						>
							<img v-if="state.ruleForm.empPic" :src="state.ruleForm.empPic" class="emp-pic-img" />
							<div v-else class="emp-pic-placeholder">
								<el-icon :size="22"><ele-Plus /></el-icon>
								<div class="emp-pic-tip">照片</div>
							</div>
						</el-upload>
						<div class="emp-pic-actions" v-if="state.ruleForm.empPic">
							<el-button type="danger" link size="small" @click="clearPic">清除</el-button>
						</div>
					</div>
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

			<el-divider content-position="left" class="hr-emp-section-divider">任职信息</el-divider>
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
		</el-form>
		<template #footer>
			<span class="dialog-footer">
				<el-button icon="ele-CircleCloseFilled" @click="state.isShowDialog = false">取消</el-button>
				<el-button type="primary" icon="ele-CircleCheckFilled" @click="submit">确定</el-button>
			</span>
		</template>
	</el-dialog>
</template>

<script lang="ts" setup name="hrEditEmp">
import { computed, reactive, ref } from 'vue';
import { ElMessage } from 'element-plus';
import type { UploadFile } from 'element-plus';
import { addHrEmp, updateHrEmp } from '/@/api/hrms/hrEmp';
import { getAPI } from '/@/utils/axios-utils';
import { SysFileApi } from '/@/api-services/system/api';

const props = defineProps({
	title: String,
	treeData: { type: Array as () => any[], default: () => [] },
});
const emits = defineEmits(['reload']);
const ruleFormRef = ref();
const cascaderProps = { checkStrictly: true, emitPath: false, value: 'ccCode', label: 'ccDesc', children: 'children' };

const state = reactive({
	isShowDialog: false,
	isEdit: false,
	picUploading: false,
	ruleForm: {} as any,
});

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
	state.ruleForm = JSON.parse(JSON.stringify(row ?? {}));
	if (state.ruleForm.empState == null) state.ruleForm.empState = 1;
	if (!isEdit && row?.ccCode) state.ruleForm.ccCode = row.ccCode;
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();
};

const handlePicChange = async (uploadFile: UploadFile) => {
	const raw = uploadFile.raw;
	if (!raw) return;
	const allow = ['image/jpeg', 'image/png', 'image/bmp', 'image/gif', 'image/jpg'];
	if (raw.type && !allow.includes(raw.type.toLowerCase())) {
		ElMessage.warning('仅支持 jpg/png/bmp/gif');
		return;
	}
	if (raw.size > 5 * 1024 * 1024) {
		ElMessage.warning('照片不能超过 5MB');
		return;
	}
	state.picUploading = true;
	try {
		const res = await getAPI(SysFileApi).apiSysFileUploadFilePostForm(raw, 'hrEmp', undefined, '.jpg,.jpeg,.png,.bmp,.gif');
		const url = res.data.result?.url;
		if (!url) {
			ElMessage.error('上传失败');
			return;
		}
		state.ruleForm.empPic = url;
		ElMessage.success('照片已上传');
	} finally {
		state.picUploading = false;
	}
};

const clearPic = () => {
	state.ruleForm.empPic = '';
};

const submit = () => {
	ruleFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		if (state.isEdit) {
			await updateHrEmp(state.ruleForm);
		} else {
			await addHrEmp(state.ruleForm);
		}
		state.isShowDialog = false;
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
}
.hr-emp-edit-form :deep(.el-date-editor.el-input),
.hr-emp-edit-form :deep(.el-date-editor.el-input__wrapper),
.hr-emp-edit-form :deep(.el-input-number),
.hr-emp-edit-form :deep(.el-select),
.hr-emp-edit-form :deep(.el-cascader) {
	width: 100%;
}
.hr-emp-edit-form :deep(.el-form-item__content) {
	min-width: 0;
	flex: 1;
}
.hr-emp-edit-form :deep(.el-date-editor .el-input__wrapper) {
	padding-left: 6px;
	padding-right: 6px;
}
.emp-pic-box {
	display: flex;
	flex-direction: column;
	align-items: center;
}
.emp-pic-label {
	font-size: 13px;
	color: var(--el-text-color-regular);
	margin-bottom: 6px;
}
.emp-pic-uploader :deep(.el-upload) {
	border: 1px dashed var(--el-border-color);
	border-radius: 6px;
	cursor: pointer;
	overflow: hidden;
	width: 100%;
	max-width: 110px;
	height: 132px;
	display: flex;
	align-items: center;
	justify-content: center;
	background: var(--el-fill-color-blank);
}
.emp-pic-uploader :deep(.el-upload:hover) {
	border-color: var(--el-color-primary);
}
.emp-pic-img {
	width: 100%;
	height: 132px;
	object-fit: cover;
	display: block;
}
.emp-pic-placeholder {
	display: flex;
	flex-direction: column;
	align-items: center;
	justify-content: center;
	color: var(--el-text-color-secondary);
	gap: 6px;
}
.emp-pic-tip {
	font-size: 12px;
}
.emp-pic-actions {
	margin-top: 4px;
}
</style>

<style>
/* dialog 非 scoped：限制最大宽度，小屏仍用 94% */
.hr-emp-edit-dialog {
	max-width: 1760px;
}
.hr-emp-edit-dialog .el-dialog__body {
	padding: 6px 12px 4px;
}
</style>
