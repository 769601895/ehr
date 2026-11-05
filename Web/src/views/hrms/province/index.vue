<template>
	<div class="hr-province">
		<splitpanes class="default-theme">
			<pane size="42" style="display: flex; flex-direction: column">
				<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', alignItems: 'start' }">
					<el-form :model="prov.queryParams" :inline="true" style="flex: 1">
						<el-form-item :label="t('message.hrms.province.keyword')">
							<el-input
								v-model="prov.queryParams.keyword"
								:placeholder="t('message.hrms.province.keywordProvPh')"
								clearable
								@keyup.enter="reloadProv(true)"
							/>
						</el-form-item>
						<el-form-item :label="t('message.hrms.province.status')">
							<el-select v-model="prov.queryParams.status" clearable :placeholder="t('message.hrms.province.statusAll')" style="width: 110px">
								<el-option :label="t('message.hrms.province.statusActive')" value="Active" />
								<el-option :label="t('message.hrms.province.statusInactive')" value="Inactive" />
							</el-select>
						</el-form-item>
					</el-form>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="reloadProv(true)">{{ t('message.hrms.province.query') }}</el-button>
						<el-button icon="ele-Refresh" @click="resetProv">{{ t('message.hrms.province.reset') }}</el-button>
					</el-button-group>
				</el-card>
				<el-card class="full-table" shadow="hover" style="margin-top: 5px; flex: 1">
					<vxe-grid ref="provGrid" class="xGrid-style" v-bind="provOptions" v-on="provEvents">
						<template #toolbar_buttons>
							<el-button type="primary" icon="ele-Plus" v-auth="'hrProvince/save'" @click="openProvEdit()">
								{{ t('message.hrms.province.addProvince') }}
							</el-button>
						</template>
						<template #empty><el-empty :image-size="120" /></template>
						<template #row_status="{ row }">
							<el-tag :type="row.status === 'Active' ? 'success' : 'info'" size="small">
								{{ row.status === 'Active' ? t('message.hrms.province.statusActive') : t('message.hrms.province.statusInactive') }}
							</el-tag>
						</template>
						<template #row_buttons="{ row }">
							<el-button icon="ele-Edit" text type="primary" v-auth="'hrProvince/save'" @click="openProvEdit(row)" />
							<el-button icon="ele-ChatLineSquare" text type="primary" v-auth="'hrProvince/saveLang'" @click="openProvLang(row)" />
							<el-button icon="ele-Delete" text type="danger" v-auth="'hrProvince/delete'" @click="deleteProv(row)" />
						</template>
					</vxe-grid>
				</el-card>
			</pane>

			<pane size="58" style="display: flex; flex-direction: column">
				<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', alignItems: 'start' }">
					<div style="flex: 1; line-height: 32px; padding-left: 4px">
						<span v-if="state.currentProvinceCode">
							{{ t('message.hrms.province.currentProvince', { name: state.currentProvinceName, code: state.currentProvinceCode }) }}
						</span>
						<span v-else style="color: #909399">{{ t('message.hrms.province.selectProvinceHint') }}</span>
					</div>
					<el-form :model="city.queryParams" :inline="true">
						<el-form-item :label="t('message.hrms.province.keyword')">
							<el-input
								v-model="city.queryParams.keyword"
								:placeholder="t('message.hrms.province.keywordCityPh')"
								clearable
								style="width: 140px"
								:disabled="!state.currentProvinceCode"
								@keyup.enter="reloadCity(true)"
							/>
						</el-form-item>
						<el-form-item :label="t('message.hrms.province.status')">
							<el-select
								v-model="city.queryParams.status"
								clearable
								:placeholder="t('message.hrms.province.statusAll')"
								style="width: 110px"
								:disabled="!state.currentProvinceCode"
							>
								<el-option :label="t('message.hrms.province.statusActive')" value="Active" />
								<el-option :label="t('message.hrms.province.statusInactive')" value="Inactive" />
							</el-select>
						</el-form-item>
					</el-form>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" :disabled="!state.currentProvinceCode" @click="reloadCity(true)">
							{{ t('message.hrms.province.query') }}
						</el-button>
						<el-button icon="ele-Refresh" :disabled="!state.currentProvinceCode" @click="resetCity">
							{{ t('message.hrms.province.reset') }}
						</el-button>
					</el-button-group>
				</el-card>
				<el-card class="full-table" shadow="hover" style="margin-top: 5px; flex: 1">
					<vxe-grid ref="cityGrid" class="xGrid-style" v-bind="cityOptions" v-on="cityEvents">
						<template #toolbar_buttons>
							<el-button type="primary" icon="ele-Plus" v-auth="'hrCity/save'" :disabled="!state.currentProvinceCode" @click="openCityEdit()">
								{{ t('message.hrms.province.addCity') }}
							</el-button>
						</template>
						<template #empty>
							<el-empty
								:image-size="120"
								:description="state.currentProvinceCode ? t('message.hrms.province.emptyCity') : t('message.hrms.province.emptySelectProvince')"
							/>
						</template>
						<template #row_status="{ row }">
							<el-tag :type="row.status === 'Active' ? 'success' : 'info'" size="small">
								{{ row.status === 'Active' ? t('message.hrms.province.statusActive') : t('message.hrms.province.statusInactive') }}
							</el-tag>
						</template>
						<template #row_buttons="{ row }">
							<el-button icon="ele-Edit" text type="primary" v-auth="'hrCity/save'" @click="openCityEdit(row)" />
							<el-button icon="ele-ChatLineSquare" text type="primary" v-auth="'hrCity/saveLang'" @click="openCityLang(row)" />
							<el-button icon="ele-Delete" text type="danger" v-auth="'hrCity/delete'" @click="deleteCity(row)" />
						</template>
					</vxe-grid>
				</el-card>
			</pane>
		</splitpanes>

		<el-dialog
			v-model="prov.editVisible"
			:title="prov.edit.id ? t('message.hrms.province.editProvince') : t('message.hrms.province.addProvince')"
			width="560px"
			destroy-on-close
		>
			<el-form :model="prov.edit" label-width="110px">
				<el-form-item :label="t('message.hrms.province.provinceCode')" required>
					<el-input v-model="prov.edit.provinceCode" maxlength="12" :disabled="!!prov.edit.id" />
				</el-form-item>
				<el-form-item :label="t('message.hrms.province.provinceName')" required>
					<el-input v-model="prov.edit.provinceName" maxlength="50" :placeholder="t('message.hrms.province.masterNamePh')" />
				</el-form-item>
				<el-divider content-position="left">{{ t('message.hrms.province.multiLangNames') }}</el-divider>
				<el-form-item v-for="item in prov.editLangs" :key="item.culture" :label="cultureLabel(item.culture)">
					<el-input v-model="item.label" maxlength="50" :placeholder="item.culture" />
				</el-form-item>
				<el-form-item :label="t('message.hrms.province.orderNo')">
					<el-input-number v-model="prov.edit.orderNo" :min="0" :max="999999" />
				</el-form-item>
				<el-form-item :label="t('message.hrms.province.status')">
					<el-select v-model="prov.edit.status" class="w100">
						<el-option :label="t('message.hrms.province.statusActive')" value="Active" />
						<el-option :label="t('message.hrms.province.statusInactive')" value="Inactive" />
					</el-select>
				</el-form-item>
				<el-form-item :label="t('message.hrms.province.remark')">
					<el-input v-model="prov.edit.remark" type="textarea" maxlength="256" />
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="prov.editVisible = false">{{ t('message.hrms.province.cancel') }}</el-button>
				<el-button type="primary" :loading="prov.saving" @click="saveProv">{{ t('message.hrms.province.save') }}</el-button>
			</template>
		</el-dialog>

		<el-dialog
			v-model="city.editVisible"
			:title="city.edit.id ? t('message.hrms.province.editCity') : t('message.hrms.province.addCity')"
			width="560px"
			destroy-on-close
		>
			<el-form :model="city.edit" label-width="110px">
				<el-form-item :label="t('message.hrms.province.belongProvince')">
					<el-input :model-value="`${state.currentProvinceName}（${state.currentProvinceCode}）`" disabled />
				</el-form-item>
				<el-form-item :label="t('message.hrms.province.cityCode')" required>
					<el-input v-model="city.edit.cityCode" maxlength="12" :disabled="!!city.edit.id" />
				</el-form-item>
				<el-form-item :label="t('message.hrms.province.cityName')" required>
					<el-input v-model="city.edit.cityName" maxlength="50" :placeholder="t('message.hrms.province.masterNamePh')" />
				</el-form-item>
				<el-divider content-position="left">{{ t('message.hrms.province.multiLangNames') }}</el-divider>
				<el-form-item v-for="item in city.editLangs" :key="item.culture" :label="cultureLabel(item.culture)">
					<el-input v-model="item.label" maxlength="50" :placeholder="item.culture" />
				</el-form-item>
				<el-form-item :label="t('message.hrms.province.orderNo')">
					<el-input-number v-model="city.edit.orderNo" :min="0" :max="999999" />
				</el-form-item>
				<el-form-item :label="t('message.hrms.province.status')">
					<el-select v-model="city.edit.status" class="w100">
						<el-option :label="t('message.hrms.province.statusActive')" value="Active" />
						<el-option :label="t('message.hrms.province.statusInactive')" value="Inactive" />
					</el-select>
				</el-form-item>
				<el-form-item :label="t('message.hrms.province.remark')">
					<el-input v-model="city.edit.remark" type="textarea" maxlength="256" />
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="city.editVisible = false">{{ t('message.hrms.province.cancel') }}</el-button>
				<el-button type="primary" :loading="city.saving" @click="saveCity">{{ t('message.hrms.province.save') }}</el-button>
			</template>
		</el-dialog>

		<el-dialog v-model="prov.langVisible" :title="t('message.hrms.province.langProvince')" width="520px" destroy-on-close>
			<el-form label-width="88px">
				<el-form-item v-for="item in prov.langs" :key="item.culture" :label="item.culture">
					<el-input v-model="item.label" maxlength="50" />
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="prov.langVisible = false">{{ t('message.hrms.province.cancel') }}</el-button>
				<el-button type="primary" :loading="prov.saving" @click="saveProvLang">{{ t('message.hrms.province.save') }}</el-button>
			</template>
		</el-dialog>

		<el-dialog v-model="city.langVisible" :title="t('message.hrms.province.langCity')" width="520px" destroy-on-close>
			<el-form label-width="88px">
				<el-form-item v-for="item in city.langs" :key="item.culture" :label="item.culture">
					<el-input v-model="item.label" maxlength="50" />
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="city.langVisible = false">{{ t('message.hrms.province.cancel') }}</el-button>
				<el-button type="primary" :loading="city.saving" @click="saveCityLang">{{ t('message.hrms.province.save') }}</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="hrProvince">
