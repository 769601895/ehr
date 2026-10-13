<template>
	<div class="layout-editor" v-loading="state.loading">
		<div class="toolbar">
			<div class="toolbar-title">{{ title }}</div>
			<div class="toolbar-actions">
				<el-button type="primary" icon="ele-Check" :loading="state.saving" v-auth="'hrUiLayout/saveDefault'" @click="handleSave">保存</el-button>
				<el-button icon="ele-RefreshLeft" @click="handleReloadDefault">加载默认</el-button>
			</div>
		</div>
		<splitpanes class="default-theme editor-panes">
			<pane size="52">
				<el-card shadow="never" class="pane-card" :body-style="{ padding: '8px', height: '100%', display: 'flex', flexDirection: 'column' }">
					<template #header>
						<span>表，字段区</span>
						<el-button type="primary" link style="float: right" @click="addRow">新增</el-button>
					</template>
					<el-table :data="state.fields" border size="small" height="100%" style="flex: 1">
						<el-table-column label="操作" width="88" align="center" fixed>
							<template #default="{ $index, row }">
								<el-button link type="primary" :disabled="$index === 0" @click="move($index, -1)">↑</el-button>
								<el-button link type="primary" :disabled="$index >= state.fields.length - 1" @click="move($index, 1)">↓</el-button>
								<el-button link type="danger" :disabled="row.locked" @click="removeRow($index)">删</el-button>
							</template>
						</el-table-column>
						<el-table-column type="index" label="序号" width="50" />
						<el-table-column label="表名" width="110" prop="tableName">
							<template #default>h_emp_mstr</template>
						</el-table-column>
						<el-table-column label="中文显示" min-width="110">
							<template #default="{ row }">
								<el-input v-model="row.label" size="small" />
							</template>
						</el-table-column>
						<el-table-column label="字段名" min-width="130">
							<template #default="{ row }">
								<el-select v-model="row.fieldKey" filterable size="small" class="w100" :disabled="row.locked" @change="(v: string) => onFieldKeyChange(row, v)">
									<el-option v-for="p in availablePool(row)" :key="p.fieldKey" :label="`${p.label} (${p.fieldKey})`" :value="p.fieldKey" />
								</el-select>
							</template>
						</el-table-column>
						<el-table-column v-if="scene === 'list'" label="宽度" width="90">
							<template #default="{ row }">
								<el-input-number v-model="row.widthPx" :min="40" :max="600" size="small" controls-position="right" class="w100" />
							</template>
						</el-table-column>
						<el-table-column v-if="scene === 'form'" label="占位" width="80">
							<template #default="{ row }">
								<el-input-number v-model="row.spanSlots" :min="1" :max="6" size="small" controls-position="right" class="w100" />
							</template>
						</el-table-column>
						<el-table-column v-if="scene === 'form'" label="分组" width="100">
							<template #default="{ row }">
								<el-select v-model="row.groupCode" size="small" class="w100">
									<el-option label="基本信息" value="basic" />
									<el-option label="任职信息" value="job" />
								</el-select>
							</template>
						</el-table-column>
						<el-table-column label="显示" width="60" align="center">
							<template #default="{ row }">
								<el-checkbox v-model="row.visible" :disabled="row.locked" />
							</template>
						</el-table-column>
					</el-table>
				</el-card>
			</pane>
			<pane size="48">
				<el-card shadow="never" class="pane-card" :body-style="{ padding: '8px', height: '100%', overflow: 'auto' }">
					<template #header>
						<span>报表显示区</span>
						<span class="source-tag">来源：{{ sourceText }}</span>
					</template>
					<!-- 列表预览：表头 -->
					<div v-if="scene === 'list'" class="preview-list">
						<table class="preview-table">
							<thead>
								<tr>
									<th v-for="f in visibleFields" :key="f.fieldKey" :style="{ width: (f.widthPx || 100) + 'px' }">{{ f.label }}</th>
								</tr>
							</thead>
							<tbody>
								<tr>
									<td v-for="f in visibleFields" :key="f.fieldKey" class="muted">—</td>
								</tr>
							</tbody>
						</table>
					</div>
					<!-- 表单预览：栅格 -->
					<div v-else class="preview-form">
						<template v-for="group in formGroups" :key="group.code">
							<div class="group-title">{{ group.title }}</div>
							<el-row :gutter="4">
								<el-col v-for="f in group.fields" :key="f.fieldKey" :span="(f.spanSlots || 1) * 4" class="preview-col">
									<div class="preview-field" :class="{ photo: f.controlType === 'photo' }">
										<span class="preview-label">{{ f.label }}</span>
										<span class="preview-box">{{ f.controlType === 'photo' ? '照片' : '' }}</span>
									</div>
								</el-col>
							</el-row>
						</template>
					</div>
				</el-card>
			</pane>
		</splitpanes>
	</div>
