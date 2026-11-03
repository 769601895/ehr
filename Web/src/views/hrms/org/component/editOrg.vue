<template>
	<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="720px">
		<template #header>
			<div style="color: #fff">
				<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"><ele-Edit /></el-icon>
				<span>{{ props.title }}</span>
			</div>
		</template>
		<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="110px">
			<el-row :gutter="10">
				<el-col :span="24" class="mb20">
					<el-form-item label="上级部门">
						<el-cascader
							:options="props.treeData"
							:props="cascaderProps"
							placeholder="空表示根级"
							clearable
							filterable
							class="w100"
							v-model="state.ruleForm.ccCodeParent"
						>
							<template #default="{ node, data }">
								<span>{{ data.ccDesc }}</span>
								<span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
							</template>
						</el-cascader>
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item label="部门名称" prop="ccDesc" :rules="[{ required: true, message: '部门名称不能为空', trigger: 'blur' }]">
						<el-input v-model="state.ruleForm.ccDesc" placeholder="部门名称" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item label="部门代号" prop="ccDept" :rules="[{ required: true, message: '部门代号不能为空', trigger: 'blur' }]">
						<el-input v-model="state.ruleForm.ccDept" placeholder="部门代号" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item label="部门编码">
						<el-input v-model="state.ruleForm.ccCode" :disabled="state.isEdit" placeholder="空则自动生成" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item label="英文名称">
						<el-input v-model="state.ruleForm.ccDescEn" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item label="层级码">
						<el-input v-model="state.ruleForm.ccLevel" disabled placeholder="保存后自动计算" />
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item label="成本类型">
						<el-select v-model="state.ruleForm.ccCost" filterable clearable class="w100">
							<el-option v-for="item in state.costList" :key="item.value" :label="item.label" :value="item.value" />
						</el-select>
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item :label="t('message.hrms.org.orgType')">
						<el-select v-model="state.ruleForm.orgType" class="w100">
							<el-option v-for="item in state.orgTypeList" :key="item.value" :label="item.label" :value="item.value" />
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
					<el-form-item label="考勤组">
						<el-input-number v-model="state.ruleForm.ccKqgroup" class="w100" />
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

<script lang="ts" setup name="hrEditOrg">
import { onMounted, reactive, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { addHrOrg, getHrCodeList, updateHrOrg } from '/@/api/hrms/hrOrg';

const { t, locale } = useI18n();
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
	ruleForm: {} as any,
	costList: [] as any[],
	statList: [] as any[],
	orgTypeList: [] as any[],
});

const loadLookups = async () => {
	try {
		const [cost, stat, orgType] = await Promise.all([
			getHrCodeList('cc_costtype'),
			getHrCodeList('cc_countbasic'),
			getHrCodeList('OrgType'),
		]);
		state.costList = cost.data.result ?? [];
		state.statList = stat.data.result ?? [];
		state.orgTypeList = orgType.data.result ?? [];
	} catch {
		state.costList = [];
		state.statList = [];
		state.orgTypeList = [
			{ value: '1', label: t('message.hrms.org.orgTypeDept') },
			{ value: '2', label: t('message.hrms.org.orgTypeCompany') },
		];
	}
};

onMounted(loadLookups);
watch(locale, loadLookups);

const openDialog = (row: any, isEdit = false) => {
	state.isEdit = isEdit;
	state.ruleForm = JSON.parse(JSON.stringify(row ?? {}));
	if (state.ruleForm.ccKqgroup == undefined) state.ruleForm.ccKqgroup = 1;
	if (!state.ruleForm.orgType) state.ruleForm.orgType = '1';
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();
};

const submit = () => {
	ruleFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		if (state.isEdit) {
			await updateHrOrg(state.ruleForm);
		} else {
			await addHrOrg(state.ruleForm);
		}
		state.isShowDialog = false;
		emits('reload');
	});
};

defineExpose({ openDialog });
</script>
