<template>
	<div class="cp-company-site-list">
		<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', alignItems: 'start' }">
			<el-form :model="queryParams" :inline="true" style="flex: 1">
				<el-form-item label="关键字">
					<el-input v-model="queryParams.keyword" placeholder="部门编码/名称/代号/厂别" clearable @keyup.enter="reload(true)" />
				</el-form-item>
			</el-form>
			<el-button-group>
				<el-button type="primary" icon="ele-Search" @click="reload(true)">查询</el-button>
				<el-button icon="ele-Refresh" @click="reset">重置</el-button>
				<el-button type="success" icon="ele-Check" :loading="saving" @click="save">保存</el-button>
			</el-button-group>
		</el-card>
		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options" v-on="gridEvents">
				<template #empty><el-empty :image-size="120" /></template>
				<template #row_orgType="{ row }">
					{{ row.orgTypeName || row.orgType || '' }}
				</template>
				<template #row_belongSiteCode="{ row }">
					<el-select
						v-model="row.belongSiteCode"
						clearable
						filterable
						placeholder="请选择归属公司"
						style="width: 100%"
						@change="onBelongChange(row)"
					>
						<el-option
							v-for="s in siteOptions"
							:key="s.siteCode"
							:label="`${s.siteCode} ${s.siteName}`"
							:value="s.siteCode"
						/>
					</el-select>
				</template>
			</vxe-grid>
		</el-card>
	</div>
</template>

<script lang="ts" setup name="hrCompanyParamCompanySiteList">
import { onMounted, reactive, ref } from 'vue';
import { ElMessage } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import {
	companyOrgPageHrCompanyParam,
	pageHrCompanyParam,
	saveCompanyOrgSiteHrCompanyParam,
	siteOptionsHrCompanyParam,
} from '/@/api/hrms/hrCompanyParam';

const xGrid = ref<VxeGridInstance>();
const localPageParamKey = 'localPageParam:hrCompanyParamCompanySite';
const queryParams = reactive({ keyword: '' });
const localPageParam = reactive({
	pageSize: 50 as number,
	defaultSort: { field: 'ccLevel', order: 'asc', descStr: 'desc' },
});
const siteOptions = ref<{ siteCode: string; siteName: string }[]>([]);
const dirtyMap = reactive<Record<string, string | null | undefined>>({});
const saving = ref(false);

const options = useVxeTable(
	{
		id: 'hrCompanyParamCompanySite',
		name: '归属公司设置',
		columns: [
			{ field: 'seq', type: 'seq', title: '序号', width: 60 },
			{ field: 'ccCode', title: '部门编码', minWidth: 120, sortable: true },
			{ field: 'ccDesc', title: '部门名称', minWidth: 160, sortable: true },
			{ field: 'ccDept', title: '部门代号', minWidth: 110, sortable: true },
			{ field: 'orgType', title: '组织类型', minWidth: 100, sortable: true, slots: { default: 'row_orgType' } },
			{ field: 'ccLevel', title: '层级码', minWidth: 140, sortable: true },
			{ field: 'belongSiteCode', title: '归属公司', minWidth: 200, slots: { default: 'row_belongSiteCode' } },
		],
	},
	{
		proxyConfig: { autoLoad: true, ajax: { query: ({ page, sort }) => handleQueryApi(page, sort) } },
		sortConfig: { defaultSort: Local.get(localPageParamKey)?.defaultSort || localPageParam.defaultSort },
		pagerConfig: { pageSize: Local.get(localPageParamKey)?.pageSize || localPageParam.pageSize },
		rowConfig: { isCurrent: true, isHover: true },
	}
);

const handleQueryApi = async (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const effective = sort?.field ? sort : localPageParam.defaultSort;
	const res = await companyOrgPageHrCompanyParam({
		...queryParams,
		page: page.currentPage,
		pageSize: page.pageSize,
		field: effective.field,
		order: effective.order,
		descStr: 'desc',
	});
	Object.keys(dirtyMap).forEach((k) => delete dirtyMap[k]);
	return res;
};

const gridEvents: VxeGridListeners = {
	sortChange({ field, order }) {
		localPageParam.defaultSort = { field, order: order || 'asc', descStr: 'desc' };
		Local.set(localPageParamKey, { pageSize: localPageParam.pageSize, defaultSort: localPageParam.defaultSort });
	},
	pageChange({ pageSize }) {
		localPageParam.pageSize = pageSize;
		Local.set(localPageParamKey, { pageSize, defaultSort: localPageParam.defaultSort });
	},
};

const onBelongChange = (row: any) => {
	if (!row?.ccCode) return;
	dirtyMap[row.ccCode] = row.belongSiteCode ?? null;
};

const loadSiteOptions = async () => {
	// 优先专用下拉；失败或空时回退已有分页接口（避免新按钮权限未刷新导致无数据）
	try {
		const res = await siteOptionsHrCompanyParam();
		const list = res.data?.result ?? [];
		if (Array.isArray(list) && list.length > 0) {
			siteOptions.value = list.map((u: any) => ({
				siteCode: u.siteCode ?? u.SiteCode,
				siteName: u.siteName ?? u.SiteName,
			}));
			return;
		}
	} catch {
		/* fall through */
	}
	const pageRes = await pageHrCompanyParam({
		page: 1,
		pageSize: 500,
		field: 'siteCode',
		order: 'asc',
		descStr: 'desc',
	});
	const items = pageRes.data?.result?.items ?? [];
	siteOptions.value = items.map((u: any) => ({
		siteCode: u.siteCode,
		siteName: u.siteName,
	}));
};

const reload = async (reset = false) => {
	if (siteOptions.value.length === 0) await loadSiteOptions();
	reset ? await xGrid.value?.commitProxy('reload') : await xGrid.value?.commitProxy('query');
};

const reset = () => {
	queryParams.keyword = '';
	reload(true);
};

const save = async () => {
	const items = Object.entries(dirtyMap).map(([ccCode, belongSiteCode]) => ({
		ccCode,
		belongSiteCode: belongSiteCode || null,
	}));
	if (items.length === 0) {
		ElMessage.info('没有需要保存的变更');
		return false;
	}
	saving.value = true;
	try {
		await saveCompanyOrgSiteHrCompanyParam({ items });
		ElMessage.success('保存成功');
		Object.keys(dirtyMap).forEach((k) => delete dirtyMap[k]);
		await reload();
		return true;
	} finally {
		saving.value = false;
	}
};

onMounted(() => {
	loadSiteOptions();
});

defineExpose({ reload, save });
</script>

<style scoped>
.cp-company-site-list {
	height: 100%;
	display: flex;
	flex-direction: column;
	overflow: hidden;
	min-height: 0;
}
.cp-company-site-list > .el-card:first-child {
	flex-shrink: 0;
}
.full-table {
	flex: 1;
	min-height: 0;
	display: flex;
	flex-direction: column;
	margin-top: 5px;
}
.full-table :deep(.el-card__body) {
	flex: 1;
	min-height: 0;
	display: flex;
	flex-direction: column;
	padding: 8px;
	overflow: hidden;
}
.xGrid-style {
	flex: 1;
	min-height: 0;
}
</style>
