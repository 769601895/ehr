<template>
	<div class="sys-pos-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="700px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ props.title }} </span>
				</div>
			</template>
			<el-form :model="state.ruleForm" ref="ruleFormRef" label-width="auto">
				<el-row :gutter="10">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item :label="$t('message.list.jobTitle')" prop="name" :rules="[{ required: true, message: $t('message.list.jobTitleRequired'), trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.name" :placeholder="$t('message.list.jobTitle')" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item :label="$t('message.list.positionCode')" prop="code" :rules="[{ required: true, message: $t('message.list.positionCodeRequired'), trigger: 'blur' }]">
							<el-input v-model="state.ruleForm.code" :placeholder="$t('message.list.positionCode')" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item :label="$t('message.list.positionType')">
							<el-input v-model="state.ruleForm.type" :placeholder="$t('message.list.positionType')" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item :label="$t('message.list.orderNo')">
							<el-input-number v-model="state.ruleForm.orderNo" :placeholder="$t('message.list.orderNo')" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item :label="$t('message.list.status')">
							<el-radio-group v-model="state.ruleForm.status">
								<el-radio :value="1">{{ $t('message.list.enable') }}</el-radio>
								<el-radio :value="2">{{ $t('message.list.disable') }}</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item :label="$t('message.list.remark')">
							<el-input v-model="state.ruleForm.remark" :placeholder="$t('message.list.needInputRemark')" clearable type="textarea" />
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

<script lang="ts" setup name="sysEditPos">
import { reactive, ref } from 'vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysPosApi } from '/@/api-services/system/api';
import { UpdatePosInput } from '/@/api-services/system/models';

const props = defineProps({
	title: String,
});
const emits = defineEmits(['handleQuery']);
const ruleFormRef = ref();
const state = reactive({
	isShowDialog: false,
	ruleForm: {} as UpdatePosInput,
});

// 打开弹窗
const openDialog = (row: any) => {
	state.ruleForm = JSON.parse(JSON.stringify(row));
	state.isShowDialog = true;
	ruleFormRef.value?.resetFields();
};

// 关闭弹窗
const closeDialog = () => {
	emits('handleQuery');
	state.isShowDialog = false;
};

// 取消
const cancel = () => {
	state.isShowDialog = false;
};

// 提交
const submit = () => {
	ruleFormRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
			await getAPI(SysPosApi).apiSysPosUpdatePost(state.ruleForm);
		} else {
			await getAPI(SysPosApi).apiSysPosAddPost(state.ruleForm);
		}
		closeDialog();
	});
};

// 导出对象
defineExpose({ openDialog });
</script>
