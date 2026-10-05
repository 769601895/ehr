<template>
	<div class="sys-upgrade-container">
		<el-dialog v-model="state.isShowDialog" draggable :close-on-click-modal="false" width="50vw" :before-close="cancel">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-InfoFilled /> </el-icon>
					<span> 系统更新日志 </span>
				</div>
			</template>

			<div v-html="state.ruleForm.content" style="padding: 20px"></div>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup>
import { reactive } from 'vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysUpgradeApi } from '/@/api-services/system/api';

const state = reactive({
	isShowDialog: false,
	ruleForm: {} as any,
});

// 打开弹窗
const openDialog = (row: any) => {
	state.ruleForm = JSON.parse(JSON.stringify(row));
	state.isShowDialog = true;
};

// 关闭弹窗
const cancel = async () => {
	// 设置当前用户已读
	await getAPI(SysUpgradeApi).apiSysUpgradeSetReadPost({ id: state.ruleForm.id });

	state.isShowDialog = false;
};

// 导出对象
defineExpose({ openDialog });
</script>
