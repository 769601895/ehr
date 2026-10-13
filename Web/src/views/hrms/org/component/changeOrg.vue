<template>
	<div class="change-org">
		<el-card class="change-org__card" shadow="never">
			<template #header>合并部门</template>
			<el-form :model="mergeForm" label-width="110px" class="merge-form">
				<div class="merge-form__left">
					<el-form-item label="合并前部门一" required>
						<el-cascader
							ref="mergeOld1Ref"
							:options="props.treeData"
							:props="cascaderProps"
							v-model="mergeForm.oldCode1"
							filterable
							clearable
							class="w100"
							placeholder="请选择"
							@change="onMergeOld1Change"
						/>
					</el-form-item>
					<el-form-item label="合并前部门二" required>
						<el-cascader
							ref="mergeOld2Ref"
							:options="props.treeData"
							:props="cascaderProps"
							v-model="mergeForm.oldCode2"
							filterable
							clearable
							class="w100"
							placeholder="请选择"
							@change="onMergeOld2Change"
						/>
					</el-form-item>
				</div>
				<div class="merge-form__right">
					<el-form-item label="合并后目标" required class="merge-form__target">
						<el-cascader
							ref="mergeNewRef"
							:options="props.treeData"
							:props="cascaderProps"
							v-model="mergeForm.newCode"
							filterable
							clearable
							class="w100"
							placeholder="请选择目标部门"
							@change="onMergeNewChange"
						/>
					</el-form-item>
					<el-button type="primary" v-auth="'hrOrg/mergeDept'" :loading="mergeLoading" @click="doMerge">合并</el-button>
				</div>
			</el-form>
		</el-card>

		<el-card class="change-org__card" shadow="never">
			<template #header>调动部门</template>
			<el-form :model="transferForm" label-width="110px" class="transfer-form">
				<el-form-item label="调动部门" required class="transfer-form__item">
					<el-cascader
						ref="transferFromRef"
						:options="props.treeData"
						:props="cascaderProps"
						v-model="transferForm.fromCcCode"
						filterable
						clearable
						class="w100"
						placeholder="请选择"
						@change="onTransferFromChange"
					/>
				</el-form-item>
				<el-form-item label="调至归属" class="transfer-form__item">
					<el-cascader
						ref="transferToRef"
						:options="props.treeData"
						:props="cascaderProps"
						v-model="transferForm.toParentCcCode"
						filterable
						clearable
						class="w100"
						placeholder="空表示挂到根级"
						@change="onTransferToChange"
					/>
				</el-form-item>
				<el-button type="primary" v-auth="'hrOrg/transferDept'" :loading="transferLoading" @click="doTransfer">调动</el-button>
			</el-form>
		</el-card>
	</div>
</template>

<script lang="ts" setup name="hrChangeOrg">
import { nextTick, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import type { CascaderInstance } from 'element-plus';
import { mergeHrDept, transferHrDept } from '/@/api/hrms/hrOrg';

const props = defineProps({
	treeData: { type: Array as () => any[], default: () => [] },
});
const emits = defineEmits(['reload']);

const cascaderProps = { checkStrictly: true, emitPath: false, value: 'ccCode', label: 'ccDesc', children: 'children' };
const mergeOld1Ref = ref<CascaderInstance>();
const mergeOld2Ref = ref<CascaderInstance>();
const mergeNewRef = ref<CascaderInstance>();
const transferFromRef = ref<CascaderInstance>();
const transferToRef = ref<CascaderInstance>();
/** checkStrictly 下选中后面板默认不关闭，选完手动收起以免挡住下方操作 */
const closeCascader = (inst?: CascaderInstance) => {
	nextTick(() => inst?.togglePopperVisible?.(false));
};
const onMergeOld1Change = () => closeCascader(mergeOld1Ref.value);
const onMergeOld2Change = () => closeCascader(mergeOld2Ref.value);
const onMergeNewChange = () => closeCascader(mergeNewRef.value);
const onTransferFromChange = () => closeCascader(transferFromRef.value);
const onTransferToChange = () => closeCascader(transferToRef.value);
const mergeLoading = ref(false);
const transferLoading = ref(false);
const mergeForm = reactive({
	oldCode1: '' as string | undefined,
	oldCode2: '' as string | undefined,
	newCode: '' as string | undefined,
});
const transferForm = reactive({
	fromCcCode: '' as string | undefined,
	toParentCcCode: '' as string | undefined,
});

const doMerge = async () => {
	if (!mergeForm.oldCode1 || !mergeForm.oldCode2 || !mergeForm.newCode) {
		ElMessage.warning('请完整选择合并部门');
		return;
	}
	await ElMessageBox.confirm('确认合并所选部门？源部门将停用，在职人员归属迁至目标部门。', '提示', { type: 'warning' });
	mergeLoading.value = true;
	try {
		await mergeHrDept({
			oldCode1: mergeForm.oldCode1,
			oldCode2: mergeForm.oldCode2,
			newCode: mergeForm.newCode,
		});
		ElMessage.success('合并完成');
		emits('reload');
	} finally {
		mergeLoading.value = false;
	}
};

const doTransfer = async () => {
	if (!transferForm.fromCcCode) {
		ElMessage.warning('请选择调动部门');
		return;
	}
	await ElMessageBox.confirm('确认调动该部门及其下级？原位置将保留为历史部门。', '提示', { type: 'warning' });
	transferLoading.value = true;
	try {
		await transferHrDept({
			fromCcCode: transferForm.fromCcCode,
			toParentCcCode: transferForm.toParentCcCode || undefined,
		});
		ElMessage.success('调动完成');
		emits('reload');
	} finally {
		transferLoading.value = false;
	}
};
</script>

<style lang="scss" scoped>
.change-org {
	display: flex;
	flex-direction: column;
	gap: 16px;
	width: 100%;
}

.change-org__card {
	:deep(.el-card__header) {
		padding: 12px 16px;
		font-weight: 600;
	}

	:deep(.el-card__body) {
		padding: 16px;
	}

	:deep(.el-cascader) {
		width: 100%;
	}

	:deep(.el-form-item) {
		margin-bottom: 16px;
	}
}

.merge-form {
	display: flex;
	align-items: center;
	gap: 24px;
}

.merge-form__left {
	flex: 0 0 380px;
	min-width: 280px;

	:deep(.el-form-item:last-child) {
		margin-bottom: 0;
	}
}

.merge-form__right {
	flex: 1;
	min-width: 0;
	display: flex;
	align-items: center;
	gap: 12px;
}

.merge-form__target {
	flex: 1;
	min-width: 0;
	margin-bottom: 0 !important;
}

.transfer-form {
	display: flex;
	align-items: center;
	gap: 16px;
	flex-wrap: wrap;

	:deep(.el-form-item) {
		margin-bottom: 0;
	}
}

.transfer-form__item {
	flex: 1 1 280px;
	min-width: 240px;
	max-width: 420px;
}

.w100 {
	width: 100%;
}

@media (max-width: 900px) {
	.merge-form {
		flex-direction: column;
		align-items: stretch;
	}

	.merge-form__left {
		flex: none;
		width: 100%;
	}

	.merge-form__right {
		width: 100%;
	}
}
</style>
