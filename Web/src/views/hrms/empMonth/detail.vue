<template>
	<div class="hr-emp-month-detail">
		<el-empty v-if="!batchId" :description="t('message.hrms.empMonth.missingBatchId')" :image-size="120">
			<el-button type="primary" @click="goBack">{{ t('message.hrms.empMonth.back') }}</el-button>
		</el-empty>
		<template v-else>
		<el-card shadow="hover" :body-style="{ padding: '10px' }">
			<div class="head-row">
				<el-button icon="ele-Back" @click="goBack">{{ t('message.hrms.empMonth.back') }}</el-button>
				<span class="title">{{ batch?.siteName || batch?.siteCode }} · {{ batch?.payPeriod }}</span>
				<el-tag v-if="batch" size="small" :type="statusTag(batch.status)">{{ batch.statusText || batch.status }}</el-tag>
				<span class="muted" v-if="batch">{{ fmtDate(batch.attFrom) }} ~ {{ fmtDate(batch.attTo) }}</span>
				<div class="spacer" />
				<el-button icon="ele-OfficeBuilding" @click="state.orgDrawer = true">{{ t('message.hrms.emp.openOrg') }}</el-button>
				<el-tag v-if="state.ccName" type="info" closable @close="clearDept">{{ state.ccName }}</el-tag>
				<template v-if="canEdit">
					<el-button type="primary" v-auth="'hrEmpMonth/generate'" @click="doGenerate(false)">{{ t('message.hrms.empMonth.generate') }}</el-button>
					<el-button v-auth="'hrEmpMonth/generate'" @click="doGenerate(true)">{{ t('message.hrms.empMonth.fullReset') }}</el-button>
					<el-button v-auth="'hrEmpMonth/adjustLine'" @click="openInclude">{{ t('message.hrms.empMonth.manualInclude') }}</el-button>
					<el-button type="success" v-auth="'hrEmpMonth/freeze'" @click="doFreeze">{{ t('message.hrms.empMonth.freeze') }}</el-button>
				</template>
				<template v-if="batch?.status === 'Frozen'">
					<el-button type="warning" v-auth="'hrEmpMonth/unfreeze'" @click="doUnfreeze">{{ t('message.hrms.empMonth.unfreeze') }}</el-button>
				</template>
				<el-button v-auth="'hrEmpMonth/export'" @click="doExport">{{ t('message.hrms.empMonth.export') }}</el-button>
				<el-button icon="ele-Filter" plain @click="openAdvQuery">
					{{ t('message.hrms.advQuery.title') }}
					<el-badge v-if="advActiveCount" :value="advActiveCount" class="adv-badge" />
				</el-button>
			</div>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options" v-on="gridEvents">
				<template #empty><el-empty :image-size="120" /></template>
				<template #row_include="{ row }">
					<el-tag size="small" :type="row.includeFlag === 'Include' ? 'success' : 'info'">{{ row.includeFlag }}</el-tag>
				</template>
				<template #row_buttons="{ row }">
					<el-button text type="primary" @click="openLineDetail(row)">{{ t('message.hrms.empMonth.lineDetail') }}</el-button>
					<el-button
						v-if="canEdit && row.includeFlag === 'Include'"
						text
						type="warning"
						v-auth="'hrEmpMonth/adjustLine'"
						@click="openExclude(row)"
					>{{ t('message.hrms.empMonth.exclude') }}</el-button>
				</template>
			</vxe-grid>
		</el-card>

		<div v-show="state.orgDrawer" class="org-panel-mask" @click="state.orgDrawer = false" />
		<aside class="org-panel" :class="{ open: state.orgDrawer }">
			<div class="org-panel-head">
				<span>{{ t('message.hrms.emp.orgDrawerTitle') }}</span>
				<el-button text @click="state.orgDrawer = false">{{ t('message.hrms.advQuery.close') }}</el-button>
			</div>
			<div class="org-panel-body">
				<OrgTree @node-click="onOrgClick" @user-select="onOrgUserSelect" />
			</div>
		</aside>

		<el-drawer v-model="state.lineDrawer" size="520px" :title="t('message.hrms.empMonth.lineDetail')">
			<template v-if="state.currentLine">
				<el-descriptions :column="1" border size="small">
					<el-descriptions-item :label="t('message.hrms.empMonth.empId')">{{ state.currentLine.empId }}</el-descriptions-item>
					<el-descriptions-item :label="t('message.hrms.empMonth.empName')">{{ state.currentLine.empName }}</el-descriptions-item>
					<el-descriptions-item :label="t('message.hrms.empMonth.ccCode')">{{ state.currentLine.ccCode }} / {{ state.currentLine.ccName }}</el-descriptions-item>
					<el-descriptions-item :label="t('message.hrms.empMonth.warn')">{{ state.currentLine.warnCodes || '-' }}</el-descriptions-item>
					<el-descriptions-item :label="t('message.hrms.empMonth.chgSummary')">{{ state.currentLine.chgSummary || '-' }}</el-descriptions-item>
				</el-descriptions>
				<h4>{{ t('message.hrms.empMonth.empSnap') }}</h4>
				<el-table :data="empSnapRows" size="small" border>
					<el-table-column prop="label" :label="t('message.hrms.empMonth.field')" />
					<el-table-column prop="value" :label="t('message.hrms.empMonth.value')" />
				</el-table>
				<h4>{{ t('message.hrms.empMonth.chgSnap') }}</h4>
				<el-table :data="chgSnapRows" size="small" border>
					<el-table-column prop="label" :label="t('message.hrms.empMonth.field')" width="100" />
					<el-table-column prop="chgNo" label="No" width="100" />
					<el-table-column prop="chgDate" :label="t('message.hrms.empMonth.chgDate')" width="110" />
					<el-table-column prop="before" :label="t('message.hrms.empMonth.before')" />
					<el-table-column prop="after" :label="t('message.hrms.empMonth.after')" />
				</el-table>
			</template>
		</el-drawer>

		<el-dialog v-model="state.adjustVisible" :title="state.adjustMode === 'exclude' ? t('message.hrms.empMonth.exclude') : t('message.hrms.empMonth.manualInclude')" width="440px">
			<el-form label-width="100px">
				<el-form-item v-if="state.adjustMode === 'include'" :label="t('message.hrms.empMonth.empId')" required>
					<EmpIdInput v-model="state.adjustForm.empId" :emp-state="1" />
				</el-form-item>
				<el-form-item v-if="state.adjustMode === 'exclude'" :label="t('message.hrms.empMonth.excludeReason')" required>
					<el-select v-model="state.adjustForm.excludeReason" style="width: 100%">
						<el-option v-for="c in state.excludeReasons" :key="c.value" :label="c.label || c.text" :value="c.value" />
					</el-select>
				</el-form-item>
				<el-form-item v-else :label="t('message.hrms.empMonth.includeReason')" required>
					<el-select v-model="state.adjustForm.includeReason" style="width: 100%">
						<el-option v-for="c in state.includeReasons" :key="c.value" :label="c.label || c.text" :value="c.value" />
					</el-select>
				</el-form-item>
				<el-form-item :label="t('message.hrms.empMonth.remark')">
					<el-input v-model="state.adjustForm.remark" type="textarea" :rows="2" />
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="state.adjustVisible = false">取消</el-button>
				<el-button type="primary" @click="submitAdjust">确定</el-button>
			</template>
		</el-dialog>

		<AdvQueryDrawer
			ref="advQueryRef"
			:module-code="advModuleCode"
			:fields="advFields"
			@search="onAdvSearch"
			@update:active-count="setAdvActiveCount"
		/>
		</template>
	</div>
