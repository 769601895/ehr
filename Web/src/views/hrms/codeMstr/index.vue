<template>
	<div class="hr-code-mstr" :class="{ 'is-embedded': embedded }">
		<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', alignItems: 'start' }">
			<el-form :model="state.queryParams" :inline="true" style="flex: 1">
				<el-form-item :label="t('message.hrms.codeMstr.codeField')">
					<el-input v-model="state.queryParams.codeField" placeholder="code_field" clearable @keyup.enter="handleQuery(true)" />
				</el-form-item>
				<el-form-item :label="t('message.hrms.codeMstr.keyword')">
					<el-input v-model="state.queryParams.keyword" :placeholder="t('message.hrms.codeMstr.keywordPh')" clearable @keyup.enter="handleQuery(true)" />
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
					<el-button type="primary" icon="ele-Plus" v-auth="'hrCodeMstr/save'" @click="openEdit()">{{ t('message.list.add') }}</el-button>
				</template>
				<template #empty><el-empty :image-size="160" /></template>
				<template #row_buttons="{ row }">
					<el-button icon="ele-Edit" text type="primary" v-auth="'hrCodeMstr/save'" @click="openEdit(row)" />
					<el-button icon="ele-ChatLineSquare" text type="primary" v-auth="'hrCodeMstr/saveLang'" @click="openLang(row)" />
					<el-button icon="ele-Delete" text type="danger" v-auth="'hrCodeMstr/delete'" @click="handleDelete(row)" />
				</template>
			</vxe-grid>
		</el-card>

		<el-dialog v-model="state.editVisible" :title="state.edit.oldCodeValue ? t('message.hrms.codeMstr.editTitle') : t('message.hrms.codeMstr.addTitle')" width="480px" destroy-on-close>
			<el-form :model="state.edit" label-width="88px">
				<el-form-item :label="t('message.hrms.codeMstr.codeField')" required><el-input v-model="state.edit.codeField" :disabled="!!state.edit.oldCodeValue" /></el-form-item>
				<el-form-item :label="t('message.hrms.codeMstr.codeValue')" required><el-input v-model="state.edit.codeValue" /></el-form-item>
				<el-form-item :label="t('message.hrms.codeMstr.codeDesc')"><el-input v-model="state.edit.codeDesc" /></el-form-item>
				<el-form-item :label="t('message.hrms.codeMstr.codeSort')"><el-input v-model="state.edit.codeSort" /></el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="state.editVisible = false">{{ t('message.list.cancel') }}</el-button>
				<el-button type="primary" @click="saveEdit">{{ t('message.list.save') }}</el-button>
			</template>
		</el-dialog>

		<el-dialog v-model="state.langVisible" :title="t('message.hrms.codeMstr.langTitle')" width="520px" destroy-on-close>
			<el-form label-width="88px">
				<el-form-item v-for="item in state.langs" :key="item.culture" :label="item.culture">
					<el-input v-model="item.label" />
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="state.langVisible = false">{{ t('message.list.cancel') }}</el-button>
				<el-button type="primary" @click="saveLang">{{ t('message.list.save') }}</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="hrCodeMstr">
import { onMounted, reactive, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage, ElMessageBox } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { deleteHrCodeMstr, langListHrCodeMstr, pageHrCodeMstr, saveHrCodeMstr, saveLangHrCodeMstr } from '/@/api/hrms/hrCodeMstr';
import { hrUiCultures } from '/@/views/hrms/composables/hrUiCulture';

const { t, locale } = useI18n();
const props = defineProps<{ presetCodeField?: string; embedded?: boolean }>();
const embedded = props.embedded;
const xGrid = ref<VxeGridInstance>();
const localPageParamKey = 'localPageParam:hrCodeMstr';
const state = reactive({
	queryParams: { codeField: props.presetCodeField || '', keyword: '' },
	localPageParam: { pageSize: 50 as number, defaultSort: { field: 'codeValue', order: 'asc', descStr: 'desc' } },
	editVisible: false,
	langVisible: false,
	edit: {} as any,
	langs: [] as { culture: string; label: string }[],
	langRow: null as any,
});