</template>

<script lang="ts" setup>
import { computed, onMounted, reactive } from 'vue';
import { ElMessage } from 'element-plus';
import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';
import { getHrUiFieldPool, getHrUiLayoutDefault, getHrUiLayoutEffective, saveHrUiLayoutDefault, type HrUiLayoutScene } from '/@/api/hrms/hrUiLayout';

const props = defineProps<{
	scene: HrUiLayoutScene;
	title: string;
}>();

const state = reactive({
	loading: false,
	saving: false,
	source: '' as string,
	pool: [] as any[],
	fields: [] as any[],
});

const sourceText = computed(() => {
	if (state.source === 'system') return '系统默认';
	if (state.source === 'user') return '个人方案';
	if (state.source === 'builtin') return '内置兜底';
	return state.source || '-';
});

const visibleFields = computed(() => state.fields.filter((f) => f.visible !== false));

const formGroups = computed(() => {
	const basic = visibleFields.value.filter((f) => (f.groupCode || 'basic') === 'basic');
	const job = visibleFields.value.filter((f) => f.groupCode === 'job');
	const groups = [];
	if (basic.length) groups.push({ code: 'basic', title: '基本信息', fields: basic });
	if (job.length) groups.push({ code: 'job', title: '任职信息', fields: job });
	return groups;
});

const availablePool = (row: any) => {
	const used = new Set(state.fields.filter((f) => f !== row).map((f) => f.fieldKey));
	return state.pool.filter((p) => p.fieldKey === row.fieldKey || !used.has(p.fieldKey));
};

const mapFields = (fields: any[]) =>
	(fields || []).map((f: any, i: number) => {
		const meta = state.pool.find((p) => p.fieldKey === f.fieldKey);
		return {
			fieldKey: f.fieldKey,
			dbColumn: f.dbColumn ?? meta?.dbColumn,
			label: f.label || meta?.label || f.fieldKey,
			sortNo: f.sortNo ?? i + 1,
			visible: f.visible !== false,
			widthPx: f.widthPx ?? meta?.defaultWidthPx ?? 100,
			spanSlots: f.spanSlots ?? meta?.defaultSpanSlots ?? 1,
			groupCode: f.groupCode ?? meta?.groupCode ?? 'basic',
			controlType: f.controlType ?? meta?.controlType ?? 'text',
			required: !!(f.required || meta?.required),
			readonly: !!(f.readonly || meta?.readonly),
			locked: !!(f.locked || meta?.locked),
			sortable: meta?.sortable !== false,
		};
	});

const loadPool = async () => {
	const res = await getHrUiFieldPool('hrEmp', props.scene);
	state.pool = res.data.result ?? [];
};

const loadEffective = async () => {
	state.loading = true;
	try {
		await loadPool();
		const res = await getHrUiLayoutEffective('hrEmp', props.scene);
		const data = res.data.result;
		state.source = data?.source ?? '';
		state.fields = mapFields(data?.fields ?? []);
	} finally {
		state.loading = false;
	}
};

const handleReloadDefault = async () => {
	state.loading = true;
	try {
		const res = await getHrUiLayoutDefault('hrEmp', props.scene);
		const data = res.data.result;
		state.source = data?.source ?? 'system';
		state.fields = mapFields(data?.fields ?? []);
		ElMessage.success('已加载系统默认');
	} finally {
		state.loading = false;
	}
};

const onFieldKeyChange = (row: any, key: string) => {
	const meta = state.pool.find((p) => p.fieldKey === key);
	if (!meta) return;
	row.dbColumn = meta.dbColumn;
	row.label = meta.label;
	row.controlType = meta.controlType;
	row.groupCode = meta.groupCode || row.groupCode;
	row.widthPx = meta.defaultWidthPx ?? row.widthPx;
	row.spanSlots = meta.defaultSpanSlots ?? row.spanSlots;
	row.required = meta.required;
	row.readonly = meta.readonly;
	row.locked = meta.locked;
};

