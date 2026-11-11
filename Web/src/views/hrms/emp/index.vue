<template>
	<div class="hr-emp-container">
		<div class="hr-emp-main">
			<el-card shadow="hover" :body-style="{ padding: '8px 10px', display: 'flex', width: '100%', alignItems: 'center', flexWrap: 'wrap', gap: '4px' }">
				<el-button icon="ele-OfficeBuilding" @click="state.orgDrawer = true">{{ t('message.hrms.emp.openOrg') }}</el-button>
				<el-tag v-if="state.parentCcDesc" type="info" closable class="dept-tag" @close="clearDeptFilter">
					{{ state.parentCcDesc }}
				</el-tag>
				<el-button type="primary" icon="ele-Plus" v-auth="'hrEmp/add'" @click="handleAdd">{{ t('message.list.add') }}</el-button>
				<el-input v-model="state.queryParams.keyword" :placeholder="t('message.hrms.common.keywordEmp')" clearable style="width: 280px" @keyup.enter="handleQuery(true)" />
				<el-button-group>
					<el-button type="primary" icon="ele-Search" :loading="options.loading" @click="handleQuery(true)">{{ t('message.list.query') }}</el-button>
					<el-button icon="ele-Refresh" :loading="options.loading" @click="resetQuery">{{ t('message.list.reset') }}</el-button>
				</el-button-group>
				<el-button icon="ele-Filter" type="primary" plain v-auth="'hrEmp/page'" @click="openAdvQuery">
					{{ t('message.hrms.advQuery.title') }}
					<el-badge v-if="advActiveCount" :value="advActiveCount" class="adv-badge" />
				</el-button>
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
				</vxe-grid>
			</el-card>
		</div>

		<!-- 遮罩 + 左侧面板：树始终挂载以便预载管辖数据 -->
		<div v-show="state.orgDrawer" class="org-panel-mask" @click="state.orgDrawer = false" />
		<aside class="org-panel" :class="{ open: state.orgDrawer }" aria-hidden="false">
			<div class="org-panel-head">
				<span>{{ t('message.hrms.emp.orgDrawerTitle') }}</span>
				<el-button text @click="state.orgDrawer = false">{{ t('message.hrms.advQuery.close') }}</el-button>
			</div>
			<div class="org-panel-body">
				<OrgTree ref="orgTreeRef" @node-click="handleNodeChange" @user-select="onOrgUserSelect" @loaded="handleTreeLoaded" />
			</div>
		</aside>

		<EditEmp ref="editEmpRef" :title="state.title" :tree-data="state.fullTree" @reload="handleQuery(false)" />
		<AdvQueryDrawer
			ref="advQueryRef"
			:module-code="advModuleCode"
			:fields="advFields"
			@search="onAdvSearch"
			@update:active-count="setAdvActiveCount"
		/>
	</div>
</template>

