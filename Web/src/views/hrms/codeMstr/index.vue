<template>
	<div class="hr-code-mstr" :class="{ 'is-embedded': embedded }">
		<!-- 专用入口：仅码值，锁定代码类 -->
		<template v-if="lockedCodeField">
			<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', alignItems: 'start' }">
				<el-form :model="state.queryParams" :inline="true" style="flex: 1">
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
						<el-button icon="ele-Delete" v-auth="'hrCodeMstr/listDeleted'" @click="openDeletedCodesDrawer">
							{{ t('message.hrms.codeMstr.deletedCodes') }}
						</el-button>
					</template>
					<template #empty><el-empty :image-size="160" /></template>
					<template #row_buttons="{ row }">
						<el-button icon="ele-Edit" text type="primary" v-auth="'hrCodeMstr/save'" @click="openEdit(row)" />
						<el-button icon="ele-ChatLineSquare" text type="primary" v-auth="'hrCodeMstr/saveLang'" @click="openLang(row)" />
						<el-button icon="ele-Delete" text type="danger" v-auth="'hrCodeMstr/delete'" @click="handleDelete(row)" />
					</template>
				</vxe-grid>
			</el-card>
		</template>

		<!-- 总入口：左代码类 + 右码值 -->
		<template v-else>
			<splitpanes class="default-theme">
				<pane size="36" style="display: flex; flex-direction: column">
					<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', alignItems: 'start' }">
						<el-form :inline="true" style="flex: 1" @submit.prevent>
							<el-form-item :label="t('message.hrms.codeMstr.keyword')">
								<el-input
									v-model="fieldState.keyword"
									:placeholder="t('message.hrms.codeMstr.fieldKeywordPh')"
									clearable
									@keyup.enter="loadFields"
								/>
							</el-form-item>
						</el-form>
						<el-button-group>
							<el-button type="primary" icon="ele-Search" @click="loadFields">{{ t('message.list.query') }}</el-button>
							<el-button icon="ele-Refresh" @click="resetFields">{{ t('message.list.reset') }}</el-button>
						</el-button-group>
					</el-card>
					<el-card class="full-table field-card" shadow="hover" style="margin-top: 5px; flex: 1">
						<div class="field-toolbar">
							<el-button type="primary" icon="ele-Plus" size="small" v-auth="'hrCodeMstr/saveField'" @click="openFieldEdit()">
								{{ t('message.hrms.codeMstr.addField') }}
							</el-button>
							<el-button icon="ele-Delete" size="small" v-auth="'hrCodeMstr/listDeletedField'" @click="openDeletedDrawer">
								{{ t('message.hrms.codeMstr.deletedFields') }}
							</el-button>
						</div>
						<div v-loading="fieldState.loading" class="field-list">
							<div
								v-for="item in fieldState.list"
								:key="item.codeField"
								class="field-item"
								:class="{ active: state.currentField === item.codeField }"
								@click="selectField(item)"
							>
								<div class="field-item-main">
									<div class="field-name">{{ item.fieldName }}</div>
									<div class="field-usage">{{ item.usageDesc || t('message.hrms.codeMstr.noUsage') }}</div>
									<div class="field-code">{{ item.codeField }}</div>
								</div>
								<div class="field-item-actions" @click.stop>
									<el-button icon="ele-Edit" text type="primary" v-auth="'hrCodeMstr/saveField'" @click="openFieldEdit(item)" />
									<el-button icon="ele-ChatLineSquare" text type="primary" v-auth="'hrCodeMstr/saveFieldLang'" @click="openFieldLang(item)" />
									<el-button icon="ele-Delete" text type="danger" v-auth="'hrCodeMstr/deleteField'" @click="handleDeleteField(item)" />
								</div>
							</div>
							<el-empty v-if="!fieldState.loading && fieldState.list.length === 0" :image-size="100" />
						</div>
					</el-card>
				</pane>

				<pane size="64" style="display: flex; flex-direction: column">
					<el-card shadow="hover" :body-style="{ padding: '5px 5px 0 5px', display: 'flex', width: '100%', alignItems: 'start' }">
						<div style="flex: 1; line-height: 32px; padding-left: 4px">
							<span v-if="state.currentField">
								{{ t('message.hrms.codeMstr.currentField', { name: state.currentFieldName, code: state.currentField }) }}
							</span>
							<span v-else style="color: #909399">{{ t('message.hrms.codeMstr.selectFieldHint') }}</span>
						</div>
						<el-form :model="state.queryParams" :inline="true">
							<el-form-item :label="t('message.hrms.codeMstr.keyword')">
								<el-input
									v-model="state.queryParams.keyword"
									:placeholder="t('message.hrms.codeMstr.keywordPh')"
									clearable
									style="width: 160px"
									:disabled="!state.currentField"
									@keyup.enter="handleQuery(true)"
								/>
							</el-form-item>
						</el-form>
						<el-button-group>
							<el-button type="primary" icon="ele-Search" :disabled="!state.currentField" @click="handleQuery(true)">
								{{ t('message.list.query') }}
							</el-button>
							<el-button icon="ele-Refresh" :disabled="!state.currentField" @click="resetQuery">{{ t('message.list.reset') }}</el-button>
						</el-button-group>
					</el-card>
					<el-card class="full-table" shadow="hover" style="margin-top: 5px; flex: 1">
						<vxe-grid ref="xGrid" class="xGrid-style" v-bind="options" v-on="gridEvents">
							<template #toolbar_buttons>
								<el-button
									type="primary"
									icon="ele-Plus"
									v-auth="'hrCodeMstr/save'"
									:disabled="!state.currentField"
									@click="openEdit()"
								>
									{{ t('message.list.add') }}
								</el-button>
								<el-button
									icon="ele-Delete"
									v-auth="'hrCodeMstr/listDeleted'"
									:disabled="!state.currentField"
									@click="openDeletedCodesDrawer"
								>
									{{ t('message.hrms.codeMstr.deletedCodes') }}
								</el-button>
							</template>
							<template #empty>
								<el-empty
									:image-size="120"
									:description="state.currentField ? undefined : t('message.hrms.codeMstr.selectFieldHint')"
								/>
							</template>
							<template #row_buttons="{ row }">
								<el-button icon="ele-Edit" text type="primary" v-auth="'hrCodeMstr/save'" @click="openEdit(row)" />
								<el-button icon="ele-ChatLineSquare" text type="primary" v-auth="'hrCodeMstr/saveLang'" @click="openLang(row)" />
								<el-button icon="ele-Delete" text type="danger" v-auth="'hrCodeMstr/delete'" @click="handleDelete(row)" />
							</template>
						</vxe-grid>
					</el-card>
				</pane>
			</splitpanes>
		</template>

		<!-- 码值编辑 -->
		<el-dialog
			v-model="state.editVisible"
			:title="state.edit.oldCodeValue ? t('message.hrms.codeMstr.editTitle') : t('message.hrms.codeMstr.addTitle')"
			width="480px"
			destroy-on-close
		>
			<el-form :model="state.edit" label-width="88px">
				<el-form-item :label="t('message.hrms.codeMstr.codeField')" required>
					<el-input v-model="state.edit.codeField" disabled />
				</el-form-item>
				<el-form-item :label="t('message.hrms.codeMstr.codeValue')" required><el-input v-model="state.edit.codeValue" /></el-form-item>
				<el-form-item :label="t('message.hrms.codeMstr.codeDesc')"><el-input v-model="state.edit.codeDesc" /></el-form-item>
				<el-form-item :label="t('message.hrms.codeMstr.codeSort')"><el-input v-model="state.edit.codeSort" /></el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="state.editVisible = false">{{ t('message.list.cancel') }}</el-button>
				<el-button type="primary" @click="saveEdit">{{ t('message.list.save') }}</el-button>
			</template>
		</el-dialog>

		<!-- 码值译文 -->
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

		<!-- 代码类新建/编辑 -->
		<el-dialog
			v-model="fieldState.editVisible"
			:title="fieldState.edit.isNew ? t('message.hrms.codeMstr.addField') : t('message.hrms.codeMstr.editField')"
			width="560px"
			destroy-on-close
		>
			<el-form :model="fieldState.edit" label-width="110px">
				<el-form-item :label="t('message.hrms.codeMstr.codeField')" required>
					<el-input v-model="fieldState.edit.codeField" maxlength="50" :disabled="!fieldState.edit.isNew" />
				</el-form-item>
				<el-form-item :label="t('message.hrms.codeMstr.fieldName')" required>
					<el-input v-model="fieldState.edit.fieldName" maxlength="100" :placeholder="t('message.hrms.codeMstr.masterNamePh')" />
				</el-form-item>
				<el-form-item :label="t('message.hrms.codeMstr.usageDesc')">
					<el-input v-model="fieldState.edit.usageDesc" type="textarea" maxlength="256" :rows="2" :placeholder="t('message.hrms.codeMstr.usagePh')" />
				</el-form-item>
				<el-form-item :label="t('message.hrms.codeMstr.orderNo')">
					<el-input-number v-model="fieldState.edit.orderNo" :min="0" :max="999999" />
				</el-form-item>
				<template v-if="fieldState.edit.isNew">
					<el-divider content-position="left">{{ t('message.hrms.codeMstr.multiLang') }}</el-divider>
					<div v-for="item in fieldState.editLangs" :key="item.culture" class="field-lang-block">
						<div class="field-lang-title">{{ cultureLabel(item.culture) }}</div>
						<el-form-item :label="t('message.hrms.codeMstr.fieldName')">
							<el-input v-model="item.fieldName" maxlength="100" />
						</el-form-item>
						<el-form-item :label="t('message.hrms.codeMstr.usageDesc')">
							<el-input v-model="item.usageDesc" maxlength="256" />
						</el-form-item>
					</div>
				</template>
			</el-form>
			<template #footer>
				<el-button @click="fieldState.editVisible = false">{{ t('message.list.cancel') }}</el-button>
				<el-button type="primary" :loading="fieldState.saving" @click="saveField">{{ t('message.list.save') }}</el-button>
			</template>
		</el-dialog>

		<!-- 代码类译文 -->
		<el-dialog v-model="fieldState.langVisible" :title="t('message.hrms.codeMstr.fieldLangTitle')" width="560px" destroy-on-close>
			<div v-for="item in fieldState.langs" :key="item.culture" class="field-lang-block">
				<div class="field-lang-title">{{ cultureLabel(item.culture) }}</div>
				<el-form label-width="110px">
					<el-form-item :label="t('message.hrms.codeMstr.fieldName')">
						<el-input v-model="item.fieldName" maxlength="100" />
					</el-form-item>
					<el-form-item :label="t('message.hrms.codeMstr.usageDesc')">
						<el-input v-model="item.usageDesc" maxlength="256" />
					</el-form-item>
				</el-form>
			</div>
			<template #footer>
				<el-button @click="fieldState.langVisible = false">{{ t('message.list.cancel') }}</el-button>
				<el-button type="primary" @click="saveFieldLang">{{ t('message.list.save') }}</el-button>
			</template>
		</el-dialog>

		<!-- 已删除代码类 -->
		<el-drawer v-model="fieldState.deletedVisible" :title="t('message.hrms.codeMstr.deletedFields')" size="420px" destroy-on-close>
			<div v-loading="fieldState.deletedLoading" class="deleted-list">
				<div v-for="item in fieldState.deletedList" :key="item.codeField" class="deleted-item">
					<div>
						<div class="field-name">{{ item.fieldName }}</div>
						<div class="field-usage">{{ item.usageDesc }}</div>
						<div class="field-code">{{ item.codeField }}</div>
						<div class="field-meta">
							{{ item.deleteUserName || '-' }}
							<span v-if="item.deleteTime"> · {{ item.deleteTime }}</span>
						</div>
					</div>
					<el-button type="primary" link v-auth="'hrCodeMstr/restoreField'" @click="handleRestoreField(item)">
						{{ t('message.hrms.codeMstr.restore') }}
					</el-button>
				</div>
				<el-empty v-if="!fieldState.deletedLoading && fieldState.deletedList.length === 0" :image-size="100" />
			</div>
		</el-drawer>

		<!-- 已删除码值 -->
		<el-drawer v-model="state.deletedCodesVisible" :title="t('message.hrms.codeMstr.deletedCodes')" size="420px" destroy-on-close>
			<div v-loading="state.deletedCodesLoading" class="deleted-list">
				<div v-for="item in state.deletedCodesList" :key="`${item.codeField}:${item.codeValue}`" class="deleted-item">
					<div>
						<div class="field-name">{{ item.codeValue }} · {{ item.codeDesc || '-' }}</div>
						<div class="field-code">{{ item.codeField }}</div>
						<div class="field-meta">
							{{ item.deleteUserName || '-' }}
							<span v-if="item.deleteTime"> · {{ item.deleteTime }}</span>
						</div>
					</div>
					<el-button type="primary" link v-auth="'hrCodeMstr/restore'" @click="handleRestoreCode(item)">
						{{ t('message.hrms.codeMstr.restore') }}
					</el-button>
				</div>
				<el-empty v-if="!state.deletedCodesLoading && state.deletedCodesList.length === 0" :image-size="100" />
			</div>
		</el-drawer>
	</div>
