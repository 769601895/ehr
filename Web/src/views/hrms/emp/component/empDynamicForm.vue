<template>
	<template v-for="group in groups" :key="group.code">
		<el-divider content-position="left" class="hr-emp-section-divider">{{ group.title }}</el-divider>
		<div class="hr-emp-group" :class="{ 'has-photo': !!group.photo }">
			<div v-if="group.photo" class="emp-pic-anchor">
				<div class="emp-pic-box">
					<el-upload class="emp-pic-uploader" action :show-file-list="false" :auto-upload="false" accept=".jpg,.jpeg,.png,.bmp,.gif" :disabled="picUploading || group.photo.readonly" :on-change="handlePicChange">
						<img v-if="ruleForm.empPic" :src="ruleForm.empPic" class="emp-pic-img" />
						<div v-else class="emp-pic-placeholder">
							<el-icon :size="22"><ele-Plus /></el-icon>
							<div class="emp-pic-tip">{{ group.photo.label }}</div>
						</div>
					</el-upload>
					<div class="emp-pic-actions" v-if="ruleForm.empPic && !group.photo.readonly">
						<el-button type="danger" link size="small" @click="ruleForm.empPic = ''">{{ t('message.hrms.common.clear') }}</el-button>
					</div>
				</div>
			</div>
			<!-- 与自定义布局一致：有照片 5 列 / 无照片 6 列；占位以布局 spanSlots 为准 -->
			<div class="hr-emp-fields" :style="{ '--hr-emp-cols': group.photo ? 5 : 6 }">
			<div
				v-for="f in group.fields"
				:key="f.fieldKey"
				class="mb-field"
				:style="{ gridColumn: `span ${fieldSpan(f, group.photo ? 5 : 6)}` }"
			>
				<el-form-item v-if="f.fieldKey === 'age'" :label="f.label">
					<el-input :model-value="ageText" disabled :placeholder="t('message.hrms.common.autoCalc')" />
				</el-form-item>
				<el-form-item v-else-if="f.fieldKey === 'ccCode'" :label="f.label" prop="ccCode">
					<el-cascader :options="treeData" :props="cascaderProps" :placeholder="t('message.hrms.common.selectDept')" clearable filterable class="w100" :disabled="f.readonly" v-model="ruleForm.ccCode" />
				</el-form-item>
				<el-form-item v-else-if="f.fieldKey === 'empState'" :label="f.label" prop="empState">
					<el-select v-model="ruleForm.empState" class="w100" :disabled="f.readonly">
						<el-option :label="t('message.hrms.common.empStateActive')" :value="1" />
						<el-option :label="t('message.hrms.common.empStateSuspend')" :value="2" />
						<el-option :label="t('message.hrms.common.empStateLeave')" :value="3" />
					</el-select>
				</el-form-item>
				<el-form-item v-else :label="f.label" :prop="f.fieldKey" :rules="f.required ? [{ required: true, message: (f.label || f.fieldKey) + t('message.hrms.common.requiredSuffix'), trigger: 'blur' }] : []">
					<el-date-picker
						v-if="f.controlType === 'date'"
						v-model="ruleForm[f.fieldKey]"
						type="date"
						format="YYYY-MM-DD"
						value-format="YYYY-MM-DD"
						class="w100 hr-emp-date"
						clearable
						:disabled="f.readonly"
					/>
					<el-input-number v-else-if="f.controlType === 'number'" v-model="ruleForm[f.fieldKey]" :min="0" controls-position="right" class="w100" :disabled="f.readonly" />
					<el-select v-else-if="f.controlType === 'select' || f.codeField || f.lookupSource" v-model="ruleForm[f.fieldKey]" class="w100" clearable filterable :disabled="f.readonly">
						<el-option v-for="o in codeOptions[optionKey(f)] || []" :key="o.value" :label="codeOptionText(o)" :value="o.value" />
					</el-select>
					<el-input v-else v-model="ruleForm[f.fieldKey]" clearable :disabled="f.readonly || (f.fieldKey === 'empId' && isEdit)" />
				</el-form-item>
			</div>
			</div>
		</div>
	</template>
</template>

<script lang="ts" setup>
import { computed, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage } from 'element-plus';
import type { UploadFile } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { SysFileApi } from '/@/api-services/system/api';
import { normalizeEmpPhoto } from '/@/views/hrms/composables/hrEmpPhoto';

const props = defineProps<{
	ruleForm: any;
	fields: any[];
	codeOptions: Record<string, { value: string; label?: string; text?: string }[]>;
	treeData: any[];
	isEdit: boolean;
	hidePhoto?: boolean;
}>();

const { t } = useI18n();

