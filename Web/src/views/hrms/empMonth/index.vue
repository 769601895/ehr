<template>
	<div class="hr-emp-month">
		<el-card shadow="hover" :body-style="{ padding: '8px 10px', display: 'flex', width: '100%', alignItems: 'center', flexWrap: 'wrap', gap: '6px' }">
			<el-input v-model="state.queryParams.keyword" :placeholder="t('message.hrms.empMonth.keyword')" clearable style="width: 200px" @keyup.enter="handleQuery(true)" />
			<el-select v-model="state.queryParams.siteCode" clearable filterable :placeholder="t('message.hrms.empMonth.siteCode')" style="width: 160px">
				<el-option v-for="s in state.siteOptions" :key="s.value" :label="s.label" :value="s.value" />
			</el-select>
			<el-input v-model="state.queryParams.payPeriod" :placeholder="t('message.hrms.empMonth.payPeriod')" clearable maxlength="6" style="width: 120px" />
			<el-select v-model="state.queryParams.status" clearable :placeholder="t('message.hrms.empMonth.status')" style="width: 120px">
				<el-option v-for="s in statusOptions" :key="s.value" :label="s.label" :value="s.value" />
			</el-select>
			<el-button-group>
				<el-button type="primary" icon="ele-Search" v-auth="'hrEmpMonth/page'" @click="handleQuery(true)">{{ t('message.list.query') }}</el-button>
				<el-button icon="ele-Refresh" @click="resetQuery">{{ t('message.list.reset') }}</el-button>
			</el-button-group>
			<el-button icon="ele-Filter" type="primary" plain @click="openAdvQuery">
				{{ t('message.hrms.advQuery.title') }}
				<el-badge v-if="advActiveCount" :value="advActiveCount" class="adv-badge" />
			</el-button>
			<el-button type="primary" icon="ele-Plus" v-auth="'hrEmpMonth/create'" @click="openCreate">{{ t('message.hrms.empMonth.create') }}</el-button>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options" v-on="gridEvents">
				<template #empty><el-empty :image-size="120" /></template>
				<template #row_status="{ row }">
					<el-tag size="small" :type="statusTag(row.status)">{{ row.statusText || row.status }}</el-tag>
				</template>
				<template #row_period="{ row }">
					<span>{{ fmtDate(row.attFrom) }} ~ {{ fmtDate(row.attTo) }}</span>
				</template>
				<template #row_buttons="{ row }">
					<el-button icon="ele-View" text type="primary" v-auth="'hrEmpMonth/get'" @click="goDetail(row)" />
					<el-button
						v-if="row.status === 'Draft'"
						icon="ele-Delete"
						text
						type="danger"
						v-auth="'hrEmpMonth/delete'"
						@click="handleDelete(row)"
					/>
				</template>
			</vxe-grid>
		</el-card>

		<el-dialog v-model="state.createVisible" :title="t('message.hrms.empMonth.create')" width="480px" destroy-on-close>
			<el-form label-width="100px">
				<el-form-item :label="t('message.hrms.empMonth.siteCode')" required>
					<el-select v-model="state.createForm.siteCode" filterable style="width: 100%" @change="previewPeriod">
						<el-option v-for="s in state.siteOptions" :key="s.value" :label="s.label" :value="s.value" />
					</el-select>
				</el-form-item>
				<el-form-item :label="t('message.hrms.empMonth.payPeriod')" required>
					<el-input v-model="state.createForm.payPeriod" maxlength="6" placeholder="yyyyMM" @blur="previewPeriod" />
				</el-form-item>
				<el-form-item :label="t('message.hrms.empMonth.attWindow')">
					<span v-if="state.preview">{{ fmtDate(state.preview.attFrom) }} ~ {{ fmtDate(state.preview.attTo) }}（{{ state.preview.attMonth }}）</span>
					<span v-else class="muted">{{ t('message.hrms.empMonth.previewHint') }}</span>
				</el-form-item>
				<el-form-item :label="t('message.hrms.empMonth.remark')">
					<el-input v-model="state.createForm.remark" type="textarea" :rows="2" />
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="state.createVisible = false">{{ t('message.list.cancel') || '取消' }}</el-button>
				<el-button type="primary" :loading="state.saving" @click="submitCreate">{{ t('message.list.confirm') || '确定' }}</el-button>
			</template>
		</el-dialog>

		<AdvQueryDrawer
			ref="advQueryRef"
			:module-code="advModuleCode"
			:fields="advFields"
			@search="onAdvSearch"
			@update:active-count="setAdvActiveCount"
		/>
	</div>
