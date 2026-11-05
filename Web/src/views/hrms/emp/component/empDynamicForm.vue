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
				<el-form-item v-else-if="f.fieldKey === 'empProvince'" :label="f.label" :prop="f.fieldKey" :rules="fieldRules(f)">
					<el-select
						v-model="ruleForm.empProvince"
						class="w100"
						clearable
						filterable
						:disabled="f.readonly"
						@change="onProvinceChange"
					>
						<el-option v-for="o in provinceOptions" :key="o.value" :label="o.label" :value="o.value" />
					</el-select>
				</el-form-item>
				<el-form-item v-else-if="f.fieldKey === 'empCity'" :label="f.label" :prop="f.fieldKey" :rules="fieldRules(f)">
					<el-select v-model="ruleForm.empCity" class="w100" clearable filterable :disabled="f.readonly || !ruleForm.empProvince" :placeholder="ruleForm.empProvince ? undefined : t('message.hrms.emp.selectProvinceFirst')">
						<el-option v-for="o in cityOptions" :key="o.value" :label="o.label" :value="o.value" />
					</el-select>
				</el-form-item>
				<el-form-item
					v-else
					:label="f.label"
					:prop="f.fieldKey"
					:rules="fieldRules(f)"
				>
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
					<el-select
						v-else-if="f.fieldKey === 'empCountry' || f.controlType === 'select' || f.codeField || f.lookupSource"
						v-model="ruleForm[f.fieldKey]"
						class="w100"
						clearable
						filterable
						:disabled="f.readonly"
						@change="f.fieldKey === 'empCountry' ? onCountryOrCardChange() : undefined"
					>
						<el-option v-for="o in codeOptions[optionKey(f)] || []" :key="o.value" :label="codeOptionText(o)" :value="o.value" />
					</el-select>
					<el-input
						v-else-if="f.fieldKey === 'empCard'"
						v-model="ruleForm.empCard"
						clearable
						:disabled="f.readonly"
						@blur="onCountryOrCardChange"
					/>
					<el-input
						v-else
						v-model="ruleForm[f.fieldKey]"
						clearable
						:disabled="f.readonly || (f.fieldKey === 'empId' && (isEdit || !!empIdLocked))"
						:placeholder="f.fieldKey === 'empId' && empIdLocked ? t('message.hrms.emp.empIdAutoHint') : undefined"
					/>
				</el-form-item>
			</div>
			</div>
		</div>
	</template>
</template>

<script lang="ts" setup>
import { computed, onMounted, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage } from 'element-plus';
import type { UploadFile } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { SysFileApi } from '/@/api-services/system/api';
import { normalizeEmpPhoto } from '/@/views/hrms/composables/hrEmpPhoto';
import { parseHrEmpIdCard } from '/@/api/hrms/hrEmp';
import { listEnabledHrProvince } from '/@/api/hrms/hrProvince';
import { listHrCityByProvince } from '/@/api/hrms/hrCity';
import type { HrIdCardDeviceResult } from '/@/views/hrms/composables/hrIdCardReader';
import { idCardPhotoToFile } from '/@/views/hrms/composables/hrIdCardReader';

const props = defineProps<{
	ruleForm: any;
	fields: any[];
	codeOptions: Record<string, { value: string; label?: string; text?: string }[]>;
	treeData: any[];
	isEdit: boolean;
	hidePhoto?: boolean;
	/** 非手工工号模式时锁定工号（由服务端发号） */
	empIdLocked?: boolean;
}>();

const { t, locale } = useI18n();

type Opt = { value: string; label: string };
const provinceOptions = ref<Opt[]>([]);
const cityOptions = ref<Opt[]>([]);
let parseSeq = 0;

const optionKey = (f: any) => {
	const lookup = String(f?.lookupSource || '').trim();
	if (lookup) return `lookup:${lookup}`;
	return String(f?.codeField || '');
};

/** 布局中是否存在该字段（找不到则跳过赋值，不报错） */
const hasField = (fieldKey: string) => (props.fields || []).some((f) => f && f.fieldKey === fieldKey && f.visible !== false && !f.isStale);

const trySet = (fieldKey: string, value: any) => {
	if (!hasField(fieldKey)) return;
	props.ruleForm[fieldKey] = value;
};

/** 占位完全按自定义布局 spanSlots（1～cols） */
const fieldSpan = (f: any, cols: number) => Math.min(Math.max(Number(f.spanSlots) || 1, 1), cols);

const fieldRules = (f: any) =>
	f.required && !(f.fieldKey === 'empId' && props.empIdLocked)
		? [{ required: true, message: (f.label || f.fieldKey) + t('message.hrms.common.requiredSuffix'), trigger: 'blur' }]
		: [];

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

const formatBirth = (v: any) => {
	if (!v) return '';
	if (typeof v === 'string' && v.length >= 10) return v.slice(0, 10);
	const d = new Date(v);
	if (Number.isNaN(d.getTime())) return '';
	const mm = String(d.getMonth() + 1).padStart(2, '0');
	const dd = String(d.getDate()).padStart(2, '0');
	return `${d.getFullYear()}-${mm}-${dd}`;
};