</template>

<script lang="ts" setup name="hrCodeMstr">
import { computed, onMounted, reactive, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage, ElMessageBox } from 'element-plus';
import { VxeGridInstance, VxeGridListeners, VxeGridPropTypes } from 'vxe-table';
import { Splitpanes, Pane } from 'splitpanes';
import 'splitpanes/dist/splitpanes.css';
import { useVxeTable } from '/@/hooks/useVxeTableOptionsHook';
import { Local } from '/@/utils/storage';
import {
	deleteHrCodeField,
	deleteHrCodeMstr,
	fieldLangListHrCodeField,
	getHrCodeField,
	langListHrCodeMstr,
	listDeletedHrCodeField,
	listDeletedHrCodeMstr,
	listHrCodeField,
	pageHrCodeMstr,
	restoreHrCodeField,
	restoreHrCodeMstr,
	saveFieldLangHrCodeField,
	saveHrCodeField,
	saveHrCodeMstr,
	saveLangHrCodeMstr,
} from '/@/api/hrms/hrCodeMstr';
import { hrUiCultures } from '/@/views/hrms/composables/hrUiCulture';

const { t, locale } = useI18n();
const props = defineProps<{ presetCodeField?: string; embedded?: boolean }>();
const embedded = props.embedded;
const lockedCodeField = computed(() => String(props.presetCodeField || '').trim());
const xGrid = ref<VxeGridInstance>();
const localPageParamKey = computed(() =>
	lockedCodeField.value ? `localPageParam:hrCodeMstr:${lockedCodeField.value}` : 'localPageParam:hrCodeMstr'
);

