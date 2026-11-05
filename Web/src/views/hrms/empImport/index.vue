<template>
	<div class="emp-import-page">
		<el-card shadow="hover" class="toolbar-card">
			<div class="toolbar-row">
				<el-radio-group v-model="state.importMode" @change="onModeChange">
					<el-radio value="Add">{{ t('message.hrms.empImport.modeAddEmp') }}</el-radio>
					<el-radio value="Update">{{ t('message.hrms.empImport.modeUpdateEmp') }}</el-radio>
				</el-radio-group>

				<div class="toolbar-actions">
					<el-button type="primary" :loading="state.busy" @click="startImport">{{ t('message.hrms.empImport.btnImport') }}</el-button>
					<el-button :disabled="!state.file || !state.previewResult?.ok" :loading="state.committing" @click="handleCommit">
						{{ t('message.hrms.empImport.btnSave') }}
					</el-button>
					<el-button @click="openTplDesign()">{{ t('message.hrms.empImport.btnRedefine') }}</el-button>
					<el-button :disabled="!state.tplId" @click="handleDownload">{{ t('message.hrms.empImport.downloadTpl') }}</el-button>
					<el-button @click="state.historyVisible = true">{{ t('message.hrms.empImport.tabHistory') }}</el-button>
				</div>
			</div>
			<div class="hint-red">{{ t('message.hrms.empImport.headerRowTip') }}</div>
		</el-card>

		<el-card v-if="state.previewResult" shadow="hover" class="mt12">
			<el-alert
				:type="state.previewResult.ok ? 'success' : 'error'"
				:closable="false"
				show-icon
				:title="
					state.previewResult.ok
						? t('message.hrms.empImport.previewOk', { n: state.previewResult.totalCount })
						: state.previewResult.headerMismatch
							? t('message.hrms.empImport.headerMismatchTitle')
							: t('message.hrms.empImport.previewFail', { n: state.previewResult.errors?.length || 0 })
				"
			/>
			<div v-if="state.previewResult.headerMismatch" class="header-mismatch mt12">
				<div v-if="state.previewResult.headerMismatch.missing?.length" class="hm-row">
					<span class="hm-label">{{ t('message.hrms.empImport.headerMissing') }}：</span>
					<span class="hm-cols">{{ state.previewResult.headerMismatch.missing.join('、') }}</span>
				</div>
				<div v-if="state.previewResult.headerMismatch.extra?.length" class="hm-row">
					<span class="hm-label">{{ t('message.hrms.empImport.headerExtra') }}：</span>
					<span class="hm-cols">{{ state.previewResult.headerMismatch.extra.join('、') }}</span>
				</div>
				<div v-if="state.previewResult.headerMismatch.orderDiffs?.length" class="hm-row">
					<span class="hm-label">{{ t('message.hrms.empImport.headerOrderDiff') }}：</span>
					<span class="hm-cols">{{ state.previewResult.headerMismatch.orderDiffs.join('；') }}</span>
				</div>
				<div class="hm-row">
					<span class="hm-label">{{ t('message.hrms.empImport.headerExpected') }}：</span>
					<span class="hm-cols muted">{{ (state.previewResult.headerMismatch.expected || []).join(' | ') }}</span>
				</div>
				<div class="hm-row">
					<span class="hm-label">{{ t('message.hrms.empImport.headerActual') }}：</span>
					<span class="hm-cols muted">{{ (state.previewResult.headerMismatch.actual || []).join(' | ') }}</span>
				</div>
			</div>
			<el-table
				v-else-if="(state.previewResult.rows || []).length"
				:data="state.previewResult.rows"
				border
				size="small"
				class="mt12 preview-data-table"
				max-height="480"
				:row-class-name="previewRowClassName"
			>
				<el-table-column prop="rowNo" :label="t('message.hrms.empImport.rowNo')" width="72" fixed />
				<el-table-column
					v-for="col in state.previewResult.columns || []"
					:key="col.fieldKey"
					:prop="'cells.' + col.fieldKey"
					:label="col.label || col.fieldKey"
					min-width="110"
					show-overflow-tooltip
				>
					<template #default="{ row }">{{ row.cells?.[col.fieldKey] ?? '' }}</template>
				</el-table-column>
				<el-table-column
					prop="errorMessage"
					:label="t('message.hrms.empImport.exceptionCol')"
					min-width="220"
					fixed="right"
					show-overflow-tooltip
				>
					<template #default="{ row }">
						<span v-if="row.errorMessage" class="err-text">{{ row.errorMessage }}</span>
					</template>
				</el-table-column>
			</el-table>
			<el-table
				v-else-if="!state.previewResult.ok"
				:data="state.previewResult.errors || []"
				border
				size="small"
				class="mt12"
				max-height="360"
			>
				<el-table-column prop="rowNo" :label="t('message.hrms.empImport.rowNo')" width="90" />
				<el-table-column prop="empId" :label="t('message.hrms.common.empId')" width="140" />
				<el-table-column prop="errorMessage" :label="t('message.hrms.empImport.errorMessage')" min-width="280" show-overflow-tooltip />
			</el-table>
		</el-card>

		<input ref="fileInputRef" type="file" accept=".xlsx" class="hidden-file" @change="onNativeFileChange" />

		<!-- 模板设计：可选多列勾选 + 已选拖拽调序 -->
		<el-dialog
			v-model="state.designVisible"
			:title="t('message.hrms.empImport.tplDesign')"
			width="1080px"
			destroy-on-close
			:close-on-click-modal="false"
			class="tpl-design-dialog"
			@opened="bindSelectedSortable"
			@closed="destroySelectedSortable"
		>
			<el-form label-width="90px">
				<el-form-item :label="t('message.hrms.empImport.importMode')">
					<el-radio-group v-model="state.design.importMode" @change="onDesignModeChange">
						<el-radio value="Add">{{ t('message.hrms.empImport.modeAdd') }}</el-radio>
						<el-radio value="Update">{{ t('message.hrms.empImport.modeUpdate') }}</el-radio>
					</el-radio-group>
				</el-form-item>
				<el-form-item :label="t('message.hrms.empImport.fields')" required>
					<div class="field-picker">
						<div class="fp-panel fp-available">
							<div class="fp-head">
								<span>{{ t('message.hrms.empImport.poolAvailable') }}（{{ filteredAvailableFields.length }}）</span>
								<el-input
									v-model="state.poolKeyword"
									clearable
									size="small"
									class="fp-search"
									:placeholder="t('message.hrms.empImport.poolSearch')"
								/>
							</div>
							<div class="fp-body fp-grid">
								<button
									v-for="item in filteredAvailableFields"
									:key="item.key"
									type="button"
									class="fp-chip"
									@click="toggleField(item.key, true)"
								>
									{{ item.label }}
								</button>
								<div v-if="!filteredAvailableFields.length" class="fp-empty">{{ t('message.hrms.empImport.poolEmpty') }}</div>
							</div>
						</div>
						<div class="fp-panel fp-selected">
							<div class="fp-head">
								<span>{{ t('message.hrms.empImport.poolSelected') }}（{{ state.design.fieldKeys.length }}）</span>
								<span class="fp-hint">{{ t('message.hrms.empImport.dragSortTip') }}</span>
							</div>
							<ul ref="selectedListRef" class="fp-body fp-selected-list">
								<li v-for="(key, idx) in state.design.fieldKeys" :key="key" class="fp-selected-item" :data-key="key">
									<span class="drag-handle" title="drag">
										<el-icon><ele-Rank /></el-icon>
									</span>
									<span class="fp-idx">{{ idx + 1 }}</span>
									<span class="fp-name" :title="poolLabel(key)">{{ poolLabel(key) }}</span>
									<div class="fp-ops">
										<el-button link size="small" :disabled="idx === 0" @click="moveSelected(idx, -1)">↑</el-button>
										<el-button
											link
											size="small"
											:disabled="idx >= state.design.fieldKeys.length - 1"
											@click="moveSelected(idx, 1)"
										>
											↓
										</el-button>
										<el-button
											link
											type="danger"
											size="small"
											:disabled="isFieldLocked(key)"
											@click="removeSelected(key)"
										>
											{{ t('message.hrms.empImport.removeField') }}
										</el-button>
									</div>
								</li>
								<li v-if="!state.design.fieldKeys.length" class="fp-empty">{{ t('message.hrms.empImport.poolEmptySelected') }}</li>
							</ul>
						</div>
					</div>
					<div v-if="state.design.importMode === 'Update'" class="tip">{{ t('message.hrms.empImport.empIdLockedTip') }}</div>
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="state.designVisible = false">{{ t('message.hrms.empImport.cancel') }}</el-button>
				<el-button v-if="!state.pickFileAfterDesign" :loading="state.savingTpl" type="primary" @click="saveDesign(false)">
					{{ t('message.hrms.empImport.save') }}
				</el-button>
				<el-button v-else type="primary" :loading="state.savingTpl" @click="confirmDesignAndPickFile">
					{{ t('message.hrms.empImport.confirm') }}
				</el-button>
			</template>
		</el-dialog>

		<el-drawer v-model="state.historyVisible" :title="t('message.hrms.empImport.tabHistory')" size="70%" destroy-on-close>
			<vxe-grid ref="batchGridRef" v-bind="batchOptions">
				<template #row_status="{ row }">
					<el-tag :type="row.status === 'Success' ? 'success' : 'danger'" size="small">{{ row.status }}</el-tag>
				</template>
				<template #row_buttons="{ row }">
					<el-button v-if="row.failCount > 0" text type="primary" @click="openFailRows(row)">
						{{ t('message.hrms.empImport.failDetail') }}
					</el-button>
				</template>
			</vxe-grid>
		</el-drawer>

		<el-drawer v-model="state.failDrawer" :title="t('message.hrms.empImport.failDetail')" size="50%" destroy-on-close>
			<el-table :data="state.failRows" border size="small" v-loading="state.failLoading">
				<el-table-column prop="rowNo" :label="t('message.hrms.empImport.rowNo')" width="90" />
				<el-table-column prop="empId" :label="t('message.hrms.common.empId')" width="140" />
				<el-table-column prop="errorMessage" :label="t('message.hrms.empImport.errorMessage')" min-width="280" show-overflow-tooltip />
			</el-table>
		</el-drawer>
	</div>
