<template>
	<el-form :model="model" label-width="110px" :disabled="readonly">
		<el-row :gutter="12">
			<el-col v-for="f in fields" :key="f.fieldKey" :span="colSpan(f)">
				<el-form-item :label="f.label" :required="!!f.required">
					<el-input
						v-if="f.controlType === 'text' || !f.controlType"
						v-model="model[f.fieldKey]"
						:placeholder="f.placeholder"
						:readonly="readonly || f.readonly"
					/>
					<el-input
						v-else-if="f.controlType === 'textarea'"
						v-model="model[f.fieldKey]"
						type="textarea"
						:rows="3"
						:placeholder="f.placeholder"
						:readonly="readonly || f.readonly"
					/>
					<el-input-number
						v-else-if="f.controlType === 'number'"
						v-model="model[f.fieldKey]"
						class="w100"
						:disabled="readonly || f.readonly"
					/>
					<el-date-picker
						v-else-if="f.controlType === 'date'"
						v-model="model[f.fieldKey]"
						type="date"
						value-format="YYYY-MM-DD"
						class="w100"
						:disabled="readonly || f.readonly"
					/>
					<el-select
						v-else-if="f.controlType === 'select' || f.codeField || f.lookupSource"
						v-model="model[f.fieldKey]"
						class="w100"
						clearable
						filterable
						:placeholder="f.placeholder || '请选择'"
						:disabled="readonly || f.readonly"
					>
						<el-option
							v-for="o in optionsOf(f)"
							:key="o.value"
							:label="optionText(o)"
							:value="o.value"
						/>
					</el-select>
					<div v-else-if="f.controlType === 'dept'" class="org-pick-field">
						<el-input
							:model-value="deptDisplay(f.fieldKey)"
							:placeholder="f.placeholder || '请选择部门'"
							readonly
							class="org-pick-input"
						/>
						<el-button
							class="org-pick-btn"
							:disabled="readonly || f.readonly"
							@click="openDeptPick(f.fieldKey)"
						>....</el-button>
						<el-button
							v-if="model[f.fieldKey] && !(readonly || f.readonly)"
							text
							type="danger"
							@click="clearDept(f.fieldKey)"
						>清除</el-button>
					</div>
					<el-input
						v-else-if="f.controlType === 'user'"
						v-model="model[f.fieldKey]"
						:placeholder="f.placeholder || '用户Id'"
						:readonly="readonly || f.readonly"
					/>
					<el-input
						v-else-if="f.controlType === 'attachment'"
						v-model="model[f.fieldKey]"
						:placeholder="f.placeholder || '附件URL'"
						:readonly="readonly || f.readonly"
					/>
					<el-input v-else v-model="model[f.fieldKey]" :readonly="readonly || f.readonly" />
				</el-form-item>
			</el-col>
		</el-row>
	</el-form>

	<OrgPickDialog
		v-model:visible="deptPick.visible"
		:org-data="orgTree"
		:model-value="deptPick.fieldKey ? model[deptPick.fieldKey] : ''"
		@confirm="onDeptConfirm"
	/>
</template>

<script lang="ts" setup>
import { computed, onMounted, reactive, ref, watch } from 'vue';
import { getHrCodeList, getHrOrgList } from '/@/api/hrms/hrOrg';
import { getHrLookupOptions } from '/@/api/hrms/hrLookupDef';
import OrgPickDialog from '/@/components/selector/orgPickDialog.vue';

type Opt = { value: string; label?: string; text?: string };

const props = defineProps<{
	fields: any[];
	modelValue: Record<string, any>;
	readonly?: boolean;
}>();

const emit = defineEmits<{ (e: 'update:modelValue', v: Record<string, any>): void }>();

const model = computed({
	get: () => props.modelValue,
	set: (v) => emit('update:modelValue', v),
});

const fields = computed(() => [...(props.fields || [])].sort((a, b) => (a.sortNo || 0) - (b.sortNo || 0)));

const optionMaps = reactive<Record<string, Opt[]>>({});
const orgTree = ref<any[]>([]);
const deptNameMap = reactive<Record<string, string>>({});
const deptPick = reactive({ visible: false, fieldKey: '' as string });