const state = reactive({
	queryParams: { codeField: props.presetCodeField || '', keyword: '' },
	localPageParam: { pageSize: 50 as number, defaultSort: { field: 'codeValue', order: 'asc', descStr: 'desc' } },
	editVisible: false,
	langVisible: false,
	edit: {} as any,
	langs: [] as { culture: string; label: string }[],
	langRow: null as any,
	currentField: '' as string,
	currentFieldName: '' as string,
	deletedCodesVisible: false,
	deletedCodesLoading: false,
	deletedCodesList: [] as any[],
});

const fieldState = reactive({
	keyword: '',
	loading: false,
	list: [] as any[],
	editVisible: false,
	saving: false,
	edit: { isNew: true, codeField: '', fieldName: '', usageDesc: '', orderNo: 100 } as any,
	editLangs: [] as { culture: string; fieldName: string; usageDesc: string }[],
	langVisible: false,
	langs: [] as { culture: string; fieldName: string; usageDesc: string }[],
	langCodeField: '',
	deletedVisible: false,
	deletedLoading: false,
	deletedList: [] as any[],
});

const cultureLabel = (culture: string) => hrUiCultures.find((c) => c.value === culture)?.label || culture;

const buildColumns = () => {
	const cols: any[] = [{ field: 'seq', type: 'seq', title: t('message.list.seq'), width: 60 }];
	if (lockedCodeField.value) {
		/* embedded: no codeField column */
	}
	cols.push(
		{ field: 'codeValue', title: t('message.hrms.codeMstr.codeValue'), minWidth: 120, sortable: true },
		{ field: 'codeDesc', title: t('message.hrms.codeMstr.codeDesc'), minWidth: 160, sortable: true },
		{ field: 'codeSort', title: t('message.hrms.codeMstr.sortCode'), width: 100, sortable: true },
		{ field: 'buttons', title: t('message.list.operation'), width: 140, fixed: 'right', slots: { default: 'row_buttons' } }
	);
	return cols;
};

