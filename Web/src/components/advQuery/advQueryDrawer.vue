<template>
	<el-drawer v-model="state.visible" :title="t('message.hrms.advQuery.title')" size="860px" destroy-on-close @closed="onClosed">
		<div class="adv-toolbar">
			<el-select
				v-model="state.schemeId"
				clearable
				filterable
				:placeholder="t('message.hrms.advQuery.schemePlaceholder')"
				style="width: 220px"
				@change="onSchemeChange"
			>
				<el-option v-for="s in state.schemes" :key="s.id" :label="schemeLabel(s)" :value="s.id" />
			</el-select>
			<el-button v-auth="'hrAdvQueryScheme/save'" @click="openSave">{{ t('message.hrms.advQuery.saveScheme') }}</el-button>
			<el-button v-if="currentIsOwner" v-auth="'hrAdvQueryScheme/delete'" type="danger" plain @click="doDeleteScheme">
				{{ t('message.hrms.advQuery.deleteScheme') }}
			</el-button>
		</div>

		<el-alert v-if="staleFields.length" type="warning" :closable="false" show-icon class="mb10" :title="staleTip" />

		<div class="adv-group-root">
			<FilterGroupEditor v-model="state.root" :fields="fields" :depth="0" />
		</div>

		<template #footer>
			<div class="adv-footer">
				<el-button @click="state.visible = false">{{ t('message.hrms.advQuery.close') }}</el-button>
				<el-button @click="resetTree">{{ t('message.hrms.advQuery.reset') }}</el-button>
				<el-button type="primary" @click="doSearch">{{ t('message.hrms.advQuery.search') }}</el-button>
			</div>
		</template>

		<el-dialog v-model="state.saveVisible" :title="t('message.hrms.advQuery.saveScheme')" width="480px" append-to-body destroy-on-close>
			<el-form label-width="96px">
				<el-form-item :label="t('message.hrms.advQuery.schemeName')">
					<el-input v-model="state.saveName" maxlength="100" />
				</el-form-item>
				<el-form-item :label="t('message.hrms.advQuery.share')">
					<el-switch v-model="state.saveShared" />
				</el-form-item>
				<el-form-item v-if="state.saveShared" :label="t('message.hrms.advQuery.shareRoles')">
					<el-select v-model="state.saveRoleIds" multiple filterable style="width: 100%">
						<el-option v-for="r in myRoles" :key="r.id" :label="r.name" :value="r.id" />
					</el-select>
				</el-form-item>
			</el-form>
			<template #footer>
				<el-button @click="state.saveVisible = false">{{ t('message.hrms.advQuery.close') }}</el-button>
				<el-button type="primary" :loading="state.saving" @click="doSave">{{ t('message.hrms.advQuery.saveScheme') }}</el-button>
			</template>
		</el-dialog>
	</el-drawer>
</template>

