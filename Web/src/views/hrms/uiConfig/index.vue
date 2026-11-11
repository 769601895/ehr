<template>
	<div class="hr-ui-config">
		<el-card shadow="hover" :body-style="{ padding: '10px 12px' }">
			<div class="toolbar">
				<span class="label">{{ t('message.hrms.uiConfig.module') }}</span>
				<el-select v-model="state.moduleCode" filterable style="width: 240px" :placeholder="t('message.hrms.uiConfig.modulePlaceholder')">
					<el-option v-for="m in state.modules" :key="m.code" :label="m.name" :value="m.code" />
				</el-select>
			</div>
		</el-card>

		<el-card class="editor-card" shadow="hover" style="margin-top: 8px">
			<el-tabs v-model="state.scene" type="border-card">
				<el-tab-pane :label="t('message.hrms.uiConfig.tabList')" name="list" :disabled="!hasScene('list')">
					<LayoutEditor
						v-if="state.moduleCode && state.scene === 'list' && hasScene('list')"
						:key="`${state.moduleCode}-list`"
						:module-code="state.moduleCode"
						:module-name="currentModule?.name"
						:main-table="currentModule?.mainTable"
						scene="list"
					/>
				</el-tab-pane>
				<el-tab-pane :label="t('message.hrms.uiConfig.tabForm')" name="form" :disabled="!hasScene('form')">
					<LayoutEditor
						v-if="state.moduleCode && state.scene === 'form' && hasScene('form')"
						:key="`${state.moduleCode}-form`"
						:module-code="state.moduleCode"
						:module-name="currentModule?.name"
						:main-table="currentModule?.mainTable"
						scene="form"
					/>
				</el-tab-pane>
			</el-tabs>
		</el-card>
	</div>
</template>

<script lang="ts" setup name="hrUiConfig">
import { computed, onMounted, reactive, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import LayoutEditor from '/@/views/hrms/reportDef/component/layoutEditor.vue';
import { listHrUiModules, type HrUiLayoutScene, type HrUiModuleItem } from '/@/api/hrms/hrUiLayout';

const { t } = useI18n();

const state = reactive({
	modules: [] as HrUiModuleItem[],
	moduleCode: '' as string,
	scene: 'list' as HrUiLayoutScene,
});

const currentModule = computed(() => state.modules.find((m) => m.code === state.moduleCode));

const hasScene = (scene: string) => (currentModule.value?.scenes || []).includes(scene);

watch(
	() => state.moduleCode,
	() => {
		if (!hasScene(state.scene)) {
			state.scene = hasScene('list') ? 'list' : hasScene('form') ? 'form' : 'list';
		}
	}
);

onMounted(async () => {
	const res = await listHrUiModules();
	const list = (res.data?.result ?? res.data ?? []) as HrUiModuleItem[];
	state.modules = list;
	state.moduleCode = list.find((m) => m.code === 'hrEmp')?.code || list[0]?.code || '';
});
</script>

<style scoped>
.hr-ui-config {
	height: 100%;
	display: flex;
	flex-direction: column;
	min-height: 0;
}
.toolbar {
	display: flex;
	align-items: center;
	gap: 10px;
}
.label {
	font-weight: 600;
}
.editor-card {
	flex: 1;
	min-height: 0;
}
.editor-card :deep(.el-card__body) {
	height: calc(100vh - 180px);
	padding: 0;
}
.editor-card :deep(.el-tabs) {
	height: 100%;
	display: flex;
	flex-direction: column;
}
.editor-card :deep(.el-tabs__content) {
	flex: 1;
	min-height: 0;
	overflow: hidden;
}
.editor-card :deep(.el-tab-pane) {
	height: 100%;
}
</style>
