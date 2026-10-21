<template>
	<div>
		<div class="child-toolbar">
			<span class="child-hint">{{ t('message.hrms.emp.dragHint') }}</span>
			<el-button type="primary" icon="ele-Plus" size="small" @click="openDrawer()">{{ t('message.hrms.emp.addRow') }}</el-button>
		</div>
		<el-table v-if="state.rows.length" ref="tableRef" :data="state.rows" row-key="id" size="small" border @row-click="onRowClick">
			<el-table-column width="42" align="center">
				<template #default>
					<el-icon class="drag-handle"><ele-Rank /></el-icon>
				</template>
			</el-table-column>
			<el-table-column v-for="col in columns" :key="col.key" :prop="col.key" :label="t(col.label)" min-width="110" show-overflow-tooltip />
		</el-table>
		<el-empty v-else :description="t('message.hrms.emp.emptyChild')">
			<el-button type="primary" link @click="openDrawer()">{{ t('message.hrms.emp.addFirst') }}</el-button>
		</el-empty>

		<el-drawer v-model="state.drawer" :title="drawerTitle" size="420px" append-to-body destroy-on-close>
			<el-form label-width="108px" size="default">
				<el-form-item v-for="col in fields" :key="col.key" :label="t(col.label)" :required="col.required">
					<el-date-picker v-if="col.type === 'date'" v-model="state.form[col.key]" type="date" value-format="YYYY-MM-DD" class="w100" clearable />
					<el-select v-else-if="col.type === 'yesno'" v-model="state.form[col.key]" class="w100">
						<el-option :label="t('message.hrms.common.yes')" value="是" />
						<el-option :label="t('message.hrms.common.no')" value="否" />
					</el-select>
					<el-input v-else-if="col.type === 'textarea'" v-model="state.form[col.key]" type="textarea" :rows="3" />
					<el-input v-else v-model="state.form[col.key]" clearable />
				</el-form-item>
			</el-form>
			<template #footer>
				<div class="drawer-footer">
					<el-button v-if="state.editingId" type="danger" link @click="removeRow">{{ t('message.hrms.emp.deleteRow') }}</el-button>
					<span class="drawer-footer-gap" />
					<el-button @click="state.drawer = false">{{ t('message.hrms.emp.cancel') }}</el-button>
					<el-button type="primary" @click="saveRow">{{ t('message.hrms.emp.saveRow') }}</el-button>
				</div>
			</template>
		</el-drawer>
	</div>
</template>

<script lang="ts" setup>
import { computed, nextTick, onBeforeUnmount, reactive, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage } from 'element-plus';
import Sortable from 'sortablejs';
import { deleteHrEmpChild, listHrEmpChild, saveHrEmpChild, sortHrEmpChild } from '/@/api/hrms/hrEmpChild';

type Col = { key: string; label: string; type?: 'text' | 'date' | 'yesno' | 'textarea'; required?: boolean; list?: boolean };

const catalogs: Record<string, { title: string; fields: Col[] }> = {
	family: {
		title: 'message.hrms.emp.sectionFamily',
		fields: [
			{ key: 'relation', label: 'message.hrms.emp.colRelation', list: true },
			{ key: 'name', label: 'message.hrms.emp.colName', required: true, list: true },
			{ key: 'workplace', label: 'message.hrms.emp.colWorkplace', list: true },
			{ key: 'duty', label: 'message.hrms.emp.colDuty', list: true },
		],
	},
	edu: {
		title: 'message.hrms.emp.sectionEdu',
		fields: [
			{ key: 'startDate', label: 'message.hrms.emp.colEnroll', type: 'date', list: true },
			{ key: 'endDate', label: 'message.hrms.emp.colGraduate', type: 'date', list: true },
			{ key: 'school', label: 'message.hrms.emp.colSchool', required: true, list: true },
			{ key: 'major', label: 'message.hrms.emp.colMajor', list: true },
			{ key: 'degree', label: 'message.hrms.emp.colDegree', list: true },
			{ key: 'highest', label: 'message.hrms.emp.colHighest', type: 'yesno', list: true },
			{ key: 'remark', label: 'message.hrms.emp.colRemark', type: 'textarea' },
		],
	},
	work: {
		title: 'message.hrms.emp.sectionWork',
		fields: [
			{ key: 'startDate', label: 'message.hrms.emp.colStart', type: 'date', list: true },
			{ key: 'endDate', label: 'message.hrms.emp.colEnd', type: 'date', list: true },
			{ key: 'workplace', label: 'message.hrms.emp.colWorkplace', required: true, list: true },
			{ key: 'deptName', label: 'message.hrms.emp.colDept', list: true },
			{ key: 'postName', label: 'message.hrms.emp.colPost', list: true },
			{ key: 'remark', label: 'message.hrms.emp.colRemark', type: 'textarea' },
		],
	},
	title: {
		title: 'message.hrms.emp.sectionTitle',
		fields: [
			{ key: 'titleName', label: 'message.hrms.emp.colTitleName', required: true, list: true },
			{ key: 'titleLevel', label: 'message.hrms.emp.colTitleLevel', list: true },
			{ key: 'certNo', label: 'message.hrms.emp.colCertNo', list: true },
			{ key: 'obtainDate', label: 'message.hrms.emp.colObtainDate', type: 'date', list: true },
			{ key: 'remark', label: 'message.hrms.emp.colRemark', type: 'textarea' },
		],
	},
};