</template>

<script lang="ts" setup name="hrEmpMonth">
import { onMounted, reactive, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';
import { ElMessage, ElMessageBox } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { AdvQueryDrawer, useAdvQuery, type AdvQueryField } from '/@/components/advQuery';
import { pageHrCompanyParam, siteOptionsHrCompanyParam } from '/@/api/hrms/hrCompanyParam';
import { createHrEmpMonth, deleteHrEmpMonth, pageHrEmpMonth, resolvePeriodPreview } from '/@/api/hrms/empMonth';

const { t } = useI18n();
const router = useRouter();
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
} = useAdvQuery('hrEmpMonth');

const localPageParamKey = 'localPageParam:hrEmpMonth';
const statusOptions = [
	{ value: 'Draft', label: t('message.hrms.empMonth.statusDraft') },
	{ value: 'Generated', label: t('message.hrms.empMonth.statusGenerated') },
	{ value: 'Frozen', label: t('message.hrms.empMonth.statusFrozen') },
	{ value: 'Reopened', label: t('message.hrms.empMonth.statusReopened') },
	{ value: 'Consumed', label: t('message.hrms.empMonth.statusConsumed') },
];

const state = reactive({
	queryParams: {
		keyword: '',
		siteCode: undefined as string | undefined,
		payPeriod: '',
		status: undefined as string | undefined,
	},
	siteOptions: [] as { value: string; label: string }[],
	createVisible: false,
	saving: false,
	createForm: { siteCode: '', payPeriod: '', remark: '' },
	preview: null as any,
	localPageParam: {
		pageSize: 50 as number,
		defaultSort: { field: 'payPeriod', order: 'desc', descStr: 'desc' },
	},
});

const columns = ref<VxeGridPropTypes.Columns>([
	{ field: 'siteCode', title: t('message.hrms.empMonth.siteCode'), minWidth: 100, sortable: true },
	{ field: 'siteName', title: t('message.hrms.empMonth.siteName'), minWidth: 120 },
	{ field: 'payPeriod', title: t('message.hrms.empMonth.payPeriod'), width: 100, sortable: true },
	{ field: 'attMonth', title: t('message.hrms.empMonth.attMonth'), width: 100, sortable: true },
	{ field: 'attFrom', title: t('message.hrms.empMonth.attWindow'), minWidth: 200, slots: { default: 'row_period' } },
	{ field: 'status', title: t('message.hrms.empMonth.status'), width: 100, sortable: true, slots: { default: 'row_status' } },
	{ field: 'freezeVersion', title: t('message.hrms.empMonth.freezeVersion'), width: 90, sortable: true },
	{ field: 'frozenTime', title: t('message.hrms.empMonth.frozenTime'), minWidth: 150, sortable: true },
	{ field: 'remark', title: t('message.hrms.empMonth.remark'), minWidth: 120 },
	{ field: 'buttons', title: t('message.list.operation') || '操作', width: 100, fixed: 'right', slots: { default: 'row_buttons' } },
]);