import { onMounted, reactive, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage, ElMessageBox } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';
import {
	deleteHrProvince,
	getHrProvince,
	langListHrProvince,
	pageHrProvince,
	saveHrProvince,
	saveLangHrProvince,
} from '/@/api/hrms/hrProvince';
import { deleteHrCity, getHrCity, langListHrCity, pageHrCity, saveHrCity, saveLangHrCity } from '/@/api/hrms/hrCity';
import { hrUiCultures } from '/@/views/hrms/composables/hrUiCulture';

const { t, locale } = useI18n();
const provGrid = ref<VxeGridInstance>();
const cityGrid = ref<VxeGridInstance>();
const provLocalKey = 'localPageParam:hrProvince';
const cityLocalKey = 'localPageParam:hrCity';

const state = reactive({
	currentProvinceCode: '' as string,
	currentProvinceName: '' as string,
});

const otherCultures = hrUiCultures.filter((c) => c.value !== 'zh-CN');
const cultureLabel = (culture: string) => hrUiCultures.find((c) => c.value === culture)?.label || culture;
const blankEditLangs = () => otherCultures.map((c) => ({ culture: c.value, label: '' }));

const prov = reactive({
	queryParams: { keyword: '', status: '' as string },
	localPageParam: { pageSize: 50 as number, defaultSort: { field: 'orderNo', order: 'asc', descStr: 'desc' } },
	editVisible: false,
	langVisible: false,
	saving: false,
	edit: blankProv(),
	editLangs: blankEditLangs(),
	langs: [] as { culture: string; label: string }[],
	langCode: '',
});

