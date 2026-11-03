<template>
	<el-form v-if="model" ref="formRef" :model="model" label-width="150px" class="cp-detail-form">
		<el-divider content-position="left">人事参数设置</el-divider>
		<el-row :gutter="12">
			<el-col :span="8">
				<el-form-item label="公司名称" required>
					<el-input v-model="model.siteName" maxlength="100" />
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="启用二代身份证">
					<el-select v-model="model.idCardV2Enabled" class="w100">
						<el-option v-for="o in opts.IdCardV2Enabled" :key="o.value" :label="o.label" :value="num(o.value)" />
					</el-select>
				</el-form-item>
			</el-col>
			<!-- 固定号位：仅含固定号前缀的编码方式显示，否则留空 -->
			<el-col :span="8">
				<el-form-item v-if="needsFixedPrefix(model.empIdMode)" label="固定号">
					<el-input v-model="model.empIdFixedPrefix" maxlength="32" />
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="工号编码方式" required>
					<el-select v-model="model.empIdMode" class="w100" filterable>
						<el-option v-for="o in opts.EmpIdMode" :key="o.value" :label="o.label" :value="num(o.value)" />
					</el-select>
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="工号长度" required>
					<el-input-number v-model="model.empIdLength" :min="0" :max="32" class="w100" />
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="试用当天是否有效">
					<el-select v-model="model.probationDayInclusive" class="w100">
						<el-option v-for="o in opts.ProbationDayInclusive" :key="o.value" :label="o.label" :value="num(o.value)" />
					</el-select>
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="新进试用否">
					<el-select v-model="model.probationOnHire" class="w100">
						<el-option v-for="o in opts.ProbationOnHire" :key="o.value" :label="o.label" :value="num(o.value)" />
					</el-select>
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="新进试用期限">
					<el-input-number v-model="model.probationHireDays" :min="0" class="w100" />
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="离职审核">
					<el-select v-model="model.dimissionAudit" class="w100">
						<el-option v-for="o in opts.DimissionAudit" :key="o.value" :label="o.label" :value="num(o.value)" />
					</el-select>
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="异动审核">
					<el-select v-model="model.transferAudit" class="w100">
						<el-option v-for="o in opts.TransferAudit" :key="o.value" :label="o.label" :value="num(o.value)" />
					</el-select>
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="转正审核">
					<el-select v-model="model.regularizeAudit" class="w100">
						<el-option v-for="o in opts.RegularizeAudit" :key="o.value" :label="o.label" :value="num(o.value)" />
					</el-select>
				</el-form-item>
			</el-col>
		</el-row>

		<el-divider content-position="left">考勤参数设置</el-divider>
		<el-row :gutter="12">
			<el-col :span="8">
				<el-form-item label="入职日考勤">
					<el-select v-model="model.hireDayAttMode" class="w100">
						<el-option v-for="o in opts.HireDayAttMode" :key="o.value" :label="o.label" :value="num(o.value)" />
					</el-select>
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="离职日考勤否">
					<el-select v-model="model.leaveDayAttMode" class="w100">
						<el-option v-for="o in opts.LeaveDayAttMode" :key="o.value" :label="o.label" :value="num(o.value)" />
					</el-select>
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="考勤结束日">
					<el-input-number v-model="model.attCloseDay" :min="0" :max="31" class="w100" />
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="应出勤天计算">
					<el-select v-model="model.requiredAttDayMode" class="w100">
						<el-option v-for="o in opts.RequiredAttDayMode" :key="o.value" :label="o.label" :value="num(o.value)" />
					</el-select>
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="薪资考勤月份">
					<el-select v-model="model.payrollAttMonth" class="w100">
						<el-option v-for="o in opts.PayrollAttMonth" :key="o.value" :label="o.label" :value="num(o.value)" />
					</el-select>
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="加班跨日计算">
					<el-select v-model="model.otCrossdayMode" class="w100">
						<el-option v-for="o in opts.OtCrossdayMode" :key="o.value" :label="o.label" :value="num(o.value)" />
					</el-select>
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="加班最小单位">
					<div class="ot-min">
						<el-input-number v-model="model.otMinUnitHour" :min="0" :step="0.1" :precision="2" />
						<span class="ot-min-unit">小时</span>
					</div>
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="加班审核">
					<el-select v-model="model.otAuditMode" class="w100">
						<el-option v-for="o in opts.OtAuditMode" :key="o.value" :label="o.label" :value="num(o.value)" />
					</el-select>
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="请假审核">
					<el-select v-model="model.leaveAudit" class="w100">
						<el-option v-for="o in opts.LeaveAudit" :key="o.value" :label="o.label" :value="num(o.value)" />
					</el-select>
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="补卡审核">
					<el-select v-model="model.cardMakeupAudit" class="w100">
						<el-option v-for="o in opts.CardMakeupAudit" :key="o.value" :label="o.label" :value="num(o.value)" />
					</el-select>
				</el-form-item>
			</el-col>
		</el-row>

		<el-divider content-position="left">保险参数设置</el-divider>
		<el-row :gutter="12">
			<el-col :span="8">
				<el-form-item label="开始月份生成月报方式" label-width="180px">
					<el-select v-model="model.insMonthReportMode" class="w100">
						<el-option v-for="o in opts.InsMonthReportMode" :key="o.value" :label="o.label" :value="num(o.value)" />
					</el-select>
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="当月投保当月退保月报产生方式" label-width="220px">
					<el-select v-model="model.insSameMonthReport" class="w100">
						<el-option v-for="o in opts.InsSameMonthReport" :key="o.value" :label="o.label" :value="num(o.value)" />
					</el-select>
				</el-form-item>
			</el-col>
			<el-col :span="8">
				<el-form-item label="保费提交薪资方式" label-width="160px">
					<el-select v-model="model.insFeeToPayroll" class="w100">
						<el-option v-for="o in opts.InsFeeToPayroll" :key="o.value" :label="o.label" :value="num(o.value)" />
					</el-select>
				</el-form-item>
			</el-col>
		</el-row>
	</el-form>
	<el-empty v-else description="请先在「系统选项设置」选择或新增公司" />