</template>

<script lang="ts" setup name="hrEmpMonthDetail">
import { computed, nextTick, onActivated, onMounted, reactive, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRoute, useRouter } from 'vue-router';
import { ElMessage, ElMessageBox } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { AdvQueryDrawer, useAdvQuery, type AdvQueryField } from '/@/components/advQuery';
import OrgTree from '/@/views/hrms/org/component/orgTree.vue';
import EmpIdInput from '/@/components/selector/empIdInput.vue';
import { getHrCodeList } from '/@/api/hrms/hrOrg';
import {
	adjustHrEmpMonthLine,
	exportHrEmpMonthUrl,
	freezeHrEmpMonth,
	generateHrEmpMonth,
	getHrEmpMonth,
	pageHrEmpMonthLines,
	unfreezeHrEmpMonth,
	validateHrEmpMonth,
} from '/@/api/hrms/empMonth';
import { getToken } from '/@/utils/request';

const { t } = useI18n();
const route = useRoute();
const router = useRouter();
const xGrid = ref<VxeGridInstance>();
const batchId = computed(() => Number(route.query.id || 0));
const batch = ref<any>(null);
const canEdit = computed(() => ['Generated', 'Reopened'].includes(batch.value?.status));

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
} = useAdvQuery('hrEmpMonthLine');