const city = reactive({
	queryParams: { keyword: '', status: '' as string },
	localPageParam: { pageSize: 50 as number, defaultSort: { field: 'orderNo', order: 'asc', descStr: 'desc' } },
	editVisible: false,
	langVisible: false,
	saving: false,
	edit: blankCity(),
	editLangs: blankEditLangs(),
	langs: [] as { culture: string; label: string }[],
	langCode: '',
});

function blankProv() {
	return { id: undefined as number | undefined, provinceCode: '', provinceName: '', orderNo: 100, status: 'Active', remark: '' };
}
function blankCity() {
	return { id: undefined as number | undefined, provinceCode: '', cityCode: '', cityName: '', orderNo: 100, status: 'Active', remark: '' };
}

/** 主表中文名 + 其它语种，一并提交 SaveLang */
function buildLangLabels(masterName: string, otherLangs: { culture: string; label: string }[]) {
	return [
		{ culture: 'zh-CN', label: masterName.trim() },
		...otherLangs.map((u) => ({ culture: u.culture, label: (u.label || '').trim() })),
	].filter((u) => !!u.label);
}

const buildProvColumns = () => [
	{ field: 'seq', type: 'seq', title: t('message.hrms.province.seq'), width: 55 },
	{ field: 'provinceCode', title: t('message.hrms.province.provinceCode'), minWidth: 90, sortable: true },
	{ field: 'provinceName', title: t('message.hrms.province.provinceName'), minWidth: 110, sortable: true },
	{ field: 'orderNo', title: t('message.hrms.province.orderNo'), width: 80, sortable: true },
	{ field: 'status', title: t('message.hrms.province.status'), width: 90, sortable: true, slots: { default: 'row_status' } },
	{ field: 'buttons', title: t('message.hrms.province.operation'), width: 120, fixed: 'right', slots: { default: 'row_buttons' } },
];

