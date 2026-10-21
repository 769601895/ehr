<template>
	<el-drawer v-model="visible" class="hr-code-setting-drawer" :title="`${t('message.hrms.fieldDef.codeSetting')} ${codeField || ''}`" size="860px" destroy-on-close>
		<CodeMstrPage v-if="codeField" embedded :preset-code-field="codeField" />
	</el-drawer>
</template>

<script lang="ts" setup>
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
import CodeMstrPage from '/@/views/hrms/codeMstr/index.vue';

const { t } = useI18n();
const visible = ref(false);
const codeField = ref('');

const open = (field: string) => {
	codeField.value = field;
	visible.value = true;
};

defineExpose({ open });
</script>

<style>
.hr-code-setting-drawer .el-drawer__body {
	display: flex;
	flex-direction: column;
	overflow: hidden;
	padding: 8px 12px 12px;
	height: calc(100% - 56px);
	box-sizing: border-box;
}
</style>