</template>

<script lang="ts" setup name="hrEmpImport">
import { computed, nextTick, onBeforeUnmount, onMounted, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import type { VxeGridInstance } from 'vxe-table';
import Sortable from 'sortablejs';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { useI18n } from 'vue-i18n';
import { downloadByData, getFileName } from '/@/utils/download';
import {
	batchPageHrEmpImport,
	commitHrEmpImport,
	downloadTemplateHrEmpImport,
	failRowsHrEmpImport,
	fieldPoolHrEmpImport,
	previewHrEmpImport,
	tplByModeHrEmpImport,
	tplSaveHrEmpImport,
	type ImportMode,
} from '/@/api/hrms/hrEmpImport';

const { t } = useI18n();
const fileInputRef = ref<HTMLInputElement | null>(null);
const batchGridRef = ref<VxeGridInstance>();
const selectedListRef = ref<HTMLElement | null>(null);
let selectedSortable: Sortable | null = null;

const state = reactive({
	importMode: 'Add' as ImportMode,
	tplId: undefined as number | undefined,
	file: null as File | null,
	busy: false,
	previewing: false,
	committing: false,
	previewResult: null as null | {
		ok: boolean;
		totalCount: number;
		errors: any[];
		columns?: { fieldKey: string; label: string }[];
		rows?: { rowNo: number; cells: Record<string, string | null>; errorMessage?: string | null }[];
		headerMismatch?: {
			expected: string[];
			actual: string[];
			missing: string[];
			extra: string[];
			orderDiffs: string[];
		} | null;
	},
	designVisible: false,
	pickFileAfterDesign: false,
	savingTpl: false,
	poolKeyword: '',
	design: {
		id: undefined as number | undefined,
		importMode: 'Add' as ImportMode,
		fieldKeys: [] as string[],
	},
	poolTransfer: [] as { key: string; label: string; disabled?: boolean }[],
	historyVisible: false,
	failDrawer: false,
	failLoading: false,
	failRows: [] as any[],
});

const filteredAvailableFields = computed(() => {
	const kw = state.poolKeyword.trim().toLowerCase();
	const selected = new Set(state.design.fieldKeys);
	return state.poolTransfer.filter((u) => {
		if (selected.has(u.key)) return false;
		if (kw && !`${u.label}${u.key}`.toLowerCase().includes(kw)) return false;
		return true;
	});
});

function poolLabel(key: string) {
	return state.poolTransfer.find((u) => u.key === key)?.label || key;
}

function isFieldLocked(key: string) {
	return state.design.importMode === 'Update' && key === 'empId';
}

function toggleField(key: string, checked: boolean) {
	if (checked) {
		if (!state.design.fieldKeys.includes(key)) state.design.fieldKeys.push(key);
		return;
	}
	if (isFieldLocked(key)) return;
	state.design.fieldKeys = state.design.fieldKeys.filter((k) => k !== key);
}

function removeSelected(key: string) {
	if (isFieldLocked(key)) return;
	state.design.fieldKeys = state.design.fieldKeys.filter((k) => k !== key);
}

function moveSelected(index: number, delta: number) {
	const next = index + delta;
	if (next < 0 || next >= state.design.fieldKeys.length) return;
	const arr = [...state.design.fieldKeys];
	const [item] = arr.splice(index, 1);
	arr.splice(next, 0, item);
	state.design.fieldKeys = arr;
}

function bindSelectedSortable() {
	destroySelectedSortable();
	nextTick(() => {
		const el = selectedListRef.value;
		if (!el) return;
		selectedSortable = Sortable.create(el, {
			handle: '.drag-handle',
			animation: 150,
			draggable: '.fp-selected-item',
			onEnd: (evt) => {
				const oldIndex = evt.oldIndex ?? -1;
				const newIndex = evt.newIndex ?? -1;
				if (oldIndex < 0 || newIndex < 0 || oldIndex === newIndex) return;
				const arr = [...state.design.fieldKeys];
				const [moved] = arr.splice(oldIndex, 1);
				arr.splice(newIndex, 0, moved);
				state.design.fieldKeys = arr;
			},
		});
	});
}

function destroySelectedSortable() {
	selectedSortable?.destroy();
	selectedSortable = null;
}

onBeforeUnmount(() => destroySelectedSortable());

const batchLocalKey = 'localPageParam:hrEmpImportBatch';
const batchOptions = useVxeTable(
	{
		id: 'hrEmpImportBatch',
		name: t('message.hrms.empImport.tabHistory'),
		columns: [
			{ field: 'seq', type: 'seq', title: t('message.hrms.empImport.seq'), width: 60 },
			{ field: 'batchNo', title: t('message.hrms.empImport.batchNo'), minWidth: 180, sortable: true },
			{ field: 'importMode', title: t('message.hrms.empImport.importMode'), width: 100 },
			{ field: 'tplNameSnapshot', title: t('message.hrms.empImport.template'), minWidth: 120 },
			{ field: 'fileName', title: t('message.hrms.empImport.fileName'), minWidth: 140 },
			{ field: 'totalCount', title: t('message.hrms.empImport.totalCount'), width: 90 },
			{ field: 'successCount', title: t('message.hrms.empImport.successCount'), width: 90 },
			{ field: 'failCount', title: t('message.hrms.empImport.failCount'), width: 90 },
			{ field: 'status', title: t('message.hrms.empImport.status'), width: 100, slots: { default: 'row_status' } },
			{ field: 'createTime', title: t('message.hrms.empImport.createTime'), width: 170, sortable: true },
			{ field: 'buttons', title: t('message.hrms.empImport.operation'), width: 120, fixed: 'right', slots: { default: 'row_buttons' } },
		],
	},
	{
		proxyConfig: {
			autoLoad: true,
			ajax: {
				query: ({ page, sort }: any) => {
					const s = sort || Local.get(batchLocalKey)?.defaultSort || { field: 'createTime', order: 'desc' };
					return batchPageHrEmpImport({
						page: page.currentPage,
						pageSize: page.pageSize,
						field: s.field,
						order: s.order,
						descStr: 'desc',
					}).then((res: any) => res.data?.result ?? res.data);
				},
			},
		},
		sortConfig: { defaultSort: Local.get(batchLocalKey)?.defaultSort || { field: 'createTime', order: 'desc', descStr: 'desc' } },
		pagerConfig: { pageSize: Local.get(batchLocalKey)?.pageSize || 20 },
	}
);

const unwrap = (res: any) => res?.data?.result ?? res?.data ?? res;

async function loadModeTpl() {
	const detail = unwrap(await tplByModeHrEmpImport(state.importMode));
	state.tplId = detail?.id;
	return detail;
}

function onModeChange() {
	clearFile();
	loadModeTpl();
}

function clearFile() {
	state.file = null;
	state.previewResult = null;
	if (fileInputRef.value) fileInputRef.value.value = '';
}

async function startImport() {
	clearFile();
	state.pickFileAfterDesign = true;
	await openTplDesign(true);
}

async function openTplDesign(forImport = false) {
	state.pickFileAfterDesign = !!forImport;
	state.poolKeyword = '';
	state.design.importMode = state.importMode;
	await applyModeToDesign(state.importMode);
	state.designVisible = true;
}

async function onDesignModeChange() {
	state.importMode = state.design.importMode;
	await applyModeToDesign(state.design.importMode);
}

async function applyModeToDesign(mode: ImportMode) {
	const detail = unwrap(await tplByModeHrEmpImport(mode));
	state.design.id = detail.id;
	state.design.importMode = detail.importMode;
	state.design.fieldKeys = (detail.fields || []).map((f: any) => f.fieldKey);
	if (mode === 'Update' && !state.design.fieldKeys.includes('empId')) {
		state.design.fieldKeys = ['empId', ...state.design.fieldKeys];
	}
	state.tplId = detail.id;
	await loadPoolForDesign();
}

async function loadPoolForDesign() {
	const pool = unwrap(await fieldPoolHrEmpImport(state.design.importMode)) || [];
	state.poolTransfer = pool.map((u: any) => ({
		key: u.fieldKey,
		label: u.label || u.fieldKey,
		disabled: state.design.importMode === 'Update' && u.fieldKey === 'empId',
	}));
}

async function saveDesign(keepOpen: boolean): Promise<number | undefined> {
	let keys = [...state.design.fieldKeys];
	if (state.design.importMode === 'Update' && !keys.includes('empId')) keys = ['empId', ...keys];
	if (keys.length === 0) {
		ElMessage.warning(t('message.hrms.empImport.fieldsRequired'));
		return;
	}
	state.savingTpl = true;
	try {
		const res = await tplSaveHrEmpImport({
			importMode: state.design.importMode,
			fields: keys.map((k, i) => ({ fieldKey: k, sortNo: (i + 1) * 10 })),
		});
		const id = unwrap(res) as number;
		state.design.id = id;
		state.tplId = id;
		state.importMode = state.design.importMode;
		ElMessage.success(t('message.hrms.empImport.saved'));
		if (!keepOpen) state.designVisible = false;
		return id;
	} finally {
		state.savingTpl = false;
	}
}

async function confirmDesignAndPickFile() {
	const id = await saveDesign(true);
	if (!id) return;
	state.designVisible = false;
	await nextTick();
	triggerPickFile();
}

function triggerPickFile() {
	if (!state.tplId) {
		ElMessage.warning(t('message.hrms.empImport.selectTpl'));
		return;
	}
	fileInputRef.value?.click();
}

async function onNativeFileChange(e: Event) {
	const input = e.target as HTMLInputElement;
	const file = input.files?.[0];
	if (!file) return;
	if (!file.name.toLowerCase().endsWith('.xlsx')) {
		ElMessage.error(t('message.hrms.empImport.xlsxOnly'));
		input.value = '';
		return;
	}
	state.file = file;
	state.previewResult = null;
	await runPreview();
}

async function runPreview() {
	if (!state.tplId || !state.file) return;
	state.previewing = true;
	state.busy = true;
	try {
		const res = await previewHrEmpImport(state.tplId, state.file);
		state.previewResult = unwrap(res);
		if (state.previewResult?.ok) {
			ElMessage.success(t('message.hrms.empImport.previewOk', { n: state.previewResult.totalCount }));
		}
	} finally {
		state.previewing = false;
		state.busy = false;
	}
}

function previewRowClassName({ row }: { row: { errorMessage?: string | null } }) {
	return row?.errorMessage ? 'preview-row-error' : '';
}

async function handleDownload() {
	if (!state.tplId) {
		await loadModeTpl();
	}
	if (!state.tplId) return;
	const res: any = await downloadTemplateHrEmpImport(state.tplId);
	const name = getFileName(res) || `emp-import-${state.importMode}.xlsx`;
	downloadByData(res.data || res, name);
}

async function handleCommit() {
	if (!state.tplId || !state.file || !state.previewResult?.ok) return;
	await ElMessageBox.confirm(t('message.hrms.empImport.confirmCommit'), t('message.hrms.empImport.tip'));
	state.committing = true;
	try {
		const res = await commitHrEmpImport(state.tplId, state.file);
		const out = unwrap(res);
		if (out?.status === 'Success') {
			ElMessage.success(t('message.hrms.empImport.commitOk', { n: out.successCount }));
			clearFile();
			batchGridRef.value?.commitProxy('query');
		} else {
			ElMessage.error(t('message.hrms.empImport.commitFail'));
			state.previewResult = { ok: false, totalCount: out?.totalCount || 0, errors: out?.errors || [] };
		}
	} finally {
		state.committing = false;
	}
}

async function openFailRows(row: any) {
	state.failDrawer = true;
	state.failLoading = true;
	try {
		const res = await failRowsHrEmpImport({ batchId: row.id, page: 1, pageSize: 500, field: 'rowNo', order: 'asc', descStr: 'desc' });
		state.failRows = unwrap(res)?.items || [];
	} finally {
		state.failLoading = false;
	}
}

onMounted(() => {
	loadModeTpl();
});
</script>

<style scoped>
.emp-import-page {
	padding: 8px;
}
.toolbar-card :deep(.el-card__body) {
	padding: 12px 16px;
}
.toolbar-row {
	display: flex;
	flex-wrap: wrap;
	align-items: center;
	gap: 12px 20px;
}
.toolbar-actions {
	display: flex;
	flex-wrap: wrap;
	gap: 8px;
}
.hint-red {
	margin-top: 10px;
	color: #d93026;
	font-size: 13px;
}
.hint-muted {
	margin-top: 4px;
	color: var(--el-text-color-secondary);
	font-size: 12px;
}
.current-file {
	margin-top: 8px;
	font-size: 13px;
}
.mt12 {
	margin-top: 12px;
}
.hidden-file {
	display: none;
}
.tip {
	margin-top: 8px;
	color: var(--el-text-color-secondary);
	font-size: 12px;
}
.header-mismatch {
	padding: 12px 14px;
	background: var(--el-color-danger-light-9);
	border: 1px solid var(--el-color-danger-light-5);
	border-radius: 4px;
	font-size: 13px;
	line-height: 1.7;
}
.hm-row + .hm-row {
	margin-top: 4px;
}
.hm-label {
	font-weight: 600;
	color: var(--el-color-danger);
	white-space: nowrap;
}
.hm-cols {
	color: var(--el-text-color-primary);
	word-break: break-all;
}
.hm-cols.muted {
	color: var(--el-text-color-regular);
}
.err-text {
	color: var(--el-color-danger);
	white-space: pre-wrap;
}
.preview-data-table :deep(.preview-row-error) {
	--el-table-tr-bg-color: var(--el-color-danger-light-9);
}
.field-picker {
	display: grid;
	grid-template-columns: 1.4fr 1fr;
	gap: 12px;
	width: 100%;
}
.fp-panel {
	border: 1px solid var(--el-border-color);
	border-radius: 6px;
	background: var(--el-bg-color);
	min-height: 420px;
	display: flex;
	flex-direction: column;
	overflow: hidden;
}
.fp-head {
	display: flex;
	align-items: center;
	justify-content: space-between;
	gap: 8px;
	padding: 8px 10px;
	border-bottom: 1px solid var(--el-border-color-lighter);
	font-weight: 600;
	font-size: 13px;
}
.fp-search {
	width: 180px;
}
.fp-hint {
	font-weight: 400;
	font-size: 12px;
	color: var(--el-text-color-secondary);
}
.fp-body {
	flex: 1;
	overflow: auto;
	padding: 8px;
}
.fp-grid {
	display: grid;
	grid-template-columns: repeat(3, minmax(0, 1fr));
	gap: 4px 8px;
	align-content: start;
}
.fp-chip {
	margin: 0;
	padding: 6px 8px;
	border-radius: 4px;
	min-width: 0;
	text-align: left;
	font-size: 13px;
	line-height: 1.35;
	color: var(--el-text-color-primary);
	background: var(--el-fill-color-blank);
	border: 1px solid var(--el-border-color-lighter);
	cursor: pointer;
	word-break: break-all;
}
.fp-chip:hover {
	border-color: var(--el-color-primary-light-3);
	color: var(--el-color-primary);
	background: var(--el-color-primary-light-9);
}
.fp-selected-list {
	list-style: none;
	margin: 0;
	padding: 0;
}
.fp-selected-item {
	display: flex;
	align-items: center;
	gap: 6px;
	padding: 6px 8px;
	border: 1px solid var(--el-border-color-lighter);
	border-radius: 4px;
	margin-bottom: 6px;
	background: var(--el-fill-color-blank);
}
.fp-selected-item:hover {
	border-color: var(--el-color-primary-light-5);
}
.drag-handle {
	cursor: grab;
	color: var(--el-text-color-secondary);
	display: inline-flex;
	padding: 2px;
}
.drag-handle:active {
	cursor: grabbing;
}
.fp-idx {
	width: 22px;
	color: var(--el-text-color-secondary);
	font-size: 12px;
	text-align: right;
}
.fp-name {
	flex: 1;
	min-width: 0;
	overflow: hidden;
	text-overflow: ellipsis;
	white-space: nowrap;
	font-size: 13px;
}
.fp-ops {
	display: flex;
	align-items: center;
	gap: 0;
	flex-shrink: 0;
}
.fp-empty {
	grid-column: 1 / -1;
	padding: 24px 8px;
	text-align: center;
	color: var(--el-text-color-secondary);
	font-size: 13px;
	list-style: none;
}
@media (max-width: 1100px) {
	.fp-grid {
		grid-template-columns: repeat(2, minmax(0, 1fr));
	}
}
</style>