const buildCityColumns = () => [
	{ field: 'seq', type: 'seq', title: t('message.hrms.province.seq'), width: 55 },
	{ field: 'cityCode', title: t('message.hrms.province.cityCode'), minWidth: 90, sortable: true },
	{ field: 'cityName', title: t('message.hrms.province.cityName'), minWidth: 120, sortable: true },
	{ field: 'orderNo', title: t('message.hrms.province.orderNo'), width: 80, sortable: true },
	{ field: 'status', title: t('message.hrms.province.status'), width: 90, sortable: true, slots: { default: 'row_status' } },
	{ field: 'buttons', title: t('message.hrms.province.operation'), width: 120, fixed: 'right', slots: { default: 'row_buttons' } },
];

const provOptions = useVxeTable(
	{
		id: 'hrProvince',
		name: t('message.hrms.province.moduleName'),
		columns: buildProvColumns(),
	},
	{
		proxyConfig: { autoLoad: true, ajax: { query: ({ page, sort }) => queryProv(page, sort) } },
		sortConfig: { defaultSort: Local.get(provLocalKey)?.defaultSort || prov.localPageParam.defaultSort },
		pagerConfig: { pageSize: Local.get(provLocalKey)?.pageSize || prov.localPageParam.pageSize },
		rowConfig: { isCurrent: true, isHover: true },
	}
);

const cityOptions = useVxeTable(
	{
		id: 'hrCity',
		name: t('message.hrms.province.moduleName'),
		columns: buildCityColumns(),
	},
	{
		proxyConfig: { autoLoad: false, ajax: { query: ({ page, sort }) => queryCity(page, sort) } },
		sortConfig: { defaultSort: Local.get(cityLocalKey)?.defaultSort || city.localPageParam.defaultSort },
		pagerConfig: { pageSize: Local.get(cityLocalKey)?.pageSize || city.localPageParam.pageSize },
	}
);

watch(locale, async () => {
	provGrid.value?.loadColumn(buildProvColumns() as any);
	cityGrid.value?.loadColumn(buildCityColumns() as any);
	await reloadProv(true);
	if (state.currentProvinceCode) await reloadCity(true);
});

const queryProv = (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const effective = sort?.field ? sort : prov.localPageParam.defaultSort;
	return pageHrProvince({
		...prov.queryParams,
		page: page.currentPage,
		pageSize: page.pageSize,
		field: effective.field,
		order: effective.order,
		descStr: 'desc',
	});
};

const queryCity = (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	if (!state.currentProvinceCode) return Promise.resolve({ data: { result: { items: [], total: 0 } } });
	const effective = sort?.field ? sort : city.localPageParam.defaultSort;
	return pageHrCity({
		provinceCode: state.currentProvinceCode,
		...city.queryParams,
		page: page.currentPage,
		pageSize: page.pageSize,
		field: effective.field,
		order: effective.order,
		descStr: 'desc',
	});
};