const options = useVxeTable(
	{
		id: 'hrCodeMstr',
		name: t('message.hrms.codeMstr.moduleName'),
		columns: buildColumns(),
	},
	{
		height: embedded || !lockedCodeField.value ? '100%' : 'auto',
		proxyConfig: {
			autoLoad: !!lockedCodeField.value,
			ajax: {
				query: ({ page, sort }) => {
					if (!lockedCodeField.value && !state.currentField) {
						return Promise.resolve({ data: { result: { items: [], total: 0 } } } as any);
					}
					return handleQueryApi(page, sort);
				},
			},
		},
		sortConfig: { defaultSort: Local.get(localPageParamKey.value)?.defaultSort || state.localPageParam.defaultSort },
		pagerConfig: { pageSize: Local.get(localPageParamKey.value)?.pageSize || state.localPageParam.pageSize },
	}
);

const handleQueryApi = (page: VxeGridPropTypes.ProxyAjaxQueryPageParams, sort: VxeGridPropTypes.ProxyAjaxQuerySortCheckedParams) => {
	const effective = sort?.field ? sort : state.localPageParam.defaultSort;
	const codeField = lockedCodeField.value || state.currentField || state.queryParams.codeField;
	return pageHrCodeMstr({
		codeField: codeField || undefined,
		keyword: state.queryParams.keyword || undefined,
		page: page.currentPage,
		pageSize: page.pageSize,
		field: effective.field,
		order: effective.order,
		descStr: 'desc',
	});
};