const localPageParamKey = 'localPageParam:hrEmpMonthLine';
const state = reactive({
	orgDrawer: false,
	ccCode: '' as string,
	ccName: '' as string,
	lineDrawer: false,
	currentLine: null as any,
	adjustVisible: false,
	adjustMode: 'exclude' as 'exclude' | 'include',
	adjustForm: { lineId: 0, empId: '', excludeReason: '', includeReason: '', remark: '' },
	excludeReasons: [] as any[],
	includeReasons: [] as any[],
	localPageParam: {
		pageSize: 50 as number,
		defaultSort: { field: 'empId', order: 'asc', descStr: 'desc' },
	},
	snapColumns: [] as VxeGridPropTypes.Columns,
});

const baseColumns = (): VxeGridPropTypes.Columns => [
	{ field: 'empId', title: t('message.hrms.empMonth.empId'), width: 110, sortable: true },
	{ field: 'empName', title: t('message.hrms.empMonth.empName'), minWidth: 100, sortable: true },
	{ field: 'includeFlag', title: t('message.hrms.empMonth.includeFlag'), width: 90, slots: { default: 'row_include' } },
	{ field: 'ccCode', title: t('message.hrms.empMonth.ccCode'), width: 110, sortable: true },
	{ field: 'ccName', title: t('message.hrms.empMonth.ccName'), minWidth: 120 },
	{ field: 'postId', title: t('message.hrms.empMonth.postId'), width: 100 },
	{ field: 'empSal', title: t('message.hrms.empMonth.empSal'), width: 80 },
	{ field: 'needAttendance', title: t('message.hrms.empMonth.needAttendance'), width: 90 },
	{ field: 'hasChgInPeriod', title: t('message.hrms.empMonth.hasChg'), width: 90 },
	{ field: 'warnCodes', title: t('message.hrms.empMonth.warn'), minWidth: 120 },
	{ field: 'buttons', title: t('message.list.operation') || '操作', width: 140, fixed: 'right', slots: { default: 'row_buttons' } },
];

const options = useVxeTable(
	{
		id: 'hrEmpMonthLine',
		name: t('message.hrms.empMonth.lines'),
		columns: baseColumns(),
		toolbarConfig: { export: false },
	},
	{
		height: 'auto',
		proxyConfig: {
			autoLoad: false,
			ajax: {
				query: async ({ page, sorts }: any) => {
					if (!batchId.value) return { data: { result: { items: [], total: 0 } } };
					const sort = sorts?.[0] || state.localPageParam.defaultSort;
					return await pageHrEmpMonthLines({
						batchId: batchId.value,
						ccCode: state.ccCode || undefined,
						page: page.currentPage,
						pageSize: page.pageSize,
						field: sort.field,
						order: sort.order,
						descStr: 'desc',
						filter: advFilter.value,
					});
				},
			},
		},
		pagerConfig: { pageSize: state.localPageParam.pageSize },
		sortConfig: { defaultSort: state.localPageParam.defaultSort },
	}
);

