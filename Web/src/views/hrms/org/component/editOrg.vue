<template>
	<div class="sys-org-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="920px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="110px">
				<el-tabs>
					<el-tab-pane label="结构信息">
						<el-row :gutter="10">
							<el-col :span="24" class="mb20">
								<el-form-item :label="$t('message.list.parentOrganization')">
									<el-cascader :options="props.orgData" :props="cascaderProps" :placeholder="$t('message.list.pleaseSelectParentOrg')" clearable filterable class="w100" v-model="state.ruleForm.pid">
										<template #default="{ node, data }">
											<span>{{ data.name }}</span>
											<span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
										</template>
									</el-cascader>
								</el-form-item>
							</el-col>
							<el-col :span="24" class="mb20">
								<el-form-item label="部门名称" prop="name" :rules="[{ required: true, message: $t('message.list.orgNameRequired'), trigger: 'blur' }]">
									<el-input v-model="state.ruleForm.name" placeholder="部门名称（写入机构名称与 cc_desc）" clearable @input="state.ruleForm.ccDesc = state.ruleForm.name" />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="部门编码" prop="code">
									<el-input v-model="state.ruleForm.code" placeholder="空则自动生成，对应 cc_code" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item :label="$t('message.list.organizationType')">
									<el-select v-model="state.ruleForm.type" filterable clearable class="w100">
										<el-option v-for="item in state.orgTypeList" :key="item.value" :label="item.label" :value="item.value" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item :label="$t('message.list.level')">
									<el-input-number v-model="state.ruleForm.level" disabled class="w100" />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item :label="$t('message.list.orderNo')">
									<el-input-number v-model="state.ruleForm.orderNo" class="w100" />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item :label="$t('message.list.status')">
									<el-radio-group v-model="state.ruleForm.status">
										<el-radio :value="1">{{ $t('message.list.enable') }}</el-radio>
										<el-radio :value="2">{{ $t('message.list.disable') }}</el-radio>
									</el-radio-group>
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item :label="$t('message.list.longitude')">
									<el-input-number v-model="state.ruleForm.longitude" class="w100" />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item :label="$t('message.list.latitude')">
									<el-input-number v-model="state.ruleForm.latitude" class="w100" />
								</el-form-item>
							</el-col>
							<el-col :span="24" class="mb20">
								<el-form-item :label="$t('message.list.remark')">
									<el-input v-model="state.ruleForm.remark" type="textarea" clearable />
								</el-form-item>
							</el-col>
						</el-row>
					</el-tab-pane>
					<el-tab-pane label="人事属性">
						<el-row :gutter="10">
							<el-col :span="12" class="mb20">
								<el-form-item label="部门代号">
									<el-input v-model="state.ruleForm.ccDept" placeholder="cc_dept" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="英文名称">
									<el-input v-model="state.ruleForm.ccDescEn" placeholder="cc_desc_en" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="层级码">
									<el-input v-model="state.ruleForm.ccLevel" disabled placeholder="由父机构自动计算" />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="父部门编码">
									<el-input v-model="state.ruleForm.ccCodeParent" disabled />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="厂区">
									<el-select v-model="state.ruleForm.ccSite" filterable clearable class="w100">
										<el-option v-for="item in state.siteList" :key="item.value" :label="item.label" :value="item.value" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="成本中心">
									<el-select v-model="state.ruleForm.ccCost" filterable clearable class="w100">
										<el-option v-for="item in state.costCenterList" :key="item.value" :label="item.label" :value="item.value" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="统计分类">
									<el-select v-model="state.ruleForm.ccStatBasic" filterable clearable class="w100">
										<el-option v-for="item in state.statList" :key="item.value" :label="item.label" :value="item.value" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="考勤组别">
									<el-select v-model="state.ruleForm.ccKqgroup" filterable clearable class="w100">
										<el-option v-for="item in state.shiftGroupList" :key="item.value" :label="item.label" :value="item.value" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="薪酬组">
									<el-input v-model="state.ruleForm.ccXzGroup" placeholder="cc_xz_group" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="工时类别">
									<el-input v-model="state.ruleForm.ccWoca" placeholder="cc_woca" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="工资率">
									<el-input-number v-model="state.ruleForm.ccWageRate" :precision="4" class="w100" />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="下属标志">
									<el-input-number v-model="state.ruleForm.ccSub" class="w100" />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="部门组">
									<el-input-number v-model="state.ruleForm.ccGroup" class="w100" />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="缩进标识">
									<el-input v-model="state.ruleForm.ccIndet" placeholder="cc_indet" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="状态码">
									<el-input-number v-model="state.ruleForm.ccState" class="w100" />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="统计组">
									<el-input v-model="state.ruleForm.ccCountGroup" placeholder="cc_count_group" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="编制单位">
									<el-input v-model="state.ruleForm.ccBzUnit" placeholder="cc_bz_unit" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="审核级别">
									<el-input v-model="state.ruleForm.ccLevelCensor" placeholder="cc_level_censor" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="辅助编码">
									<el-input v-model="state.ruleForm.ccCode2" placeholder="cc_code2" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="楼层">
									<el-input v-model="state.ruleForm.machineFloor" placeholder="machine_floor" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="停用标志">
									<el-input-number v-model="state.ruleForm.ccEnable" disabled class="w100" />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="删除日期">
									<el-date-picker v-model="state.ruleForm.ccDeleDate" type="datetime" class="w100" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="24" class="mb20">
								<el-form-item label="删除备注">
									<el-input v-model="state.ruleForm.ccDeleRemark" type="textarea" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="创建日期">
									<el-date-picker v-model="state.ruleForm.ccCreateDate" type="datetime" class="w100" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="维护人">
									<el-input v-model="state.ruleForm.ccUser" disabled />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="维护时间">
									<el-date-picker v-model="state.ruleForm.ccDate" type="datetime" class="w100" disabled />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="考勤同步人">
									<el-input v-model="state.ruleForm.kqNewSysuser" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="考勤同步时间">
									<el-date-picker v-model="state.ruleForm.kqNewSysdate" type="datetime" class="w100" clearable />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="遗留ID">
									<el-input-number v-model="state.ruleForm.ccIdentityId" disabled class="w100" />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="遗留父ID">
									<el-input-number v-model="state.ruleForm.parentId" disabled class="w100" />
								</el-form-item>
							</el-col>
							<el-col :span="12" class="mb20">
								<el-form-item label="机构Id">
									<el-input v-model="state.ruleForm.orgId" disabled />
								</el-form-item>
							</el-col>
						</el-row>
					</el-tab-pane>
				</el-tabs>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button icon="ele-CircleCloseFilled" @click="cancel">{{ $t('message.list.cancelButtonText') }}</el-button>
					<el-button type="primary" icon="ele-CircleCheckFilled" @click="submit">{{ $t('message.list.confirmButtonText') }}</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="hrEditOrg">