<script lang="ts" setup name="hrEmp">
import { onMounted, reactive, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { auth } from '/@/utils/authFunction';

import OrgTree from '/@/views/hrms/org/component/orgTree.vue';
import EditEmp from '/@/views/hrms/emp/component/editEmp.vue';
import { AdvQueryDrawer, useAdvQuery } from '/@/components/advQuery';
import type { AdvQueryField, AdvValueType } from '/@/components/advQuery';
import { pageHrEmp } from '/@/api/hrms/hrEmp';
import { getHrCodeList } from '/@/api/hrms/hrOrg';
import { getHrLookupOptions } from '/@/api/hrms/hrLookupDef';
import { getHrUiFieldPool, getHrUiLayoutEffective } from '/@/api/hrms/hrUiLayout';

const { t, locale } = useI18n();
const orgTreeRef = ref<InstanceType<typeof OrgTree>>();
const editEmpRef = ref<InstanceType<typeof EditEmp>>();
const xGrid = ref<VxeGridInstance>();

const {
	moduleCode: advModuleCode,
	drawerRef: advQueryRef,
	fields: advFields,
	filter: advFilter,
	activeCount: advActiveCount,
	open: openAdvQuery,
	reset: resetAdvQuery,
	applySearch,
	setActiveCount: setAdvActiveCount,
	setFields: setAdvFields,
} = useAdvQuery('hrEmp');

const localPageParamKey = 'localPageParam:hrEmp';

/** codeField 或 lookup:source → (codeValue → 仅显示值/描述) */
const codeTextMaps = reactive<Record<string, Record<string, string>>>({});

const state = reactive({
	title: '',
	orgDrawer: false,
	parentCcCode: '' as string,
	parentCcDesc: '' as string,
	fullTree: [] as any[],
	/** 与组织树「人员」状态一致：1在职 2停职 3离职 -1全部 */
	empState: 1 as number,
	queryParams: {
		keyword: '',
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

const mapDataType = (dataType?: string, controlType?: string): AdvValueType => {
	const d = (dataType || '').toLowerCase();
	const c = (controlType || '').toLowerCase();
	if (c === 'select' || c === 'radio') return 'enum';
	if (d.includes('date') || c === 'date' || c === 'datetime') return d.includes('time') || c === 'datetime' ? 'datetime' : 'date';
	if (d.includes('int') || d.includes('decimal') || d.includes('float') || d.includes('money') || c === 'number') return 'number';
	if (d.includes('bit') || c === 'switch' || c === 'checkbox') return 'boolean';
	return 'string';
};

/** 从字段池/布局生成高级查询字段；跳过 computed/photo；部门名作跨表补丁 */
const rebuildAdvFields = async (layoutFields: any[]) => {
	let pool: any[] = [];
	try {
		const res = await getHrUiFieldPool('hrEmp', 'list');
		pool = res.data?.result ?? res.data ?? [];
	} catch {
		pool = [];
	}
	const poolMap = new Map(pool.map((p: any) => [String(p.fieldKey), p]));
	const source = layoutFields?.length
		? layoutFields.filter((f: any) => f.visible !== false && !f.isStale)
		: pool.filter((p: any) => p.isInPool !== false);

	const adv: AdvQueryField[] = [];
	const seen = new Set<string>();
	for (const f of source) {
		const key = String(f.fieldKey || '');
		if (!key || seen.has(key)) continue;
		const meta = poolMap.get(key) || f;
		const origin = String(meta.origin || f.origin || 'physical');
		const control = String(meta.controlType || f.controlType || 'text');
		if (origin === 'computed' || control === 'photo' || control === 'computed') continue;
		if (key === 'age' || key === 'empPic') continue;
		seen.add(key);
		const valueType = mapDataType(meta.dataType || f.dataType, control);
		const mapKey = optionMapKey({
			codeField: meta.codeField || f.codeField,
			lookupSource: meta.lookupSource || f.lookupSource,
		});
		const options =
			valueType === 'enum' && mapKey && codeTextMaps[mapKey]
				? Object.entries(codeTextMaps[mapKey]).map(([value, label]) => ({ value, label }))
				: undefined;
		adv.push({
			field: key,
			label: String(f.label || meta.label || key),
			valueType: options?.length ? 'enum' : valueType,
			options,
			origin,
		});
	}
	if (!seen.has('ccDesc')) {
		adv.push({ field: 'ccDesc', label: t('message.hrms.common.dept'), valueType: 'string', origin: 'join' });
	}
	if (!seen.has('empState')) {
		adv.push({
			field: 'empState',
			label: t('message.hrms.common.empState'),
			valueType: 'enum',
			origin: 'physical',
			options: [
				{ value: 1, label: t('message.hrms.common.empStateActive') },
				{ value: 2, label: t('message.hrms.common.empStateSuspend') },
				{ value: 3, label: t('message.hrms.common.empStateLeave') },
			],
		});
	}
	setAdvFields(adv);
};

const applyListLayout = async () => {
	try {
		const res = await getHrUiLayoutEffective('hrEmp', 'list');
		const fields = res.data.result?.fields ?? [];
		if (fields.length) {
			await loadCodeTextMaps(fields.filter((x: any) => x.visible !== false && !x.isStale));
			const cols = buildColumnsFromLayout(fields);
			options.columns = cols;
			xGrid.value?.reloadColumn?.(cols);
			await rebuildAdvFields(fields);
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
	await rebuildAdvFields([]);
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
		// 左树选「全部」时不传 empState，与组织人数统计一致
		empState: state.empState === -1 ? undefined : state.empState,
		ccCode: state.parentCcCode || undefined,
		filter: advFilter.value || undefined,
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
	state.parentCcCode = '';
	state.parentCcDesc = '';
	resetAdvQuery();
	handleQuery(true);
};

const clearDeptFilter = () => {
	state.parentCcCode = '';
	state.parentCcDesc = '';
	handleQuery(true);
};

const onAdvSearch = (filter: Parameters<typeof applySearch>[0]) => {
	applySearch(filter);
	handleQuery(true);
};

const handleTreeLoaded = (tree: any[], meta: { includeDisabled: boolean; empState: number }) => {
	state.fullTree = tree ?? [];
	const next = meta?.empState ?? 1;
	const changed = state.empState !== next;
	state.empState = next;
	if (changed) handleQuery(true);
};

const handleNodeChange = (node: any) => {
	state.parentCcCode = node?.ccCode || '';
	state.parentCcDesc = node?.ccDesc || '';
	handleQuery(true);
};

/** 用户点选部门后收起面板，列表占满 */
const onOrgUserSelect = () => {
	if (state.orgDrawer) state.orgDrawer = false;
};

const handleAdd = () => {
	state.title = t('message.hrms.emp.addTitle');
	editEmpRef.value?.openDialog({ ccCode: state.parentCcCode || undefined, empState: 1 }, false);
};

const handleEdit = async (row: any) => {
	state.title = t('message.hrms.emp.editTitle');
	editEmpRef.value?.openDialog(row, true);
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
	async cellDblclick({ row }) {
		if (auth('hrEmp/update')) await handleEdit(row);
	},
};

watch(locale, async () => {
	options.name = t('message.hrms.emp.moduleName');
	await applyListLayout();
});

onMounted(async () => {
	state.localPageParam = Local.get(localPageParamKey) || state.localPageParam;
	await applyListLayout();
	// 左树 loaded 会回填 empState/fullTree；此处兜底查询，避免树早于表格就绪时漏查
	await handleQuery(true);
});
</script>

<style scoped>
.hr-emp-container {
	position: relative;
	height: 100%;
	overflow: hidden;
}
.hr-emp-main {
	display: flex;
	flex-direction: column;
	width: 100%;
	height: 100%;
	overflow: hidden;
}
.dept-tag {
	max-width: 220px;
	overflow: hidden;
	text-overflow: ellipsis;
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
.adv-badge {
	margin-left: 6px;
}
.org-panel-mask {
	position: absolute;
	inset: 0;
	z-index: 20;
	background: rgba(0, 0, 0, 0.35);
}
.org-panel {
	position: absolute;
	top: 0;
	left: 0;
	z-index: 21;
	display: flex;
	flex-direction: column;
	width: min(360px, 86vw);
	height: 100%;
	background: var(--el-bg-color);
	box-shadow: 2px 0 12px rgba(0, 0, 0, 0.12);
	transform: translateX(-105%);
	transition: transform 0.2s ease;
	pointer-events: none;
}
.org-panel.open {
	transform: translateX(0);
	pointer-events: auto;
}
.org-panel-head {
	display: flex;
	align-items: center;
	justify-content: space-between;
	padding: 10px 12px;
	border-bottom: 1px solid var(--el-border-color-lighter);
	font-weight: 600;
}
.org-panel-body {
	flex: 1;
	min-height: 0;
	display: flex;
	padding: 8px;
	overflow: hidden;
}
.org-panel-body :deep(.box-card) {
	width: 100%;
}
</style>