const reloadProv = async (reset = false) => {
	reset ? await provGrid.value?.commitProxy('reload') : await provGrid.value?.commitProxy('query');
};
const resetProv = () => {
	prov.queryParams.keyword = '';
	prov.queryParams.status = '';
	reloadProv(true);
};
const reloadCity = async (reset = false) => {
	if (!state.currentProvinceCode) return;
	reset ? await cityGrid.value?.commitProxy('reload') : await cityGrid.value?.commitProxy('query');
};
const resetCity = () => {
	city.queryParams.keyword = '';
	city.queryParams.status = '';
	reloadCity(true);
};

const selectProvince = async (row: any) => {
	state.currentProvinceCode = row?.provinceCode || '';
	state.currentProvinceName = row?.provinceName || '';
	city.queryParams.keyword = '';
	city.queryParams.status = '';
	await reloadCity(true);
};

const openProvEdit = async (row?: any) => {
	if (row?.id) {
		const res = await getHrProvince(row.id);
		const d = res.data?.result ?? res.data;
		prov.edit = {
			id: d.id,
			provinceCode: d.provinceCode,
			provinceName: d.provinceName,
			orderNo: d.orderNo ?? 100,
			status: d.status || 'Active',
			remark: d.remark || '',
		};
		const langRes = await langListHrProvince(d.provinceCode);
		const map = new Map((langRes.data?.result ?? []).map((x: any) => [x.culture, x.label]));
		prov.editLangs = blankEditLangs().map((c) => ({ ...c, label: (map.get(c.culture) as string) || '' }));
	} else {
		prov.edit = blankProv();
		prov.editLangs = blankEditLangs();
	}
	prov.editVisible = true;
};

const saveProv = async () => {
	if (!prov.edit.provinceCode?.trim() || !prov.edit.provinceName?.trim()) {
		ElMessage.warning(t('message.hrms.province.requiredProvince'));
		return;
	}
	prov.saving = true;
	try {
		const code = prov.edit.provinceCode.trim();
		const name = prov.edit.provinceName.trim();
		await saveHrProvince({
			id: prov.edit.id,
			provinceCode: code,
			provinceName: name,
			orderNo: prov.edit.orderNo ?? 100,
			status: prov.edit.status,
			remark: prov.edit.remark || null,
		});
		const labels = buildLangLabels(name, prov.editLangs);
		if (labels.length) await saveLangHrProvince({ provinceCode: code, labels });
		ElMessage.success(t('message.hrms.province.saved'));
		prov.editVisible = false;
		await reloadProv(true);
	} finally {
		prov.saving = false;
	}
};

const deleteProv = async (row: any) => {
	await ElMessageBox.confirm(t('message.hrms.province.confirmDeleteProvince', { name: row.provinceName }), t('message.hrms.province.tip'), {
		type: 'warning',
	});
	await deleteHrProvince({ id: row.id });
	ElMessage.success(t('message.hrms.province.deleted'));
	if (state.currentProvinceCode === row.provinceCode) {
		state.currentProvinceCode = '';
		state.currentProvinceName = '';
		await cityGrid.value?.commitProxy('reload');
	}
	await reloadProv(true);
};

const openProvLang = async (row: any) => {
	prov.langCode = row.provinceCode;
	const res = await langListHrProvince(row.provinceCode);
	const map = new Map((res.data?.result ?? []).map((x: any) => [x.culture, x.label]));
	prov.langs = hrUiCultures.map((c) => ({ culture: c.value, label: (map.get(c.value) as string) || '' }));
	prov.langVisible = true;
};
const saveProvLang = async () => {
	prov.saving = true;
	try {
		await saveLangHrProvince({ provinceCode: prov.langCode, labels: prov.langs });
		ElMessage.success(t('message.hrms.province.langSaved'));
		prov.langVisible = false;
		await reloadProv(true);
	} finally {
		prov.saving = false;
	}
};

