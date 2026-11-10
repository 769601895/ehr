<template>
	<div class="emp-id-input">
		<el-input
			:model-value="modelValue"
			:placeholder="placeholder"
			:disabled="disabled"
			:clearable="!disabled"
			@update:model-value="onInput"
			@keyup.enter="resolve()"
			@blur="resolve()"
			@clear="onClear"
		/>
		<el-button v-if="allowPick && !disabled" @click="openPick">选人</el-button>

		<el-dialog v-model="pickVisible" :title="pickTitle" width="720px" append-to-body destroy-on-close>
			<div class="emp-pick-bar">
				<el-input v-model="keyword" placeholder="工号/姓名" clearable style="width: 220px" @keyup.enter="loadPage" />
				<el-button type="primary" @click="loadPage">查询</el-button>
			</div>
			<el-table :data="rows" height="360" highlight-current-row @row-dblclick="onPickRow" @current-change="(r: any) => (current = r)">
				<el-table-column prop="empId" label="工号" width="120" />
				<el-table-column prop="empName" label="姓名" width="120" />
				<el-table-column prop="ccDesc" label="部门" min-width="160" />
				<slot name="pick-extra-columns" />
			</el-table>
			<template #footer>
				<el-button @click="pickVisible = false">取消</el-button>
				<el-button type="primary" @click="confirmPick">选定</el-button>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup>
import { ref } from 'vue';
import { ElMessage } from 'element-plus';
import { lookupHrEmpByEmpId, pageHrEmp } from '/@/api/hrms/hrEmp';

export type ScopedEmp = {
	empId: string;
	empName?: string;
	ccCode?: string;
	ccDesc?: string;
	postId?: string;
	empState?: number;
	[key: string]: any;
};

const props = withDefaults(
	defineProps<{
		modelValue?: string;
		disabled?: boolean;
		/** 允许旁路选人弹窗（辅助，非唯一选人方式） */
		allowPick?: boolean;
		placeholder?: string;
		pickTitle?: string;
		/** 1在职 2停职 3离职；空=不限 */
		empState?: number | null;
	}>(),
	{
		modelValue: '',
		disabled: false,
		allowPick: true,
		placeholder: '输入工号后回车或失焦校验',
		pickTitle: '选择员工',
		empState: 1,
	}
);

const emits = defineEmits<{
	'update:modelValue': [string];
	/** 管辖校验通过后回填；清空或失败时为 null */
	resolved: [ScopedEmp | null];
}>();

const lastResolved = ref('');
const resolving = ref(false);
const pickVisible = ref(false);
const keyword = ref('');
const rows = ref<any[]>([]);
const current = ref<any>(null);

const onInput = (v: string) => {
	emits('update:modelValue', v ?? '');
	if (!(v || '').trim()) {
		lastResolved.value = '';
		emits('resolved', null);
	}
};

const onClear = () => {
	lastResolved.value = '';
	emits('update:modelValue', '');
	emits('resolved', null);
};

const applyEmp = (emp: ScopedEmp) => {
	const id = String(emp.empId || '').trim();
	lastResolved.value = id;
	emits('update:modelValue', id);
	emits('resolved', emp);
};

const resolve = async (force = false) => {
	if (props.disabled || resolving.value) return;
	const id = String(props.modelValue || '').trim();
	if (!id) {
		if (lastResolved.value) {
			lastResolved.value = '';
			emits('resolved', null);
		}
		return;
	}
	if (!force && id === lastResolved.value) return;

	resolving.value = true;
	try {
		const res = await lookupHrEmpByEmpId({
			empId: id,
			empState: props.empState == null ? undefined : props.empState,
		});
		const emp = (res.data?.result ?? res.data) as ScopedEmp;
		if (!emp?.empId) {
			lastResolved.value = '';
			emits('resolved', null);
			ElMessage.warning('人员不存在或不在管辖范围内');
			return;
		}
		applyEmp(emp);
	} catch {
		lastResolved.value = '';
		emits('resolved', null);
	} finally {
		resolving.value = false;
	}
};

const openPick = () => {
	pickVisible.value = true;
	keyword.value = '';
	current.value = null;
	loadPage();
};

const loadPage = async () => {
	const res = await pageHrEmp({
		page: 1,
		pageSize: 50,
		keyword: keyword.value || undefined,
		empState: props.empState == null ? undefined : props.empState,
		field: 'empId',
		order: 'asc',
		descStr: 'desc',
	});
	const page = res.data?.result ?? res.data;
	rows.value = page?.items ?? [];
};

const onPickRow = (row: any) => {
	current.value = row;
	confirmPick();
};

const confirmPick = () => {
	const row = current.value;
	if (!row?.empId) {
		ElMessage.warning('请选择员工');
		return;
	}
	applyEmp(row);
	pickVisible.value = false;
};

/** 外部在打开已有单据时同步「已校验」标记，避免失焦重复查 */
const markResolved = (empId?: string) => {
	lastResolved.value = String(empId || props.modelValue || '').trim();
};

defineExpose({ resolve, markResolved });
</script>

<style scoped>
.emp-id-input {
	display: flex;
	gap: 8px;
	width: 100%;
}
.emp-pick-bar {
	display: flex;
	gap: 8px;
	margin-bottom: 8px;
}
</style>