import { onMounted, reactive, ref } from 'vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysDictDataApi } from '/@/api-services/system/api';
import { SysOrg } from '/@/api-services/system/models';
import { addHrOrg, getHrCodeList, getHrCostCenterList, getHrShiftGroupList, updateHrOrg } from '/@/api/hrms/hrOrg';

const props = defineProps({
	title: String,
	orgData: Array<SysOrg>,
});
const emits = defineEmits(['reload']);
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	ruleForm: {} as any,
	orgTypeList: [] as any,
	siteList: [] as any[],
	statList: [] as any[],
	costCenterList: [] as any[],
	shiftGroupList: [] as any[],
});

const cascaderProps = { checkStrictly: true, emitPath: false, value: 'id', label: 'name' };

onMounted(async () => {
	let resDicData = await getAPI(SysDictDataApi).apiSysDictDataDataListCodeGet('org_type');
	state.orgTypeList = resDicData.data.result;
	try {
		const [site, stat, cost, shift] = await Promise.all([
			getHrCodeList('cc_site'),
			getHrCodeList('cc_countbasic'),
			getHrCostCenterList(),
			getHrShiftGroupList(),
		]);
		state.siteList = site.data.result ?? [];
		state.statList = stat.data.result ?? [];
		state.costCenterList = cost.data.result ?? [];
		state.shiftGroupList = shift.data.result ?? [];
	} catch {
		state.siteList = [];
		state.statList = [];
		state.costCenterList = [];
		state.shiftGroupList = [];
	}
});

const openDialog = (row: any) => {
	const data = JSON.parse(JSON.stringify(row));
	if (!data.ccDesc) data.ccDesc = data.name;
	if (!data.name && data.ccDesc) data.name = data.ccDesc;
	if (data.status == undefined) data.status = 1;
	if (data.orderNo == undefined) data.orderNo = 100;
	if (data.ccKqgroup == undefined) data.ccKqgroup = 1;
	if (data.ccSub == undefined) data.ccSub = 0;
	if (data.ccWageRate == undefined) data.ccWageRate = 0;
	state.ruleForm = data;
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();
};

const closeDialog = () => {
	emits('reload', true);
	state.isShowDialog = false;
};

const cancel = () => {
	state.isShowDialog = false;
};

const submit = () => {
	ruleFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		state.ruleForm.ccDesc = state.ruleForm.name;
		if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
			await updateHrOrg(state.ruleForm);
		} else {
			await addHrOrg(state.ruleForm);
		}
		closeDialog();
	});
};

defineExpose({ openDialog });
</script>
