<template>
	<div class="hr-emp-container">
		<splitpanes class="default-theme">
			<pane size="22" style="display: flex">
				<OrgTree ref="orgTreeRef" @node-click="handleNodeChange" />
			</pane>
			<pane size="78" class="hr-emp-right-pane">
				<div class="hr-emp-right">
					<el-card shadow="hover" :body-style="{ padding: '8px 10px', display: 'flex', width: '100%', alignItems: 'center', flexWrap: 'wrap', gap: '4px' }">
						<el-input v-model="state.queryParams.keyword" :placeholder="t('message.hrms.common.keywordEmp')" clearable style="width: 220px" @keyup.enter="handleQuery(true)" />
						<el-input v-model="state.queryParams.empId" :placeholder="t('message.hrms.common.empId')" clearable style="width: 120px" @keyup.enter="handleQuery(true)" />
						<el-input v-model="state.queryParams.empName" :placeholder="t('message.hrms.common.empName')" clearable style="width: 120px" @keyup.enter="handleQuery(true)" />
						<el-select v-model="state.queryParams.empState" clearable :placeholder="t('message.hrms.common.empState')" style="width: 110px" @change="handleQuery(true)">
							<el-option :label="t('message.hrms.common.empStateActive')" :value="1" />
							<el-option :label="t('message.hrms.common.empStateSuspend')" :value="2" />
							<el-option :label="t('message.hrms.common.empStateLeave')" :value="3" />
						</el-select>
						<el-button-group style="margin-left: 4px">
							<el-button type="primary" icon="ele-Search" :loading="options.loading" @click="handleQuery(true)">{{ t('message.list.query') }}</el-button>
							<el-button icon="ele-Refresh" :loading="options.loading" @click="resetQuery">{{ t('message.list.reset') }}</el-button>
						</el-button-group>
						<el-button type="primary" icon="ele-Plus" style="margin-left: 8px" v-auth="'hrEmp/add'" @click="handleAdd">{{ t('message.list.add') }}</el-button>
					</el-card>

					<el-card class="full-table" shadow="hover" style="margin-top: 5px; flex: 1">
						<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options" v-on="gridEvents">
							<template #empty>
								<el-empty :image-size="160" />
							</template>
							<template #row_empState="{ row }">
								<el-tag v-if="row.empState === 1" type="success">{{ t('message.hrms.common.empStateActive') }}</el-tag>
								<el-tag v-else-if="row.empState === 2" type="warning">{{ t('message.hrms.common.empStateSuspend') }}</el-tag>
								<el-tag v-else-if="row.empState === 3" type="info">{{ t('message.hrms.common.empStateLeave') }}</el-tag>
								<span v-else>{{ row.empState ?? '-' }}</span>
							</template>
							<template #row_buttons="{ row }">
								<el-button icon="ele-Edit" text type="primary" v-auth="'hrEmp/update'" @click="handleEdit(row)">{{ t('message.list.edit') }}</el-button>
								<el-button icon="ele-Delete" text type="danger" v-auth="'hrEmp/delete'" :disabled="row.empState === 3" @click="handleLeave(row)">{{ t('message.hrms.emp.leaveAction') }}</el-button>
							</template>
						</vxe-grid>
					</el-card>
				</div>
			</pane>
		</splitpanes>

		<EditEmp ref="editEmpRef" :title="state.title" :tree-data="state.fullTree" @reload="handleQuery(false)" />
	</div>
</template>