const optionKey = (f: any) => {
	const lookup = String(f?.lookupSource || '').trim();
	if (lookup) return `lookup:${lookup}`;
	return String(f?.codeField || '');
};

/** 占位完全按自定义布局 spanSlots（1～cols） */
const fieldSpan = (f: any, cols: number) => Math.min(Math.max(Number(f.spanSlots) || 1, 1), cols);

const codeOptionText = (o: { value: string; label?: string; text?: string }) => {
	const text = (o.text ?? '').trim();
	if (text) return text;
	const value = String(o.value ?? '');
	const label = String(o.label ?? '');
	const prefix = value + ' ';
	if (label.startsWith(prefix)) return label.slice(prefix.length).trim() || value;
	return label || value;
};
const cascaderProps = { checkStrictly: true, emitPath: false, value: 'ccCode', label: 'ccDesc', children: 'children' };
const picUploading = ref(false);

const groups = computed(() => {
	const titles: Record<string, string> = {
		basic: t('message.hrms.common.groupBasic'),
		job: t('message.hrms.common.groupJob'),
	};
	const map = new Map<string, any[]>();
	for (const f of props.fields || []) {
		if (f.isStale || f.visible === false) continue;
		const code = f.groupCode || 'basic';
		if (!map.has(code)) map.set(code, []);
		map.get(code)!.push(f);
	}
	return [...map.entries()].map(([code, fields]) => {
		const photo = props.hidePhoto ? null : fields.find((f) => f.fieldKey === 'empPic') || null;
		return { code, title: titles[code] || code, photo, fields: fields.filter((f) => f.fieldKey !== 'empPic') };
	});
});

const ageText = computed(() => {
	const birth = props.ruleForm?.empBirth;
	if (!birth) return '';
	const d = new Date(birth);
	if (Number.isNaN(d.getTime())) return '';
	const today = new Date();
	let age = today.getFullYear() - d.getFullYear();
	const m = today.getMonth() - d.getMonth();
	if (m < 0 || (m === 0 && today.getDate() < d.getDate())) age--;
	return age < 0 ? '0' : String(age);
});

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
	picUploading.value = true;
	try {
		const photo = await normalizeEmpPhoto(raw);
		const res = await getAPI(SysFileApi).apiSysFileUploadFilePostForm(photo, 'hrEmp', undefined, '.jpg,.jpeg,.png,.bmp,.gif');
		const url = res.data.result?.url;
		if (!url) {
			ElMessage.error('上传失败');
			return;
		}
		props.ruleForm.empPic = url;
		ElMessage.success('照片已上传');
	} catch {
		ElMessage.error('照片处理失败');
	} finally {
		picUploading.value = false;
	}
};
</script>

<style scoped>
.hr-emp-fields {
	display: grid;
	/* 与自定义布局预览一致：按 spanSlots 均分，不单独拉宽日期列 */
	grid-template-columns: repeat(var(--hr-emp-cols, 6), minmax(0, 1fr));
	column-gap: 0;
	row-gap: 0;
}
.hr-emp-group.has-photo {
	position: relative;
	padding-right: 136px;
	min-height: 188px;
}
.emp-pic-anchor {
	position: absolute;
	top: 0;
	right: 0;
	width: 120px;
	z-index: 2;
}
.emp-pic-box {
	display: flex;
	flex-direction: column;
	align-items: center;
}
.emp-pic-uploader :deep(.el-upload) {
	border: 1px dashed var(--el-border-color);
	border-radius: 6px;
	cursor: pointer;
	overflow: hidden;
	width: 120px;
	height: 160px;
	display: flex;
	align-items: center;
	justify-content: center;
	background: var(--el-fill-color-blank);
}
.emp-pic-uploader :deep(.el-upload:hover) {
	border-color: var(--el-color-primary);
}
.emp-pic-img,
.emp-pic-uploader :deep(img) {
	width: 120px;
	height: 160px;
	max-width: 120px;
	max-height: 160px;
	object-fit: cover;
	object-position: center top;
	display: block;
	flex-shrink: 0;
}
.emp-pic-placeholder {
	display: flex;
	flex-direction: column;
	align-items: center;
	justify-content: center;
	width: 120px;
	height: 160px;
	color: var(--el-text-color-secondary);
	gap: 6px;
}
.emp-pic-tip {
	font-size: 12px;
}
.emp-pic-actions {
	margin-top: 4px;
}
.hr-emp-date {
	width: 100%;
}
.hr-emp-date :deep(.el-input__wrapper) {
	padding-left: 4px;
	padding-right: 4px;
}
.hr-emp-date :deep(.el-input__prefix) {
	margin-right: 2px;
}
.hr-emp-date :deep(.el-input__prefix .el-icon) {
	font-size: 12px;
}
</style>
