<template>
	<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="920px">
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
					<el-form-item label="厂区">
						<el-select v-model="state.ruleForm.ccSite" filterable clearable class="w100">
							<el-option v-for="item in state.siteList" :key="item.value" :label="item.label" :value="item.value" />
						</el-select>
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
					<el-form-item label="统计分类">
						<el-select v-model="state.ruleForm.ccStatBasic" filterable clearable class="w100">
							<el-option v-for="item in state.statList" :key="item.value" :label="item.label" :value="item.value" />
						</el-select>
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item label="薪酬组">
						<el-input v-model="state.ruleForm.ccXzGroup" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item label="考勤组">
						<el-input-number v-model="state.ruleForm.ccKqgroup" class="w100" />
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item label="工时类别">
						<el-input v-model="state.ruleForm.ccWoca" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item label="工资率">
						<el-input-number v-model="state.ruleForm.ccWageRate" :precision="4" class="w100" />
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item label="部门组">
						<el-input-number v-model="state.ruleForm.ccGroup" class="w100" />
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item label="下属标志">
						<el-input-number v-model="state.ruleForm.ccSub" class="w100" />
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item label="缩进标识">
						<el-input v-model="state.ruleForm.ccIndet" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item label="状态码">
						<el-input-number v-model="state.ruleForm.ccState" class="w100" />
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item label="统计组">
						<el-input v-model="state.ruleForm.ccCountGroup" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item label="编制单位">
						<el-input v-model="state.ruleForm.ccBzUnit" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item label="审核级别">
						<el-input v-model="state.ruleForm.ccLevelCensor" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item label="辅助编码">
						<el-input v-model="state.ruleForm.ccCode2" clearable />
					</el-form-item>
				</el-col>
				<el-col :span="12" class="mb20">
					<el-form-item label="楼层">
						<el-input v-model="state.ruleForm.machineFloor" clearable />
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
import { onMounted, reactive, ref } from 'vue';
import { addHrOrg, getHrCodeList, updateHrOrg } from '/@/api/hrms/hrOrg';

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
	siteList: [] as any[],
	costList: [] as any[],
	statList: [] as any[],
});

onMounted(async () => {
	try {
		const [site, cost, stat] = await Promise.all([getHrCodeList('cc_site'), getHrCodeList('cc_costtype'), getHrCodeList('cc_countbasic')]);
		state.siteList = site.data.result ?? [];
		state.costList = cost.data.result ?? [];
		state.statList = stat.data.result ?? [];
	} catch {
		state.siteList = [];
		state.costList = [];
		state.statList = [];
	}
});

const openDialog = (row: any, isEdit = false) => {
	state.isEdit = isEdit;
	state.ruleForm = JSON.parse(JSON.stringify(row ?? {}));
	if (state.ruleForm.ccKqgroup == undefined) state.ruleForm.ccKqgroup = 1;
	if (state.ruleForm.ccSub == undefined) state.ruleForm.ccSub = 0;
	if (state.ruleForm.ccWageRate == undefined) state.ruleForm.ccWageRate = 0;
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
