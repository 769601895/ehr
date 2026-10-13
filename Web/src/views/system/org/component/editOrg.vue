<template>
	<div class="sys-org-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="100px">
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
					<el-col :span="12" class="mb20">
						<el-form-item :label="$t('message.list.organizationName')" prop="name" :rules="[{ required: true, message: $t('message.list.orgNameRequired'), trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.name" :placeholder="$t('message.list.organizationName')" clearable />
						</el-form-item>
					</el-col>
					<el-col :span="12" class="mb20">
						<el-form-item :label="$t('message.list.organizationCode')" prop="code">
							<el-input v-model="state.ruleForm.code" :placeholder="$t('message.list.organizationCode')" clearable />
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

<script lang="ts" setup name="sysEditOrg">
import { onMounted, reactive, ref } from 'vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysDictDataApi, SysOrgApi } from '/@/api-services/system/api';
import { SysOrg } from '/@/api-services/system/models';

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
});

const cascaderProps = { checkStrictly: true, emitPath: false, value: 'id', label: 'name' };

onMounted(async () => {
	let resDicData = await getAPI(SysDictDataApi).apiSysDictDataDataListCodeGet('org_type');
	state.orgTypeList = resDicData.data.result;
});

const openDialog = (row: any) => {
	state.ruleForm = JSON.parse(JSON.stringify(row));
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
		if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
			await getAPI(SysOrgApi).apiSysOrgUpdatePost(state.ruleForm);
		} else {
			await getAPI(SysOrgApi).apiSysOrgAddPost(state.ruleForm);
		}
		closeDialog();
	});
};

defineExpose({ openDialog });
</script>
