<template>
	<div class="hr-field-def">
		<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', alignItems: 'start' }">
			<el-form :model="state.queryParams" :inline="true" label-width="auto" style="flex: 1">
				<el-form-item :label="t('message.hrms.fieldDef.keyword')">
					<el-input v-model="state.queryParams.keyword" :placeholder="t('message.hrms.fieldDef.keywordPh')" clearable @keyup.enter="handleQuery(true)" />
				</el-form-item>
				<el-form-item :label="t('message.hrms.fieldDef.enabled')">
					<el-select v-model="state.queryParams.enabled" clearable :placeholder="t('message.hrms.common.all')" style="width: 110px">
						<el-option :label="t('message.hrms.common.enabled')" :value="true" />
						<el-option :label="t('message.hrms.common.disabled')" :value="false" />
					</el-select>
				</el-form-item>
				<el-form-item :label="t('message.hrms.fieldDef.inPool')">
					<el-select v-model="state.queryParams.isInPool" clearable :placeholder="t('message.hrms.common.all')" style="width: 110px">
						<el-option :label="t('message.hrms.common.yes')" :value="true" />
						<el-option :label="t('message.hrms.common.no')" :value="false" />
					</el-select>
				</el-form-item>
			</el-form>
			<el-button-group>
				<el-button type="primary" icon="ele-Search" @click="handleQuery(true)">{{ t('message.list.query') }}</el-button>
				<el-button icon="ele-Refresh" @click="resetQuery">{{ t('message.list.reset') }}</el-button>
			</el-button-group>
		</el-card>
		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options" v-on="gridEvents">
				<template #toolbar_buttons>
					<el-button type="primary" icon="ele-Refresh" v-auth="'hrUiFieldDef/syncFromDb'" :loading="state.syncing" @click="handleSync">{{ t('message.hrms.fieldDef.syncFromDb') }}</el-button>
				</template>
				<template #empty><el-empty :image-size="160" /></template>
				<template #row_codeField="{ row }">
					<span>{{ row.codeField || '—' }}</span>
					<el-button
						v-if="row.codeField"
						link
						type="primary"
						size="small"
						@click="openCodeSetting(row.codeField)"
					>{{ t('message.hrms.fieldDef.codeSetting') }}</el-button>
				</template>
				<template #row_buttons="{ row }">
					<el-button icon="ele-Edit" text type="primary" v-auth="'hrUiFieldDef/update'" @click="openEdit(row)" />
					<el-button icon="ele-ChatLineSquare" text type="primary" v-auth="'hrUiFieldDef/saveLang'" @click="openLang(row)" />
				</template>
			</vxe-grid>
		</el-card>

		<CodeSettingDrawer ref="codeDrawerRef" />

		<el-dialog v-model="state.editVisible" :title="t('message.hrms.fieldDef.dialogTitle')" width="640px" destroy-on-close>
			<el-form :model="state.edit" label-width="96px">
				<el-form-item :label="t('message.hrms.fieldDef.fieldKey')"><el-input :model-value="state.edit.fieldKey" disabled /></el-form-item>
				<el-form-item :label="t('message.hrms.fieldDef.label')" required><el-input v-model="state.edit.label" maxlength="256" /></el-form-item>
				<el-form-item :label="t('message.hrms.fieldDef.origin')">
					<el-select v-model="state.edit.origin" class="w100">
						<el-option label="physical" value="physical" />
						<el-option label="computed" value="computed" />
						<el-option label="lookup" value="lookup" />
					</el-select>
				</el-form-item>
				<el-form-item :label="t('message.hrms.fieldDef.dbColumn')"><el-input v-model="state.edit.dbColumn" /></el-form-item>
				<el-form-item :label="t('message.hrms.fieldDef.dataType')"><el-input v-model="state.edit.dataType" /></el-form-item>
				<el-form-item :label="t('message.hrms.fieldDef.control')">
					<el-select v-model="state.edit.controlType" class="w100">
						<el-option v-for="c in controls" :key="c" :label="c" :value="c" />
					</el-select>
				</el-form-item>
				<el-form-item :label="t('message.hrms.fieldDef.codeField')">
					<div class="code-field-row">
						<el-input
							v-model="state.edit.codeField"
							:placeholder="t('message.hrms.fieldDef.codeFieldPh')"
							clearable
							:disabled="!!state.edit.lookupSource?.trim()"
							@change="onCodeFieldChange"
						/>
						<el-button
							type="primary"
							plain
							:disabled="!state.edit.codeField?.trim()"
							@click="openCodeSetting(state.edit.codeField)"
						>{{ t('message.hrms.fieldDef.codeSetting') }}</el-button>
					</div>
					<div class="hint">{{ t('message.hrms.fieldDef.codeFieldHint') }}</div>
				</el-form-item>
				<el-form-item :label="t('message.hrms.fieldDef.lookupSource')">
					<div class="code-field-row">
						<el-select
							v-model="state.edit.lookupSource"
							clearable
							filterable
							class="w100"
							:placeholder="t('message.hrms.fieldDef.lookupSourcePh')"
							:disabled="!!state.edit.codeField?.trim()"
							@change="onLookupSourceChange"
						>
							<el-option v-for="s in state.lookupSources" :key="s.code" :label="`${s.code} · ${s.name}`" :value="s.code" />
						</el-select>
						<el-button type="primary" plain :disabled="!currentLookupMaintainPath" @click="openLookupMaintain">
							{{ t('message.hrms.fieldDef.lookupMaintain') }}
						</el-button>
					</div>
					<div class="hint">{{ t('message.hrms.fieldDef.lookupSourceHint') }}</div>
					<div v-if="state.edit.controlType === 'select' && !state.edit.codeField && !state.edit.lookupSource" class="hint">{{ t('message.hrms.fieldDef.selectNoCode') }}</div>
				</el-form-item>
				<el-form-item :label="t('message.hrms.fieldDef.width')"><el-input-number v-model="state.edit.defaultWidthPx" :min="40" :max="600" /></el-form-item>
				<el-form-item :label="t('message.hrms.fieldDef.span')"><el-input-number v-model="state.edit.defaultSpanSlots" :min="1" :max="6" /></el-form-item>
				<el-form-item :label="t('message.hrms.fieldDef.group')"><el-input v-model="state.edit.groupCode" /></el-form-item>
				<el-form-item :label="t('message.hrms.fieldDef.sortNo')"><el-input-number v-model="state.edit.sortNo" /></el-form-item>
				<el-form-item :label="t('message.hrms.fieldDef.switches')">
					<el-checkbox v-model="state.edit.isInPool">{{ t('message.hrms.fieldDef.inPool') }}</el-checkbox>
					<el-checkbox v-model="state.edit.sortable">{{ t('message.hrms.fieldDef.sortable') }}</el-checkbox>
					<el-checkbox v-model="state.edit.required">{{ t('message.hrms.fieldDef.required') }}</el-checkbox>
					<el-checkbox v-model="state.edit.readonly">{{ t('message.hrms.fieldDef.readonly') }}</el-checkbox>
					<el-checkbox v-model="state.edit.locked">{{ t('message.hrms.fieldDef.locked') }}</el-checkbox>
					<el-checkbox v-model="state.edit.enabled">{{ t('message.hrms.fieldDef.enabled') }}</el-checkbox>
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="state.editVisible = false">{{ t('message.list.cancel') }}</el-button>
				<el-button type="primary" :loading="state.saving" @click="saveEdit">{{ t('message.list.save') }}</el-button>
			</template>
		</el-dialog>

		<el-dialog v-model="state.langVisible" :title="t('message.hrms.fieldDef.langTitle')" width="520px" destroy-on-close>
			<el-form label-width="88px">
				<el-form-item v-for="item in state.langs" :key="item.culture" :label="item.culture">
					<el-input v-model="item.label" maxlength="256" />
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="state.langVisible = false">{{ t('message.list.cancel') }}</el-button>
				<el-button type="primary" :loading="state.saving" @click="saveLang">{{ t('message.list.save') }}</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="hrUiFieldDef">
