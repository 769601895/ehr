<template>
	<div class="hr-leave-reason-page">
		<el-tabs v-model="activeTab" class="hr-leave-code-tabs" @tab-change="onTabChange">
			<el-tab-pane :label="t('message.hrms.empLeave.codeLeaveReason')" name="LeaveReason" />
			<el-tab-pane :label="t('message.hrms.empLeave.codeLeaveType')" name="LeaveType" />
			<el-tab-pane :label="t('message.hrms.empLeave.codeLeaveSalarySettle')" name="LeaveSalarySettle" />
		</el-tabs>
		<div class="hr-leave-code-body">
			<CodeMstrPage :key="activeTab" embedded :preset-code-field="activeTab" />
		</div>
	</div>
</template>

<script lang="ts" setup name="hrLeaveReason">
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
import CodeMstrPage from '/@/views/hrms/codeMstr/index.vue';

const { t } = useI18n();
const activeTab = ref('LeaveReason');
const onTabChange = (name: string | number) => {
	activeTab.value = String(name);
};
</script>

<style scoped>
.hr-leave-reason-page {
	height: 100%;
	min-height: calc(100vh - 120px);
	display: flex;
	flex-direction: column;
	padding: 8px;
	box-sizing: border-box;
}
.hr-leave-code-tabs {
	flex-shrink: 0;
}
.hr-leave-code-body {
	flex: 1;
	min-height: 0;
	display: flex;
	flex-direction: column;
}
</style>
