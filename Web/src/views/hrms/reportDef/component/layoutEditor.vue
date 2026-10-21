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
					<el-table :key="tableRenderKey" ref="tableRef" :data="state.fields" border size="small" height="100%" style="flex: 1" row-key="fieldKey" :row-class-name="rowClass">
						<el-table-column label="操作" width="72" align="center">
							<template #default="{ $index, row }">
								<span class="drag-btn" title="按住拖拽排序">
									<el-icon><ele-Rank /></el-icon>
								</span>
								<el-button link type="danger" :disabled="row.locked" @click="removeRow($index)">删</el-button>
							</template>
						</el-table-column>
						<el-table-column type="index" label="序号" width="50" />
						<el-table-column label="表名" width="110" prop="tableName">
							<template #default>h_emp_mstr</template>
						</el-table-column>
						<el-table-column label="显示名" min-width="140">
							<template #default="{ row }">
								<el-input v-model="row.label" size="small" />
								<el-button link type="primary" size="small" @click="restoreLabel(row)">恢复池默认</el-button>
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
						<el-table-column label="控件" width="90" prop="controlType" />
						<el-table-column label="代码类" min-width="120" show-overflow-tooltip>
							<template #default="{ row }">
								<span>{{ row.codeField || '—' }}</span>
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
						<el-button type="primary" link style="margin-left: 8px" @click="generatePreview">生成</el-button>
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
							<div class="preview-group" :class="{ 'has-photo': !!group.photo }">
								<div v-if="group.photo" class="preview-photo-anchor">
									<div class="preview-field photo">
										<span class="preview-label">{{ group.photo.label }}</span>
										<span class="preview-box">照片</span>
									</div>
								</div>
								<div class="preview-fields" :style="{ '--hr-emp-cols': group.photo ? 5 : 6 }">
									<div
										v-for="f in group.fields"
										:key="f.fieldKey"
										class="preview-col"
										:style="{ gridColumn: `span ${Math.min(Number(f.spanSlots) || 1, group.photo ? 5 : 6)}` }"
									>
										<div class="preview-field">
											<span class="preview-label">{{ f.label }}</span>
											<span class="preview-box"></span>
										</div>
									</div>
								</div>
							</div>
						</template>
					</div>
				</el-card>
			</pane>
		</splitpanes>
	</div>
</template>

<script lang="ts" setup>
import { computed, nextTick, onMounted, onUnmounted, reactive, ref, watch } from 'vue';
import { ElMessage } from 'element-plus';
import { useI18n } from 'vue-i18n';
import Sortable from 'sortablejs';
import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';
import { getHrUiFieldPool, getHrUiLayoutDefault, getHrUiLayoutEffective, saveHrUiLayoutDefault, type HrUiLayoutScene } from '/@/api/hrms/hrUiLayout';
import { hrUiCulture } from '/@/views/hrms/composables/hrUiCulture';

const props = defineProps<{
	scene: HrUiLayoutScene;
	title: string;
}>();

const { locale } = useI18n();
const tableRef = ref();
const tableRenderKey = ref(0);
let sortableInst: Sortable | null = null;
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

/** 报表显示区快照。改左侧字段不会改它，只有「生成」或重新加载后才更新。 */
const previewFields = ref<any[]>([]);

const cloneFields = (list: any[]) => JSON.parse(JSON.stringify(list ?? []));

const generatePreview = () => {
	previewFields.value = cloneFields(state.fields);
};

const visibleFields = computed(() => previewFields.value.filter((f) => f.visible !== false));

const formGroups = computed(() => {
	const basic = visibleFields.value.filter((f) => (f.groupCode || 'basic') === 'basic');
	const job = visibleFields.value.filter((f) => f.groupCode === 'job');
	const groups = [];
	const splitPhoto = (fields: any[]) => {
		const photo = fields.find((f) => f.fieldKey === 'empPic' || f.controlType === 'photo') || null;
		return { photo, fields: fields.filter((f) => f !== photo) };
	};
	if (basic.length) groups.push({ code: 'basic', title: '基本信息', ...splitPhoto(basic) });
	if (job.length) groups.push({ code: 'job', title: '任职信息', ...splitPhoto(job) });
	return groups;
});

const availablePool = (row: any) => {
	const used = new Set(state.fields.filter((f) => f !== row).map((f) => f.fieldKey));
	return state.pool.filter((p) => p.fieldKey === row.fieldKey || !used.has(p.fieldKey));
};

const mapFields = (fields: any[]) => {
	const seen = new Set<string>();
	return (fields || [])
		.map((f: any, i: number) => {
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
				codeField: f.codeField ?? meta?.codeField,
				controlType: (f.codeField ?? meta?.codeField)
					? 'select'
					: (f.controlType || meta?.controlType || 'text'),
				required: !!(f.required || meta?.required),
				readonly: !!(f.readonly || meta?.readonly),
				locked: !!(f.locked || meta?.locked),
				sortable: meta?.sortable !== false,
				isStale: !!f.isStale,
				poolLabel: meta?.label || f.fieldKey,
			};
		})
		.filter((f) => {
			const key = (f.fieldKey || '').toLowerCase();
			if (!key || seen.has(key)) return false;
			seen.add(key);
			return true;
		});
};