import { computed, onMounted, reactive, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { useRouter } from 'vue-router';
import { langListHrUiFieldDef, pageHrUiFieldDef, saveLangHrUiFieldDef, syncHrUiFieldDef, updateHrUiFieldDef } from '/@/api/hrms/hrUiFieldDef';
import { listEnabledHrLookupDef } from '/@/api/hrms/hrLookupDef';
import { hrUiCultures } from '/@/views/hrms/composables/hrUiCulture';
import CodeSettingDrawer from '/@/views/hrms/reportDef/component/codeSettingDrawer.vue';

const { t, locale } = useI18n();
const router = useRouter();
const controls = ['text', 'number', 'date', 'select', 'photo', 'cascader', 'computed'];
const xGrid = ref<VxeGridInstance>();
const codeDrawerRef = ref<InstanceType<typeof CodeSettingDrawer>>();
const localPageParamKey = 'localPageParam:hrUiFieldDef';
const state = reactive({
	queryParams: { moduleCode: 'hrEmp', keyword: '', enabled: undefined as boolean | undefined, isInPool: undefined as boolean | undefined },
	localPageParam: { pageSize: 50 as number, defaultSort: { field: 'sortNo', order: 'asc', descStr: 'desc' } },
	syncing: false,
	saving: false,
	editVisible: false,
	langVisible: false,
	edit: {} as any,
	langs: [] as { culture: string; label: string }[],
	langFieldKey: '',
	lookupSources: [] as { code: string; name: string; maintainPath?: string }[],
});

const currentLookupMaintainPath = computed(() => {
	const code = String(state.edit.lookupSource || '').trim();
	if (!code) return '';
	return state.lookupSources.find((s) => s.code === code)?.maintainPath || '';
});

const buildColumns = () => [
	{ field: 'seq', type: 'seq', title: t('message.list.seq'), width: 60 },
	{ field: 'sortNo', title: t('message.list.orderNo'), width: 80, sortable: true },
	{ field: 'fieldKey', title: t('message.hrms.fieldDef.fieldKey'), minWidth: 140, sortable: true },
	{ field: 'label', title: t('message.hrms.fieldDef.label'), minWidth: 140, sortable: true },
	{ field: 'origin', title: t('message.hrms.fieldDef.origin'), width: 100, sortable: true },
	{ field: 'dbColumn', title: t('message.hrms.fieldDef.dbColumn'), minWidth: 140, sortable: true },
	{ field: 'controlType', title: t('message.hrms.fieldDef.control'), width: 100, sortable: true },
	{ field: 'codeField', title: t('message.hrms.fieldDef.codeField'), minWidth: 120, slots: { default: 'row_codeField' } },
	{ field: 'lookupSource', title: t('message.hrms.fieldDef.lookupSource'), minWidth: 120 },
	{ field: 'enabled', title: t('message.hrms.fieldDef.enabled'), width: 80, sortable: true },
	{ field: 'buttons', title: t('message.list.operation'), width: 110, fixed: 'right', slots: { default: 'row_buttons' } },
];

const options = useVxeTable(
	{
		id: 'hrUiFieldDef',
		name: t('message.hrms.fieldDef.moduleName'),
		columns: buildColumns(),
	},
	{
		proxyConfig: {
			autoLoad: true,
			ajax: { query: ({ page, sort }) => handleQueryApi(page, sort) },
		},
		sortConfig: { defaultSort: Local.get(localPageParamKey)?.defaultSort || state.localPageParam.defaultSort },
		pagerConfig: { pageSize: Local.get(localPageParamKey)?.pageSize || state.localPageParam.pageSize },
	}
);

const handleQueryApi = (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const effective = sort?.field ? sort : state.localPageParam.defaultSort;
	return pageHrUiFieldDef({
		...state.queryParams,
		page: page.currentPage,
		pageSize: page.pageSize,
		field: effective.field,
		order: effective.order,
		descStr: 'desc',
	});
};

const handleQuery = async (reset = false) => {
	reset ? await xGrid.value?.commitProxy('reload') : await xGrid.value?.commitProxy('query');
};
const resetQuery = () => {
	state.queryParams.keyword = '';
	state.queryParams.enabled = undefined;
	state.queryParams.isInPool = undefined;
	handleQuery(true);
};

const handleSync = async () => {
	state.syncing = true;
	try {
		const res = await syncHrUiFieldDef('hrEmp');
		const r = res.data.result;
		ElMessage.success(
			t('message.hrms.fieldDef.syncOk', {
				inserted: r?.inserted ?? 0,
				structuralUpdated: r?.structuralUpdated ?? 0,
				disabled: r?.disabled ?? 0,
				reenabled: r?.reenabled ?? 0,
			})
		);
		if (r?.warnings?.length) ElMessage.warning(r.warnings.join('；'));
		handleQuery(true);
	} finally {
		state.syncing = false;
	}
};

const openCodeSetting = (codeField?: string) => {
	const cf = (codeField || '').trim();
	if (!cf) {
		ElMessage.warning(t('message.hrms.fieldDef.codeFieldRequired'));
		return;
	}
	codeDrawerRef.value?.open(cf);
};

const loadLookupSources = async () => {
	if (state.lookupSources.length) return;
	try {
		const res = await listEnabledHrLookupDef();
		state.lookupSources = res.data?.result ?? res.data ?? [];
	} catch {
		state.lookupSources = [];
	}
};

const onCodeFieldChange = () => {
	if (String(state.edit.codeField || '').trim()) state.edit.lookupSource = '';
};
const onLookupSourceChange = () => {
	if (String(state.edit.lookupSource || '').trim()) state.edit.codeField = '';
};
const openLookupMaintain = () => {
	const path = currentLookupMaintainPath.value;
	if (!path) {
		ElMessage.warning(t('message.hrms.fieldDef.lookupRequired'));
		return;
	}
	router.push(path);
};

const openEdit = async (row: any) => {
	await loadLookupSources();
	state.edit = { ...row, defaultSpanSlots: row.defaultSpanSlots || 1, lookupSource: row.lookupSource || '' };
	state.editVisible = true;
};
const saveEdit = async () => {
	if (!state.edit.label?.trim()) {
		ElMessage.warning(t('message.hrms.fieldDef.labelRequired'));
		return;
	}
	const code = String(state.edit.codeField || '').trim();
	const lookup = String(state.edit.lookupSource || '').trim();
	if (code && lookup) {
		ElMessage.warning(t('message.hrms.fieldDef.codeLookupMutex'));
		return;
	}
	// 绑了代码类或数据源一律按下拉
	if (code || lookup) state.edit.controlType = 'select';
	state.edit.codeField = code || null;
	state.edit.lookupSource = lookup || null;
	state.saving = true;
	try {
		await updateHrUiFieldDef(state.edit);
		ElMessage.success(t('message.hrms.common.saved'));
		state.editVisible = false;
		handleQuery(false);
	} finally {
		state.saving = false;
	}
};

const openLang = async (row: any) => {
	state.langFieldKey = row.fieldKey;
	const res = await langListHrUiFieldDef('hrEmp', row.fieldKey);
	const map = new Map((res.data.result ?? []).map((x: any) => [x.culture, x.label]));
	state.langs = hrUiCultures.map((c) => ({ culture: c.value, label: map.get(c.value) || '' }));
	state.langVisible = true;
};
const saveLang = async () => {
	state.saving = true;
	try {
		await saveLangHrUiFieldDef({ moduleCode: 'hrEmp', fieldKey: state.langFieldKey, labels: state.langs });
		ElMessage.success(t('message.hrms.fieldDef.langSaved'));
		state.langVisible = false;
	} finally {
		state.saving = false;
	}
};

const gridEvents: VxeGridListeners = {
	async pageChange({ pageSize }) {
		state.localPageParam.pageSize = pageSize;
		Local.set(localPageParamKey, state.localPageParam);
	},
	async sortChange({ field, order }) {
		if (!field || !order) return;
		state.localPageParam.defaultSort = { field, order, descStr: 'desc' };
		Local.set(localPageParamKey, state.localPageParam);
	},
};

watch(locale, () => {
	options.name = t('message.hrms.fieldDef.moduleName');
	const cols = buildColumns();
	options.columns = cols;
	xGrid.value?.reloadColumn?.(cols);
});

onMounted(() => {
	state.localPageParam = Local.get(localPageParamKey) || state.localPageParam;
});
</script>

<style scoped>
.w100 { width: 100%; }
.hint { color: var(--el-text-color-secondary); font-size: 12px; line-height: 1.4; margin-top: 4px; }
.code-field-row {
	display: flex;
	gap: 8px;
	width: 100%;
	align-items: center;
}
.code-field-row .el-input {
	flex: 1;
}
</style>