const buildColumns = () => [
	{ field: 'seq', type: 'seq', title: t('message.list.seq'), width: 60 },
	{ field: 'codeField', title: t('message.hrms.codeMstr.codeField'), minWidth: 140, sortable: true },
	{ field: 'codeValue', title: t('message.hrms.codeMstr.codeValue'), minWidth: 120, sortable: true },
	{ field: 'codeDesc', title: t('message.hrms.codeMstr.codeDesc'), minWidth: 160, sortable: true },
	{ field: 'codeSort', title: t('message.hrms.codeMstr.sortCode'), width: 100, sortable: true },
	{ field: 'buttons', title: t('message.list.operation'), width: 140, fixed: 'right', slots: { default: 'row_buttons' } },
];

const options = useVxeTable(
	{
		id: 'hrCodeMstr',
		name: t('message.hrms.codeMstr.moduleName'),
		columns: buildColumns(),
	},
	{
		height: embedded ? '100%' : 'auto',
		proxyConfig: { autoLoad: true, ajax: { query: ({ page, sort }) => handleQueryApi(page, sort) } },
		sortConfig: { defaultSort: Local.get(localPageParamKey)?.defaultSort || state.localPageParam.defaultSort },
		pagerConfig: { pageSize: Local.get(localPageParamKey)?.pageSize || state.localPageParam.pageSize },
	}
);

const handleQueryApi = (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const effective = sort?.field ? sort : state.localPageParam.defaultSort;
	return pageHrCodeMstr({
		codeField: state.queryParams.codeField || undefined,
		keyword: state.queryParams.keyword || undefined,
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
	state.queryParams.codeField = props.presetCodeField || '';
	state.queryParams.keyword = '';
	handleQuery(true);
};
const openEdit = (row?: any) => {
	state.edit = row
		? { codeField: row.codeField, codeValue: row.codeValue, oldCodeValue: row.codeValue, codeDesc: row.codeDesc, codeSort: row.codeSort }
		: { codeField: state.queryParams.codeField, codeValue: '', codeDesc: '', codeSort: '' };
	state.editVisible = true;
};
const saveEdit = async () => {
	if (!state.edit.codeField?.trim() || !state.edit.codeValue?.trim()) {
		ElMessage.warning(t('message.hrms.codeMstr.requiredFields'));
		return;
	}
	await saveHrCodeMstr(state.edit);
	ElMessage.success(t('message.hrms.common.saved'));
	state.editVisible = false;
	handleQuery(false);
};
const handleDelete = (row: any) => {
	ElMessageBox.confirm(t('message.hrms.codeMstr.confirmDelete', { codeField: row.codeField, codeValue: row.codeValue }), t('message.list.hint'), { type: 'warning' })
		.then(async () => {
			await deleteHrCodeMstr({ codeField: row.codeField, codeValue: row.codeValue });
			ElMessage.success(t('message.hrms.common.deleted'));
			handleQuery(false);
		})
		.catch(() => {});
};
const openLang = async (row: any) => {
	state.langRow = row;
	const res = await langListHrCodeMstr(row.codeField, row.codeValue);
	const map = new Map((res.data.result ?? []).map((x: any) => [x.culture, x.label]));
	state.langs = hrUiCultures.map((c) => ({ culture: c.value, label: map.get(c.value) || '' }));
	state.langVisible = true;
};
const saveLang = async () => {
	await saveLangHrCodeMstr({ codeField: state.langRow.codeField, codeValue: state.langRow.codeValue, labels: state.langs });
	ElMessage.success(t('message.hrms.codeMstr.langSaved'));
	state.langVisible = false;
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
	options.name = t('message.hrms.codeMstr.moduleName');
	const cols = buildColumns();
	options.columns = cols;
	xGrid.value?.reloadColumn?.(cols);
});

onMounted(() => {
	state.localPageParam = Local.get(localPageParamKey) || state.localPageParam;
	if (props.presetCodeField) state.queryParams.codeField = props.presetCodeField;
});
</script>

<style scoped>
.hr-code-mstr.is-embedded {
	display: flex;
	flex-direction: column;
	height: 100%;
	min-height: 0;
}
.hr-code-mstr.is-embedded .full-table {
	flex: 1;
	min-height: 0;
	display: flex;
	flex-direction: column;
	margin-top: 8px;
}
.hr-code-mstr.is-embedded .full-table :deep(.el-card__body) {
	flex: 1;
	min-height: 0;
	display: flex;
	flex-direction: column;
	padding: 5px 5px 0;
}
.hr-code-mstr.is-embedded :deep(.vxe-grid) {
	flex: 1;
	min-height: 0;
}
</style>