const options = useVxeTable(
	{
		id: 'hrEmpMonth',
		name: t('message.hrms.empMonth.title'),
		columns: columns.value,
		toolbarConfig: { export: false },
	},
	{
		proxyConfig: {
			autoLoad: true,
			ajax: {
				query: async ({ page, sorts }: any) => {
					const sort = sorts?.[0] || state.localPageParam.defaultSort;
					const res = await pageHrEmpMonth({
						...state.queryParams,
						page: page.currentPage,
						pageSize: page.pageSize,
						field: sort.field,
						order: sort.order,
						descStr: 'desc',
						filter: advFilter.value,
					});
					return res;
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

const handleQuery = (reset = false) => {
	xGrid.value?.commitProxy(reset ? 'reload' : 'query');
};
const resetQuery = () => {
	state.queryParams.keyword = '';
	state.queryParams.siteCode = undefined;
	state.queryParams.payPeriod = '';
	state.queryParams.status = undefined;
	resetAdvQuery();
	handleQuery(true);
};
const onAdvSearch = (f: any) => {
	applySearch(f);
	handleQuery(true);
};

const statusTag = (s: string) => {
	if (s === 'Frozen') return 'success';
	if (s === 'Consumed') return 'info';
	if (s === 'Reopened') return 'warning';
	if (s === 'Generated') return 'primary';
	return '';
};
const fmtDate = (d: any) => (d ? String(d).slice(0, 10) : '');

const goDetail = (row: any) => {
	if (!row?.id) return;
	router.push({ name: 'hrEmpMonthDetail', query: { id: String(row.id) } });
};

const openCreate = () => {
	state.createForm = { siteCode: '', payPeriod: '', remark: '' };
	state.preview = null;
	state.createVisible = true;
};

const previewPeriod = async () => {
	if (!state.createForm.siteCode || !/^\d{6}$/.test(state.createForm.payPeriod || '')) {
		state.preview = null;
		return;
	}
	try {
		const res = await resolvePeriodPreview({
			siteCode: state.createForm.siteCode,
			payPeriod: state.createForm.payPeriod,
		});
		state.preview = res.data?.result ?? null;
	} catch {
		state.preview = null;
	}
};

const submitCreate = async () => {
	if (!state.createForm.siteCode || !state.createForm.payPeriod) {
		ElMessage.warning(t('message.hrms.empMonth.createRequired'));
		return;
	}
	state.saving = true;
	try {
		const res = await createHrEmpMonth(state.createForm);
		const id = res.data?.result;
		ElMessage.success(t('message.hrms.empMonth.created'));
		state.createVisible = false;
		handleQuery(true);
		if (id) router.push({ name: 'hrEmpMonthDetail', query: { id: String(id) } });
	} finally {
		state.saving = false;
	}
};

const handleDelete = async (row: any) => {
	await ElMessageBox.confirm(t('message.hrms.empMonth.confirmDelete'), { type: 'warning' });
	await deleteHrEmpMonth({ id: row.id });
	ElMessage.success(t('message.hrms.empMonth.deleted'));
	handleQuery(false);
};

const mapSiteOption = (x: any) => ({
	value: String(x.value ?? x.siteCode ?? x.code ?? ''),
	label: String(x.label ?? x.siteName ?? x.name ?? x.siteCode ?? x.value ?? ''),
});

const loadSiteOptions = async () => {
	// 与公司参数页一致：取 res.data.result；失败或空时回退分页，避免权限未刷/解析错误导致无数据
	try {
		const res = await siteOptionsHrCompanyParam();
		const list = res.data?.result ?? [];
		if (Array.isArray(list) && list.length > 0) {
			state.siteOptions = list.map(mapSiteOption).filter((x) => x.value);
			return;
		}
	} catch {
		/* fall through */
	}
	try {
		const pageRes = await pageHrCompanyParam({
			page: 1,
			pageSize: 500,
			field: 'siteCode',
			order: 'asc',
			descStr: 'desc',
		});
		const items = pageRes.data?.result?.items ?? [];
		state.siteOptions = items.map(mapSiteOption).filter((x: { value: string }) => x.value);
	} catch {
		state.siteOptions = [];
	}
};

onMounted(async () => {
	const cached = Local.get(localPageParamKey);
	if (cached) Object.assign(state.localPageParam, cached);
	setAdvFields([
		{ fieldKey: 'siteCode', label: t('message.hrms.empMonth.siteCode'), valueType: 'string' },
		{ fieldKey: 'payPeriod', label: t('message.hrms.empMonth.payPeriod'), valueType: 'string' },
		{ fieldKey: 'status', label: t('message.hrms.empMonth.status'), valueType: 'string' },
		{ fieldKey: 'siteName', label: t('message.hrms.empMonth.siteName'), valueType: 'string' },
		{ fieldKey: 'remark', label: t('message.hrms.empMonth.remark'), valueType: 'string' },
	] as AdvQueryField[]);
	await loadSiteOptions();
});
</script>

<style scoped>
.hr-emp-month { padding: 4px; }
.adv-badge { margin-left: 6px; }
.muted { color: var(--el-text-color-secondary); }
</style>