<script lang="ts" setup name="hrEmp">
import { onMounted, reactive, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';

import OrgTree from '/@/views/hrms/org/component/orgTree.vue';
import EditEmp from '/@/views/hrms/emp/component/editEmp.vue';
import { deleteHrEmp, pageHrEmp } from '/@/api/hrms/hrEmp';
import { getHrCodeList, getHrOrgList } from '/@/api/hrms/hrOrg';
import { getHrLookupOptions } from '/@/api/hrms/hrLookupDef';
import { getHrUiLayoutEffective } from '/@/api/hrms/hrUiLayout';

const { t, locale } = useI18n();
const orgTreeRef = ref<InstanceType<typeof OrgTree>>();
const editEmpRef = ref<InstanceType<typeof EditEmp>>();
const xGrid = ref<VxeGridInstance>();

const localPageParamKey = 'localPageParam:hrEmp';

/** codeField 或 lookup:source → (codeValue → 仅显示值/描述) */
const codeTextMaps = reactive<Record<string, Record<string, string>>>({});

const state = reactive({
	title: '',
	parentCcCode: '' as string,
	fullTree: [] as any[],
	queryParams: {
		keyword: '',
		empId: '',
		empName: '',
		empState: 1 as number | undefined,
	},
	localPageParam: {
		pageSize: 50 as number,
		defaultSort: { field: 'empId', order: 'asc' as string, descStr: 'desc' },
	},
});

/** 列表只用描述；兼容旧接口无 text 时从 label 去掉「码值 」前缀 */
const pickCodeText = (o: { value: string; label?: string; text?: string }) => {
	const value = String(o.value ?? '');
	const text = (o.text ?? '').trim();
	if (text) return text;
	const label = String(o.label ?? '');
	const prefix = value + ' ';
	if (label.startsWith(prefix)) return label.slice(prefix.length).trim() || value;
	return label || value;
};

const resolveCodeText = (mapKey: string, cellValue: unknown) => {
	if (cellValue == null || cellValue === '') return '';
	const raw = String(cellValue);
	const map = codeTextMaps[mapKey];
	if (!map) return raw;
	return map[raw] ?? map[raw.trim()] ?? raw;
};

const optionMapKey = (f: any) => {
	const lookup = String(f?.lookupSource || '').trim();
	if (lookup) return `lookup:${lookup}`;
	return String(f?.codeField || '');
};

const loadCodeTextMaps = async (fields: any[]) => {
	const codes = [...new Set(fields.filter((f) => f.codeField && !f.lookupSource).map((f) => String(f.codeField)))];
	const lookups = [...new Set(fields.filter((f) => f.lookupSource).map((f) => String(f.lookupSource)))];
	for (const k of Object.keys(codeTextMaps)) delete codeTextMaps[k];
	if (!codes.length && !lookups.length) return;
	const codeEntries = await Promise.all(
		codes.map(async (c) => {
			const list = ((await getHrCodeList(c)).data.result ?? []) as { value: string; label?: string; text?: string }[];
			const map: Record<string, string> = {};
			for (const o of list) map[String(o.value)] = pickCodeText(o);
			return [c, map] as const;
		})
	);
	const lookupEntries = await Promise.all(
		lookups.map(async (s) => {
			const list = ((await getHrLookupOptions(s)).data?.result ?? []) as { value: string; label?: string; text?: string }[];
			const map: Record<string, string> = {};
			for (const o of list) map[String(o.value)] = pickCodeText(o);
			return [`lookup:${s}`, map] as const;
		})
	);
	for (const [c, map] of [...codeEntries, ...lookupEntries]) codeTextMaps[c] = map;
};

const resolveSort = (sort?: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams | null, fallback?: { field: string; order: string; descStr: string }) => {
	const fb = fallback || state.localPageParam.defaultSort;
	const effective = sort?.field && sort?.order ? sort : { field: fb.field, order: fb.order };
	return { field: effective.field!, order: String(effective.order), descStr: 'desc' as const };
};

const pickProxySort = (params: { sort?: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams; sorts?: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams[] }) =>
	params.sort?.field ? params.sort : params.sorts?.[0];

const defaultColumns = () => [
	{ field: 'seq', type: 'seq', title: '#', width: 55 },
	{ field: 'empId', title: t('message.hrms.common.empId'), minWidth: 100, sortable: true },
	{ field: 'empName', title: t('message.hrms.common.empName'), minWidth: 100, sortable: true },
	{ field: 'ccDesc', title: t('message.hrms.common.dept'), minWidth: 140, showOverflow: 'tooltip' },
	{ field: 'empSex', title: t('message.hrms.common.gender'), width: 70, sortable: true },
	{ field: 'age', title: t('message.hrms.common.age'), width: 70 },
	{ field: 'empAddrter', title: t('message.hrms.common.phone'), minWidth: 120, showOverflow: 'tooltip', sortable: true },
	{ field: 'dutyId', title: t('message.hrms.common.duty'), minWidth: 90, sortable: true },
	{ field: 'postId', title: t('message.hrms.common.post'), minWidth: 90, sortable: true },
	{ field: 'empInDate', title: t('message.hrms.common.inDate'), minWidth: 120, sortable: true },
	{ field: 'empType', title: t('message.hrms.common.empType'), minWidth: 100, sortable: true },
	{ field: 'empSal', title: t('message.hrms.common.salType'), width: 90, sortable: true },
	{ field: 'empState', title: t('message.hrms.common.status'), width: 80, align: 'center', sortable: true, slots: { default: 'row_empState' } },
	{ field: 'buttons', title: t('message.list.operation'), fixed: 'right', width: 140, showOverflow: true, slots: { default: 'row_buttons' } },
];

const buildColumnsFromLayout = (fields: any[]) => {
	const cols: any[] = [{ field: 'seq', type: 'seq', title: '#', width: 55 }];
	const seen = new Set<string>();
	for (const f of fields) {
		if (f.isStale) console.warn('布局字段已失效', f.fieldKey);
	}
	for (const f of fields.filter((x) => x.visible !== false && !x.isStale)) {
		const key = String(f.fieldKey || '').toLowerCase();
		if (!key || seen.has(key)) continue;
		seen.add(key);
		const col: any = {
			field: f.fieldKey,
			title: f.label,
			showOverflow: 'tooltip',
			sortable: f.sortable !== false && !['age', 'ccDesc'].includes(f.fieldKey),
		};
		if (f.widthPx) col.width = f.widthPx;
		else col.minWidth = 90;
		if (f.fieldKey === 'empState') {
			col.align = 'center';
			col.slots = { default: 'row_empState' };
		} else if (f.lookupSource || f.codeField) {
			const mapKey = optionMapKey(f);
			col.formatter = ({ cellValue }: { cellValue: unknown }) => resolveCodeText(mapKey, cellValue);
		}
		cols.push(col);
	}
	cols.push({ field: 'buttons', title: t('message.list.operation'), fixed: 'right', width: 140, showOverflow: true, slots: { default: 'row_buttons' } });
	return cols;
};

const options = useVxeTable<any>(
	{
		id: 'hrEmp',
		name: t('message.hrms.emp.moduleName'),
		columns: defaultColumns(),
	},
	{
		height: '100%',
		proxyConfig: {
			autoLoad: false,
			ajax: {
				query: (params) => handleQueryApi(params.page, pickProxySort(params)),
			},
		},
		sortConfig: {
			remote: true,
			defaultSort: Local.get(localPageParamKey)?.defaultSort || state.localPageParam.defaultSort,
		},
		pagerConfig: { pageSize: Local.get(localPageParamKey)?.pageSize || state.localPageParam.pageSize },
		toolbarConfig: { enabled: false },
		rowConfig: { keyField: 'empId' },
	}
);

const applyListLayout = async () => {
	try {
		const res = await getHrUiLayoutEffective('hrEmp', 'list');
		const fields = res.data.result?.fields ?? [];
		if (fields.length) {
			await loadCodeTextMaps(fields.filter((x: any) => x.visible !== false && !x.isStale));
			const cols = buildColumnsFromLayout(fields);
			options.columns = cols;
			xGrid.value?.reloadColumn?.(cols);
			const sortField = state.localPageParam.defaultSort.field;
			if (sortField && !fields.some((f: any) => f.visible !== false && f.fieldKey === sortField)) {
				state.localPageParam.defaultSort = { field: 'empId', order: 'asc', descStr: 'desc' };
				Local.set(localPageParamKey, state.localPageParam);
			}
			return;
		}
	} catch {
		/* 布局服务未就绪时沿用内置列 */
	}
	for (const k of Object.keys(codeTextMaps)) delete codeTextMaps[k];
	const cols = defaultColumns();
	options.columns = cols;
	xGrid.value?.reloadColumn?.(cols);
};

const handleQueryApi = async (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const effectiveSort = resolveSort(sort, Local.get(localPageParamKey)?.defaultSort || state.localPageParam.defaultSort);
	return pageHrEmp({
		page: page.currentPage,
		pageSize: page.pageSize,
		field: effectiveSort.field,
		order: effectiveSort.order,
		descStr: effectiveSort.descStr,
		keyword: state.queryParams.keyword || undefined,
		empId: state.queryParams.empId || undefined,
		empName: state.queryParams.empName || undefined,
		empState: state.queryParams.empState,
		ccCode: state.parentCcCode || undefined,
	});
};

const handleQuery = async (resetPage = false) => {
	const $grid = xGrid.value;
	if (!$grid) return;
	if (resetPage) await $grid.commitProxy('reload');
	else await $grid.commitProxy('query');
};

const resetQuery = () => {
	state.queryParams.keyword = '';
	state.queryParams.empId = '';
	state.queryParams.empName = '';
	state.queryParams.empState = 1;
	handleQuery(true);
};

const handleNodeChange = (node: any) => {
	state.parentCcCode = node?.ccCode || '';
	handleQuery(true);
};

const loadTree = async () => {
	const res = await getHrOrgList({ includeDisabled: false, empState: 1 });
	state.fullTree = res.data.result ?? [];
};

const handleAdd = () => {
	state.title = t('message.hrms.emp.addTitle');
	editEmpRef.value?.openDialog({ ccCode: state.parentCcCode || undefined, empState: 1 }, false);
};

const handleEdit = async (row: any) => {
	state.title = t('message.hrms.emp.editTitle');
	editEmpRef.value?.openDialog(row, true);
};

const handleLeave = (row: any) => {
	ElMessageBox.confirm(t('message.hrms.emp.confirmLeave', { name: row.empName || row.empId }), t('message.list.hint'), { type: 'warning' })
		.then(async () => {
			await deleteHrEmp({ empId: row.empId });
			ElMessage.success(t('message.hrms.common.leaveDone'));
			handleQuery(false);
		})
		.catch(() => {});
};

const gridEvents: VxeGridListeners = {
	async sortChange({ field, order }) {
		if (!field || !order) return;
		state.localPageParam.defaultSort = { field, order, descStr: 'desc' };
		Local.set(localPageParamKey, state.localPageParam);
	},
	async pageChange({ pageSize }) {
		state.localPageParam.pageSize = pageSize;
		Local.set(localPageParamKey, state.localPageParam);
	},
};

watch(locale, async () => {
	options.name = t('message.hrms.emp.moduleName');
	await applyListLayout();
});

onMounted(async () => {
	state.localPageParam = Local.get(localPageParamKey) || state.localPageParam;
	await Promise.all([loadTree(), applyListLayout()]);
	await handleQuery(true);
});
</script>

<style scoped>
.hr-emp-container {
	height: 100%;
}
.hr-emp-right-pane {
	display: flex;
}
.hr-emp-right {
	display: flex;
	flex-direction: column;
	width: 100%;
	height: 100%;
	padding: 0 0 0 5px;
	overflow: hidden;
}
.full-table {
	display: flex;
	flex-direction: column;
	min-height: 0;
}
.full-table :deep(.el-card__body) {
	flex: 1;
	min-height: 0;
	display: flex;
	flex-direction: column;
}
</style>
