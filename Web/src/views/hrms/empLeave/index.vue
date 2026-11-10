<template>
	<div class="hr-emp-leave-placeholder">
		<el-card shadow="hover">
			<el-empty :description="t('message.hrms.empLeave.placeholder')" :image-size="160" />
		</el-card>
	</div>
</template>

<script lang="ts" setup name="hrEmpLeave">
import { useI18n } from 'vue-i18n';

const { t } = useI18n();
</script>

<style scoped>
.hr-emp-leave-placeholder {
	padding: 8px;
}
.hr-emp-leave-placeholder :deep(.el-card__body) {
	min-height: 360px;
	display: flex;
	align-items: center;
	justify-content: center;
}
</style>