<script lang="ts" setup>
import { computed, defineComponent, h, reactive, resolveComponent, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { ElMessage, ElMessageBox } from 'element-plus';
import { storeToRefs } from 'pinia';
import { useUserInfo } from '/@/stores/userInfo';
import { deleteHrAdvQueryScheme, listHrAdvQueryScheme, saveHrAdvQueryScheme } from '/@/api/hrms/hrAdvQueryScheme';
import {
	AdvFilterNode,
	AdvQueryField,
	collectStale,
	defaultOperators,
	emptyGroup,
	emptyLeaf,
	markStale,
	sanitizeForSubmit,
} from './types';

const props = defineProps<{
	moduleCode: string;
	fields: AdvQueryField[];
}>();

const emits = defineEmits<{
	search: [filter: AdvFilterNode | null];
	'update:activeCount': [n: number];
}>();

const { t } = useI18n();
const { userInfos } = storeToRefs(useUserInfo());

const state = reactive({
	visible: false,
	root: emptyGroup('And') as AdvFilterNode,
	schemes: [] as any[],
	schemeId: undefined as number | undefined,
	saveVisible: false,
	saveName: '',
	saveShared: false,
	saveRoleIds: [] as number[],
	saving: false,
});

const fieldSet = computed(() => new Set((props.fields || []).map((f) => f.field)));
const staleFields = computed(() => collectStale(state.root));
const staleTip = computed(() => t('message.hrms.advQuery.staleTip', { fields: staleFields.value.join(', ') }));
const currentIsOwner = computed(() => {
	const s = state.schemes.find((x) => x.id === state.schemeId);
	return !!s?.isOwner;
});
const myRoles = computed(() => {
	const roles = (userInfos.value as any)?.roles || [];
	return (Array.isArray(roles) ? roles : []).map((r: any) => ({
		id: Number(r.id ?? r.roleId ?? r),
		name: String(r.name ?? r.roleName ?? r.id ?? r),
	})).filter((r: any) => r.id);
});

const countLeaves = (n: AdvFilterNode | null | undefined): number => {
	if (!n) return 0;
	if (n.filters?.length) return n.filters.reduce((s, c) => s + countLeaves(c), 0);
	return n.field ? 1 : 0;
};

watch(
	() => state.root,
	() => emits('update:activeCount', countLeaves(sanitizeForSubmit(state.root))),
	{ deep: true }
);

const schemeLabel = (s: any) => (s.isOwner ? s.name : `${s.name} (${t('message.hrms.advQuery.shared')})`);

const loadSchemes = async () => {
	const res = await listHrAdvQueryScheme(props.moduleCode);
	state.schemes = res.data?.result ?? res.data ?? [];
};

const open = async (filter?: AdvFilterNode | null) => {
	state.visible = true;
	await loadSchemes();
	if (filter) state.root = markStale(JSON.parse(JSON.stringify(filter)), fieldSet.value) || emptyGroup('And');
	else if (!state.root?.filters?.length) state.root = emptyGroup('And');
};

const resetTree = () => {
	state.root = emptyGroup('And');
	state.schemeId = undefined;
};

const onSchemeChange = (id?: number) => {
	if (!id) return;
	const s = state.schemes.find((x) => x.id === id);
	if (!s?.filterJson) return;
	try {
		const parsed = JSON.parse(s.filterJson);
		state.root = markStale(parsed, fieldSet.value) || emptyGroup('And');
	} catch {
		ElMessage.error(t('message.hrms.advQuery.schemeInvalid'));
	}
};

const doSearch = () => {
	if (staleFields.value.length) {
		ElMessage.warning(t('message.hrms.advQuery.fixStaleFirst'));
		return;
	}
	const cleaned = sanitizeForSubmit(state.root);
	emits('search', cleaned);
	emits('update:activeCount', countLeaves(cleaned));
	state.visible = false;
};

const openSave = () => {
	if (staleFields.value.length) {
		ElMessage.warning(t('message.hrms.advQuery.fixStaleFirst'));
		return;
	}
	const cleaned = sanitizeForSubmit(state.root);
	if (!cleaned) {
		ElMessage.warning(t('message.hrms.advQuery.emptyFilter'));
		return;
	}
	const cur = state.schemes.find((x) => x.id === state.schemeId);
	state.saveName = cur?.isOwner ? cur.name : '';
	state.saveShared = !!cur?.isShared;
	state.saveRoleIds = cur?.shareRoleIds ? [...cur.shareRoleIds] : myRoles.value.map((r) => r.id);
	state.saveVisible = true;
};

const doSave = async () => {
	if (!state.saveName.trim()) {
		ElMessage.warning(t('message.hrms.advQuery.schemeNameRequired'));
		return;
	}
	const cleaned = sanitizeForSubmit(state.root);
	if (!cleaned) return;
	state.saving = true;
	try {
		const cur = state.schemes.find((x) => x.id === state.schemeId);
		const res = await saveHrAdvQueryScheme({
			id: cur?.isOwner ? cur.id : undefined,
			moduleCode: props.moduleCode,
			name: state.saveName.trim(),
			filterJson: JSON.stringify(cleaned),
			isShared: state.saveShared,
			shareRoleIds: state.saveShared ? state.saveRoleIds : [],
		});
		const id = res.data?.result ?? res.data;
		ElMessage.success(t('message.hrms.common.saved'));
		state.saveVisible = false;
		await loadSchemes();
		if (typeof id === 'number') state.schemeId = id;
	} finally {
		state.saving = false;
	}
};

const doDeleteScheme = async () => {
	if (!state.schemeId || !currentIsOwner.value) return;
	await ElMessageBox.confirm(t('message.hrms.advQuery.confirmDelete'), t('message.list.hint'), { type: 'warning' });
	await deleteHrAdvQueryScheme(state.schemeId);
	ElMessage.success(t('message.hrms.common.deleted'));
	state.schemeId = undefined;
	await loadSchemes();
};

const onClosed = () => {
	/* keep tree for reopen */
};

/** 行节点稳定 key，避免点「添加」后 el-select 复用错显上一行的值 */
type RowNode = AdvFilterNode & { _rowKey?: string };
let rowKeySeq = 1;
const ensureRowKey = (n: AdvFilterNode): string => {
	const row = n as RowNode;
	if (!row._rowKey) row._rowKey = `r${rowKeySeq++}`;
	return row._rowKey;
};
/** 有子条件才当括号组；空白 leaf 绝不能带 filters */
const isGroupNode = (n: AdvFilterNode) => Array.isArray(n.filters) && (n.filters?.length ?? 0) > 0;

/** 行式编辑：字段 | 运算符 | 值 | 并且/或者 | 添加 | 删除（对齐截图一） */
const FilterGroupEditor = defineComponent({
	name: 'FilterGroupEditor',
	props: {
		modelValue: { type: Object as () => AdvFilterNode, required: true },
		fields: { type: Array as () => AdvQueryField[], required: true },
		depth: { type: Number, default: 0 },
	},
	emits: ['update:modelValue'],
	setup(p, { emit }) {
		const ElSelect = resolveComponent('el-select');
		const ElOption = resolveComponent('el-option');
		const ElInput = resolveComponent('el-input');
		const ElInputNumber = resolveComponent('el-input-number');
		const ElDatePicker = resolveComponent('el-date-picker');
		const ElButton = resolveComponent('el-button');
		const ElTag = resolveComponent('el-tag');

		const update = (patch: Partial<AdvFilterNode>) => emit('update:modelValue', { ...p.modelValue, ...patch });

		/** 在指定行后插入空白条件行（不克隆上一行） */
		const addLeafAfter = (idx: number) => {
			const leaf = emptyLeaf();
			ensureRowKey(leaf);
			const filters = [...(p.modelValue.filters || [])];
			filters.splice(idx + 1, 0, leaf);
			update({ filters });
		};
		const addGroup = () => {
			if (p.depth >= 2) {
				ElMessage.warning(t('message.hrms.advQuery.maxDepth'));
				return;
			}
			const g = emptyGroup('And');
			ensureRowKey(g);
			(g.filters || []).forEach(ensureRowKey);
			const filters = [...(p.modelValue.filters || []), g];
			update({ filters });
		};
		const removeAt = (idx: number) => {
			const filters = (p.modelValue.filters || []).filter((_, i) => i !== idx);
			if (!filters.length) {
				const leaf = emptyLeaf();
				ensureRowKey(leaf);
				update({ filters: [leaf] });
				return;
			}
			update({ filters });
		};
		const setChild = (idx: number, child: AdvFilterNode) => {
			ensureRowKey(child);
			const filters = [...(p.modelValue.filters || [])];
			filters[idx] = child;
			update({ filters });
		};

		const opLabel = (op: string) => t(`message.hrms.advQuery.op.${op}` as any) || op;

		return () => {
			const node = p.modelValue;
			const kids = node.filters || [];
			kids.forEach(ensureRowKey);
			return h('div', { class: ['adv-group', p.depth > 0 ? 'is-nested' : ''], 'data-depth': p.depth }, [
				// 顶部仅「添加括号组」（截图一）
				h('div', { class: 'adv-group-head' }, [
					h(ElButton as any, { type: 'primary', link: true, onClick: addGroup }, () => t('message.hrms.advQuery.addGroup')),
				]),
				...kids.map((child, idx) => {
					const rowKey = ensureRowKey(child);
					if (isGroupNode(child)) {
						return h('div', { class: 'adv-child-group', key: rowKey }, [
							h(FilterGroupEditor, {
								modelValue: child,
								fields: p.fields,
								depth: p.depth + 1,
								'onUpdate:modelValue': (v: AdvFilterNode) => setChild(idx, v),
							}),
							h('div', { class: 'adv-child-group-actions' }, [
								h(ElSelect as any, {
									modelValue: child.logic || 'And',
									style: 'width: 88px',
									'onUpdate:modelValue': (v: string) => setChild(idx, { ...child, logic: v as any }),
								}, () => [
									h(ElOption as any, { label: t('message.hrms.advQuery.and'), value: 'And' }),
									h(ElOption as any, { label: t('message.hrms.advQuery.or'), value: 'Or' }),
								]),
								h(ElButton as any, { type: 'primary', link: true, onClick: () => addLeafAfter(idx) }, () => t('message.hrms.advQuery.add')),
								h(ElButton as any, { type: 'danger', link: true, onClick: () => removeAt(idx) }, () => t('message.hrms.advQuery.remove')),
							]),
						]);
					}
					const meta = (p.fields || []).find((f) => f.field === child.field);
					const ops = meta?.operators?.length ? meta.operators : defaultOperators(meta?.valueType || 'string');
					const valueNode = (() => {
						const vt = meta?.valueType || 'string';
						if (vt === 'number') {
							return h(ElInputNumber as any, {
								modelValue: child.value as number,
								controls: false,
								style: 'width: 160px',
								'onUpdate:modelValue': (v: any) => setChild(idx, { ...child, value: v }),
							});
						}
						if (vt === 'date' || vt === 'datetime') {
							return h(ElDatePicker as any, {
								modelValue: child.value,
								type: vt === 'datetime' ? 'datetime' : 'date',
								valueFormat: vt === 'datetime' ? 'YYYY-MM-DD HH:mm:ss' : 'YYYY-MM-DD',
								style: 'width: 160px',
								'onUpdate:modelValue': (v: any) => setChild(idx, { ...child, value: v }),
							});
						}
						return h(ElInput as any, {
							modelValue: child.value ?? '',
							clearable: true,
							placeholder: t('message.hrms.advQuery.inputValue'),
							style: 'width: 160px',
							'onUpdate:modelValue': (v: any) => setChild(idx, { ...child, value: v }),
						});
					})();

					// 字段 | 运算符 | 值 | 并且/或者 | 添加 | 删除（与截图一同一套，新增行也必须完整）
					return h('div', { class: ['adv-leaf', child.stale ? 'is-stale' : ''], key: rowKey }, [
						h(ElSelect as any, {
							modelValue: child.field || undefined,
							filterable: true,
							clearable: true,
							placeholder: t('message.hrms.advQuery.field'),
							style: 'width: 140px',
							class: child.stale ? 'is-stale-input' : '',
							'onUpdate:modelValue': (v: string) => {
								const m = (p.fields || []).find((f) => f.field === v);
								const nextOps = m ? defaultOperators(m.valueType) : ops;
								setChild(idx, {
									...child,
									field: v,
									operator: nextOps.includes(child.operator as any) ? child.operator : nextOps[0],
									stale: false,
									value: '',
									filters: undefined,
								});
							},
						}, () => (p.fields || []).map((f) => h(ElOption as any, { label: f.label, value: f.field, key: f.field }))),
						h(ElSelect as any, {
							modelValue: child.operator,
							style: 'width: 110px',
							'onUpdate:modelValue': (v: string) => setChild(idx, { ...child, operator: v as any }),
						}, () => ops.map((op) => h(ElOption as any, { label: opLabel(op), value: op, key: op }))),
						valueNode,
						h(ElSelect as any, {
							modelValue: child.logic || 'And',
							style: 'width: 88px',
							'onUpdate:modelValue': (v: string) => setChild(idx, { ...child, logic: v as any }),
						}, () => [
							h(ElOption as any, { label: t('message.hrms.advQuery.and'), value: 'And' }),
							h(ElOption as any, { label: t('message.hrms.advQuery.or'), value: 'Or' }),
						]),
						h(ElButton as any, { type: 'primary', link: true, onClick: () => addLeafAfter(idx) }, () => t('message.hrms.advQuery.add')),
						h(ElButton as any, { type: 'danger', link: true, onClick: () => removeAt(idx) }, () => t('message.hrms.advQuery.remove')),
						child.stale ? h(ElTag as any, { type: 'danger', size: 'small' }, () => t('message.hrms.advQuery.stale')) : null,
					]);
				}),
			]);
		};
	},
});

defineExpose({ open, resetTree, getFilter: () => sanitizeForSubmit(state.root) });
</script>

<style scoped>
.adv-toolbar {
	display: flex;
	flex-wrap: wrap;
	gap: 8px;
	margin-bottom: 12px;
}
.mb10 {
	margin-bottom: 10px;
}
.adv-group-root {
	max-height: calc(100vh - 220px);
	overflow: auto;
}
.adv-footer {
	display: flex;
	justify-content: flex-end;
	gap: 8px;
}
:deep(.adv-group) {
	padding: 4px 0;
}
:deep(.adv-group.is-nested) {
	border: 1px dashed var(--el-border-color);
	border-radius: 6px;
	padding: 10px;
	margin-bottom: 8px;
	background: var(--el-fill-color-blank);
}
:deep(.adv-group-head) {
	display: flex;
	align-items: center;
	gap: 8px;
	margin-bottom: 8px;
}
:deep(.adv-leaf) {
	display: flex;
	flex-wrap: wrap;
	align-items: center;
	gap: 8px;
	margin-bottom: 10px;
}
:deep(.adv-leaf.is-stale),
:deep(.is-stale-input) {
	outline: 1px solid var(--el-color-danger);
	border-radius: 4px;
}
:deep(.adv-child-group) {
	margin: 0 0 10px 0;
}
:deep(.adv-child-group-actions) {
	display: flex;
	align-items: center;
	gap: 8px;
	margin: 0 0 8px 12px;
}
</style>