const gridEvents: VxeGridListeners = {
	sortChange({ field, order }) {
		state.localPageParam.defaultSort = { field, order: order || 'asc', descStr: 'desc' };
		Local.set(localPageParamKey, state.localPageParam);
	},
	pageChange({ pageSize }) {
		state.localPageParam.pageSize = pageSize;
		Local.set(localPageParamKey, state.localPageParam);
	},
};

const handleQuery = (reset = false) => xGrid.value?.commitProxy(reset ? 'reload' : 'query');
const onAdvSearch = (f: any) => {
	applySearch(f);
	handleQuery(true);
};
const goBack = () => router.push('/hr/empMonth');
const fmtDate = (d: any) => (d ? String(d).slice(0, 10) : '');
const statusTag = (s: string) => {
	if (s === 'Frozen') return 'success';
	if (s === 'Consumed') return 'info';
	if (s === 'Reopened') return 'warning';
	return 'primary';
};

const loadBatch = async () => {
	if (!batchId.value) {
		batch.value = null;
		return;
	}
	const res = await getHrEmpMonth(batchId.value);
	batch.value = res.data?.result ?? null;
	options.columns = baseColumns();
	await nextTick();
	handleQuery(true);
};

const doGenerate = async (fullReset: boolean) => {
	if (fullReset) await ElMessageBox.confirm(t('message.hrms.empMonth.confirmFullReset'), { type: 'warning' });
	await generateHrEmpMonth({ batchId: batchId.value, fullReset });
	ElMessage.success(t('message.hrms.empMonth.generated'));
	await loadBatch();
};

const doFreeze = async () => {
	const vRes = await validateHrEmpMonth({ id: batchId.value });
	const v = vRes.data?.result;
	if (v?.warnCount > 0) {
		await ElMessageBox.confirm(t('message.hrms.empMonth.confirmFreezeWarn', { n: v.warnCount }), { type: 'warning' });
	}
	await freezeHrEmpMonth({ batchId: batchId.value, confirmWarn: true });
	ElMessage.success(t('message.hrms.empMonth.frozen'));
	await loadBatch();
};

const doUnfreeze = async () => {
	const { value } = await ElMessageBox.prompt(t('message.hrms.empMonth.unfreezeReason'), t('message.hrms.empMonth.unfreeze'), {
		inputPattern: /\S+/,
		inputErrorMessage: t('message.hrms.empMonth.unfreezeReason'),
	});
	await unfreezeHrEmpMonth({ batchId: batchId.value, reason: value });
	ElMessage.success(t('message.hrms.empMonth.unfrozen'));
	await loadBatch();
};

const doExport = () => {
	const url = exportHrEmpMonthUrl(batchId.value);
	window.open(`${url}&access-token=${getToken() || ''}`, '_blank');
};

const onOrgClick = (node: any) => {
	state.ccCode = node?.ccCode || node?.id || '';
	state.ccName = node?.ccDesc || node?.label || state.ccCode;
};
const onOrgUserSelect = () => {
	state.orgDrawer = false;
	handleQuery(true);
};
const clearDept = () => {
	state.ccCode = '';
	state.ccName = '';
	handleQuery(true);
};

const openExclude = (row: any) => {
	state.adjustMode = 'exclude';
	state.adjustForm = { lineId: row.id, empId: row.empId, excludeReason: '', includeReason: '', remark: '' };
	state.adjustVisible = true;
};
const openInclude = () => {
	state.adjustMode = 'include';
	state.adjustForm = { lineId: 0, empId: '', excludeReason: '', includeReason: '', remark: '' };
	state.adjustVisible = true;
};
const submitAdjust = async () => {
	if (state.adjustMode === 'exclude') {
		await adjustHrEmpMonthLine({
			batchId: batchId.value,
			lineId: state.adjustForm.lineId,
			includeFlag: 'Exclude',
			excludeReason: state.adjustForm.excludeReason,
			remark: state.adjustForm.remark,
		});
	} else {
		await adjustHrEmpMonthLine({
			batchId: batchId.value,
			includeFlag: 'Include',
			empId: state.adjustForm.empId,
			includeReason: state.adjustForm.includeReason,
			remark: state.adjustForm.remark,
		});
	}
	ElMessage.success(t('message.hrms.empMonth.adjusted'));
	state.adjustVisible = false;
	handleQuery(false);
};