const handleQuery = async (reset = false) => {
	if (!lockedCodeField.value && !state.currentField) return;
	reset ? await xGrid.value?.commitProxy('reload') : await xGrid.value?.commitProxy('query');
};
const resetQuery = () => {
	state.queryParams.keyword = '';
	handleQuery(true);
};

const loadFields = async () => {
	fieldState.loading = true;
	try {
		const res = await listHrCodeField(fieldState.keyword);
		fieldState.list = res.data.result ?? [];
		if (state.currentField) {
			const still = fieldState.list.find((x) => x.codeField === state.currentField);
			if (!still) {
				state.currentField = '';
				state.currentFieldName = '';
				state.queryParams.codeField = '';
			} else {
				state.currentFieldName = still.fieldName;
			}
		}
		if (!state.currentField && fieldState.list.length > 0) {
			selectField(fieldState.list[0]);
		}
	} finally {
		fieldState.loading = false;
	}
};
const resetFields = () => {
	fieldState.keyword = '';
	loadFields();
};
const selectField = (item: any) => {
	state.currentField = item.codeField;
	state.currentFieldName = item.fieldName;
	state.queryParams.codeField = item.codeField;
	state.queryParams.keyword = '';
	handleQuery(true);
};

const openFieldEdit = async (row?: any) => {
	if (row) {
		const res = await getHrCodeField(row.codeField);
		const master = res.data.result ?? row;
		fieldState.edit = {
			isNew: false,
			codeField: master.codeField,
			fieldName: master.fieldName,
			usageDesc: master.usageDesc || '',
			orderNo: master.orderNo ?? 100,
		};
		fieldState.editLangs = [];
	} else {
		fieldState.edit = { isNew: true, codeField: '', fieldName: '', usageDesc: '', orderNo: 100 };
		fieldState.editLangs = hrUiCultures
			.filter((c) => c.value !== 'zh-CN')
			.map((c) => ({ culture: c.value, fieldName: '', usageDesc: '' }));
	}
	fieldState.editVisible = true;
};
const saveField = async () => {
	if (!fieldState.edit.codeField?.trim() || !fieldState.edit.fieldName?.trim()) {
		ElMessage.warning(t('message.hrms.codeMstr.fieldRequired'));
		return;
	}
	fieldState.saving = true;
	try {
		await saveHrCodeField({
			isNew: fieldState.edit.isNew,
			codeField: fieldState.edit.codeField.trim(),
			fieldName: fieldState.edit.fieldName.trim(),
			usageDesc: fieldState.edit.usageDesc?.trim(),
			orderNo: fieldState.edit.orderNo ?? 100,
			langs: fieldState.edit.isNew ? fieldState.editLangs : [],
		});
		ElMessage.success(t('message.hrms.common.saved'));
		fieldState.editVisible = false;
		const created = fieldState.edit.codeField.trim();
		await loadFields();
		const hit = fieldState.list.find((x) => x.codeField === created);
		if (hit) selectField(hit);
	} finally {
		fieldState.saving = false;
	}
};
const handleDeleteField = (row: any) => {
	ElMessageBox.confirm(t('message.hrms.codeMstr.confirmDeleteField', { name: row.fieldName, code: row.codeField }), t('message.list.hint'), {
		type: 'warning',
	})
		.then(async () => {
			await deleteHrCodeField({ codeField: row.codeField });
			ElMessage.success(t('message.hrms.common.deleted'));
			if (state.currentField === row.codeField) {
				state.currentField = '';
				state.currentFieldName = '';
				state.queryParams.codeField = '';
			}
			await loadFields();
		})
		.catch(() => {});
};
const openFieldLang = async (row: any) => {
	fieldState.langCodeField = row.codeField;
	const res = await fieldLangListHrCodeField(row.codeField);
	const map = new Map((res.data.result ?? []).map((x: any) => [x.culture, x]));
	fieldState.langs = hrUiCultures.map((c) => {
		const hit = map.get(c.value);
		return { culture: c.value, fieldName: hit?.fieldName || '', usageDesc: hit?.usageDesc || '' };
	});
	fieldState.langVisible = true;
};
const saveFieldLang = async () => {
	await saveFieldLangHrCodeField({ codeField: fieldState.langCodeField, langs: fieldState.langs });
	ElMessage.success(t('message.hrms.codeMstr.langSaved'));
	fieldState.langVisible = false;
	await loadFields();
};
const openDeletedDrawer = async () => {
	fieldState.deletedVisible = true;
	fieldState.deletedLoading = true;
	try {
		const res = await listDeletedHrCodeField();
		fieldState.deletedList = res.data.result ?? [];
	} finally {
		fieldState.deletedLoading = false;
	}
};
const handleRestoreField = async (row: any) => {
	await restoreHrCodeField({ codeField: row.codeField });
	ElMessage.success(t('message.hrms.codeMstr.restored'));
	await openDeletedDrawer();
	await loadFields();
	const hit = fieldState.list.find((x) => x.codeField === row.codeField);
	if (hit) selectField(hit);
};