/** 按 fieldKey 去重，保留首次出现（拖拽与 el-table 不同步时可能复制出行） */
const dedupeFields = (list: any[]) => {
	const seen = new Set<string>();
	return list.filter((f) => {
		const key = (f.fieldKey || '').toLowerCase();
		if (!key || seen.has(key)) return false;
		seen.add(key);
		return true;
	});
};

const remountTable = async () => {
	destroyRowDrop();
	tableRenderKey.value += 1;
	await nextTick();
	initRowDrop();
};

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
		generatePreview();
		const stale = state.fields.filter((f) => f.isStale).map((f) => f.fieldKey);
		if (stale.length) ElMessage.warning(`以下字段已失效，运行时不会显示：${stale.join('、')}`);
		await remountTable();
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
		generatePreview();
		ElMessage.success('已加载系统默认');
		await remountTable();
	} finally {
		state.loading = false;
	}
};

const onFieldKeyChange = (row: any, key: string) => {
	const meta = state.pool.find((p) => p.fieldKey === key);
	if (!meta) return;
	row.dbColumn = meta.dbColumn;
	row.label = meta.label;
	row.controlType = meta.codeField ? 'select' : meta.controlType;
	row.groupCode = meta.groupCode || row.groupCode;
	row.widthPx = meta.defaultWidthPx ?? row.widthPx;
	row.spanSlots = meta.defaultSpanSlots ?? row.spanSlots;
	row.required = meta.required;
	row.readonly = meta.readonly;
	row.locked = meta.locked;
	row.codeField = meta.codeField;
	row.poolLabel = meta.label;
	row.isStale = false;
};

const restoreLabel = (row: any) => {
	row.label = row.poolLabel || row.fieldKey;
};

const rowClass = ({ row }: { row: any }) => (row.isStale ? 'is-stale' : '');

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
		codeField: next.codeField,
		poolLabel: next.label,
		isStale: false,
	});
};

const removeRow = (index: number) => {
	if (state.fields[index]?.locked) return;
	state.fields.splice(index, 1);
};

const destroyRowDrop = () => {
	sortableInst?.destroy();
	sortableInst = null;
};

const initRowDrop = () => {
	destroyRowDrop();
	const root = tableRef.value?.$el as HTMLElement | undefined;
	const tbody = root?.querySelector('.el-table__body-wrapper tbody') as HTMLElement | null;
	if (!tbody) return;
	sortableInst = Sortable.create(tbody, {
		animation: 150,
		handle: '.drag-btn',
		ghostClass: 'drag-ghost',
		chosenClass: 'drag-chosen',
		// 避免与 el-table 虚拟 DOM 抢行节点导致保存后行翻倍
		forceFallback: true,
		fallbackOnBody: true,
		onEnd: async (evt) => {
			const oldIndex = evt.oldIndex;
			const newIndex = evt.newIndex;
			if (oldIndex == null || newIndex == null || oldIndex === newIndex) return;
			const list = dedupeFields([...state.fields]);
			const [moved] = list.splice(oldIndex, 1);
			if (!moved) return;
			list.splice(newIndex, 0, moved);
			state.fields = list;
			// 整表重挂，清掉 Sortable 挪过的 DOM，防止下次保存/重绘复制行
			await remountTable();
		},
	});
};

const handleSave = async () => {
	destroyRowDrop();
	state.fields = dedupeFields(state.fields);
	if (!state.fields.length) {
		ElMessage.warning('至少保留一个字段');
		await remountTable();
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
				required: f.required,
				readonly: f.readonly,
				labels: [{ culture: hrUiCulture(locale.value), label: f.label === f.poolLabel ? '' : f.label }],
			})),
		});
		ElMessage.success('已保存为系统默认');
		// 从服务端重载，避免页面上残留拖拽导致的重复行
		await loadEffective();
	} finally {
		state.saving = false;
	}
};

watch(locale, () => loadEffective());
onMounted(loadEffective);
onUnmounted(destroyRowDrop);
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
.preview-group.has-photo {
	position: relative;
	padding-right: 108px;
	min-height: 132px;
}
.preview-photo-anchor {
	position: absolute;
	top: 0;
	right: 0;
	width: 96px;
}
.preview-fields {
	display: grid;
	grid-template-columns: repeat(var(--hr-emp-cols, 6), minmax(0, 1fr));
	column-gap: 6px;
}
.preview-col {
	margin-bottom: 6px;
	min-width: 0;
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
	width: 90px;
	height: 120px;
	min-height: 120px;
	align-items: center;
	justify-content: center;
	box-sizing: border-box;
}
.preview-label {
	font-size: 12px;
	color: var(--el-text-color-secondary);
}
:deep(.is-stale) {
	background: #fff7e6;
}
.drag-btn {
	display: inline-flex;
	align-items: center;
	justify-content: center;
	margin-right: 4px;
	cursor: grab;
	color: var(--el-text-color-secondary);
	vertical-align: middle;
}
.drag-btn:active {
	cursor: grabbing;
}
:deep(.drag-ghost) {
	opacity: 0.55;
	background: var(--el-color-primary-light-9) !important;
}
:deep(.drag-chosen) {
	background: var(--el-fill-color-light) !important;
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