</template>

<script lang="ts" setup name="hrCompanyParamDetailForm">
import { onMounted, reactive, watch } from 'vue';
import { getHrCodeList } from '/@/api/hrms/hrOrg';

const model = defineModel<any>('model', { default: null });

type Opt = { value: string; label: string };
const CODE_FIELDS = [
	'EmpIdMode',
	'ProbationOnHire',
	'ProbationDayInclusive',
	'DimissionAudit',
	'TransferAudit',
	'RegularizeAudit',
	'IdCardV2Enabled',
	'HireDayAttMode',
	'LeaveDayAttMode',
	'RequiredAttDayMode',
	'PayrollAttMonth',
	'OtAuditMode',
	'LeaveAudit',
	'OtCrossdayMode',
	'CardMakeupAudit',
	'InsMonthReportMode',
	'InsFeeToPayroll',
	'InsSameMonthReport',
] as const;

const opts = reactive<Record<string, Opt[]>>(Object.fromEntries(CODE_FIELDS.map((k) => [k, []])));

const num = (v: string) => Number(v);
/** EmpIdMode：4/8/9 需录入固定号前缀 */
const needsFixedPrefix = (mode: number | null | undefined) => mode === 4 || mode === 8 || mode === 9;

const loadOptions = async () => {
	await Promise.all(
		CODE_FIELDS.map(async (field) => {
			const res = await getHrCodeList(field);
			opts[field] = (res.data?.result ?? []).map((x: any) => ({ value: String(x.value), label: x.label || x.text || x.value }));
		})
	);
};

watch(
	() => model.value?.empIdMode,
	(v) => {
		if (!needsFixedPrefix(v) && model.value) model.value.empIdFixedPrefix = null;
	}
);

onMounted(loadOptions);

defineExpose({ reloadOptions: loadOptions });
</script>

<style scoped>
.w100 {
	width: 100%;
}
.ot-min {
	display: flex;
	align-items: center;
	gap: 8px;
}
.ot-min-unit {
	color: var(--el-text-color-secondary);
}
.cp-detail-form {
	height: 100%;
	padding: 8px 12px 24px;
	overflow: auto;
	box-sizing: border-box;
}
</style>