const openEdit = (row?: any) => {
	const field = lockedCodeField.value || state.currentField || state.queryParams.codeField;
	if (!field) {
		ElMessage.warning(t('message.hrms.codeMstr.selectFieldHint'));
		return;
	}
	state.edit = row
		? { codeField: row.codeField, codeValue: row.codeValue, oldCodeValue: row.codeValue, codeDesc: row.codeDesc, codeSort: row.codeSort }
		: { codeField: field, codeValue: '', codeDesc: '', codeSort: '' };
	state.editVisible = true;
};
const saveEdit = async () => {
	if (lockedCodeField.value) state.edit.codeField = lockedCodeField.value;
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
	ElMessageBox.confirm(t('message.hrms.codeMstr.confirmDelete', { codeField: row.codeField, codeValue: row.codeValue }), t('message.list.hint'), {
		type: 'warning',
	})
		.then(async () => {
			await deleteHrCodeMstr({ codeField: row.codeField, codeValue: row.codeValue });
			ElMessage.success(t('message.hrms.common.deleted'));
			handleQuery(false);
		})
		.catch(() => {});
};
const openDeletedCodesDrawer = async () => {
	const field = lockedCodeField.value || state.currentField;
	if (!field) {
		ElMessage.warning(t('message.hrms.codeMstr.selectFieldHint'));
		return;
	}
	state.deletedCodesVisible = true;
	state.deletedCodesLoading = true;
	try {
		const res = await listDeletedHrCodeMstr(field);
		state.deletedCodesList = res.data.result ?? [];
	} finally {
		state.deletedCodesLoading = false;
	}
};
const handleRestoreCode = async (row: any) => {
	await restoreHrCodeMstr({ codeField: row.codeField, codeValue: row.codeValue });
	ElMessage.success(t('message.hrms.codeMstr.restored'));
	await openDeletedCodesDrawer();
	handleQuery(false);
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
		Local.set(localPageParamKey.value, state.localPageParam);
	},
	async sortChange({ field, order }) {
		if (!field || !order) return;
		state.localPageParam.defaultSort = { field, order, descStr: 'desc' };
		Local.set(localPageParamKey.value, state.localPageParam);
	},
};