const props = defineProps<{ section: string; empId: string }>();
const emit = defineEmits<{ changed: [] }>();
const { t } = useI18n();
const tableRef = ref();
const state = reactive({
	rows: [] as Record<string, string>[],
	drawer: false,
	editingId: '' as string,
	form: {} as Record<string, string>,
	sorting: false,
});
let sortable: Sortable | null = null;

const spec = computed(() => catalogs[props.section]);
const fields = computed(() => spec.value?.fields ?? []);
const columns = computed(() => fields.value.filter((f) => f.list));
const drawerTitle = computed(() => `${state.editingId ? t('message.hrms.emp.editRow') : t('message.hrms.emp.addRow')} · ${t(spec.value?.title || '')}`);

const load = async () => {
	if (!props.empId || !spec.value) {
		state.rows = [];
		return;
	}
	const res = await listHrEmpChild(props.section, props.empId);
	state.rows = (res.data.result ?? []).map((row: Record<string, string>) => ({ ...row, id: String(row.id) }));
	await nextTick();
	bindSortable();
};

const bindSortable = () => {
	sortable?.destroy();
	sortable = null;
	const tbody = tableRef.value?.$el?.querySelector('.el-table__body-wrapper tbody');
	if (!tbody) return;
	sortable = Sortable.create(tbody, {
		handle: '.drag-handle',
		animation: 150,
		onEnd: async (evt) => {
			const oldIndex = evt.oldIndex ?? -1;
			const newIndex = evt.newIndex ?? -1;
			if (oldIndex < 0 || newIndex < 0 || oldIndex === newIndex || state.sorting) return;
			const next = state.rows.slice();
			const [moved] = next.splice(oldIndex, 1);
			next.splice(newIndex, 0, moved);
			state.rows = next;
			state.sorting = true;
			try {
				await sortHrEmpChild({ section: props.section, empId: props.empId, ids: next.map((r) => r.id) });
			} catch {
				await load();
			} finally {
				state.sorting = false;
			}
		},
	});
};

const openDrawer = (row?: Record<string, string>) => {
	state.editingId = row?.id || '';
	const form: Record<string, string> = {};
	for (const col of fields.value) form[col.key] = row?.[col.key] ?? (col.type === 'yesno' ? '否' : '');
	state.form = form;
	state.drawer = true;
};

const onRowClick = (row: Record<string, string>, _column: unknown, event: MouseEvent) => {
	if ((event.target as HTMLElement)?.closest('.drag-handle')) return;
	openDrawer(row);
};

const saveRow = async () => {
	const required = fields.value.find((f) => f.required && !String(state.form[f.key] || '').trim());
	if (required) {
		ElMessage.warning(t(required.label) + t('message.hrms.common.requiredSuffix'));
		return;
	}
	await saveHrEmpChild({
		section: props.section,
		empId: props.empId,
		id: state.editingId || undefined,
		values: { ...state.form },
	});
	state.drawer = false;
	ElMessage.success(t('message.hrms.emp.rowSaved'));
	await load();
	emit('changed');
};

const removeRow = async () => {
	if (!state.editingId) return;
	await deleteHrEmpChild({ section: props.section, empId: props.empId, id: state.editingId });
	state.drawer = false;
	ElMessage.success(t('message.hrms.common.deleted'));
	await load();
	emit('changed');
};

watch(() => [props.section, props.empId], load, { immediate: true });
onBeforeUnmount(() => sortable?.destroy());
</script>

<style scoped>
.child-toolbar {
	display: flex;
	align-items: center;
	justify-content: space-between;
	margin-bottom: 8px;
}
.child-hint {
	font-size: 12px;
	color: var(--el-text-color-secondary);
}
.drag-handle {
	cursor: grab;
	color: var(--el-text-color-placeholder);
}
.w100 {
	width: 100%;
}
.drawer-footer {
	display: flex;
	align-items: center;
}
.drawer-footer-gap {
	flex: 1;
}
</style>