const optionKey = (f: any) => {
	const lookup = String(f?.lookupSource || '').trim();
	if (lookup) return `lookup:${lookup}`;
	const code = String(f?.codeField || '').trim();
	if (code) return `code:${code}`;
	return '';
};

const optionsOf = (f: any) => optionMaps[optionKey(f)] || [];

const optionText = (o: Opt) => {
	const text = (o.text ?? '').trim();
	if (text) return text;
	const value = String(o.value ?? '');
	const label = String(o.label ?? '');
	const prefix = value + ' ';
	if (label.startsWith(prefix)) return label.slice(prefix.length).trim() || value;
	return label || value;
};

const colSpan = (f: any) => {
	const slots = f.spanSlots ?? 12;
	if (slots >= 24) return 24;
	if (slots >= 12) return 12;
	if (slots >= 8) return 8;
	return 12;
};

const loadOptions = async () => {
	const codes = [...new Set(fields.value.filter((f) => f.codeField && !f.lookupSource).map((f) => String(f.codeField).trim()).filter(Boolean))];
	const lookups = [...new Set(fields.value.filter((f) => f.lookupSource).map((f) => String(f.lookupSource).trim()).filter(Boolean))];
	for (const k of Object.keys(optionMaps)) delete optionMaps[k];
	const codeEntries = await Promise.all(
		codes.map(async (c) => {
			try {
				const list = ((await getHrCodeList(c)).data?.result ?? []) as Opt[];
				return [`code:${c}`, list] as const;
			} catch {
				return [`code:${c}`, []] as const;
			}
		})
	);
	const lookupEntries = await Promise.all(
		lookups.map(async (s) => {
			try {
				const list = ((await getHrLookupOptions(s)).data?.result ?? []) as Opt[];
				return [`lookup:${s}`, list] as const;
			} catch {
				return [`lookup:${s}`, []] as const;
			}
		})
	);
	for (const [k, list] of [...codeEntries, ...lookupEntries]) optionMaps[k] = list;
};

const mapOrgNodes = (nodes: any[]): any[] =>
	(nodes || []).map((n) => ({
		id: n.ccCode,
		name: n.ccDesc || n.ccCode,
		children: mapOrgNodes(n.children || []),
	}));

const walkOrgNames = (nodes: any[], path: string[] = []) => {
	for (const n of nodes || []) {
		const name = n.ccDesc || n.name || n.ccCode || n.id;
		const code = String(n.ccCode || n.id || '');
		const next = [...path, name];
		if (code) deptNameMap[code] = next.join(' / ');
		walkOrgNames(n.children || [], next);
	}
};

const ensureOrgTree = async () => {
	if (orgTree.value.length) return;
	try {
		const res = await getHrOrgList({ includeDisabled: false, empState: 1 });
		const raw = res.data?.result ?? res.data ?? [];
		walkOrgNames(raw);
		orgTree.value = mapOrgNodes(raw);
	} catch {
		orgTree.value = [];
	}
};

const deptDisplay = (fieldKey: string) => {
	const code = String(model.value?.[fieldKey] ?? '');
	if (!code) return '';
	return deptNameMap[code] || code;
};

const openDeptPick = async (fieldKey: string) => {
	if (props.readonly) return;
	await ensureOrgTree();
	deptPick.fieldKey = fieldKey;
	deptPick.visible = true;
};

const clearDept = (fieldKey: string) => {
	model.value[fieldKey] = '';
};

const onDeptConfirm = (payload: { id: string | number; name: string }) => {
	const key = deptPick.fieldKey;
	if (!key) return;
	const code = String(payload.id ?? '');
	model.value[key] = code;
	if (code) deptNameMap[code] = payload.name || code;
};

watch(
	() => props.fields,
	() => {
		loadOptions();
		if (fields.value.some((f) => f.controlType === 'dept')) ensureOrgTree();
	},
	{ deep: true, immediate: true }
);

onMounted(() => {
	loadOptions();
	if (fields.value.some((f) => f.controlType === 'dept')) ensureOrgTree();
});
</script>

<style scoped>
.w100 {
	width: 100%;
}
.org-pick-field {
	display: flex;
	align-items: center;
	gap: 6px;
	width: 100%;
}
.org-pick-input {
	flex: 1;
}
.org-pick-btn {
	flex-shrink: 0;
}
</style>