watch(locale, () => {
	options.name = t('message.hrms.codeMstr.moduleName');
	const cols = buildColumns();
	options.columns = cols;
	xGrid.value?.reloadColumn?.(cols);
	if (!lockedCodeField.value) loadFields();
});

onMounted(() => {
	state.localPageParam = Local.get(localPageParamKey.value) || state.localPageParam;
	if (lockedCodeField.value) state.queryParams.codeField = lockedCodeField.value;
	else loadFields();
});
</script>

<style scoped>
.hr-code-mstr {
	height: 100%;
	min-height: calc(100vh - 120px);
}
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
.hr-code-mstr :deep(.splitpanes) {
	height: 100%;
}
.hr-code-mstr :deep(.splitpanes__pane) {
	min-height: 0;
}
.field-card :deep(.el-card__body) {
	display: flex;
	flex-direction: column;
	height: 100%;
	min-height: 0;
	padding: 8px;
}
.field-toolbar {
	display: flex;
	gap: 8px;
	margin-bottom: 8px;
	flex-shrink: 0;
}
.field-list {
	flex: 1;
	min-height: 0;
	overflow: auto;
}
.field-item {
	display: flex;
	align-items: flex-start;
	justify-content: space-between;
	gap: 8px;
	padding: 10px 8px;
	border-radius: 6px;
	cursor: pointer;
	border: 1px solid transparent;
}
.field-item:hover {
	background: var(--el-fill-color-light);
}
.field-item.active {
	background: var(--el-color-primary-light-9);
	border-color: var(--el-color-primary-light-5);
}
.field-name {
	font-weight: 600;
	font-size: 14px;
	line-height: 1.3;
}
.field-usage {
	margin-top: 4px;
	color: var(--el-text-color-secondary);
	font-size: 12px;
	line-height: 1.4;
}
.field-code {
	margin-top: 4px;
	color: var(--el-text-color-placeholder);
	font-size: 12px;
	font-family: ui-monospace, Consolas, monospace;
}
.field-item-actions {
	flex-shrink: 0;
	display: flex;
}
.field-lang-block {
	margin-bottom: 8px;
	padding-bottom: 4px;
	border-bottom: 1px dashed var(--el-border-color-lighter);
}
.field-lang-title {
	font-weight: 600;
	margin: 8px 0;
}
.deleted-list {
	padding: 0 4px;
}
.deleted-item {
	display: flex;
	justify-content: space-between;
	align-items: flex-start;
	gap: 12px;
	padding: 12px 0;
	border-bottom: 1px solid var(--el-border-color-lighter);
}
.field-meta {
	margin-top: 6px;
	font-size: 12px;
	color: var(--el-text-color-placeholder);
}
</style>