const openCityEdit = async (row?: any) => {
	if (!state.currentProvinceCode) {
		ElMessage.warning(t('message.hrms.province.needProvince'));
		return;
	}
	if (row?.id) {
		const res = await getHrCity(row.id);
		const d = res.data?.result ?? res.data;
		city.edit = {
			id: d.id,
			provinceCode: d.provinceCode,
			cityCode: d.cityCode,
			cityName: d.cityName,
			orderNo: d.orderNo ?? 100,
			status: d.status || 'Active',
			remark: d.remark || '',
		};
		const langRes = await langListHrCity(d.cityCode);
		const map = new Map((langRes.data?.result ?? []).map((x: any) => [x.culture, x.label]));
		city.editLangs = blankEditLangs().map((c) => ({ ...c, label: (map.get(c.culture) as string) || '' }));
	} else {
		city.edit = blankCity();
		city.edit.provinceCode = state.currentProvinceCode;
		city.editLangs = blankEditLangs();
	}
	city.editVisible = true;
};

const saveCity = async () => {
	if (!city.edit.cityCode?.trim() || !city.edit.cityName?.trim()) {
		ElMessage.warning(t('message.hrms.province.requiredCity'));
		return;
	}
	city.saving = true;
	try {
		const code = city.edit.cityCode.trim();
		const name = city.edit.cityName.trim();
		await saveHrCity({
			id: city.edit.id,
			provinceCode: state.currentProvinceCode,
			cityCode: code,
			cityName: name,
			orderNo: city.edit.orderNo ?? 100,
			status: city.edit.status,
			remark: city.edit.remark || null,
		});
		const labels = buildLangLabels(name, city.editLangs);
		if (labels.length) await saveLangHrCity({ cityCode: code, labels });
		ElMessage.success(t('message.hrms.province.saved'));
		city.editVisible = false;
		await reloadCity(true);
	} finally {
		city.saving = false;
	}
};

const deleteCity = async (row: any) => {
	await ElMessageBox.confirm(t('message.hrms.province.confirmDeleteCity', { name: row.cityName }), t('message.hrms.province.tip'), {
		type: 'warning',
	});
	await deleteHrCity({ id: row.id });
	ElMessage.success(t('message.hrms.province.deleted'));
	await reloadCity(true);
};

const openCityLang = async (row: any) => {
	city.langCode = row.cityCode;
	const res = await langListHrCity(row.cityCode);
	const map = new Map((res.data?.result ?? []).map((x: any) => [x.culture, x.label]));
	city.langs = hrUiCultures.map((c) => ({ culture: c.value, label: (map.get(c.value) as string) || '' }));
	city.langVisible = true;
};
const saveCityLang = async () => {
	city.saving = true;
	try {
		await saveLangHrCity({ cityCode: city.langCode, labels: city.langs });
		ElMessage.success(t('message.hrms.province.langSaved'));
		city.langVisible = false;
		await reloadCity(true);
	} finally {
		city.saving = false;
	}
};

const provEvents: VxeGridListeners = {
	currentChange({ row }) {
		if (row) selectProvince(row);
	},
	cellClick({ row }) {
		if (row) selectProvince(row);
	},
	sortChange({ field, order }) {
		prov.localPageParam.defaultSort = { field, order: order || 'asc', descStr: 'desc' };
		Local.set(provLocalKey, { pageSize: prov.localPageParam.pageSize, defaultSort: prov.localPageParam.defaultSort });
	},
	pageChange({ pageSize }) {
		prov.localPageParam.pageSize = pageSize;
		Local.set(provLocalKey, { pageSize, defaultSort: prov.localPageParam.defaultSort });
	},
};

const cityEvents: VxeGridListeners = {
	sortChange({ field, order }) {
		city.localPageParam.defaultSort = { field, order: order || 'asc', descStr: 'desc' };
		Local.set(cityLocalKey, { pageSize: city.localPageParam.pageSize, defaultSort: city.localPageParam.defaultSort });
	},
	pageChange({ pageSize }) {
		city.localPageParam.pageSize = pageSize;
		Local.set(cityLocalKey, { pageSize, defaultSort: city.localPageParam.defaultSort });
	},
};

onMounted(() => {
	const pc = Local.get(provLocalKey);
	if (pc?.pageSize) prov.localPageParam.pageSize = pc.pageSize;
	if (pc?.defaultSort) prov.localPageParam.defaultSort = pc.defaultSort;
	const cc = Local.get(cityLocalKey);
	if (cc?.pageSize) city.localPageParam.pageSize = cc.pageSize;
	if (cc?.defaultSort) city.localPageParam.defaultSort = cc.defaultSort;
});
</script>

<style scoped>
.hr-province {
	height: 100%;
}
.w100 {
	width: 100%;
}
</style>