const openLineDetail = (row: any) => {
	state.currentLine = row;
	state.lineDrawer = true;
};

const empSnapRows = computed(() => {
	const json = state.currentLine?.empFieldSnapshot;
	if (!json) return [];
	try {
		const o = typeof json === 'string' ? JSON.parse(json) : json;
		return (o.fields || []).map((f: any) => ({ label: f.label || f.fieldKey, value: f.value }));
	} catch {
		return [];
	}
});
const chgSnapRows = computed(() => {
	const json = state.currentLine?.chgFieldSnapshot;
	if (!json) return [];
	try {
		const o = typeof json === 'string' ? JSON.parse(json) : json;
		const rows: any[] = [];
		for (const item of o.items || []) {
			for (const c of item.changes || []) {
				rows.push({
					label: item.label || item.fieldKey,
					chgNo: c.chgNo,
					chgDate: c.chgDate,
					before: c.valueBefore,
					after: c.valueAfter,
				});
			}
		}
		return rows;
	} catch {
		return [];
	}
});

watch(batchId, () => loadBatch());

onMounted(async () => {
	const cached = Local.get(localPageParamKey);
	if (cached) Object.assign(state.localPageParam, cached);
	setAdvFields([
		{ fieldKey: 'empId', label: t('message.hrms.empMonth.empId'), valueType: 'string' },
		{ fieldKey: 'empName', label: t('message.hrms.empMonth.empName'), valueType: 'string' },
		{ fieldKey: 'ccCode', label: t('message.hrms.empMonth.ccCode'), valueType: 'string' },
		{ fieldKey: 'includeFlag', label: t('message.hrms.empMonth.includeFlag'), valueType: 'string' },
		{ fieldKey: 'hasChgInPeriod', label: t('message.hrms.empMonth.hasChg'), valueType: 'boolean' },
	] as AdvQueryField[]);
	try {
		const [ex, inc] = await Promise.all([
			getHrCodeList('EmpMonthExcludeReason'),
			getHrCodeList('EmpMonthIncludeReason'),
		]);
		state.excludeReasons = (ex.data?.result ?? []) as any[];
		state.includeReasons = (inc.data?.result ?? []) as any[];
	} catch {
		/* ignore */
	}
	await loadBatch();
});

onActivated(async () => {
	if (!batchId.value) return;
	await nextTick();
	handleQuery(false);
});
</script>

<style scoped>
.hr-emp-month-detail { position: relative; padding: 4px; min-height: 60vh; display: flex; flex-direction: column; }
.hr-emp-month-detail .full-table { flex: 1; min-height: 360px; }
.hr-emp-month-detail .full-table :deep(.el-card__body) { height: 100%; min-height: 320px; }
.head-row { display: flex; flex-wrap: wrap; gap: 8px; align-items: center; }
.title { font-weight: 600; }
.spacer { flex: 1; }
.muted { color: var(--el-text-color-secondary); font-size: 13px; }
.adv-badge { margin-left: 4px; }
.org-panel-mask {
	position: fixed; inset: 0; background: rgba(0, 0, 0, 0.25); z-index: 40;
}
.org-panel {
	position: fixed; top: 0; left: 0; width: 320px; height: 100%; background: var(--el-bg-color);
	z-index: 41; transform: translateX(-100%); transition: transform 0.2s; display: flex; flex-direction: column;
	box-shadow: 2px 0 8px rgba(0, 0, 0, 0.08);
}
.org-panel.open { transform: translateX(0); }
.org-panel-head {
	display: flex; justify-content: space-between; align-items: center; padding: 10px 12px; border-bottom: 1px solid var(--el-border-color);
}
.org-panel-body { flex: 1; overflow: auto; padding: 8px; }
h4 { margin: 16px 0 8px; }
</style>