const addRow = () => {
	const used = new Set(state.fields.map((f) => f.fieldKey));
	const next = state.pool.find((p) => !used.has(p.fieldKey));
	if (!next) {
		ElMessage.warning('字段池已全部添加');
		return;
	}
	state.fields.push({
		fieldKey: next.fieldKey,
		dbColumn: next.dbColumn,
		label: next.label,
		sortNo: state.fields.length + 1,
		visible: true,
		widthPx: next.defaultWidthPx ?? 100,
		spanSlots: next.defaultSpanSlots ?? 1,
		groupCode: next.groupCode ?? 'basic',
		controlType: next.controlType,
		required: next.required,
		readonly: next.readonly,
		locked: next.locked,
	});
};

const removeRow = (index: number) => {
	if (state.fields[index]?.locked) return;
	state.fields.splice(index, 1);
};

const move = (index: number, delta: number) => {
	const target = index + delta;
	if (target < 0 || target >= state.fields.length) return;
	const arr = state.fields;
	const tmp = arr[index];
	arr[index] = arr[target];
	arr[target] = tmp;
};

const handleSave = async () => {
	if (!state.fields.length) {
		ElMessage.warning('至少保留一个字段');
		return;
	}
	state.saving = true;
	try {
		await saveHrUiLayoutDefault({
			moduleCode: 'hrEmp',
			scene: props.scene,
			name: props.scene === 'list' ? '人员资料默认列表' : '人员资料默认表单',
			fields: state.fields.map((f, i) => ({
				fieldKey: f.fieldKey,
				dbColumn: f.dbColumn,
				label: f.label,
				sortNo: i + 1,
				visible: f.visible,
				widthPx: props.scene === 'list' ? f.widthPx : undefined,
				spanSlots: props.scene === 'form' ? f.spanSlots : undefined,
				groupCode: props.scene === 'form' ? f.groupCode : undefined,
				controlType: f.controlType,
				required: f.required,
				readonly: f.readonly,
			})),
		});
		ElMessage.success('已保存为系统默认');
		state.source = 'system';
	} finally {
		state.saving = false;
	}
};

onMounted(loadEffective);
</script>

<style scoped>
.layout-editor {
	height: 100%;
	display: flex;
	flex-direction: column;
	padding: 6px;
	box-sizing: border-box;
}
.toolbar {
	display: flex;
	align-items: center;
	justify-content: space-between;
	margin-bottom: 6px;
}
.toolbar-title {
	font-weight: 600;
	font-size: 15px;
}
.editor-panes {
	flex: 1;
	min-height: 0;
}
.pane-card {
	height: 100%;
}
.source-tag {
	float: right;
	font-size: 12px;
	color: var(--el-text-color-secondary);
	font-weight: normal;
}
.w100 {
	width: 100%;
}
.preview-table {
	width: 100%;
	border-collapse: collapse;
	font-size: 13px;
}
.preview-table th,
.preview-table td {
	border: 1px solid var(--el-border-color);
	padding: 6px 8px;
	text-align: center;
	white-space: nowrap;
}
.preview-table th {
	background: var(--el-fill-color-light);
}
.muted {
	color: var(--el-text-color-placeholder);
}
.group-title {
	font-weight: 600;
	margin: 8px 0 6px;
	font-size: 13px;
}
.preview-col {
	margin-bottom: 6px;
}
.preview-field {
	border: 1px dashed var(--el-border-color);
	border-radius: 4px;
	padding: 4px 6px;
	min-height: 36px;
	display: flex;
	flex-direction: column;
	gap: 2px;
}
.preview-field.photo {
	min-height: 80px;
	align-items: center;
	justify-content: center;
}
.preview-label {
	font-size: 12px;
	color: var(--el-text-color-secondary);
}
.preview-box {
	flex: 1;
	background: var(--el-fill-color-blank);
	border-radius: 2px;
	min-height: 22px;
	width: 100%;
	display: flex;
	align-items: center;
	justify-content: center;
	font-size: 12px;
	color: var(--el-text-color-placeholder);
}
</style>