const loadProvinces = async () => {
	if (!hasField('empProvince')) {
		provinceOptions.value = [];
		return;
	}
	try {
		const res = await listEnabledHrProvince();
		const list = res.data?.result ?? res.data ?? [];
		provinceOptions.value = (list as any[]).map((u) => ({
			value: String(u.provinceCode ?? u.ProvinceCode ?? ''),
			label: String(u.provinceName ?? u.ProvinceName ?? u.provinceCode ?? ''),
		})).filter((o) => o.value);
	} catch {
		provinceOptions.value = [];
	}
};

const loadCities = async (provinceCode?: string | null) => {
	if (!hasField('empCity')) {
		cityOptions.value = [];
		return;
	}
	const code = String(provinceCode || '').trim();
	if (!code) {
		cityOptions.value = [];
		return;
	}
	try {
		const res = await listHrCityByProvince(code);
		const list = res.data?.result ?? res.data ?? [];
		cityOptions.value = (list as any[]).map((u) => ({
			value: String(u.cityCode ?? u.CityCode ?? ''),
			label: String(u.cityName ?? u.CityName ?? u.cityCode ?? ''),
		})).filter((o) => o.value);
	} catch {
		cityOptions.value = [];
	}
};

const onProvinceChange = async () => {
	if (hasField('empCity')) props.ruleForm.empCity = '';
	await loadCities(props.ruleForm.empProvince);
};

/** 中国（空/CN）校验证件并始终覆盖生日/性别/省市；布局无字段则跳过 */
const onCountryOrCardChange = async (opts?: { fromButton?: boolean }) => {
	const card = String(props.ruleForm?.empCard ?? '').trim();
	if (!card) {
		if (opts?.fromButton) ElMessage.warning(t('message.hrms.emp.needIdCardFirst'));
		return;
	}
	const seq = ++parseSeq;
	try {
		const res = await parseHrEmpIdCard({
			empCard: card,
			empCountry: props.ruleForm?.empCountry ?? null,
			ccCode: props.ruleForm?.ccCode ?? null,
		});
		if (seq !== parseSeq) return;
		const r = res.data?.result ?? res.data ?? {};
		if (!r.needValidate) {
			if (opts?.fromButton) ElMessage.info(t('message.hrms.emp.idCardSkipNonChina'));
			return;
		}
		if (!r.valid) {
			ElMessage.warning(r.message || t('message.hrms.emp.idCardInvalid'));
			return;
		}
		if (r.empBirth != null) trySet('empBirth', formatBirth(r.empBirth));
		if (r.empSex != null) trySet('empSex', r.empSex);
		// 仅当解析到省码时覆盖省市；未命中主数据则跳过（不清空）
		if (r.empProvince) {
			if (hasField('empProvince')) {
				props.ruleForm.empProvince = r.empProvince;
				await loadCities(props.ruleForm.empProvince);
			}
			if (hasField('empCity')) {
				props.ruleForm.empCity = r.empCity ?? '';
			}
		}
	} catch (e: any) {
		if (seq !== parseSeq) return;
		const msg = e?.response?.data?.message || e?.message;
		if (msg) ElMessage.warning(String(msg));
	}
};

/**
 * 读卡器结果回填（布局无字段则跳过）。
 * 覆盖：证号/姓名/性别/民族/出生/住址/户口地/照片，再解析省市码。
 */
const applyDeviceCardResult = async (dev: HrIdCardDeviceResult) => {
	if (!dev?.ok || !dev.idNumber) return;
	trySet('empCard', String(dev.idNumber).trim());
	if (dev.name) trySet('empName', String(dev.name).trim());
	if (dev.sex) trySet('empSex', String(dev.sex).trim());
	if (dev.nation) trySet('empNation', String(dev.nation).trim());
	if (dev.birth) trySet('empBirth', formatBirth(dev.birth));
	if (dev.address) {
		trySet('empAddr', String(dev.address).trim());
		trySet('empResidence', String(dev.address).trim());
	}
	if (dev.expireEnd) trySet('empCardOverdate', formatBirth(dev.expireEnd));
	if (dev.expireStart) trySet('empCardDate', formatBirth(dev.expireStart));

	if (dev.photoBase64 && hasField('empPic')) {
		const file = idCardPhotoToFile(dev.photoBase64);
		if (file) {
			try {
				const photo = await normalizeEmpPhoto(file);
				const res = await getAPI(SysFileApi).apiSysFileUploadFilePostForm(photo, 'hrEmp', undefined, '.jpg,.jpeg,.png,.bmp,.gif');
				const url = res.data.result?.url;
				if (url) props.ruleForm.empPic = url;
			} catch {
				/* 照片失败不阻断其它字段 */
			}
		}
	}

	// 籍贯省市：走服务端解析（始终覆盖省/市码）
	await onCountryOrCardChange({ fromButton: false });
};

defineExpose({ applyDeviceCardResult });

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

onMounted(async () => {
	await loadProvinces();
	await loadCities(props.ruleForm?.empProvince);
});

watch(locale, async () => {
	await loadProvinces();
	await loadCities(props.ruleForm?.empProvince);
});

watch(
	() => props.fields,
	async () => {
		await loadProvinces();
		await loadCities(props.ruleForm?.empProvince);
	},
	{ deep: true }
);
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
